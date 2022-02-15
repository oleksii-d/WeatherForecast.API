using Microsoft.AspNetCore.Mvc;
using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.Models.ViewModels;

namespace WeatherForecast.API.Controllers
{
    [ApiController]
    [Route("api/weather-forecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IForecastService _weatherForecastService;

        public WeatherForecastController(
            IForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;

        }

        [HttpGet]
        public async Task<ActionResult<FullWeatherForecastViewModel>> GetAsync()
        {
            return Ok(await _weatherForecastService.GetWeatherForecastAsync());
        }
    }
}