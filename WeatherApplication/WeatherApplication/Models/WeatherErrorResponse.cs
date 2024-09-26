namespace WeatherApplication.Models
{
    public class WeatherErrorResponse : AbstractResponse 
    { 
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
    }
}
