using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamingPlatform.Dao.Properties;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// Represents a Song.
    /// </summary>
    [Table("Songs")]
    public class Song
    {
        public Song()
        {
            this.Title = string.Empty;
            this.Artist = null;
            this.Album = null;
            this.SavedPath = string.Empty;
        }

        public Song(Guid id, string title, User artist, Album? album)
        {
            this.Id = id;
            this.Title = title;
            this.Artist = artist;
            this.Album = album;
            this.SavedPath = string.Empty;
        }

        public Song(Guid id, string title, User artist, Album? album, string savedPath)
        {
            this.Id = id;
            this.Title = title;
            this.Artist = artist;
            this.Album = album;
            this.SavedPath = savedPath;
        }

        /// <summary>
        /// The song's unique identifier.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The song's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The artist that created the song.
        /// </summary>
        public User? Artist { get; set; }
        /// <summary>
        /// The album the song belongs to.
        /// </summary>
        public Album? Album { get; set; }

        [SecureProperty]
        public string SavedPath { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            Song otherSong = (Song)obj;
            return this.Id == otherSong.Id
                   && this.Title == otherSong.Title
                   && Guid.Equals(this.Album, otherSong.Album);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id, this.Title, this.Artist, this.Album);
        }
    }
}