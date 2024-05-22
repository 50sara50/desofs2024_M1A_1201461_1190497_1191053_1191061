using StreamingPlatform.Dtos.Response;

namespace StreamingPlatform.Services.Interfaces
{
    public interface ISongService
    {
        public Task<SongResponseDto> GetSongById(string id);
    }
}