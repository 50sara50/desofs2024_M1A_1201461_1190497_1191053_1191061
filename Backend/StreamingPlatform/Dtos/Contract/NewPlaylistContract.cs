using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos.Contract
{
    public class NewPlaylistContract
    {
        /// <summary>
        /// The playlist's title.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(30, ErrorMessage = "Title is too long. Max length is 30 characters.")]
        public string Title { get; set; }
        
        /// <summary>
        /// The unique identifier of the owner.
        /// </summary>
        [Required (ErrorMessage = "You must provide the id of the owner of the playlist.")]
        public string UserId { get; set; }
    }
}