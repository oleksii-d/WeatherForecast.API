using WeatherForecast.API.DAL.Repositories.Interfaces;
using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.Models.ViewModels;
using WeatherForecast.API.Helpers.Constants;
using System.Text.Json;
using WeatherForecast.API.Models.DALModels;
using WeatherForecast.API.Models.ApiResponseModels;
using WeatherForecast.API.Models.BLLModels;

namespace WeatherForecast.API.BLL.Services.Concretes
{
    public class ForecastService : IForecastService
    {
        private readonly ILocationService _locationService;
        private readonly ILocationForecastRepository _locationForecastRepository;
        private readonly IAccuWeatherHttpRequestService _accuWeatherHttpRequestService;
        private readonly IConfiguration _configuration;


        public ForecastService(
            ILocationService locationService,
            ILocationForecastRepository locationForecastRepository,
            IAccuWeatherHttpRequestService accuWeatherHttpRequestService,
            IConfiguration configuration
        )
        {
            _locationService = locationService;
            _locationForecastRepository = locationForecastRepository;
            _accuWeatherHttpRequestService = accuWeatherHttpRequestService;
            _configuration = configuration;
        }

        public async Task<FullWeatherForecastViewModel> GetWeatherForecastAsync()
        {
            LocationDTO location = await _locationService.GetLocationAsync();

            LocationForecast locationForecast = await _locationForecastRepository.GetAsync(location.Key);

            if (locationForecast == null || locationForecast.ForecastEffectiveDateUTC <= DateTime.UtcNow)
            {
                locationForecast = await GetForecastFromAccuWeatherAndUpdateDatabaseAsync(location);
            }

            return new FullWeatherForecastViewModel
            {
                LocationName = locationForecast.LocationName,
                Forecasts = MapForecastDataJsonToForecastsViewModel(locationForecast.ForecastDataJson)
            };
        }

        private async Task<LocationForecast> GetForecastFromAccuWeatherAndUpdateDatabaseAsync(LocationDTO location)
        {
            var additionalParameters = new Dictionary<string, string> { { "metric", "true" } };

            ForecastApiResponse forecastResponse =
                await _accuWeatherHttpRequestService.GetAsync<ForecastApiResponse>(
                    string.Format(
                        _configuration[SettingKeys.AccuWeatherUriForecast5DaysByLocationKey],
                        location.Key),
                    additionalParameters: additionalParameters);

            var updatedLocationForecast = new LocationForecast
            {
                LocationKey = location.Key,
                LocationName = location.Name,
                ForecastEffectiveDateUTC = forecastResponse.Headline.EffectiveDate.ToUniversalTime(),
                ForecastDataJson = JsonSerializer.Serialize(forecastResponse)
            };

            await _locationForecastRepository.AddOrUpdateAsync(updatedLocationForecast);

            return updatedLocationForecast;
        }

        private IEnumerable<SingleForecastViewModel> MapForecastDataJsonToForecastsViewModel(string forecastDataJson)
        {
            ForecastApiResponse forecastData = JsonSerializer.Deserialize<ForecastApiResponse>(forecastDataJson);

            return forecastData.DailyForecasts.Select(x => new SingleForecastViewModel
            {
                Date = x.Date,
                MinTemperature = $"{x.Temperature.Minimum.Value} {x.Temperature.Minimum.Unit}",
                MaxTemperature = $"{x.Temperature.Maximum.Value} {x.Temperature.Maximum.Unit}",
                DayDescription = x.Day.IconPhrase,
                NightDescription = x.Night.IconPhrase,
                DayPrecipitation = x.Day.HasPrecipitation ? $"{x.Day.PrecipitationType} ({x.Day.PrecipitationIntensity})" : null,
                NightPrecipitation = x.Night.HasPrecipitation ? $"{x.Night.PrecipitationType} ({x.Night.PrecipitationIntensity})" : null,
            });
        }
    }
}
