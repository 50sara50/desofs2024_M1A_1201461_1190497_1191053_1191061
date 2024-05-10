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
        
        /// <summary>
        /// The songs in the playlist.
        /// </summary>
        public ICollection<Song> Songs { get; set; }
        
        public Playlist()
        {
            this.Title = string.Empty;
            this.Songs = new List<Song>();
            this.UserId = new Guid();
        }

        public Playlist(Guid id, string title, Guid userId)
        {
            this.Id = id;
            this.Title = title;
            this.Songs = new List<Song>();
            this.UserId = userId;
        }

        /// <summary>
        /// Adds a new song to the playlist
        /// </summary>
        /// <param name="song"></param>
        public void AddSong(Song song)
        {
            if (this.Songs.Contains(song))
            {
                throw new Exception($"This song has already been added to the playlist '${this.Title}'");
            }
            else
            {
                this.Songs.Add(song);
            }
        }

        /// <summary>
        /// Removes a song from the playlist.
        /// </summary>
        /// <param name="song"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveSong(Song song)
        {
            if (this.Songs.Contains(song))
            {
                this.Songs.Remove(song);
            }
            else
            {
                throw new Exception($"This song was already removed from the playlist. '${this.Title}'");
            }
        }
    }
}