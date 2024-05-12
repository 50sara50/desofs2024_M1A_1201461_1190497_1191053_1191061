using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;

namespace StreamingPlatform;

public interface IAuthService
{
    public Task<GenericResponseDto> Register(NewUserContract newUser);

    public Task<TokenResponseDto> Login(UserLoginContract user);
}