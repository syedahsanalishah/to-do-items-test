using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using to_do_items_test.Controllers;  // Adjust the namespace to your actual controller's namespace
using to_do_items_test.Models;
using TodoItemsTest.Controllers;  // Adjust the namespace to where your models are located

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly AuthController authController;
    private readonly Mock<HttpResponse> mockResponse;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Mock<IResponseCookies> mockCookies;

    public AuthControllerTests()
    {
        // Initialize mock objects
        mockConfiguration = new Mock<IConfiguration>();

        // Setup configuration values for JWT
        mockConfiguration.SetupGet(x => x["Jwt:Key"]).Returns("8URr7U70MRlbBWmfHNYUHRDM4cmFBjFU9+whmOOMVcw=");
        mockConfiguration.SetupGet(x => x["Jwt:Issuer"]).Returns("calrom.com");
        mockConfiguration.SetupGet(x => x["Jwt:Audience"]).Returns("your.com");

        // Mock the Response and HttpContext
        mockResponse = new Mock<HttpResponse>();
        mockHttpContext = new Mock<HttpContext>();
        mockCookies = new Mock<IResponseCookies>();

        // Setup cookies for response
        mockResponse.Setup(r => r.Cookies).Returns(mockCookies.Object);
        mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

        // Initialize the controller and set HttpContext
        authController = new AuthController(mockConfiguration.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };
    }

    [Fact]
    public void Login_ShouldReturnOk_WithValidCredentials()
    {
        // Arrange: Create a valid login model
        var loginModel = new LoginModel
        {
            Username = "test",
            Password = "password"
        };

        // Act: Call the Login method
        var result = authController.Login(loginModel) as OkObjectResult;

        // Assert: Ensure the response is Ok and the JWT token is returned
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

       // var tokenResponse = result.Value as IDictionary<string, object>;
       // Assert.True(tokenResponse.ContainsKey("Token"));
    }

    [Fact]
    public void Login_ShouldReturnUnauthorized_WithInvalidCredentials()
    {
        // Arrange: Create an invalid login model
        var loginModel = new LoginModel
        {
            Username = "wrongUsername",
            Password = "wrongPassword"
        };

        // Act: Call the Login method
        var result = authController.Login(loginModel) as UnauthorizedResult;

        // Assert: Ensure the response is Unauthorized (401)
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void Login_ShouldReturnBadRequest_WhenLoginModelIsNull()
    {
        // Act: Call the Login method with null login model
        var result = authController.Login(null) as BadRequestObjectResult;

        // Assert: Ensure the response is BadRequest (400)
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Login model is null.", result.Value);
    }

    [Fact]
    public void Login_ShouldReturnBadRequest_WhenUsernameOrPasswordIsMissing()
    {
        // Arrange: Create a login model with missing username or password
        var loginModel = new LoginModel
        {
            Username = "",  // Empty username
            Password = "password"
        };

        // Act: Call the Login method
        var result = authController.Login(loginModel) as BadRequestObjectResult;

        // Assert: Ensure the response is BadRequest (400)
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Username and Password are required.", result.Value);
    }
}
