using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    [EnableRateLimiting("fixed-by-user-id-or-ip")]
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
    [EnableRateLimiting("fixed-by-user-id-or-ip")]

    public async Task<IActionResult> Login(UserLoginContract user)
    {
        try
        {
            var userToken = await this._authService.Login(user);
            this._logger.LogInformation($"User {user.Email} logged in successfully");
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = userToken.expirationDate,
                SameSite = SameSiteMode.None,
                Path = "/",

            };
            this.Response.Cookies.Append("__Host-userBearerToken", userToken.token, cookieOptions);
            this.Response.Cookies.Append("__Host-expiresAt", userToken.expirationDate.ToString(), cookieOptions);
            return this.Ok(userToken);
        }
        catch (ServiceBaseException ex)
        {
            ErrorResponseObject errorResponseObject = MapResponse.BadRequest(ex.Message);
            return this.BadRequest(errorResponseObject);
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> ChangePassword(ChangePasswordContract changePasswordContract)
    {
        try
        {
            var result = await this._authService.ChangePassword(changePasswordContract);
            return this.Ok(result);
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

    [HttpPost("logout")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public IActionResult Logout()
    {
        CookieOptions cookieOptions = new()
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
        };

        this.Response.Cookies.Append("__Host-userBearerToken", string.Empty, cookieOptions);
        this.Response.Cookies.Append("__Host-expiresAt", string.Empty, cookieOptions);

        return this.Ok();
    }

    [HttpGet]
    [Route("status")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult GetAuthStatus()
    {
       bool isAuthenticated = this.User.Identity?.IsAuthenticated ?? false;
       return this.Ok(new { isAuthenticated });
    }
}
