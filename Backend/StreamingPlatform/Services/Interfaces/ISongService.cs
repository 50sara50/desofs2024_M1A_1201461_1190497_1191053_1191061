using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;

namespace StreamingPlatform.Services.Interfaces
{
    public interface ISongService
    {
        public Task<SongResponse> CreateSong(CreateSongContract songDto, IFormFile music, string? userName);

        public Task<DownloadSongResponse> DownloadSong(string songName, string artistName, string? albumName);

        public Task<SongResponseDto> GetSongById(Guid id);
    }
}