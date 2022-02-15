using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.Controllers;
using WeatherForecast.API.Models.ViewModels;
using Xunit;

namespace WeatherForecast.API.Test.Controllers
{
    public class WeatherForecastControllerTest
    {
        WeatherForecastController _forecastController;
        private readonly Mock<IForecastService> _mockForecastService;

        public WeatherForecastControllerTest()
        {
            _mockForecastService = new Mock<IForecastService>();
            _forecastController = new WeatherForecastController(_mockForecastService.Object);

        }

        [Fact]
        public async Task Get_ReturnsWeatherForecast()
        {
            //arrange
            var fakeForecast = GetFakeWeatherForecast();
            _mockForecastService.Setup(x => x.GetWeatherForecastAsync())
                .ReturnsAsync(fakeForecast);

            //act
            var actionResult = await _forecastController.GetAsync();
            var result = actionResult.Result as OkObjectResult;
            var actual = result.Value as FullWeatherForecastViewModel;

            //assert
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual.LocationName);
            Assert.Equal(fakeForecast.Forecasts.Count(), actual.Forecasts.Count());
        }

        private FullWeatherForecastViewModel GetFakeWeatherForecast()
        {
            return new FullWeatherForecastViewModel
            {
                LocationName = "Sample City",
                Forecasts = new List<SingleForecastViewModel>
                {
                    new SingleForecastViewModel
                    {
                        Date = new DateTime(2001, 10, 10),
                        DayDescription ="Day description",
                        DayPrecipitation ="Day precipitation",
                        NightDescription ="Night description",
                        NightPrecipitation ="Night precipitation",
                        MaxTemperature = "-10 C",
                        MinTemperature = "10 C"
                    },
                    new SingleForecastViewModel
                    {
                        Date = new DateTime(2001, 10, 10),
                        DayDescription ="Day description",
                        DayPrecipitation ="Day precipitation",
                        NightDescription ="Night description",
                        NightPrecipitation ="Night precipitation",
                        MaxTemperature = "-10 C",
                        MinTemperature = "10 C"
                    }
                }
            };
        }
    }
}
