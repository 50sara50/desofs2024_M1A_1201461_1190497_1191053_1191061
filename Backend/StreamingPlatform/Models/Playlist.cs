using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// Represents a Playlist.
    /// </summary>
    [Table("Playlists")]
    public class Playlist
    {
        /// <summary>
        /// The playlist's unique identifier.
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// The playlist's title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// The unique identifier of the owner of the playlist.
        /// </summary>
        public Guid UserId { get; set; }
        
        public ICollection<SongPlaylist> SongPlaylists { get; set; }
        
        public Playlist()
        {
            this.Title = string.Empty;
            // this.Songs = new List<Song>();
            this.UserId = new Guid();
            this.SongPlaylists = new List<SongPlaylist>();

        }

        public Playlist(Guid id, string title, Guid userId)
        {
            this.Id = id;
            this.Title = title;
            // this.Songs = new List<Song>();
            this.UserId = userId;
            this.SongPlaylists = new List<SongPlaylist>();
        }
    }
}