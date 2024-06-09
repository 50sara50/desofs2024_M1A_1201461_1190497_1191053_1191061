using StreamingPlatform.Dtos.Response;

namespace StreamingPlatform.Dtos.Contract
{
    public class PlaylistResponseDto(Guid id, string title, Guid userId, List<Guid> songIds)
    {
        public Guid Id { get; set; } = id;

        public string Title { get; set; } = title;

        public Guid UserId { get; set; } = userId;

        public List<Guid> Songs { get; set; } = songIds;
    }
}