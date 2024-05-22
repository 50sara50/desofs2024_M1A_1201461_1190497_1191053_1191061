using StreamingPlatform.Dtos.Contract;

namespace StreamingPlatform.Services.Interfaces
{
    public interface IPlaylistService
    {
        public Task<PlaylistResponseDto> AddPlaylist(NewPlaylistContract newPlaylistContract);

        public Task<PlaylistResponseDto> GetPlaylistById(string id);

        public Task<PlaylistResponseDto> AddSongToPlaylist(AddSongToPlaylistContract dto);

        public Task<IEnumerable<PlaylistResponseDto>> GetUserPlaylist(string userId);
    }
}