using Microsoft.AspNetCore.Mvc;
using StreamingPlatform.Dtos;

namespace StreamingPlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly ILogger<PlaylistController> _logger;

        private readonly IPlaylistService _playlistService;
        
        public PlaylistController(ILogger<PlaylistController> logger, IPlaylistService playlistService)
        {
            _logger = logger;
            _playlistService = playlistService;
        }

        /// <summary>
        /// Add new playlist
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddPlaylist")]
        public async Task<IActionResult> AddPlaylist([FromBody] NewPlaylistContract newPlaylistContract)
        {
            try
            {
                var playlist = await _playlistService.AddPlaylist(newPlaylistContract);
                 _logger.LogInformation($"New playlist '${newPlaylistContract.Title}' added on user's '${playlist.UserId}' account.");
                return Ok(playlist);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Add a song to the playlist.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public async Task AddSong([FromBody] AddSongToPlaylistContract contract)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}