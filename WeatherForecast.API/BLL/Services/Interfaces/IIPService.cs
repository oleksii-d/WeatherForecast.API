using System.Net;

namespace WeatherForecast.API.BLL.Services.Interfaces
{
    public interface IIPService
    {
        IPAddress GetCurrentHttpClientIPAddress();
    }
}
