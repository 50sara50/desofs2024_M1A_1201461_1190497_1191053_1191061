using Microsoft.EntityFrameworkCore;
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
        /// Set of albums in the database.
        /// </summary>
        public DbSet<Album> Albums { get; set; }
    }
}
