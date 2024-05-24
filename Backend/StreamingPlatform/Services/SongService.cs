using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using nClam;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Models.Enums.Mappers;
using StreamingPlatform.Models;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Services
{
    public class SongService(IUnitOfWork unitOfWork, IConfiguration configuration) : ISongService
    {

        private readonly IUnitOfWork unitOfWork = unitOfWork;

        private readonly IConfiguration configuration = configuration;
        
         public async Task<SongResponseDto> GetSongById(string id)
        {
            IGenericRepository<Song> repository = this.unitOfWork.Repository<Song>();
            Song song = await repository.GetRecordByIdAsync(new Guid(id));
            return new SongResponseDto(song.Title, song.ArtistId.ToString(), song.Duration.ToString(), song.AlbumId.ToString());
        }

        /// <summary>
        /// Stores a song in the database and saves the music file in the file system.
        /// </summary>
        /// <param name="songDto">Data transfer object containing song information</param>
        /// <param name="music">The music file to be stored</param>
        /// <param name="userName">Name of the user performing the action</param>
        /// <returns>Response containing information about the created song</returns>
        /// <exception cref="ValidationException">Thrown when song data validation fails</exception>
        /// <exception cref="InvalidDataException">Thrown when music file data is invalid</exception>
        /// <exception cref="InvalidOperationException">Thrown when an invalid operation is performed</exception>
        /// <exception cref="Exception">Thrown for other exceptions</exception>
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

            // Validate filename characters and length
            if (!IsValidFilename(music.FileName))
            {
                throw new InvalidDataException("Invalid filename characters or length.");
            }

            // Sanitize filename to prevent path traversal
            string sanitizedFilename = Path.GetInvalidFileNameChars().Aggregate(music.FileName, (current, c) => current.Replace(c.ToString(), "-"));
            Guid? albumId = songDto.AlbumId;

            Album? album = null;
            if (albumId != null)
            {
                IGenericRepository<Album> albumRepository = this.unitOfWork.Repository<Album>();

                // Check if album exists when provided
                album = await albumRepository.GetRecordAsync(a => a.Id == albumId) ?? throw new InvalidOperationException("Album does not exist.");
            }

            using MemoryStream stream = new();
            music.CopyTo(stream);
            byte[] fileData = stream.ToArray();

            string clamIp = this.configuration.GetValue<string>("Client:ClamIP") ?? throw new Exception("Cannot scan file for virus. ClamAV IP not found.");
            ClamClient clam = new(clamIp, 3310);
            await clam.PingAsync();
            ClamScanResult scanResult = await clam.SendAndScanFileAsync(fileData);
            if (scanResult.Result != ClamScanResults.Clean)
            {
                throw new InvalidDataException("Virus detected in file.");
            }

            IGenericRepository<User> userRepository = this.unitOfWork.Repository<User>();
            User user = await userRepository.GetRecordAsync(u => u.UserName == userName) ?? throw new InvalidOperationException("User does not exist.");

            // create a directory with musics of the user and album if it does not exist
            string userDirectory = Path.Combine("wwwroot", "Songs", user.Id);
            if (album != null)
            {
                userDirectory = Path.Combine(userDirectory, album.Title.ToString());
            }

            //check if directory exists
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            Guid songId = Guid.NewGuid();
            string validatedFileName = $"{sanitizedFilename}";
            string fileName = Path.Combine(userDirectory, validatedFileName);
            await File.WriteAllBytesAsync(fileName, fileData);
            string fileExtension = Path.GetExtension(fileName);
            FileType fileType = FileTypeMapper.ExtensionToFilePath(fileExtension) ?? throw new InvalidOperationException("Invalid file type.");
            Song song = new(songId, songDto.Title, user, album, fileName, fileType);
            IGenericRepository<Song> songRepository = this.unitOfWork.Repository<Song>();
            songRepository.Create(song);
            await this.unitOfWork.SaveChangesAsync();
            return new SongResponse(song.Title, song.Artist?.Name, song.Album?.Title);
        }


        /// <summary>
        /// Retrieves a song from the database and returns the file data.
        /// </summary>
        /// <param name="songName">Name of the song to retrieve</param>
        /// <param name="artistName">Name of the artist of the song</param>
        /// <param name="albumName">Name of the album containing the song (optional)</param>
        /// <returns>Response containing the song file data</returns>
        /// <exception cref="InvalidOperationException">Thrown when an invalid operation is performed</exception>
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

        /// <summary>
        /// Ensures that the filename is valid.
        /// </summary>
        /// <param name="fileName">the file name</param>
        /// <returns>true if is valid false otherwise</returns>
        private static bool IsValidFilename(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidChars) >= 0)
            {
                return false;
            }

            if (fileName.Length > 255)
            {
                return false;
            }

            return true;
        }
    }
}
