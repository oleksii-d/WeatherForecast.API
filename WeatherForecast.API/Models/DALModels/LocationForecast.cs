using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.API.Models.DALModels
{
    public class LocationForecast
	{
		[Key]
		public string LocationKey { get; set; }
		public string LocationName { get; set; }
		public DateTime ForecastEffectiveDateUTC { get; set; }
		public string ForecastDataJson { get; set; }
    }
}
