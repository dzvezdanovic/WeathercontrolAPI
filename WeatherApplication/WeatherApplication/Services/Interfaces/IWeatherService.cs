using WeatherApplication.Models;

namespace WeatherApplication.Services.Interfaces
{
    public interface IWeatherService
    {
        public Task<List<Messages<T>>> GetWeatherForCityAsync<T>(string city);
        public Task<List<Messages<T>>> GetWeatherForCityAndTimeAsync<T>(string city, DateTime time);
    }
}
