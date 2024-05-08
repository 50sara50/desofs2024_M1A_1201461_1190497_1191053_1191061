using StreamingPlatform.Dtos;
using StreamingPlatform.Models;

namespace StreamingPlatform
{
    public interface IPlaylistService
    {
        public Task<PlaylistResponseDto> AddPlaylist(NewPlaylistContract newPlaylistContract);

        public Task<NewPlaylistContract> AddSongToPlaylist(AddSongToPlaylistContract dto);

        public Task<IEnumerable<NewPlaylistContract>> GetUserPlaylist(string userId);
    }
}