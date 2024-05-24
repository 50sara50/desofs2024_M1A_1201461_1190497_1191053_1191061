using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Interfaces
{
    public interface ISongRepository
    {
        public Task<Song> GetById(Guid id);
    }
}