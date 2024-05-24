using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamingPlatform.Dao.Properties;
using StreamingPlatform.Models.Enums;

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
            this.FileType = FileType.MP3;
            this.SongPlaylists = new List<SongPlaylist>();

        }

        public Song(Guid id, string title, User artist, Album? album, FileType type)
        {
            this.Id = id;
            this.Title = title;
            this.Artist = artist;
            this.Album = album;
            this.SavedPath = string.Empty;
            this.FileType = type;
            this.SongPlaylists = new List<SongPlaylist>();

        }

        public Song(Guid id, string title, User artist, Album? album, string savedPath, FileType type)
        {
            this.Id = id;
            this.Title = title;
            this.Artist = artist;
            this.Album = album;
            this.SavedPath = savedPath;
            this.FileType = type;
            this.SongPlaylists = new List<SongPlaylist>();

        }

        /// <summary>
        /// The song's unique identifier.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The song's title.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Title is too long. Max length is 50 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// The artist that created the song.
        /// </summary>
        public User? Artist { get; set; }
        /// <summary>
        /// The album the song belongs to.
        /// </summary>
        public Album? Album { get; set; }

        /// <summary>
        /// The type of file the song is.
        [EnumDataType(typeof(FileType))]
        [Required(ErrorMessage = "FileType is required")]
        public FileType FileType { get; set; }

        [SecureProperty]
        [Required(ErrorMessage = "SavedPath is required")]
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