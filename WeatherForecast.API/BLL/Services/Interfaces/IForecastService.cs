using WeatherForecast.API.Models.ViewModels;

namespace WeatherForecast.API.BLL.Services.Interfaces
{
    public interface IForecastService
    {
        Task<FullWeatherForecastViewModel> GetWeatherForecastAsync();
    }
}
