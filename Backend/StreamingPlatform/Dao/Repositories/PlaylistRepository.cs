using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Repositories
{
    public class PlaylistRepository(StreamingDbContext streamingDbContext) : GenericRepository<Playlist>(streamingDbContext)
    {
        private readonly StreamingDbContext context = streamingDbContext;

        /// <summary>
        /// Gets a playlist by id.
        /// </summary>
        /// <param name="id">the playlist id</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Playlist> GetById(Guid id)
        {
            return await this.context.Playlists.Where(
                           x => id.Equals(x.Id))
                       .FirstOrDefaultAsync() ??
                   throw new InvalidOperationException($"There is no playlist with the id '${id}'.");
        }

        public async Task<IEnumerable<Playlist>> GetUserPlaylists(Guid userId)
        {
            return await this.context.Playlists.Where(x => userId.Equals(x.UserId)).Include(x => x.SongPlaylists)
                       .ToListAsync() ??
                   throw new InvalidOperationException($"The user '${userId}' doesn't have any playlist.");
        }

        public async Task<Playlist?> GetRecordByIdAsync(Guid id)
        {
            return await this.context.Playlists.Where(x => id.Equals(x.Id)).Include(x => x.SongPlaylists)
                       .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets playlists by user id.
        /// </summary>
        /// <param name="id">the identifier of the owner</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IEnumerable<Playlist>> GetByUserId(Guid id)
        {
            return await this.context.Playlists.Where(x => id.Equals(x.UserId))
                       .ToListAsync() ??
                   throw new InvalidOperationException($"The user '${id}' doesn't have any playlist.");
        }
    }
}