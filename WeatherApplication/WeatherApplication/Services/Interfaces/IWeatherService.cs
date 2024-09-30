using WeatherApplication.Models;

namespace WeatherApplication.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<ResultMessage<List<SuccessModel>>> GetWeatherForCityAndTimeAsync(string city, DateTime time);
        Task<ResultMessage<SuccessModel>> GetWeatherForCityAsync(string city);
    }
}
