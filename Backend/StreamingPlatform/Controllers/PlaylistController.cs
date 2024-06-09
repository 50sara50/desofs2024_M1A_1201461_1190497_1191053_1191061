using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dtos.Contract;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class PlaylistController(ILogger<AuthController> logger, IPlaylistService playlistService) : ControllerBase
    {

        [HttpGet("GetPlaylistById")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            try
            {
                PlaylistResponseDto playlistResponseDto = await playlistService.GetPlaylistById(id);
                logger.LogInformation(
                    $"Playlist '${playlistResponseDto.Title}' added on user's '${playlistResponseDto.UserId}' account.");
                return this.Ok(playlistResponseDto);
            }
            catch (InvalidOperationException i)
            {
                logger.LogError($"Invalid operation exceptions: ${i.Message}.");
                return this.BadRequest(i.Message);
            }
            catch (Exception e)
            {
                logger.LogError($"Exception: ${e.Message}.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Add new playlist
        /// </summary>
        /// <returns></returns>
        [HttpPost("CreatePlaylist")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlaylist([FromBody] NewPlaylistContract newPlaylistContract)
        {
            try
            {
                PlaylistResponseDto playlist = await playlistService.AddPlaylist(newPlaylistContract);
                logger.LogInformation(
                    $"New playlist '${playlist.Title}' added on user's '${playlist.UserId}' account.");
                return this.Ok(playlist);
            }
            catch (ValidationException v)
            {
                logger.LogError($"Validation exception: ${v.Message}.");
                return this.BadRequest(v.Message);
            }
            catch (InvalidOperationException i)
            {
                logger.LogError($"Invalid operation exceptions: ${i.Message}.");
                return this.Conflict(i.Message);
            }
            catch (Exception e)
            {
                logger.LogError($"Exception: ${e.Message}.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Add a song to the playlist.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        [HttpPatch("AddSongToPlaylist")]
        [Authorize]
        public async Task<IActionResult> AddSong([FromBody] AddSongToPlaylistContract contract)
        {
            try
            {
                PlaylistResponseDto result = await playlistService.AddSongToPlaylist(contract);
                logger.LogInformation($"Song added to playlist.");
                return this.Ok(result);
            }
            catch (InvalidOperationException e)
            {
                ErrorResponseObject errorResponseObject = MapResponse.BadRequest(e.Message);
                return this.BadRequest(errorResponseObject);
            }
        }

        /// <summary>
        /// Gets user's playlists.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("GetUserPlaylists")]
        [Authorize]
        public async Task<IActionResult> GetUserPlaylists([FromQuery] string userId)
        {
            try
            {
                var results = await playlistService.GetUserPlaylist(userId);
                logger.LogInformation(@"Success");
                return this.Ok(results);
            }
            catch (ValidationException v)
            {
                logger.LogError($"Validation exception: ${v.Message}.");
                return this.BadRequest(v.Message);
            }
            catch (InvalidOperationException i)
            {
                logger.LogError($"Invalid operation exceptions: ${i.Message}.");
                return this.Conflict(i.Message);
            }
            catch (Exception e)
            {
                logger.LogError($"Exception: ${e.Message}.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}