using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Dtos;
using StreamingPlatform.Models;

namespace StreamingPlatform
{
    public class PlaylistService(IUnitOfWork unitOfWork) : IPlaylistService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        
        /// <summary>
        /// Adds new playlist to user's account.
        /// </summary>
        /// <param name="newPlaylistContract"></param>
        /// <returns></returns>
        public async Task<PlaylistResponseDto> AddPlaylist(NewPlaylistContract newPlaylistContract)
        {
            IGenericRepository<Playlist> repository = this.unitOfWork.Repository<Playlist>();
            Playlist newPlaylist = new (Guid.NewGuid(), newPlaylistContract.Title, Guid.NewGuid());
            repository.Create(newPlaylist);
            await this.unitOfWork.SaveChangesAsync();
            
            //TODO: create mapper
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