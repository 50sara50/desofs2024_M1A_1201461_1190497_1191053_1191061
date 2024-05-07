using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Models;

namespace StreamingPlatform;

public interface IAuthService
{
    public Task<User> Register(NewUserContract newUser);
}