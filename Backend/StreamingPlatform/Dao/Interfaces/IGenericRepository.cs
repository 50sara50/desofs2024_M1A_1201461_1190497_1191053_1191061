using System.Linq.Expressions;
using StreamingPlatform.Dao.Helper;

namespace StreamingPlatform.Dao.Interfaces
{
    /// <summary>
    /// Interface for the generic repository.
    /// </summary>
    /// <typeparam name="TEntity">the type of record being accessed</typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> GetAllRecords(bool tracked = false);

        PagedResponseOffset<TEntity> GetAllRecords(int numberOfRecords, int pageNumber, bool tracked = false);

        Task<IEnumerable<TEntity>> GetAllRecordsAsync(bool tracked = false);

        Task<PagedResponseOffset<TEntity>> GetAllRecordsAsync(int numberOfRecords, int pageNumber, bool tracked = false);

        PagedResponseOffset<TEntity> GetRecords(Expression<Func<TEntity, bool>> expression, int numberOfRecords, int pageNumber, bool tracked = false);

        IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> expression, bool tracked = false);

        Task<PagedResponseOffset<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> expression, int numberOfRecords, int pageNumber, bool tracked = false);

        Task<IEnumerable<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> expression, bool tracked = false);

        TEntity? GetRecord(Expression<Func<TEntity, bool>> expression);

        Task<TEntity?> GetRecordAsync(Expression<Func<TEntity, bool>> expression);

        TEntity? GetRecordById(object id);

        Task<TEntity?> GetRecordByIdAsync(object id);

        void Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entity);
    }
}
