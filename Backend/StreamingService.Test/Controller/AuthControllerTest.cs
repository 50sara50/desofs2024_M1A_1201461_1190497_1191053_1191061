using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StreamingPlatform;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;

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
            UserName = "testuser",
            Email = "testuserdesofs@email.com",
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
            UserName = "testuser",
            Email = "testuserdesofs@email.com",
            Password = "Teste"
        };

        _authServiceMock.Setup(x => x.Register(newUser)).Throws<ValidationException>();
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        var result = await controller.Register(newUser);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)result).StatusCode);
    }

    [TestMethod]
    public async Task ShouldInformMandatoryPassword()
    {
        // Arrange
        var newUser = new NewUserContract
        {
            UserName = "testuser",
            Email = "testuserdesofs@email.com"
        };

        _authServiceMock.Setup(x => x.Register(newUser)).Throws<ValidationException>();
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        var result = await controller.Register(newUser);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)result).StatusCode);
    }

    [TestMethod]
    public async Task ShouldLoginSuccessfully()
    {
        // Arrange
        var user = new UserLoginContract()
        {
            Email = "testuserdesofs@email.com",
            Password = "Teste@123.Teste123"
        };

        var response = new TokenResponseDto("token", DateTime.Now.AddHours(1));

        _authServiceMock.Setup(x => x.Login(user))
            .Returns(Task.FromResult(response));
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        var result = await controller.Login(user);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
        Assert.AreEqual(response, ((OkObjectResult)result).Value);
    }

    [TestMethod]
    public async Task ShouldFailLogin()
    {
        // Arrange
        var user = new UserLoginContract()
        {
            Email = "testuserdesofs@email.com",
            Password = "Teste@123.Teste123"
        };
        
        _authServiceMock.Setup(x => x.Login(user))
            .Throws<Exception>();
        var controller = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        var result = await controller.Login(user);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)result).StatusCode);
    }
}