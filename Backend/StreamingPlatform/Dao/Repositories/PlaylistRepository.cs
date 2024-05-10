using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Repositories
{
    public class PlaylistRepository
    {
        private readonly StreamingDbContext _context;

        public PlaylistRepository(StreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a playlist by id.
        /// </summary>
        /// <param name="id">the playlist id</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Playlist> GetById(Guid id)
        {
            return await _context.Playlists.Where(
                           x => id.Equals(x.Id))
                       .FirstOrDefaultAsync() ??
                   throw new InvalidOperationException($"There is no playlist with the id '${id}'.");
        }

        /// <summary>
        /// Gets playlists by user id.
        /// </summary>
        /// <param name="id">the identifier of the owner</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IEnumerable<Playlist>> GetByUserId(Guid id)
        {
            return await _context.Playlists.Where(x => id.Equals(x.UserId))
                       .ToListAsync() ??
                   throw new InvalidOperationException($"The user '${id}' doesn't have any playlist.");
        }
    }
}