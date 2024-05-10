using System.Linq.Expressions;
using StreamingPlatform.Models;

namespace StreamingPlatform.Dao.Interfaces
{
    /// <summary>
    /// Interface for the generic repository.
    /// </summary>
    /// <typeparam name="TEntity">the type of record being accessed</typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllRecordsAsync(bool tracked = false);

        List<TEntity> GetRecords(Expression<Func<TEntity, bool>> expression, bool tracked = false, int? numberOfRecords = null, int? pageNumber = null);

        Task<List<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> expression, bool tracked = false, int? numberOfRecords = null, int? pageNumber = null);

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
