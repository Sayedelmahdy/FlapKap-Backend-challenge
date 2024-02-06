using BLL.DTOs;
using BLL.Interfaces;
using BLL.Services;
using FlapKap_TechnicalChallenge.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace UnitTestCase.Test.TestController
{
    public class AuthenticationControllerTest
    {
        AuthenticationController _authenticationController;
       private Mock<IAuthService> _authServiceMock;
        public AuthenticationControllerTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authenticationController = new AuthenticationController(_authServiceMock.Object);
        }
        [Fact]
        public async Task RegisterAsync_ValidModel_ReturnsOkResult()
        {
           
            var validModel = new RegisterDto
            {
               Username="SayedAhmed",
               Email="sayed@gmail.com",
              Password="Sayed@#1234",
              PhoneNumber="01093307397",
                Role = new List<string> { "Seller"},
              
            };

            var expectedResult = new UserDetailDto
            {
               
                Message = "User Created Successfully",
                Email = "sayed@gmail.com", 
                PhoneNumber = "01093307397",
                Role = new List<string> { "Seller" },
                UserName= "SayedAhmed"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticationController.RegisterAsync(validModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualResult = Assert.IsType<UserDetailDto>(okResult.Value);

            Assert.Equal(expectedResult.Message, actualResult.Message);
            
        }
        [Fact]
        public async Task RegisterAsync_InvalidModel_ReturnsBadRequest()
        {
         
            var invalidModel = new RegisterDto
            {
                Username = "Ahmed",
                Email = "Ahmed@gmail.com",
                Password = "ahmed",
                PhoneNumber = "01147582122",
                Role = new List<string> { "Seller" },
            };

            _authenticationController.ModelState.AddModelError("PropertyName", "Error Message");

            // Act
            var result = await _authenticationController.RegisterAsync(invalidModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);

           
        }
       /* [Fact]
        public async Task LoginAsync_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var validModel = new TokenRequestDto { *//* Initialize with valid data *//* };
            var expectedResult = new AuthenticationDetailDto { *//* Set expected result *//* };
            _authServiceMock.Setup(x => x.GetTokenAsync(validModel)).ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticationController.LoginAsync(validModel) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().Be(expectedResult);
        }

        [Fact]
        public async Task LoginAsync_InvalidModel_ReturnsBadRequestWithErrorMessage()
        {
            // Arrange
            var invalidModel = new TokenRequestDto { *//* Initialize with invalid data *//* };
            _authenticationController.ModelState.AddModelError("PropertyName", "Error Message");

            // Act
            var result = await _authenticationController.LoginAsync(invalidModel) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            result.Value.Should().BeOfType<string>().And.NotBeNull();
        }*/
    }
}
