using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StreamingPlatform.Models
{
    public class SongsPlaylists
    {
        public Guid PlaylistId { get; set; }
        
        public Guid SongId { get; set; }
    }
}