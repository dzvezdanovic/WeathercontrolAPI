using WeatherApplication.Services.Interfaces;

namespace WeatherApplication.Models
{
    public abstract class Messages<T>
    {
        public T SpecificMessage { get; set; }
        public bool isSuccess { get; set; }
    }

    public class ErrorMessage : Messages<ErrorModel>
    {
        public ErrorMessage(ErrorModel error)
        {
            SpecificMessage = error;
            isSuccess = false;
        }
    }

    public class SuccessMessage : Messages<WeatherModel>
    {
        public SuccessMessage(WeatherModel success)
        {
            SpecificMessage = success;
            isSuccess = true;
        }
    }
}
