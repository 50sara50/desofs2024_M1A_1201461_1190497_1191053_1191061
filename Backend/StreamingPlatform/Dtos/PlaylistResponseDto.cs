namespace StreamingPlatform.Dtos
{
    public class PlaylistResponseDto(string id, string title, string userId, List<string> songIds)
    {
        public string Id { get; set; } = id;

        public string Title { get; set; } = title;

        public string UserId { get; set; } = userId;

        public List<string> SongIds { get; set; } = songIds;
    }
}