using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingPlatform.Models
{
    [Table("SongPlaylists")]
    public class SongPlaylist
    {
        public Guid SongId { get; set; }
        public Song Song { get; set; }
        
        public Guid PlaylistId { get; set; }
        public Playlist Playlist { get; set; }
    }
}