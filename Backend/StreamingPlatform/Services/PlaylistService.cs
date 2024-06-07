using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Models;
using StreamingPlatform.Services.Exceptions;
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
            var user = await this.userManager.FindByIdAsync(newPlaylistContract.UserId) ?? throw new ServiceBaseException("Invalid user id.");

            IGenericRepository<Playlist> repository = this.unitOfWork.Repository<Playlist>();
            Playlist newPlaylist = new(Guid.NewGuid(), newPlaylistContract.Title, new Guid(user.Id));
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
            var playlist = await (repository.GetRecordByIdAsync(new Guid(id)) ?? throw new ServiceBaseException("Invalid playlist id."));
            return new PlaylistResponseDto(playlist.Id.ToString(), playlist.Title, playlist.UserId.ToString(), new List<string>());
        }

        /// <summary>
        /// Adds song to playlist.
        /// </summary>
        /// <returns></returns>
        public async Task<PlaylistResponseDto> AddSongToPlaylist(AddSongToPlaylistContract dto)
        {
            IGenericRepository<Playlist> repository = this.unitOfWork.Repository<Playlist>();
            IGenericRepository<Song> songRepository = this.unitOfWork.Repository<Song>();

            //verify if the song exists
            var song = await songRepository.GetRecordByIdAsync(dto.SongId) ??
                       throw new ServiceBaseException("There is no song with the specified id.");

            //verify if playlist exists
            var playlist = await repository.GetRecordByIdAsync(new Guid(dto.PlaylistId)) ??
                           throw new ServiceBaseException("There is no playlist with the specified id.");

            //add song to playlist
            playlist.SongPlaylists.Add(new SongPlaylist(song.Id, song, playlist.Id, playlist));
            await this.unitOfWork.SaveChangesAsync();

            var songsInPlaylist = new List<string>();
            foreach (var s in songsInPlaylist)
            {
                songsInPlaylist.Add(s.ToString());
            }

            return new PlaylistResponseDto(playlist.Id.ToString(), playlist.Title, playlist.UserId.ToString(), songsInPlaylist);
        }

        /// <summary>
        /// Gets user's playlists.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PlaylistResponseDto>> GetUserPlaylist(string userId)
        {
            //verify if user exists
            var user = await this.userManager.FindByIdAsync(userId) ?? throw new ServiceBaseException("Invalid user id.");

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