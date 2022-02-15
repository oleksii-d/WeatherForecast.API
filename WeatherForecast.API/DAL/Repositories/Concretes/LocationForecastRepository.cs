using WeatherForecast.API.DAL.Repositories.Interfaces;
using WeatherForecast.API.Models.DALModels;

namespace WeatherForecast.API.DAL.Repositories.Concretes
{
    public class LocationForecastRepository : ILocationForecastRepository
    {
        private readonly ApplicationDBContext _context;

        public LocationForecastRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<LocationForecast> GetAsync(string locationKey)
        {
            return await _context.LocationForecasts.FindAsync(locationKey);
        }

        public async Task AddOrUpdateAsync(LocationForecast location)
        {
            var locationForecastFromDB = await GetAsync(location.LocationKey);

            if (locationForecastFromDB == null)
            {
                await _context.AddAsync(location);
            }
            else
            {
                locationForecastFromDB.LocationName = location.LocationName;
                locationForecastFromDB.ForecastEffectiveDateUTC = location.ForecastEffectiveDateUTC;
                locationForecastFromDB.ForecastDataJson = location.ForecastDataJson;
            }

            await _context.SaveChangesAsync();
        }
    }
}
