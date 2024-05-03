using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao
{
    public class StreamingDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Plan> Plans { get; set; }
        
        public DbSet<Song> Songs { get; set; }

        public DbSet<Album> Albums { get; set; }
    }
}
