using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Dtos.Responses;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace StreamingPlatform;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    private async Task CreateRole(Role role)
    {
        if (!await _roleManager.RoleExistsAsync(role.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
        }
    }

    private JwtSecurityToken GenerateJwtToken(User user)
    {
        var userClaims = _userManager.GetClaimsAsync(user).Result;

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.Union(userClaims);
        
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        
        return new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signinCredentials);
    }

    public async Task<GenericResponseDto> Register(NewUserContract newUser)
    {
        var userExists = await _userManager.FindByEmailAsync(newUser.Email);
        if (userExists != null)
        {
            throw new Exception("User already exists!");
        }

        var user = new User
        {
            Email = newUser.Email,
            UserName = newUser.UserName,
            Name = newUser.Name,
            Age = newUser.Age,
            Address = newUser.Address,
        };

        var result = await _userManager.CreateAsync(user, newUser.Password);
        if (!result.Succeeded)
        {
            throw new Exception("User creation failed! Please check user details and try again.");
        }
        
        await CreateRole(newUser.Role);
        var roleResult = await _userManager.AddToRoleAsync(user, newUser.Role.ToString());
        if (!roleResult.Succeeded)
        {
            throw new Exception("Failed to assign role to user.");
        }

        var token = GenerateJwtToken(user);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new GenericResponseDto($"User created successfully!");
    }

    public async Task<TokenResponseDto> Login(UserLoginContract userContract)
    {
        var user = await _userManager.FindByEmailAsync(userContract.Email);
        if (user == null)
        {
            throw new Exception("User does not exist");
        }

        if (!await _userManager.CheckPasswordAsync(user, userContract.Password))
        {
            throw new Exception("Wrong Passsword");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id)
        };
        
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Issuer"],
            expires: DateTime.Now.AddHours(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        //var token = GenerateJwtToken(user);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResponseDto(tokenString, token.ValidTo);
    }
}