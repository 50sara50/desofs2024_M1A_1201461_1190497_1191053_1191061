using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Dtos.Contract;

namespace StreamingPlatform;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(NewUserContract newUser)
    {
        try
        {
            var registerMessage = await _authService.Register(newUser);
            _logger.LogInformation($"User {newUser.UserName} registered successfully.");
            return Ok(registerMessage);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginContract user)
    {
        try
        {
            var userToken = await _authService.Login(user);
            _logger.LogInformation($"User {user.Email} logged in successfully");
            return Ok(userToken);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
