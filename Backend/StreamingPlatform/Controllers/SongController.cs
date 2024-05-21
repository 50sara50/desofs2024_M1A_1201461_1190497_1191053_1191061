using System.ComponentModel.DataAnnotations;
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
        [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongContract songDto, [FromForm] IFormFile music)
        {
            if (music == null || music.Length == 0)
            {
                logger.LogInformation($"No file was uploaded");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("No file was uploaded");
                return this.BadRequest(errorResponseObject);
            }

            // get the file extension
            string extension = Path.GetExtension(music.FileName);

            FileType? fileType = FileTypeMapper.ExtensionToFilePath(extension);
            if (fileType == null)
            {
                logger.LogInformation($"Invalid file type. App only allows mp3, m4A and wav files");
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest("Invalid file type. App only allows mp3, m4A and wav files");
                return this.BadRequest(errorResponseObject);
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

    }
}
