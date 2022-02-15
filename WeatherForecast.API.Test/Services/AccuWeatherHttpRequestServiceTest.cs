using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherForecast.API.BLL.Services.Concretes;
using WeatherForecast.API.Helpers.Constants;
using WeatherForecast.API.Models.ApiResponseModels;
using Xunit;

namespace WeatherForecast.API.Test.Services
{
    public class AccuWeatherHttpRequestServiceTest
    {
        private readonly AccuWeatherHttpRequestService _accuWeatherHttpRequestService;
        private readonly IConfiguration _configuration;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        
        public AccuWeatherHttpRequestServiceTest()
        {
            _configuration = TestConfigurationInitializer.InitTestConfiguration();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            _accuWeatherHttpRequestService = new AccuWeatherHttpRequestService(
                _configuration,
                _mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsData_LocationEndpoint()
        {
            //arrange
            var client = new HttpClient();
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            //act
            var actual = await _accuWeatherHttpRequestService.GetAsync<LocationApiResponse> (
                _configuration[SettingKeys.AccuWeatherUriLocationByIpAddress],
                additionalParameters: null,
                additionalHeaders: new Dictionary<string, string> { { HeaderNames.XForwardedFor, "8.8.8.8" } });

            //assert
            Assert.NotNull(actual);
            Assert.IsType<LocationApiResponse>(actual);
            Assert.NotEmpty(actual.EnglishName);
            Assert.NotEmpty(actual.Key);
        }

        [Fact]
        public async Task GetAsync_ReturnsData_ForecastEndpoint()
        {
            //arrange
            var client = new HttpClient();
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            //act
            var actual = await _accuWeatherHttpRequestService.GetAsync<ForecastApiResponse>(
                string.Format(
                        _configuration[SettingKeys.AccuWeatherUriForecast5DaysByLocationKey],
                        _configuration[SettingKeys.AccuWeatherDefaultLocationKey]),
                additionalParameters: new Dictionary<string, string> { { "metric", "true" } },
                additionalHeaders: null);

            //assert
            Assert.NotNull(actual);
            Assert.IsType<ForecastApiResponse>(actual);
            Assert.Equal(5, actual.DailyForecasts?.Count);
        }

    }
}
