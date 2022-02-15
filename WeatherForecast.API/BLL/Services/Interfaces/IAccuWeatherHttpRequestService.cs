using System.Net;
using WeatherForecast.API.Models.ViewModels;

namespace WeatherForecast.API.BLL.Services.Interfaces
{
    public interface IAccuWeatherHttpRequestService
    {
        Task<T> GetAsync<T>(
            string relativeEndpointUrl, 
            Dictionary<string, string>? additionalParameters = null, 
            Dictionary<string, string>? additionalHeaders = null);
    }
}
