using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherApplication.Models;
using WeatherApplication.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherApplication.Services.Implementation
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;
        private readonly string _apiKey = "3e308bafc147f49c52b7bfee1c1c0e97";

        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<WeatherResponse>> GetWeatherForCityAndTimeAsync(string city, DateTime time)
        {
            var weatherResponses = new List<WeatherResponse>();

            _logger.LogInformation($"Fetching weather data for {city} and {time}");

            // Construct the URL for the 5-day forecast API
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            //response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // Parse the JSON response
            var forecastData = JsonConvert.DeserializeObject<dynamic>(responseBody);

            // Loop through the forecast list
            foreach (var item in forecastData["list"])
            {
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds((long)item["dt"]).DateTime;

                // Check if the date matches the specific day
                if (dateTime.Date == time.Date)
                {
                    // Extract weather details
                    var temp = item["main"]["temp"];
                    var weatherDescription = item["weather"][0]["description"];

                    Console.WriteLine($"Date: {dateTime}, Temperature: {temp}°C, Description: {weatherDescription}");

                    weatherResponses.Add(new WeatherResponse { City = city, Temperature = temp, Description = weatherDescription, Date = dateTime });
                }
            }

            _logger.LogInformation($"Successfully fetched weather data for {city} and {time}");

            return weatherResponses;
        }

        public async Task<WeatherResponse> GetWeatherForCityAsync(string city)
        {
            _logger.LogInformation($"Fetching weather data for {city}");

            //URL with city and apikey
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=en";

            //Get quest to OpenWeather Api
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to fetch weather data for {city}, Status Code: {response.StatusCode}");
                throw new Exception("Error fetching weather data");
            }

            var data = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(data))
            {
                var weatherData = JsonConvert.DeserializeObject<dynamic>(data);

                _logger.LogInformation($"Successfully fetched weather data for {city}");

                return new WeatherResponse
                {
                    City = city,
                    Description = weatherData.weather[0].description,
                    Temperature = weatherData.main.temp,
                    Date = DateTime.UtcNow
                };
            }
            else
            {
                return new WeatherResponse { City = "Unknown" };
            }
        }
    }
}
