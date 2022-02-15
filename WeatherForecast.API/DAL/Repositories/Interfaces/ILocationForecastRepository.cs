using WeatherForecast.API.Models.DALModels;

namespace WeatherForecast.API.DAL.Repositories.Interfaces
{
    public interface ILocationForecastRepository
    {
        Task<LocationForecast> GetAsync(string locationKey);
        Task AddOrUpdateAsync(LocationForecast locationKey);
    }
}
