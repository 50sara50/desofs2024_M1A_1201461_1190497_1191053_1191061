using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao.Helper;
using StreamingPlatform.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StreamingPlatform.Dao
{
    /// <summary>
    /// Class that represents the database context.
    /// </summary>
    /// <param name="options"></param>
    public class StreamingDbContext(DbContextOptions<StreamingDbContext> options) : DbContext(options)
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
        /// Set of albums in the database.
        /// </summary>
        public DbSet<Album> Albums { get; set; }

        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption();
            modelBuilder.Entity<User>().HasMany(u => u.Albums).WithOne(a => a.Artist);
            modelBuilder.Entity<User>().HasMany(u => u.Songs).WithOne(s => s.Artist);
            // modelBuilder.Entity<User>(entity =>
            // {
            //     entity.OwnsOne(u => u.Password, password =>
            //     {
            //         password.Property(p => p.Value).HasColumnName("Password");
            //         password.Property(p => p.Salt).HasColumnName("Salt");
            //     });
            // });
        }
    }
}
