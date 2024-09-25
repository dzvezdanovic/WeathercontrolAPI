using Microsoft.AspNetCore.Mvc;
using WeatherApplication.Services.Interfaces;
using WeatherApplication.ViewModels;

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

        /// <summary>
        /// This is method for UI
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Index(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return View(new WeatherViewModel());
            }

            var weatherData = await _weatherService.GetWeatherForCityAsync(city);
            var viewModel = new WeatherViewModel
            {
                City = city,
                Description = weatherData.Description,
                Temperature = weatherData.Temperature,
                Date = DateTime.Now
            };

            return View(viewModel);
        }
    }
}
