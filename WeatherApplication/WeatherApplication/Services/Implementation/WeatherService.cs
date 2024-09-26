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
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["WeatherAPI:ApiKey"];
        }

        public async Task<List<WeatherResponse>> GetWeatherForCityAndTimeAsync(string city, DateTime time)
        {
            var weatherResponses = new List<WeatherResponse>();

            _logger.LogInformation($"Fetching weather data for {city} and {time}");

            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric&lang=en";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var forecastData = JsonConvert.DeserializeObject<dynamic>(responseBody);
            var cityFromApi = forecastData.city.name;

            foreach (var item in forecastData["list"])
            {
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds((long)item["dt"]).DateTime;

                if (dateTime.Date == time.Date)
                {
                    // Extract weather details
                    var temp = item["main"]["temp"];
                    var weatherDescription = item["weather"][0]["description"];

                    Console.WriteLine($"Date: {dateTime}, Temperature: {temp}°C, Description: {weatherDescription}");

                    weatherResponses.Add(new WeatherResponse { City = cityFromApi, Temperature = temp, Description = weatherDescription, Date = dateTime });
                }
            }

            _logger.LogInformation($"Successfully fetched weather data for {city} and {time}");

            return weatherResponses;
        }

        public async Task<WeatherResponse> GetWeatherForCityAsync(string city)
        {
            try
            {
                _logger.LogInformation($"Fetching weather data for {city}");

                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=en";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

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
                        City = weatherData.name,
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
            catch(TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled, possibly due to a timeout.");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"{ex}");
                return new WeatherResponse { City = "Unknown" };
            }
        }
    }
}
