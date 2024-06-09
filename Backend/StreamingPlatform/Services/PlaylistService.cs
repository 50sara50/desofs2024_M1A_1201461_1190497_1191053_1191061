using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
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
            return new PlaylistResponseDto(newPlaylist.Id, newPlaylist.Title, newPlaylist.UserId, []);
        }

        /// <summary>
        /// returns a playlist by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<PlaylistResponseDto> GetPlaylistById(Guid id)
        {
            PlaylistRepository repository = new(this.unitOfWork.GetContext());

            var playlist = await (repository.GetRecordByIdAsync(id) ?? throw new ServiceBaseException("Invalid playlist id.")) ??
                throw new InvalidDataException("Playlist does not exist.");

            return new PlaylistResponseDto(playlist.Id, playlist.Title, playlist.UserId, []);
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
                       throw new InvalidDataException("There is no song with the specified id.");

            //verify if playlist exists
            var playlist = await repository.GetRecordByIdAsync(dto.PlaylistId) ??
                           throw new InvalidDataException("There is no playlist with the specified id.");

            //add song to playlist
            playlist.SongPlaylists.Add(new SongPlaylist(song.Id, song, playlist.Id, playlist));
            await this.unitOfWork.SaveChangesAsync();

            var songsInPlaylist = new List<string>();
            foreach (var s in songsInPlaylist)
            {
                songsInPlaylist.Add(s.ToString());
            }

            List<Guid> songIds = playlist.SongPlaylists.Select(sp => sp.SongId).ToList();

            return new PlaylistResponseDto(playlist.Id, playlist.Title, playlist.UserId, songIds);
        }

        /// <summary>
        /// Gets user's playlists.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PlaylistResponseDto>> GetUserPlaylist(string userId)
        {
            //verify if user exists
            var user = await this.userManager.FindByIdAsync(userId) ?? throw new ServiceBaseException("Invalid user id.");

            PlaylistRepository playlistRepository = new(this.unitOfWork.GetContext());
            IEnumerable<Playlist> playlists = await playlistRepository.GetUserPlaylists(new Guid(user.Id))
                ?? throw new InvalidDataException("User does not have any playlists.");
            var results = new List<PlaylistResponseDto>();
            foreach (var playlist in playlists)
            {
                List<Guid> songIds = playlist.SongPlaylists.Select(sp => sp.SongId).ToList();
                results.Add(new PlaylistResponseDto(playlist.Id, playlist.Title, playlist.UserId, songIds));
            }

            return results;
        }
    }
}