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

        public async Task<List<Messages<T>>> GetWeatherForCityAndTimeAsync<T>(string city, DateTime time)
        {
            var weatherResponses = new List<Messages<T>>();

            _logger.LogInformation($"Fetching weather data for {city} and {time}");

            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = new ErrorModel { ErrorCode = (int)response.StatusCode, ErrorMessage = $"Could not found information for {city}!" };
                weatherResponses.Add(new ErrorMessage(error) as Messages<T>);

                return weatherResponses;
            }

            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                var forecastData = JsonConvert.DeserializeObject<dynamic>(responseBody);
                var cityFromApi = forecastData.city.name;

                foreach (var item in forecastData["list"])
                {
                    DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds((long)item["dt"]).DateTime;

                    if (dateTime.Date == time.Date)
                    {
                        var temp = item["main"]["temp"];
                        var weatherDescription = item["weather"][0]["description"];

                        var success = new SuccessModel { City = cityFromApi, Date = dateTime, Temperature = temp, Description = weatherDescription };
                        weatherResponses.Add(new SuccessMessage(success) as Messages<T>);
                    }
                }

                _logger.LogInformation($"Successfully fetched weather data for {city} and {time}");

                return weatherResponses;
            }
        }

        public async Task<List<Messages<T>>> GetWeatherForCityAsync<T>(string city)
        {
            _logger.LogInformation($"Fetching weather data for {city}");
            var weatherResponses = new List<Messages<T>>();
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(data))
            {
                var error = new ErrorModel { ErrorCode = (int)response.StatusCode, ErrorMessage = $"Could not found information for {city}!" };
                weatherResponses.Add(new ErrorMessage(error) as Messages<T>);
                return weatherResponses;
            }
            else
            {
                var weatherData = JsonConvert.DeserializeObject<dynamic>(data);

                _logger.LogInformation($"Successfully fetched weather data for {city}");

                var success = new SuccessModel
                {
                    City = weatherData.name,
                    Description = weatherData.weather[0].description,
                    Temperature = weatherData.main.temp,
                    Date = DateTime.UtcNow
                };
                weatherResponses.Add(new SuccessMessage(success) as Messages<T>);

                return weatherResponses;
            }
        }
    }
}
