using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Dtos;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Models;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform
{
    public class PlaylistService(IUnitOfWork unitOfWork, UserManager<User> userManager) : IPlaylistService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly UserManager<User> userManager = userManager;

        /// <summary>
        /// Adds new playlist to user's account.
        /// </summary>
        /// <param name="newPlaylistContract"></param>
        /// <returns></returns>
        public async Task<PlaylistResponseDto> AddPlaylist(NewPlaylistContract newPlaylistContract)
        {
            //verify if user exists
            var user = await this.userManager.FindByIdAsync(newPlaylistContract.UserId) ?? throw new ValidationException("Invalid user id.");
            
            IGenericRepository<Playlist> repository = this.unitOfWork.Repository<Playlist>();
            Playlist newPlaylist = new (Guid.NewGuid(), newPlaylistContract.Title, new Guid(user.Id));
            repository.Create(newPlaylist);
            await this.unitOfWork.SaveChangesAsync();
            
            //TODO: create mapper
            return new PlaylistResponseDto(newPlaylist.Id.ToString(), newPlaylist.Title, newPlaylist.UserId.ToString(), new List<string>());
        }

        /// <summary>
        /// returns a playlist by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<PlaylistResponseDto> GetPlaylistById(string id)
        {
            IGenericRepository<Playlist> repository = this.unitOfWork.Repository<Playlist>();
            var playlist = await (repository.GetRecordByIdAsync(new Guid(id)) ?? throw new ValidationException("Invalid playlist id."));
            return new PlaylistResponseDto(playlist.Id.ToString(), playlist.Title, playlist.UserId.ToString(), new List<string>());
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
        public async Task<IEnumerable<PlaylistResponseDto>> GetUserPlaylist(string userId)
        {
            //verify if user exists
            var user = await this.userManager.FindByIdAsync(userId) ?? throw new ValidationException("Invalid user id.");
            
            IGenericRepository<Playlist> repository = this.unitOfWork.Repository<Playlist>();
            var playlists = await repository.GetRecordsAsync(p => p.UserId.ToString().Equals(userId)) 
                ?? throw new Exception("That user doe not have any playlists.");
            
            //TODO: create mapper
            var results = new List<PlaylistResponseDto>();
            foreach (var playlist in playlists)
            {
                results.Add(new PlaylistResponseDto(playlist.Id.ToString(), playlist.Title,
                    playlist.UserId.ToString(), new List<string>()));
            }

            return results;
        }
    }
}