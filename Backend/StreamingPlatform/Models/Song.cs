using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingPlatform.Models
{
	[Table("Songs")]
	public class Song
	{
		[Key]
		public Guid Id { get; set; }

		public string Title { get; set; }

		public Guid ArtistId { get; set; }

		public TimeSpan Duration { get; set; }

		public Guid? AlbumId { get; set; }

		public Song()
		{
			this.Title = string.Empty;
			this.ArtistId = Guid.NewGuid();
			this.Duration = new TimeSpan(0, 0, 0);
			this.AlbumId = null;
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