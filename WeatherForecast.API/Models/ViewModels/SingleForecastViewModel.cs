namespace WeatherForecast.API.Models.ViewModels
{
    public class SingleForecastViewModel
    {
        public DateTime Date { get; set; }
        public string MinTemperature { get; set; }
        public string MaxTemperature { get; set; }
        public string DayDescription { get; set; }
        public string DayPrecipitation { get; set; }
        public string NightDescription { get; set; }
        public string NightPrecipitation { get; set; }
    }
}
