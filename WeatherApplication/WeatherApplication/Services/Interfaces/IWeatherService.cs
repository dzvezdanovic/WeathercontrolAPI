using WeatherApplication.Models;

namespace WeatherApplication.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<ResultMessage<List<WeatherModel>>> GetWeatherForCityAndTimeAsync(string city, DateTime time);
        Task<ResultMessage<WeatherModel>> GetWeatherForCityAsync(string city);
    }
}
