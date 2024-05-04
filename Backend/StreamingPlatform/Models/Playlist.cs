using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Components.Web;

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
        public Guid Id { get; set; }
        
        /// <summary>
        /// The playlist's title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// The songs in the playlist.
        /// </summary>
        public List<Song> SongList { get; set; }

        public Playlist()
        {
            this.Title = string.Empty;
            this.SongList = new List<Song>();
        }

        public Playlist(Guid id, string title)
        {
            this.Id = id;
            this.Title = title;
            this.SongList = new List<Song>();
        }

        /// <summary>
        /// Adds a new song to the playlist
        /// </summary>
        /// <param name="song"></param>
        public void AddSong(Song song)
        {
            if (this.SongList.Contains(song))
            {
                throw new Exception($"This song has already been added to the playlist '${this.Title}'");
            }
            else
            {
                this.SongList.Add(song);
            }
        }

        /// <summary>
        /// Removes a song from the playlist.
        /// </summary>
        /// <param name="song"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveSong(Song song)
        {
            if (this.SongList.Contains(song))
            {
                this.SongList.Remove(song);
            }
            else
            {
                throw new Exception($"This song was already removed from the playlist. '${this.Title}'");
            }
        }
    }
}