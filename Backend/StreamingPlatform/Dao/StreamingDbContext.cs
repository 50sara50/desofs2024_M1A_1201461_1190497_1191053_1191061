using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao.Helper;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao
{
    /// <summary>
    /// Class that represents the database context.
    /// </summary>
    /// <param name="options"></param>
    public class StreamingDbContext(DbContextOptions<StreamingDbContext> options) : IdentityDbContext<User>(options)
    {

        /// <summary>
        /// Set of users in the database.
        /// </summary>
        public DbSet<Plan> Plans { get; set; }

        /// <summary>
        /// Set of songs in the database.
        /// </summary>
        public DbSet<Song> Songs { get; set; }

        /// <summary>
        /// Set of playlists in the database.
        /// </summary>
        public DbSet<Playlist> Playlists { get; set; }

        /// <summary>
        /// Set of SongPlaylists.
        /// </summary>
        public DbSet<SongPlaylist> SongPlaylists { get; set; }

        /// <summary>
        /// Set of albums in the database.
        /// </summary>
        public DbSet<Album> Albums { get; set; }

        /// <summary>
        /// Set of subscriptions in the database.
        /// </summary>
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption();
            modelBuilder.Entity<User>().HasMany(u => u.Albums).WithOne(a => a.Artist);
            modelBuilder.Entity<User>().HasMany(u => u.Songs).WithOne(s => s.Artist);
            modelBuilder.Entity<SongPlaylist>()
                .HasKey(sp => new { sp.SongId, sp.PlaylistId });

            modelBuilder.Entity<SongPlaylist>()
                .HasOne(sp => sp.Song)
                .WithMany(s => s.SongPlaylists)
                .HasForeignKey(sp => sp.SongId);

            modelBuilder.Entity<SongPlaylist>()
                .HasOne(sp => sp.Playlist)
                .WithMany(p => p.SongPlaylists)
                .HasForeignKey(sp => sp.PlaylistId);
            base.OnModelCreating(modelBuilder);

        }
    }
}