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

        public WeatherServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _weatherServiceMock = new Mock<IWeatherService>();
            ILogger<WeatherController> logger = Mock.Of<ILogger<WeatherController>>();
            _weatherController = new WeatherController(_weatherServiceMock.Object, logger);
        }

        [Fact]
        public async Task GetWeather_Returns200_WhenCityAreValid()
        {
            // Arrange
            var city = "Belgrade";
            var mockWeatherData = new WeatherResponse
            {
                City = city,
                Temperature = 22.3,
                Description = "Clear sky"
            };

            // Mock the weather service to return the desired result
            _weatherServiceMock.Setup(service => service.GetWeatherForCityAsync(city))
                .ReturnsAsync(mockWeatherData);

            // Act
            var result = await _weatherController.GetWeather(city);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            // Optionally check the returned value
            var weatherData = Assert.IsType<WeatherResponse>(okResult.Value);
            Assert.Equal("Belgrade", weatherData.City);
        }

        [Fact]
        public async Task GetWeather_Returns200_WhenCityAndDateAreValid()
        {
            // Arrange
            var city = "Belgrade";
            var expectedDate = new DateTime(2024, 09, 26);
            var mockWeatherData = new WeatherResponse
            {
                City = city,
                Temperature = 22.3,
                Description = "Clear sky",
                Date = new DateTime(2024, 09, 26)
            };

            // Mock the weather service to return the desired result
            _weatherServiceMock.Setup(service => service.GetWeatherForCityAsync(city))
                .ReturnsAsync(mockWeatherData);

            // Act
            var result = await _weatherController.GetWeather(city);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            // Optionally check the returned value
            var weatherData = Assert.IsType<WeatherResponse>(okResult.Value);
            Assert.Equal("Belgrade", weatherData.City);
            Assert.Equal(expectedDate, weatherData.Date);
        }
    }
}