using StreamingPlatform.Dao.Interfaces;

namespace StreamingPlatform.Dao.Repositories
{
    /// <summary>
    /// Class that represents a Unit of Work that keeps track of everything you do during a business transaction that can affect the database
    /// </summary>
    /// <param name="context">the database context</param>
    public class UnitOfWork(StreamingDbContext context) : IUnitOfWork, IDisposable
    {
        private readonly StreamingDbContext context = context;

        private bool disposed = false;

        /// <summary>
        /// Performs the cleanup of the context.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public StreamingDbContext GetContext()
        {
            return this.context;
        }
        /// <summary>
        /// Creates a repository for the given entity.
        /// </summary>
        /// <typeparam name="TEntity">the type of entity </typeparam>
        /// <returns>the new repository class</returns>
        public IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class
        {
            return new GenericRepository<TEntity>(this.context);
        }

        /// <summary>
        /// Saves the changes made to the database.
        /// </summary>
        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        /// <summary>
        /// Saves the changes made to the database asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Perfomrs the cleanup of the context.
        /// </summary>
        /// <param name="disposing">indicates if we are doing the cleanup or not</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}
