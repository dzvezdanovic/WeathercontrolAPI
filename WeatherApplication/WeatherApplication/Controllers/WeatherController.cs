using Microsoft.AspNetCore.Mvc;
using WeatherApplication.Models;
using WeatherApplication.Services.Interfaces;

namespace WeatherApplication.Controllers
{
    [ApiController]
    [Route("api/{city}")]
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherController> _logger;

        public List<Messages<SuccessModel>> successData = new();
        public List<Messages<ErrorModel>> errorData = new();

        public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromRoute] string city, [FromQuery] DateTime? date = null)
        {
            _logger.LogInformation($"Received request for weather data in {city}");

            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            if (date.HasValue)
            {
                successData = await _weatherService.GetWeatherForCityAndTimeAsync<SuccessModel>(city, date.Value);

                if (successData.FirstOrDefault() == null)
                {
                    errorData = await _weatherService.GetWeatherForCityAndTimeAsync<ErrorModel>(city, date.Value);
                    return BadRequest(errorData);
                }

                return Ok(successData);

            }
            else
            {
                successData = await _weatherService.GetWeatherForCityAsync<SuccessModel>(city);

                if (successData.FirstOrDefault() == null)
                {
                    errorData = await _weatherService.GetWeatherForCityAsync<ErrorModel>(city);
                    return BadRequest(errorData);
                }

                return Ok(successData);
            }
        }
    }
}
