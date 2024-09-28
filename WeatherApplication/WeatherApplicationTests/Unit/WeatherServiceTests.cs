using Microsoft.Extensions.Logging;
using Moq;
using WeatherApplication.Controllers;
using WeatherApplication.Models;
using WeatherApplication.Services.Interfaces;

namespace WeatherApplicationTests.Unit
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly ILogger<WeatherController> _logger;

        public WeatherServiceTests()
        {
            _weatherServiceMock = new Mock<IWeatherService>();
            _logger = Mock.Of<ILogger<WeatherController>>();
        }

        [Fact]
        public async Task SuccessFirstMethod()
        {
            var city = "Belgrade";
            var expectedResponse = new ResultMessage<WeatherModel>
            {
                IsSuccess = true,
                Result = new WeatherModel
                {
                    City = "Belgrade",
                    Description = "clear sky",
                    Temperature = 25.5
                }
            };

            _weatherServiceMock
                .Setup(s => s.GetWeatherForCityAsync(city))
                .ReturnsAsync(expectedResponse);

            var response = await _weatherServiceMock.Object.GetWeatherForCityAsync(city);

            Assert.True(response.IsSuccess);
            Assert.Equal("Belgrade", response.Result?.City);
            Assert.Equal("clear sky", response.Result?.Description);
            Assert.Equal(25.5, response.Result?.Temperature);
        }

        [Fact]
        public async Task BadRequestStatusCode()
        {
            var city = "NonExistentCity";
            var expectedErrorResponse = new ResultMessage<WeatherModel>
            {
                IsSuccess = false,
                ErrorCode = "NotFound",
                ErrorMessage = $"Could not found information for {city}!"
            };

            _weatherServiceMock
                .Setup(s => s.GetWeatherForCityAsync(city))
                .ReturnsAsync(expectedErrorResponse);

            var response = await _weatherServiceMock.Object.GetWeatherForCityAsync(city);

            Assert.False(response.IsSuccess);
            Assert.Equal("NotFound", response.ErrorCode);
            Assert.Equal($"Could not found information for {city}!", response.ErrorMessage);
        }
    }
}