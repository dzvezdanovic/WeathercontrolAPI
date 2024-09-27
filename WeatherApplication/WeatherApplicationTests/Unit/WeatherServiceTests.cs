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
            var expectedResponse = new List<Messages<SuccessModel>>
            {
                new SuccessMessage(new SuccessModel
                {
                    City = "Belgrade",
                    Description = "clear sky",
                    Temperature = 25.5
                })
            };

            _weatherServiceMock
                .Setup(s => s.GetWeatherForCityAsync<SuccessModel>(city))
                .ReturnsAsync(expectedResponse);

            var result = await _weatherServiceMock.Object.GetWeatherForCityAsync<SuccessModel>(city);

            Assert.Single(result);
            var successMessage = Assert.IsType<SuccessMessage>(result.First());
            Assert.Equal("Belgrade", successMessage.SpecificMessage.City);
            Assert.Equal("clear sky", successMessage.SpecificMessage.Description);
            Assert.Equal(25.5, successMessage.SpecificMessage.Temperature);
        }

        [Fact]
        public async Task BadRequestStatusCode()
        {
            var city = "NonExistentCity";
            var expectedErrorResponse = new List<Messages<ErrorModel>>
            {
                new ErrorMessage(new ErrorModel
                {
                    ErrorCode = 404,
                    ErrorMessage = $"Could not found information for {city}!"
                })
            };

            _weatherServiceMock
                .Setup(s => s.GetWeatherForCityAsync<ErrorModel>(city))
                .ReturnsAsync(expectedErrorResponse);

            var result = await _weatherServiceMock.Object.GetWeatherForCityAsync<ErrorModel>(city);

            Assert.Single(result);
            var errorMessage = Assert.IsType<ErrorMessage>(result.First());
            Assert.Equal(404, errorMessage.SpecificMessage.ErrorCode);
            Assert.Equal($"Could not found information for {city}!", errorMessage.SpecificMessage.ErrorMessage);
        }
    }
}