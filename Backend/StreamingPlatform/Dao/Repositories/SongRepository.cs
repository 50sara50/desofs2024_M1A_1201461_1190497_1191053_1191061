using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Repositories
{
    public class SongRepository(StreamingDbContext context) : GenericRepository<Song>(context)
    {

        private readonly StreamingDbContext context = context;


        /// <summary>
        /// Gets a song by id.
        /// </summary>
        /// <param name="id">the song id</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Song?> GetSongByIdWithUserAndAlbumData(Guid id)
        {
            return await this.context.Songs.Where(
                           x => id.Equals(x.Id))
                        .Include(x => x.Artist)
                        .Include(x => x.Album)
                       .FirstOrDefaultAsync();
        }
    }
}