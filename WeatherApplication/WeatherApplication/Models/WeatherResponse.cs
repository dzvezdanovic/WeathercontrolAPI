namespace WeatherApplication.Models
{
    public class WeatherResponse
    {
        public string City { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public DateTime Date { get; set; }
    }
}
