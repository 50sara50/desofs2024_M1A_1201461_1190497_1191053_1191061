namespace StreamingPlatform.Dao.Interfaces
{
    /// <summary>
    /// Interface that represents a Unit of Work that keeps track of everything you do during a business transaction that can affect the database
    /// </summary>
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();

        void SaveChanges();
        StreamingDbContext GetContext();
        IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class;
    }
}
