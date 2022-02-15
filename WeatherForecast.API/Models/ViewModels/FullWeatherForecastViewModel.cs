namespace WeatherForecast.API.Models.ViewModels
{
    public class FullWeatherForecastViewModel
    {
        public string LocationName { get; set; }
        public IEnumerable<SingleForecastViewModel> Forecasts { get; set; }
    }
}
