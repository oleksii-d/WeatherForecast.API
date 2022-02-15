using Microsoft.EntityFrameworkCore;
using WeatherForecast.API.Models.DALModels;

namespace WeatherForecast.API.DAL
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<LocationForecast> LocationForecasts { get; set; }
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
    }
}