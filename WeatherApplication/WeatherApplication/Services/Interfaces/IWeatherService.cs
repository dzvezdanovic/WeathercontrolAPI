using WeatherApplication.Models;

namespace WeatherApplication.Services.Interfaces
{
    public interface IWeatherService
    {
        public Task<WeatherResponse> GetWeatherForCityAsync(string city);
        public Task<List<WeatherResponse>> GetWeatherForCityAndTimeAsync(string city, DateTime time);
    }
}
