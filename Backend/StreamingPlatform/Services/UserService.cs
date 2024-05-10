using StreamingPlatform.Dao;
using StreamingPlatform.Models;

namespace StreamingPlatform
{
    public class UserService : IUserService
    {
        private readonly StreamingDbContext _context;

        public UserService(StreamingDbContext context)
        {
            this._context = context;
        }
        
        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<NewUserDto> GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}