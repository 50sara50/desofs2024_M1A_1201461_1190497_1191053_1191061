using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models.Enums;
using StreamingPlatform.Models.Enums.Mappers;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Controllers
{
    [ApiController]
    [Route("api/song")]
    public class SongController(ILogger<SongController> logger, ISongService songService, IConfiguration configuration) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongContract songDto, [FromForm] IFormFile music)
        {
            if (music == null || music.Length == 0)
            {
                logger.LogInformation($"No file was uploaded");
                ErrorResponseObject errorResponseObject = MapResponse.UnsportedMediaType("No file was uploaded");
                return this.StatusCode(StatusCodes.Status415UnsupportedMediaType, errorResponseObject);
            }

            // get the file extension
            string extension = Path.GetExtension(music.FileName);

            FileType? fileType = FileTypeMapper.ExtensionToFilePath(extension);
            if (fileType == null)
            {
                logger.LogInformation($"Invalid file type. App only allows mp3, m4A and wav files");
                ErrorResponseObject errorResponseObject = MapResponse.UnsportedMediaType("Invalid file type. App only allows mp3, m4A and wav files");
                return this.StatusCode(StatusCodes.Status415UnsupportedMediaType, errorResponseObject);
            }

            long fileSize = music.Length;

            long maxFileSize = configuration.GetValue<long>("MaxFileSize");

            if (fileSize > maxFileSize)
            {
                logger.LogInformation($"File size is too large. Max file size is 10MB");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("File size is too large. Max file size is 10MB");
                return this.BadRequest(errorResponseObject);
            }

            string? userName = this.User.Identity?.Name;
            try
            {
                SongResponse songResponse = await songService.CreateSong(songDto, music, userName);
                return this.Ok(songResponse);
            }
            catch (ValidationException e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("Invalid song data");
                return this.BadRequest(errorResponseObject);
            }
            catch (InvalidDataException e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("File provided was identified as a virus or malware");
                return this.BadRequest(errorResponseObject);
            }
            catch (InvalidOperationException e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.Conflict(e.Message);
                return this.Conflict(errorResponseObject);
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.InternalServerError("An unexpected error occurred");
                return this.StatusCode(StatusCodes.Status500InternalServerError, errorResponseObject);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SongResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> DownloadSong([FromQuery][Required] string songName, [FromQuery][Required] string artistName, [FromQuery] string? albumName)
        {
            // ensure that at least song name and artist name are provided
            if (string.IsNullOrEmpty(songName) || string.IsNullOrEmpty(artistName))
            {
                logger.LogInformation("Song name and artist name are required");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("Song name and artist name are required");
                return this.BadRequest(errorResponseObject);
            }

            try
            {
                DownloadSongResponse music = await songService.DownloadSong(songName, artistName, albumName);
                string fileExtension = music.FileType.Replace("audio/", string.Empty);
                ContentDisposition contentDisposition = new()
                {
                    FileName = music.Name,
                    Inline = false,
                };

                this.Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

                return this.File(music.Data, music.FileType, $"{music.Name}.");
            }
            catch (InvalidOperationException e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.NotFound("Song not found");
                return this.NotFound(errorResponseObject);
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e.Message}");
                ErrorResponseObject errorResponseObject = MapResponse.InternalServerError("An unexpected error occurred");
                return this.StatusCode(StatusCodes.Status500InternalServerError, errorResponseObject);
            }
        }
    }
}
