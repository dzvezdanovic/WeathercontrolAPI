using Microsoft.Extensions.Logging;
using Moq;
using WeatherApplication.Services.Interfaces;

namespace WeatherAppTests.Unit
{
    public class WeatherServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly IWeatherService _weatherService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IWeatherService> _logger;

        public WeatherServiceTests(IWeatherService weatherService)
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _logger = Mock.Of<ILogger<IWeatherService>>();
            _weatherService = weatherService;
        }
    }
}