using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Repositories
{
    public class SongRepository
    {
        
        private readonly StreamingDbContext _context;

        public SongRepository(StreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a song by id.
        /// </summary>
        /// <param name="id">the song id</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Song> GetById(Guid id)
        {
            return await _context.Songs.Where(
                           x => id.Equals(x.Id))
                       .FirstOrDefaultAsync() ??
                   throw new InvalidOperationException($"There is no song with the id '${id}'.");
        }
    }
}