using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StreamingPlatform;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Services.Exceptions;

namespace StreamingService.Test.Controller;

[TestClass]
public sealed class AuthControllerTest
{
    private Mock<ILogger<AuthController>> _loggerMock;
    private Mock<IAuthService> _authServiceMock;
    private ControllerContext _controllerContext;


    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<AuthController>>();
        _authServiceMock = new Mock<IAuthService>();
        _controllerContext = new ControllerContext();
    }

    [TestMethod]
    public async Task ShouldRegisterUser()
    {
        // Arrange
        var newUser = new NewUserContract
        {
            UserName = Faker.Name.FullName(),
            Email = Faker.Internet.Email(),
            Password = "Teste@123.Teste123"
        };

        var response = new GenericResponseDto("User registered successfully");

        _authServiceMock.Setup(x => x.Register(newUser)).Returns(Task.FromResult(response));
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        var result = await controller.Register(newUser);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
        Assert.AreEqual(response, ((OkObjectResult)result).Value);
    }

    [TestMethod]
    public async Task ShouldInformTooShortPassword()
    {
        // Arrange
        var newUser = new NewUserContract
        {
            UserName = Faker.Internet.UserName(),
            Email = Faker.Internet.Email(),
            Password = "Teste"
        };

        _authServiceMock.Setup(x => x.Register(newUser)).Throws<ValidationException>();
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        await Assert.ThrowsExceptionAsync<ValidationException>(() => controller.Register(newUser));
    }

    [TestMethod]
    public async Task ShouldInformMandatoryPassword()
    {
        // Arrange
        var newUser = new NewUserContract
        {
            UserName = Faker.Internet.UserName(),
            Email = Faker.Internet.Email(),
        };

        _authServiceMock.Setup(x => x.Register(newUser)).Throws<ValidationException>();
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        await Assert.ThrowsExceptionAsync<ValidationException>(() => controller.Register(newUser));
    }

    [TestMethod]
    public async Task ShouldLoginSuccessfully()
    {

        Mock<HttpContext> _httpContextMock = new();
        Mock<HttpResponse> _httpResponseMock = new();
        _httpContextMock.Setup(c => c.Response).Returns(_httpResponseMock.Object);
        _httpResponseMock.Setup(r => r.Cookies).Returns(new Mock<IResponseCookies>().Object);


        // Arrange
        var user = new UserLoginContract()
        {
            Email = Faker.Internet.Email(),
            Password = "Teste@123.Teste123"
        };

        var response = new TokenResponseDto("token", DateTime.Now.AddHours(1));

        _authServiceMock.Setup(x => x.Login(user))
            .Returns(Task.FromResult(response));

        ControllerContext controllerContext = new()
        {
            HttpContext = _httpContextMock.Object
        };
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object)
        {
            ControllerContext = controllerContext
        };
        var result = await controller.Login(user);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
        Assert.AreEqual(response, ((OkObjectResult)result).Value);
        _httpResponseMock.Verify(r => r.Cookies.Append(
           "__Host-userBearerToken",
           response.token,
           It.Is<CookieOptions>(c => c.HttpOnly && c.Secure && c.IsEssential && c.Expires == response.expirationDate && c.SameSite == SameSiteMode.None && c.Path == "/")
       ), Times.Once);

        _httpResponseMock.Verify(r => r.Cookies.Append(
            "__Host-expiresAt",
            response.expirationDate.ToString(),
            It.Is<CookieOptions>(c => c.HttpOnly && c.Secure && c.IsEssential && c.Expires == response.expirationDate && c.SameSite == SameSiteMode.None && c.Path == "/")
        ), Times.Once);
    }
}