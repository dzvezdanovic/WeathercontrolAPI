using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using WeatherApplication.Models;
using WeatherApplication.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<List<AbstractResponse>> GetWeatherForCityAndTimeAsync(string city, DateTime time)
        {
            var weatherResponses = new List<WeatherResponse>();

            _logger.LogInformation($"Fetching weather data for {city} and {time}");

            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return [new WeatherErrorResponse { ErrorMessage = $"{city} doesn't exists in our DB!", ErrorCode = (int)response.StatusCode}];
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

                        weatherResponses.Add(new WeatherResponse { City = cityFromApi, Temperature = temp, Description = weatherDescription, Date = dateTime });
                    }
                }

                _logger.LogInformation($"Successfully fetched weather data for {city} and {time}");

                return weatherResponses.Cast<AbstractResponse>().ToList();
            }
        }

        public async Task<AbstractResponse> GetWeatherForCityAsync(string city)
        {
            _logger.LogInformation($"Fetching weather data for {city}");

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(data))
            {
                return new WeatherErrorResponse() 
                { 
                    ErrorCode = (int)response.StatusCode,
                    ErrorMessage = $"{city} doesn't exists in our DB!"
                };
            }
            else
            {
                var weatherData = JsonConvert.DeserializeObject<dynamic>(data);

                _logger.LogInformation($"Successfully fetched weather data for {city}");

                return new WeatherResponse
                {
                    City = weatherData.name,
                    Description = weatherData.weather[0].description,
                    Temperature = weatherData.main.temp,
                    Date = DateTime.UtcNow
                };

            }
        }
    }
}
