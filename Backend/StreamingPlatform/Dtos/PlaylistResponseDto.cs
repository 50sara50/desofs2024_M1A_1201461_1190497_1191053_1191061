namespace StreamingPlatform.Dtos
{
    public record PlaylistResponseDto(string PlaylistId, string Title, string UserId, List<string> songIds)
    {
    }
}