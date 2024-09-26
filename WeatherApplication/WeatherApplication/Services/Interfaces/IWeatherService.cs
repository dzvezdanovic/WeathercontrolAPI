using WeatherApplication.Models;

namespace WeatherApplication.Services.Interfaces
{
    public interface IWeatherService
    {
        public Task<AbstractResponse> GetWeatherForCityAsync(string city);
        public Task<List<AbstractResponse>> GetWeatherForCityAndTimeAsync(string city, DateTime time);
    }
}
