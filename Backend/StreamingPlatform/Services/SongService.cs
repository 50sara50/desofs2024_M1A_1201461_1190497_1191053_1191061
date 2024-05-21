using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic.FileIO;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Services
{
    public class SongService(IUnitOfWork unitOfWork, UserManager<User> userManager) : ISongService
    {

        private readonly IUnitOfWork unitOfWork = unitOfWork;

        private readonly UserManager<User> userManager = userManager;

        public async Task<SongResponse> CreateSong(CreateSongContract songDto, IFormFile music, string? userName)
        {
            ValidationContext validationContext = new(songDto, serviceProvider: null, items: null);
            List<ValidationResult> validationResults = [];
            bool isValid = Validator.TryValidateObject(songDto, validationContext, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                var errorMessages = validationResults.Select(r => r.ErrorMessage);
                throw new ValidationException(string.Join(" ", errorMessages));
            }

            Guid? albumId = songDto.AlbumId;

            Album? album = null;
            if (albumId != null)
            {
                IGenericRepository<Album> albumRepository = this.unitOfWork.Repository<Album>();

                // Check if album exists when provided
                album = await albumRepository.GetRecordAsync(a => a.Id == albumId) ?? throw new InvalidOperationException("Album does not exist.");
            }

            User user = await this.userManager.FindByNameAsync(userName ?? string.Empty) ?? throw new InvalidOperationException("User does not exist.");

            using MemoryStream stream = new();
            music.CopyTo(stream);
            byte[] fileData = stream.ToArray();

            // create a directory with musics of the user and album if it does not exist
            string userDirectory = Path.Combine("wwwroot", "Songs", user.Id.ToString());
            if (album != null)
            {
                userDirectory = Path.Combine(userDirectory, album.Id.ToString());
            }

            //check if directory exists
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            string fileName = Path.Combine(userDirectory, music.FileName);
            await File.WriteAllBytesAsync(fileName, fileData);
            Song song = new(Guid.NewGuid(), songDto.Title, user, album, fileName);
            IGenericRepository<Song> songRepository = this.unitOfWork.Repository<Song>();
            songRepository.Create(song);
            List<Song> songs = user.Songs.ToList();
            await this.unitOfWork.SaveChangesAsync();
            return new SongResponse(song.Title, song.Artist?.Name, song.Album?.Title);
        }
    }
}
