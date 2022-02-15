using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherForecast.API.BLL.Services.Concretes;
using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.DAL.Repositories.Interfaces;
using WeatherForecast.API.Helpers.Constants;
using WeatherForecast.API.Models.ApiResponseModels;
using WeatherForecast.API.Models.BLLModels;
using WeatherForecast.API.Models.DALModels;
using WeatherForecast.API.Models.ViewModels;
using Xunit;

namespace WeatherForecast.API.Test.Services
{
    public class ForecastServiceTest
    {
        private readonly ForecastService _forecastService;
        private readonly IConfiguration _configuration;
        private readonly Mock<IAccuWeatherHttpRequestService> _mockAccuWeatherHttpRequestService;
        private readonly Mock<ILocationService> _mockLocationService;
        private readonly Mock<ILocationForecastRepository> _mockLocationForecastRepository;

        private const string LOCATION_NAME_FROM_DB = "Fake city from DB";
        private const string LOCATION_NAME_FROM_GET_LOCATION = "Fake city from GetLocation";

        public ForecastServiceTest()
        {
            _configuration = TestConfigurationInitializer.InitTestConfiguration();
            _mockAccuWeatherHttpRequestService = new Mock<IAccuWeatherHttpRequestService>();
            _mockLocationService = new Mock<ILocationService>();
            _mockLocationForecastRepository = new Mock<ILocationForecastRepository>();

            _forecastService = new ForecastService(
                _mockLocationService.Object,
                _mockLocationForecastRepository.Object,
                _mockAccuWeatherHttpRequestService.Object,
                _configuration);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task GetWeatherForecastAsync_ReturnsData(DateTime fakeUtcNow, string expectedLocationName)
        {
            //arrange
            LocationDTO fakeLocation = GetFakeLocation();
            LocationForecast fakeForecastFromDB = GetFakeForecastFromDB(fakeUtcNow);
            ForecastApiResponse fakeForecastFromAccuWeather = GetFakeForecastFromAccuWeather();

            _mockLocationService.Setup(x => x.GetLocationAsync()).ReturnsAsync(fakeLocation);

            _mockLocationForecastRepository
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(fakeForecastFromDB);

            _mockLocationForecastRepository
                .Setup(x => x.AddOrUpdateAsync(It.IsAny<LocationForecast>()));

            _mockAccuWeatherHttpRequestService
                .Setup(x => x.GetAsync<ForecastApiResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(fakeForecastFromAccuWeather);

            //act
            var actual = await _forecastService.GetWeatherForecastAsync();

            //assert
            Assert.NotNull(actual);
            Assert.IsType<FullWeatherForecastViewModel>(actual);
            Assert.Equal(actual.LocationName, expectedLocationName);
        }
    
        LocationDTO GetFakeLocation()
        {
            return new LocationDTO
            {
                Key = _configuration[SettingKeys.AccuWeatherDefaultLocationKey],
                Name = LOCATION_NAME_FROM_GET_LOCATION
            };
        }

        LocationForecast GetFakeForecastFromDB(DateTime fakeUtcNow)
        {
            return new LocationForecast
            {
                ForecastEffectiveDateUTC = fakeUtcNow,
                LocationKey = _configuration[SettingKeys.AccuWeatherDefaultLocationKey],
                LocationName = LOCATION_NAME_FROM_DB,
                ForecastDataJson = "{\"Headline\":{\"EffectiveDate\":\"2022-02-17T19:00:00+02:00\",\"EffectiveEpochDate\":1645117200,\"Severity\":5,\"Text\":\"Expect showers Thursday evening\",\"Category\":\"rain\",\"EndDate\":\"2022-02-18T01:00:00+02:00\",\"EndEpochDate\":1645138800,\"MobileLink\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?unit=c\\u0026lang=en-us\",\"Link\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?unit=c\\u0026lang=en-us\"},\"DailyForecasts\":[{\"Date\":\"2022-02-13T07:00:00+02:00\",\"EpochDate\":1644728400,\"Temperature\":{\"Minimum\":{\"Value\":-10.6,\"Unit\":\"C\",\"UnitType\":17},\"Maximum\":{\"Value\":2.2,\"Unit\":\"C\",\"UnitType\":17}},\"Day\":{\"Icon\":1,\"IconPhrase\":\"Sunny\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Night\":{\"Icon\":33,\"IconPhrase\":\"Clear\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Sources\":[\"AccuWeather\"],\"MobileLink\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=1\\u0026unit=c\\u0026lang=en-us\",\"Link\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=1\\u0026unit=c\\u0026lang=en-us\"},{\"Date\":\"2022-02-14T07:00:00+02:00\",\"EpochDate\":1644814800,\"Temperature\":{\"Minimum\":{\"Value\":-10.9,\"Unit\":\"C\",\"UnitType\":17},\"Maximum\":{\"Value\":1.1,\"Unit\":\"C\",\"UnitType\":17}},\"Day\":{\"Icon\":1,\"IconPhrase\":\"Sunny\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Night\":{\"Icon\":33,\"IconPhrase\":\"Clear\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Sources\":[\"AccuWeather\"],\"MobileLink\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=2\\u0026unit=c\\u0026lang=en-us\",\"Link\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=2\\u0026unit=c\\u0026lang=en-us\"},{\"Date\":\"2022-02-15T07:00:00+02:00\",\"EpochDate\":1644901200,\"Temperature\":{\"Minimum\":{\"Value\":-6.1,\"Unit\":\"C\",\"UnitType\":17},\"Maximum\":{\"Value\":1.8,\"Unit\":\"C\",\"UnitType\":17}},\"Day\":{\"Icon\":2,\"IconPhrase\":\"Mostly sunny\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Night\":{\"Icon\":34,\"IconPhrase\":\"Mostly clear\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Sources\":[\"AccuWeather\"],\"MobileLink\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=3\\u0026unit=c\\u0026lang=en-us\",\"Link\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=3\\u0026unit=c\\u0026lang=en-us\"},{\"Date\":\"2022-02-16T07:00:00+02:00\",\"EpochDate\":1644987600,\"Temperature\":{\"Minimum\":{\"Value\":2.5,\"Unit\":\"C\",\"UnitType\":17},\"Maximum\":{\"Value\":3,\"Unit\":\"C\",\"UnitType\":17}},\"Day\":{\"Icon\":4,\"IconPhrase\":\"Intermittent clouds\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Night\":{\"Icon\":7,\"IconPhrase\":\"Cloudy\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Sources\":[\"AccuWeather\"],\"MobileLink\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=4\\u0026unit=c\\u0026lang=en-us\",\"Link\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=4\\u0026unit=c\\u0026lang=en-us\"},{\"Date\":\"2022-02-17T07:00:00+02:00\",\"EpochDate\":1645074000,\"Temperature\":{\"Minimum\":{\"Value\":4.1,\"Unit\":\"C\",\"UnitType\":17},\"Maximum\":{\"Value\":5.9,\"Unit\":\"C\",\"UnitType\":17}},\"Day\":{\"Icon\":4,\"IconPhrase\":\"Intermittent clouds\",\"HasPrecipitation\":false,\"PrecipitationType\":null,\"PrecipitationIntensity\":null},\"Night\":{\"Icon\":12,\"IconPhrase\":\"Showers\",\"HasPrecipitation\":true,\"PrecipitationType\":\"Rain\",\"PrecipitationIntensity\":\"Moderate\"},\"Sources\":[\"AccuWeather\"],\"MobileLink\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=5\\u0026unit=c\\u0026lang=en-us\",\"Link\":\"http://www.accuweather.com/en/ua/kharkiv/323903/daily-weather-forecast/323903?day=5\\u0026unit=c\\u0026lang=en-us\"}]}"
            };
        }

        ForecastApiResponse GetFakeForecastFromAccuWeather()
        {
            return new ForecastApiResponse
            {
                Headline = new Headline
                {
                    EffectiveDate = DateTime.UtcNow,
                    EffectiveEpochDate = 1234567,
                },
                DailyForecasts = new List<DailyForecast>()
            };
        }

        public static IEnumerable<object[]> Data()
        {
            yield return new object[] 
            { 
                DateTime.UtcNow.AddHours(1),
                LOCATION_NAME_FROM_DB
            };
            yield return new object[]
            {
                DateTime.UtcNow.AddHours(-1),
                LOCATION_NAME_FROM_GET_LOCATION
            };
        }
    }
}
