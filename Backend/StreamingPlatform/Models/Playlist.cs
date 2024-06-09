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
        public Playlist()
        {
            this.Title = string.Empty;
            this.UserId = Guid.Empty;
            this.SongPlaylists = [];
        }

        public Playlist(Guid id, string title, Guid userId)
        {
            this.Id = id;
            this.Title = title;
            this.UserId = userId;
            this.SongPlaylists = [];
        }

        public Playlist(Guid id, string title, Guid userId, ICollection<SongPlaylist> songPlaylists)
        {
            this.Id = id;
            this.Title = title;
            this.UserId = userId;
            this.SongPlaylists = songPlaylists;
        }

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
    }
}