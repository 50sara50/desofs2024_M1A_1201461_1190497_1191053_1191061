using StreamingPlatform.Models;

namespace StreamingPlatform;

public interface IAuthService
{
    public Task<User> Register(NewUserDto newUser);
}