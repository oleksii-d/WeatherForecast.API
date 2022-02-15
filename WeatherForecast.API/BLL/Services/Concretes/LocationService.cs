using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.Helpers.Constants;
using WeatherForecast.API.Models.ApiResponseModels;
using WeatherForecast.API.Models.BLLModels;

namespace WeatherForecast.API.BLL.Services.Concretes
{
    public class LocationService : ILocationService
    {
        private readonly IConfiguration _configuration;
        private readonly IIPService _ipService;
        private readonly IAccuWeatherHttpRequestService _accuWeatherHttpRequestService;

        public LocationService(
            IConfiguration configuration,
            IIPService ipService,
            IAccuWeatherHttpRequestService accuWeatherHttpRequestService
        )
        {
            _configuration = configuration;
            _ipService = ipService;
            _accuWeatherHttpRequestService = accuWeatherHttpRequestService;
        }

        public async Task<LocationDTO> GetLocationAsync()
        {
            return bool.Parse(_configuration[SettingKeys.AccuWeatherUseDefaultLocation]) ?
                    GetDefaultLocation() :
                    await GetLocationByClientIPAsync();
        }

        private LocationDTO GetDefaultLocation()
        {
            return new LocationDTO
            {
                Key = _configuration[SettingKeys.AccuWeatherDefaultLocationKey],
                Name = _configuration[SettingKeys.AccuWeatherDefaultLocationName]
            };
        }

        private async Task<LocationDTO> GetLocationByClientIPAsync()
        {
            var clientIp = _ipService.GetCurrentHttpClientIPAddress();

            var additionalHeaders = new Dictionary<string, string> { { HeaderNames.XForwardedFor, clientIp.ToString() } };

            LocationApiResponse locationResponse =
                await _accuWeatherHttpRequestService.GetAsync<LocationApiResponse>(
                    _configuration[SettingKeys.AccuWeatherUriLocationByIpAddress],
                    additionalHeaders: additionalHeaders);

            return new LocationDTO
            {
                Key = locationResponse.Key,
                Name = locationResponse.EnglishName
            };
        }
    }
}
