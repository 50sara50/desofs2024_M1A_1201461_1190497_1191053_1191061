using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Register(NewUserDto newUser)
    {
        try
        {
            var user = await _authService.Register(newUser);
            _logger.LogInformation($"User {user.UserName} registered successfully.");
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
