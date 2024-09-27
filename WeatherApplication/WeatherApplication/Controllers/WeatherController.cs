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

            List<Messages<SuccessModel>> successData = new();
            List<Messages<ErrorModel>> errorData = new();

            if (date.HasValue)
            {
                successData = await _weatherService.GetWeatherForCityAndTimeAsync<SuccessModel>(city, date.Value);

                for(int i = 0; i < successData.Count; ++i)
                {
                    if (successData[i] == null) 
                    {
                        errorData = await _weatherService.GetWeatherForCityAndTimeAsync<ErrorModel>(city, date.Value);
                        return BadRequest(errorData);
                    }
                    else return Ok(successData);
                }
            }
            else
            {
                successData = await _weatherService.GetWeatherForCityAsync<SuccessModel>(city);
                if (successData[0] != null) return Ok(successData);
                else
                {
                    errorData = await _weatherService.GetWeatherForCityAsync<ErrorModel>(city);
                    return BadRequest(errorData);
                }
            }
            return NoContent();
        }
    }
}
