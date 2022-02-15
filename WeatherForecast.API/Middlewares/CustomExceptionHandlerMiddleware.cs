using System.Net;
using System.Text.Json;
using WeatherForecast.API.Helpers.Constants;

namespace WeatherForecast.API.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;

        public CustomExceptionHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var _logger = _loggerFactory.CreateLogger<CustomExceptionHandlerMiddleware>();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{DateTime.UtcNow} {ex.Message}");

                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await response.WriteAsync(JsonSerializer.Serialize(new { error = ErrorMessages.TryAgainMessage }));
            }
        }

    }
}
