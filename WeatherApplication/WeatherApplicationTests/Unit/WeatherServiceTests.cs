using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using WeatherApplication.Controllers;
using WeatherApplication.Models;
using WeatherApplication.Services.Implementation;
using WeatherApplication.Services.Interfaces;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

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

        [Fact]
        public async Task SuccessFirstMethod()
        {
            // Arrange
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

            // Act
            var result = await _weatherServiceMock.Object.GetWeatherForCityAsync<SuccessModel>(city);

            // Assert
            Assert.Single(result);
            var successMessage = Assert.IsType<SuccessMessage>(result.First());
            Assert.Equal("Belgrade", successMessage.SpecificMessage.City);
            Assert.Equal("clear sky", successMessage.SpecificMessage.Description);
            Assert.Equal(25.5, successMessage.SpecificMessage.Temperature);
        }

        [Fact]
        public async Task BadRequestStatusCode()
        {
            // Arrange
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

            // Act
            var result = await _weatherServiceMock.Object.GetWeatherForCityAsync<ErrorModel>(city);

            // Assert
            Assert.Single(result);
            var errorMessage = Assert.IsType<ErrorMessage>(result.First());
            Assert.Equal(404, errorMessage.SpecificMessage.ErrorCode);
            Assert.Equal($"Could not found information for {city}!", errorMessage.SpecificMessage.ErrorMessage);
        }
    }
}