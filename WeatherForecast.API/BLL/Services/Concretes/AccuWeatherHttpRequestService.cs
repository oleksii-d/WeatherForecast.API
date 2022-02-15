using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.Helpers.Constants;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace WeatherForecast.API.BLL.Services.Concretes
{
    public class AccuWeatherHttpRequestService : IAccuWeatherHttpRequestService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccuWeatherHttpRequestService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory
        )
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> GetAsync<T>(
            string relativeEndpointUrl,
            Dictionary<string, string>? additionalParameters = null,
            Dictionary<string, string>? additionalHeaders = null)
        {

            var urlWithoutParameters = $"{_configuration[SettingKeys.AccuWeatherUriBaseUrl]}{relativeEndpointUrl}";

            var urlWithParameters = new Uri(QueryHelpers.AddQueryString(
                urlWithoutParameters,
                BuildGetParameters(additionalParameters)));

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, urlWithParameters);

            AddHeaders(httpRequestMessage, additionalHeaders);

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                var apiResponse = await JsonSerializer.DeserializeAsync<T>(contentStream);

                if (apiResponse == null)
                {
                    throw new Exception($"{ErrorMessages.HttpRequestErrorMessage} Response is null.");
                }

                return apiResponse;
            }
            else
            {
                throw new Exception($"{ErrorMessages.HttpRequestErrorMessage} {httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}");
            }
        }

        private Dictionary<string, string> BuildGetParameters(Dictionary<string, string>? additionalParameters)
        {
            var parameters = new Dictionary<string, string>() { { "apikey", _configuration[SettingKeys.AccuWeatherApiKey] } };

            if (additionalParameters != null)
            {
                foreach (var parameter in additionalParameters)
                {
                    parameters.Add(parameter.Key, parameter.Value);
                }
            }

            return parameters;
        }

        private void AddHeaders(HttpRequestMessage httpRequestMessage, Dictionary<string, string>? additionalHeaders)
        {
            if (additionalHeaders != null)
            {
                foreach (var parameter in additionalHeaders)
                {
                    httpRequestMessage.Headers.Add(parameter.Key, parameter.Value);
                }
            }
        }
    }
}
