using WeatherForecast.API.Models;
using WeatherForecast.API.Models.BLLModels;

namespace WeatherForecast.API.BLL.Services.Interfaces
{
    public interface ILocationService
    {
        Task<LocationDTO> GetLocationAsync();
    }
}
