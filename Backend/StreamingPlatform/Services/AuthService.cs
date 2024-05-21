using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Services.Exceptions;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace StreamingPlatform;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IUnitOfWork unitOfWork;

    public AuthService(IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        this.unitOfWork = unitOfWork;
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

    /// <summary>
    /// Checks if password has been breached.
    /// Even tho password is hashed with sha1, this algorithm is not 100% secure to brute force attacks.
    /// So, to ensure anonymity, we use a k-anonymity approach to check if the password has been breached,
    /// we send the first 5 characters of the hashed password to the pwnedpasswords API.
    /// The API returns a list of all hashed passwords that start with the same 5 characters and
    /// we then check if the password is in the list.
    /// </summary>
    /// <param name="password">Password we want to check</param>
    /// <returns>Confirmation if the password has been breached</returns>
    /// <exception cref="ServiceBaseException"></exception>
    public async Task<GenericResponseDto> PasswordBreached(string password)
    {
        var sha1 = string.Empty;
        byte[] inputBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = SHA1.HashData(inputBytes);

        // Convert byte array to a hex string
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("X2")); // X2 formats the byte as a hexadecimal string
        }

        sha1 = sb.ToString();
        sha1 = sha1.ToUpper();

        try
        {
            var url = $"https://api.pwnedpasswords.com/range/{sha1.Substring(0, 5)}";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var lines = content.Split("\n");
                    foreach (var line in lines)
                    {
                        var parts = line.Split(":");
                        var suffix = parts[0];
                        var hash = sha1.Substring(0, 5) + suffix;
                        if (sha1.Equals(hash, StringComparison.OrdinalIgnoreCase))
                        {
                            return new GenericResponseDto("Password has been breached. Maybe choose a different password.");
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new ServiceBaseException("Failed to check password. Please try again later.");
        }

        return new GenericResponseDto("Password is safe to use.");
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

        IGenericRepository<User> userRepository = this.unitOfWork.Repository<User>();
        userRepository.Create(user);
        await this.unitOfWork.SaveChangesAsync();
        var token = GenerateJwtToken(user);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new GenericResponseDto($"User created successfully!");
    }

    public async Task<TokenResponseDto> Login(UserLoginContract userContract)
    {
        var user = await _userManager.FindByEmailAsync(userContract.Email);
        if (user == null)
        {
            throw new ServiceBaseException("User does not exist");
        }

        if (!await _userManager.CheckPasswordAsync(user, userContract.Password))
        {
            throw new ServiceBaseException("Wrong Passsword");
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