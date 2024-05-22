using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Interfaces
{
    public interface IPlaylistRepository
    {
        public Task<Playlist> GetById(Guid id);
        
        public Task<IEnumerable<Playlist>> GetByUserId(Guid id);
    }
}