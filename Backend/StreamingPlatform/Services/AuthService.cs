using Microsoft.AspNetCore.Identity;
using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;

namespace StreamingPlatform;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<User> Register(NewUserContract newUser)
    {
        var userExists = await _userManager.FindByEmailAsync(newUser.Email);
        if(userExists != null)
        {
            throw new Exception("User already exists!");
        }
        
        User user = new User
        {
            Email = newUser.Email,
            UserName = newUser.Email,
            Name = newUser.Name,
            Age = newUser.Age,
            Address = newUser.Address,
        };

        var result = await _userManager.CreateAsync(user, newUser.Password);
        if(!result.Succeeded)
        {
            throw new Exception("User creation failed! Please check user details and try again.");
        }
        
        var roleResult = await _userManager.AddToRoleAsync(user, Role.Subscriber.ToString());
        if (!roleResult.Succeeded)
        {
            throw new Exception("Failed to assign role to user.");
        }

        return user;
    }
}
