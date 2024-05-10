using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// Represents a Song.
    /// </summary>
	[Table("Songs")]
	public class Song
	{
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
        /// The unique identifier of the owner of the song.
        /// </summary>
		public Guid ArtistId { get; set; }

        /// <summary>
        /// The song's duration in minutes.
        /// </summary>
		public TimeSpan Duration { get; set; }

        /// <summary>
        /// The album the song belongs to.
        /// </summary>
		public Guid? AlbumId { get; set; }
        
        /// <summary>
        /// Playlists where the song was included.
        /// </summary>
		public ICollection<Playlist> Playlists { get; set; } 
        
		public Song()
		{
			this.Title = string.Empty;
			this.ArtistId = Guid.NewGuid();
			this.Duration = new TimeSpan(0, 0, 0);
			this.AlbumId = null;
			this.Playlists = new List<Playlist>();
        }

		public Song(Guid id, string title, Guid artistId, TimeSpan duration, Guid albumId)
		{
			this.Id = id;
			this.Title = title;
			this.ArtistId = artistId;
			this.Duration = duration;
			this.AlbumId = albumId;
		}
	}
}