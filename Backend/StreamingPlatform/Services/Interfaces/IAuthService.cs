using StreamingPlatform.Dtos.Contracts;
using StreamingPlatform.Dtos.Responses;

namespace StreamingPlatform;

public interface IAuthService
{
    public Task<TokenResponseDto> Register(NewUserContract newUser);
}