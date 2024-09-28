using Newtonsoft.Json;
using System.Net;
using WeatherApplication.Models;
using WeatherApplication.Services.Interfaces;

namespace WeatherApplication.Services.Implementation
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["WeatherAPI:ApiKey"];
        }

        public async Task<ResultMessage<List<WeatherModel>>> GetWeatherForCityAndTimeAsync(string city, DateTime time)
        {
            _logger.LogInformation($"Fetching weather data for {city} and {time}");

            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new ResultMessage<List<WeatherModel>>
                {
                    IsSuccess = false,
                    ErrorCode = response.StatusCode.ToString(),
                    ErrorMessage = $"Could not found information for {city}!"
                };
            }

            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                var forecastData = JsonConvert.DeserializeObject<dynamic>(responseBody);
                var cityFromApi = forecastData.city.name;

                var weatherList = new List<WeatherModel>();

                foreach (var item in forecastData["list"])
                {
                    DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds((long)item["dt"]).DateTime;

                    if (dateTime.Date == time.Date)
                    {
                        var temp = item["main"]["temp"];
                        var weatherDescription = item["weather"][0]["description"];

                        var weather = new WeatherModel { City = cityFromApi, Date = dateTime, Temperature = temp, Description = weatherDescription };
                        weatherList.Add(weather);
                    }
                }

                _logger.LogInformation($"Successfully fetched weather data for {city} and {time}");

                return new ResultMessage<List<WeatherModel>>
                {
                    IsSuccess = true,
                    Result = weatherList
                };
            }
        }

        public async Task<ResultMessage<WeatherModel>> GetWeatherForCityAsync(string city)
        {
            _logger.LogInformation($"Fetching weather data for {city}");
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(data))
            {
                return new ResultMessage<WeatherModel>
                {
                    IsSuccess = false,
                    ErrorCode = response.StatusCode.ToString(),
                    ErrorMessage = $"Could not found information for {city}!"
                };
            }
            else
            {
                var weatherData = JsonConvert.DeserializeObject<dynamic>(data);

                _logger.LogInformation($"Successfully fetched weather data for {city}");

                var weather = new WeatherModel
                {
                    City = weatherData.name,
                    Description = weatherData.weather[0].description,
                    Temperature = weatherData.main.temp,
                    Date = DateTime.UtcNow
                };

                return new ResultMessage<WeatherModel>
                {
                    IsSuccess = true,
                    Result = weather
                };
            }
        }
    }
}
