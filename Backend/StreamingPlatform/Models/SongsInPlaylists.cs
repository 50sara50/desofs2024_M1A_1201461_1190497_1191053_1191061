using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingPlatform.Models
{
    [Table("SongsInPlaylists")]
    public class SongsInPlaylists
    {
        /// <summary>
        /// Playlist Id
        /// </summary>
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Playlists")]
        public Guid PlaylistId { get; set; }
        
        /// <summary>
        /// Song id
        /// </summary>
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Songs")]
        public Guid SongId { get; set; }
        
        public Playlist Playlist { get; set; }

        public Song Song { get; set; }
    }
}