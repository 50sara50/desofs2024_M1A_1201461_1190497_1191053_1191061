using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos
{
    public class AddSongToPlaylistContract
    {
        /// <summary>
        /// The identifier of the playlist.
        /// </summary>
        [Required (ErrorMessage = "PlaylistId is required.")]
        public string PlaylistId;

        /// <summary>
        /// The identifier of the song to be added.
        /// </summary>
        [Required (ErrorMessage = "The song id is required.")]
        public string SongId;
    }
}