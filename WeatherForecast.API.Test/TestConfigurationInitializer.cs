using Microsoft.Extensions.Configuration;

namespace WeatherForecast.API.Test
{
    internal static class TestConfigurationInitializer
    {
        public static IConfiguration InitTestConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.UnitTests.json")
                .AddEnvironmentVariables()
                .Build();
            return config;
        }
    }
}
