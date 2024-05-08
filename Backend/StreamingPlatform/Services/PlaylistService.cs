using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Dtos;
using StreamingPlatform.Models;

namespace StreamingPlatform
{
    public class PlaylistService : IPlaylistService
    {
        private readonly StreamingDbContext _context;

        public PlaylistService(StreamingDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Adds new playlist to user's account.
        /// </summary>
        /// <param name="newPlaylistContract"></param>
        /// <returns></returns>
        public async Task<PlaylistResponseDto> AddPlaylist(NewPlaylistContract newPlaylistContract)
        {
            var repository = new GenericRepository<Playlist>(_context);
            var newPlaylist = new Playlist(Guid.NewGuid(), newPlaylistContract.Title,
                Guid.Parse(newPlaylistContract.UserId));
            repository.Create(newPlaylist);
            return new PlaylistResponseDto(newPlaylist.Id.ToString(), newPlaylist.Title, newPlaylist.UserId.ToString(), new List<string>());
        }

        /// <summary>
        /// Adds song to playlist.
        /// </summary>
        /// <returns></returns>
        public Task<NewPlaylistContract> AddSongToPlaylist(AddSongToPlaylistContract dto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets user's playlists.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<NewPlaylistContract>> GetUserPlaylist(string userId)
        {
            throw new NotImplementedException();
        }
    }
}