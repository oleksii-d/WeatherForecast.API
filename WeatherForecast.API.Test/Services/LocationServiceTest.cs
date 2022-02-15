using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WeatherForecast.API.BLL.Services.Concretes;
using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.Helpers.Constants;
using WeatherForecast.API.Models.ApiResponseModels;
using Xunit;

namespace WeatherForecast.API.Test.Services
{
    public class LocationServiceTest
    {
        private readonly LocationService _locationService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IIPService> _mockIpService;
        private readonly Mock<IAccuWeatherHttpRequestService> _mockAccuWeatherHttpRequestService;

        private const string FAKE_DEFAULT_KEY = "13579";
        private const string FAKE_DEFAULT_NAME = "Default City";

        private const string FAKE_NON_DEFAULT_KEY = "24680";
        private const string FAKE_NON_DEFAULT_NAME = "My Fake City";

        public LocationServiceTest()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockIpService = new Mock<IIPService>();
            _mockAccuWeatherHttpRequestService = new Mock<IAccuWeatherHttpRequestService>();

            _locationService = new LocationService(
                _mockConfiguration.Object,
                _mockIpService.Object,
                _mockAccuWeatherHttpRequestService.Object);
        }

        [Theory]
        [InlineData("true", FAKE_DEFAULT_KEY, FAKE_DEFAULT_NAME)]
        [InlineData("false", FAKE_NON_DEFAULT_KEY, FAKE_NON_DEFAULT_NAME)]
        public async Task GetLocationAsync_ReturnsLocation(string useDefault, string expectedKey, string expectedName)
        {
            //arrange
            _mockConfiguration
                .Setup(x => x[SettingKeys.AccuWeatherUseDefaultLocation])
                .Returns(useDefault);

            _mockConfiguration
                .Setup(x => x[SettingKeys.AccuWeatherDefaultLocationKey])
                .Returns(FAKE_DEFAULT_KEY);

            _mockConfiguration
                .Setup(x => x[SettingKeys.AccuWeatherDefaultLocationName])
                .Returns(FAKE_DEFAULT_NAME);

            _mockIpService.Setup(x => x.GetCurrentHttpClientIPAddress()).Returns(GetFakeIPAddress);

            _mockAccuWeatherHttpRequestService
                .Setup(x => x.GetAsync<LocationApiResponse>(
                    It.IsAny<string>(), 
                    It.IsAny<Dictionary<string,string>>(),
                    It.IsAny<Dictionary<string,string>>()))
                .ReturnsAsync(GetFakeLocationApiResponse());

            //act
            var actual = await _locationService.GetLocationAsync();

            //assert
            Assert.NotNull(actual);
            Assert.Equal(actual.Key, expectedKey);
            Assert.Equal(actual.Name, expectedName);
        }

        private IPAddress GetFakeIPAddress()
        {
            return IPAddress.Parse("8.8.8.8");
        }

        private LocationApiResponse GetFakeLocationApiResponse()
        {
            return new LocationApiResponse
            {
                Key = FAKE_NON_DEFAULT_KEY,
                EnglishName = FAKE_NON_DEFAULT_NAME
            };
        }
    }
}
