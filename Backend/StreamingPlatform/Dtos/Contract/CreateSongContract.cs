using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos.Contract
{
    public class CreateSongContract
    {

        [Required(ErrorMessage = "Song title is required.")]
        [MaxLength(50, ErrorMessage = "Song title is too long. Max length is 50 characters.")]
        required public string Title { get; set; }

        public Guid? AlbumId { get; set; }
    }
}
