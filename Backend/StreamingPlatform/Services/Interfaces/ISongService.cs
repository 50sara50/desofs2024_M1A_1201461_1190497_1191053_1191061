using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;

namespace StreamingPlatform.Services.Interfaces
{
    public interface ISongService
    {
        public Task<SongResponse> CreateSong(CreateSongContract songDto, IFormFile music, string? userName);
    }
}
