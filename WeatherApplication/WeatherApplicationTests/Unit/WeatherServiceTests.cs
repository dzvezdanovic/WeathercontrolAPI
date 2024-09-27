using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApplication.Controllers;
using WeatherApplication.Models;
using WeatherApplication.Services.Implementation;
using WeatherApplication.Services.Interfaces;

namespace WeatherApplicationTests.Unit
{
    public class WeatherServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly WeatherController _weatherController;
        private readonly ILogger<WeatherController> _logger;

        public WeatherServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _weatherServiceMock = new Mock<IWeatherService>();
            _logger = Mock.Of<ILogger<WeatherController>>();
            _weatherController = new WeatherController(_weatherServiceMock.Object, _logger);
        }
    }
}