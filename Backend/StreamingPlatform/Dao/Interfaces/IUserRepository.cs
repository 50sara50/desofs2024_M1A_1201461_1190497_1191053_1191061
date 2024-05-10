using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Interfaces
{
    public interface IUserRepository 
    {
        public Task<User> GetById(Guid id);
        
        public Task<IEnumerable<User>> GetAll();

        public Task<User> GetByEmail(string email);
    }
}