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
                var response = await _weatherService.GetWeatherForCityAndTimeAsync(city, date.Value);

                if (!response.IsSuccess)
                {
                    return BadRequest(new ErrorModel
                    {
                        ErrorCode = response.ErrorCode,
                        ErrorMessage = response.ErrorMessage
                    });
                }

                return Ok(response.Result);
            }
            else
            {
                var response = await _weatherService.GetWeatherForCityAsync(city);

                if (!response.IsSuccess)
                {
                    return BadRequest(new ErrorModel
                    {
                        ErrorCode = response.ErrorCode,
                        ErrorMessage = response.ErrorMessage
                    });
                }

                return Ok(response.Result);
            }
        }
    }
}
