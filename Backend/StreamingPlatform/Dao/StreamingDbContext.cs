using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao.Helper;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao
{
    /// <summary>
    /// Class that represents the database context.
    /// </summary>
    /// <param name="options"></param>
    public class StreamingDbContext(DbContextOptions options) : DbContext(options)
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
        /// Set of users in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Set of subscriptions in the database.
        /// </summary>
        public DbSet<Subscription> Subscriptions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption();
            modelBuilder.Entity<User>(entity =>
            {
                entity.OwnsOne(u => u.Password, password =>
                {
                    password.Property(p => p.Value).HasColumnName("Password");
                    password.Property(p => p.Salt).HasColumnName("Salt");
                });
            });

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
        }
    }
}
