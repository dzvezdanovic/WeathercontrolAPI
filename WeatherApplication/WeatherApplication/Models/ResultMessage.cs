using WeatherApplication.Services.Interfaces;

namespace WeatherApplication.Models
{

    public class ResultMessage<T>
    {
        public T? Result { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}
