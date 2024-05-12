namespace StreamingPlatform.Dtos.Response;

public record TokenResponseDto (string token, DateTime expirationDate)
{
}
