using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos.Contract
{
    public class AddSongToPlaylistContract
    {
        /// <summary>
        /// The identifier of the playlist.
        /// </summary>
        [Required(ErrorMessage = "PlaylistId is required.")]
        public Guid PlaylistId { get; set; }

        /// <summary>
        /// The identifier of the song to be added.
        /// </summary>
        [Required(ErrorMessage = "The song id is required.")]
        public Guid SongId { get; set; }
    }
}