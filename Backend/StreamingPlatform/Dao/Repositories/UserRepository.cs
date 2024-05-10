using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StreamingDbContext _context;

        //TODO: set context to users
        public UserRepository(StreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns user by its id.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<User> GetById(Guid id)
        {
            return await _context.Users.Where(
                           x => id.Equals(x.Id))
                       .FirstOrDefaultAsync() ??
                   throw new InvalidOperationException($"There is no user with the id '${id}'.");
        }

        /// <summary>
        /// Returns all existing users.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Returns an user by its email.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users.Where(
                x => email.Equals(x.Email))
                       .FirstOrDefaultAsync() ??
                   throw new InvalidOperationException($"There is no user with the email '${email}'.");
        }
    }
}