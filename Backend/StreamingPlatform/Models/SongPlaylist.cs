using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// Intermediary class that represents the many-to-many relationship between songs and playlists.
    /// </summary>
    [Table("SongPlaylists")]
    [PrimaryKey(nameof(SongId), nameof(PlaylistId))]

    public class SongPlaylist
    {
        public Guid SongId { get; set; }

        public Song? Song { get; set; }

        public Guid PlaylistId { get; set; }

        public Playlist? Playlist { get; set; }

        public SongPlaylist()
        {
            this.SongId = Guid.Empty;
            this.Song = null;
            this.PlaylistId = Guid.Empty;
            this.Playlist = null;
        }

        public SongPlaylist(Guid songId, Song song, Guid playlistId, Playlist playlist)
        {
            this.SongId = songId;
            this.Song = song;
            this.PlaylistId = playlistId;
            this.Playlist = playlist;
        }
    }
}