using Microsoft.AspNetCore.Mvc;
using WeatherApplication.Services.Interfaces;

namespace WeatherApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            _logger.LogInformation($"Received request for weather data in {city}");

            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            try
            {
                var weatherData = await _weatherService.GetWeatherForCityAsync(city);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching weather data: {ex.Message}");
                return BadRequest($"Error fetching weather data: {ex.Message}");
            }
        }

        [HttpGet("{city}/{date}")]
        public async Task<IActionResult> GetWeather(string city, DateTime date)
        {
            _logger.LogInformation($"Received request for weather data in {city}");

            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            try
            {
                var weatherData = await _weatherService.GetWeatherForCityAndTimeAsync(city, date);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching weather data: {ex.Message}");
                return BadRequest($"Error fetching weather data: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] string city, [FromQuery] DateTime? date = null)
        {
            _logger.LogInformation($"Received request for weather data in {city}");

            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            if (date.HasValue)
            {
                var weatherForSpecificDate = _weatherService.GetWeatherForCityAndTimeAsync(city, date.Value);
                return Ok(weatherForSpecificDate);
            }
            else
            {
                var currenetWeather = _weatherService.GetWeatherForCityAsync(city);
                return Ok(currenetWeather);
            }
        }
    }
}
