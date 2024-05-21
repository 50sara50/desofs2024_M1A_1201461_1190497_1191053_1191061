using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic.FileIO;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Models.Enums.Mappers;
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

            IGenericRepository<User> userRepository = this.unitOfWork.Repository<User>();
            User user = await userRepository.GetRecordAsync(u => u.UserName == userName) ?? throw new InvalidOperationException("User does not exist.");
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
            string fileExtension = Path.GetExtension(fileName);
            FileType fileType = FileTypeMapper.ExtensionToFilePath(fileExtension) ?? throw new InvalidOperationException("Invalid file type.");
            Song song = new(Guid.NewGuid(), songDto.Title, user, album, fileName, fileType);
            IGenericRepository<Song> songRepository = this.unitOfWork.Repository<Song>();
            songRepository.Create(song);
            await this.unitOfWork.SaveChangesAsync();
            return new SongResponse(song.Title, song.Artist?.Name, song.Album?.Title);
        }

        public async Task<DownloadSongResponse> DownloadSong(string songName, string artistName, string? albumName)
        {
            IGenericRepository<Song> songRepository = this.unitOfWork.Repository<Song>();
            Expression<Func<Song, bool>> expression;

            if (albumName != null)
            {
                expression = s => s.Title == songName && s.Artist!.Name == artistName && s.Album != null && s.Album.Title == albumName;
            }
            else
            {
                expression = s => s.Title == songName && s.Artist!.Name == artistName;
            }

            Song? song = await songRepository.GetRecordAsync(expression);
            if (song == null)
            {
                throw new InvalidOperationException("Song does not exist.");
            }

            // read the file and return it
            string filePath = song.SavedPath;
            byte[] fileData = await File.ReadAllBytesAsync(filePath);
            FileType fileType = song.FileType;
            string? fileExtension = FileTypeMapper.FileTypeToExtension(song.FileType) ?? throw new InvalidOperationException("Invalid file type.");
            return new DownloadSongResponse(songName, fileExtension, fileData);
        }
    }
}
