using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Services.Exceptions;

namespace StreamingPlatform;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        this._logger = logger;
        this._authService = authService;
    }

    [HttpPost("register")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(NewUserContract newUser)
    {
        try
        {
            var registerMessage = await this._authService.Register(newUser);
            this._logger.LogInformation($"User {newUser.UserName} registered successfully.");
            return this.Ok(registerMessage);
        }
        catch (ServiceBaseException ex)
        {
            ErrorResponseObject errorResponseObject = MapResponse.BadRequest(ex.Message);
            return this.BadRequest(errorResponseObject);
        }
    }

    [HttpPost("login")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(UserLoginContract user)
    {
        try
        {
            var userToken = await this._authService.Login(user);
            this._logger.LogInformation($"User {user.Email} logged in successfully");
            return this.Ok(userToken);
        }
        catch (ServiceBaseException ex)
        {
            ErrorResponseObject errorResponseObject = MapResponse.BadRequest(ex.Message);
            return this.BadRequest(errorResponseObject);
        }
    }

    [HttpGet("check-password")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckPassword(string password)
    {
        try
        {
            var result = await this._authService.PasswordBreached(password);
            return this.Ok(result);
        }
        catch (ServiceBaseException ex)
        {
            ErrorResponseObject errorResponseObject = MapResponse.BadRequest(ex.Message);
            return this.BadRequest(errorResponseObject);
        }
    }
}
