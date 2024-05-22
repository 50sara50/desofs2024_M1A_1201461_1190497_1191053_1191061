using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingPlatform.Models
{
    [Table("SongPlaylists")]
    public class SongPlaylist (Guid songId, Song song, Guid playlisId, Playlist playlist)
    {
        public Guid SongId { get; set; } = songId;

        public Song Song { get; set; } = song;

        public Guid PlaylistId { get; set; } = playlisId;

        public Playlist Playlist { get; set; } = playlist;
    }
}