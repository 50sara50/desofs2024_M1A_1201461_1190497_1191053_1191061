using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Dtos.Responses;

namespace StreamingPlatform;

public interface IAuthService
{
    public Task<GenericResponseDto> Register(NewUserContract newUser);

    public Task<TokenResponseDto> Login(UserLoginContract user);
}