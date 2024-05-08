using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao.Helper;
using StreamingPlatform.Dao.Interfaces;

namespace StreamingPlatform.Dao.Repositories
{
    /// <summary>
    /// Generic repository for CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">the entity to which the CRUD operations will be done</typeparam>
    /// <param name="context">the database context</param>
    public class GenericRepository<TEntity>(StreamingDbContext context) : IGenericRepository<TEntity>
        where TEntity : class
    {
        private readonly StreamingDbContext context = context;
        private readonly DbSet<TEntity> dbSet = context.Set<TEntity>();

        /// <summary>
        /// Retrieves all records from the database.
        /// </summary>
        /// <param name="tracked">
        ///     Indicates whether changes detected in the entity should be persisted to the database during SaveChanges.
        ///     Set to true to enable change tracking (default is false).
        /// </param>
        /// <returns>
        ///     An enumerable collection of entities representing the retrieved records.
        /// </returns>
        public virtual IEnumerable<TEntity> GetAllRecords(bool tracked = false)
        {
            if (tracked)
            {
                return this.dbSet.ToList();
            }

            return this.dbSet.AsNoTracking().ToList();
        }

        /// <summary>
        /// Retrieves a paginated list of records from the database.
        /// </summary>
        /// <param name="numberOfRecords">The maximum number of records to retrieve per page.</param>
        /// <param name="pageNumber">The page number of the paginated results (1-based index).</param>
        /// <param name="tracked">
        ///     Indicates whether changes detected in the entity should be persisted to the database during SaveChanges.
        ///     Set to true to enable change tracking (default is false).
        /// </param>
        /// <returns>
        ///     A <see cref="PagedResponseOffset{TEntity}"/> object representing the paginated list of retrieved records.
        /// </returns>
        public virtual PagedResponseOffset<TEntity> GetAllRecords(int numberOfRecords, int pageNumber, bool tracked = false)
        {
            int totalNumberOfRecords = this.dbSet.AsNoTracking().Count();
            IQueryable<TEntity> query = this.dbSet.AsNoTracking();
            if (tracked)
            {
                query = this.dbSet;
            }

            return this.GetPagedResponse(query, numberOfRecords, pageNumber, totalNumberOfRecords);
        }

        /// <summary>
        /// Get all records from the database asynchronously.
        /// </summary>
        /// <param name="tracked">indicates if changes detected in the entity are persisted to the database during SaveChanges</param>
        /// <returns>the list of records</returns>
        public virtual async Task<PagedResponseOffset<TEntity>> GetAllRecordsAsync(int numberOfRecords, int pageNumber, bool tracked = false)
        {
            int totalNumberOfRecords = await this.dbSet.AsNoTracking().CountAsync();
            IQueryable<TEntity> query = this.dbSet.AsNoTracking();

            if (tracked)
            {
                query = this.dbSet;
            }

            return await this.GetPagedResponseAsync(query, numberOfRecords, pageNumber, totalNumberOfRecords);
        }

        /// <summary>
        /// Get all records from the database asynchronously.
        /// </summary>
        /// <param name="tracked">indicates if changes detected in the entity are persisted to the database during SaveChanges</param>
        /// <returns>the list of records</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllRecordsAsync(bool tracked = false)
        {
            if (tracked)
            {
                return await this.dbSet.ToListAsync();
            }

            return await this.dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Gets a paginated list of records from the database asynchronously.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to return per page.</param>
        /// <param name="pageNumber">The page number (starting from 1) to retrieve.</param>
        /// <param name="tracked">Indicates if changes detected in the entity are persisted to the database during SaveChanges.</param>
        /// <returns>A PagedResponseOffset containing the requested page of data and total record count.</returns>
        public virtual PagedResponseOffset<TEntity> GetRecords(Expression<Func<TEntity, bool>> filter, int numberOfRecords, int pageNumber, bool tracked = false)
        {
            int totalNumberOfRecords = this.dbSet.AsNoTracking().Count();

            IQueryable<TEntity> query = this.dbSet.Where(filter).AsNoTracking();
            if (tracked)
            {
                query = this.dbSet.Where(filter);
            }

            return this.GetPagedResponse(query, numberOfRecords, pageNumber, totalNumberOfRecords);
        }

        /// <summary>
        /// Get records from the database based on the given filtering expression.
        /// </summary>
        /// <param name="filter">he filter being applied</param>
        /// <param name="tracked">indicates if changes detected in the entity are persisted to the database during SaveChanges</param>
        /// <returns>the list of records retrieved from the database</returns>
        public virtual IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> filter, bool tracked = false)
        {

            IQueryable<TEntity> query = this.dbSet.Where(filter).AsNoTracking();
            if (tracked)
            {
                query = this.dbSet.Where(filter);
            }

            return query.ToList();
        }

        /// <summary>
        /// Retrieves records from the database asynchronously based on the specified filter and pagination settings.
        /// </summary>
        /// <param name="filter">The filter expression to apply.
        /// </param>
        /// <param name="tracked">Indicates whether entity changes should be tracked.</param>
        /// <returns>A list of records retrieved from the database
        /// .</returns>
        public virtual async Task<IEnumerable<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> filter, bool tracked = false)
        {
            IQueryable<TEntity> query = this.dbSet.Where(filter).AsNoTracking();
            if (tracked)
            {
                query = this.dbSet.Where(filter);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves records from the database asynchronously based on the specified filter and pagination settings.
        /// </summary>
        /// <param name="filter">The filter expression to apply.</param>
        /// <param name="tracked">Indicates whether entity changes should be tracked.</param>
        /// <param name="numberOfRecords">The number of records to retrieve per page (optional).</param>
        /// <param name="pageNumber">The current page number (1-based) for pagination (optional).</param>
        /// <returns>A PagedResponseOffset containing the requested page of data and total record count.
        /// </returns>
        public virtual async Task<PagedResponseOffset<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> filter, int numberOfRecords, int pageNumber, bool tracked = false)
        {
            int totalNumberOfRecords = await this.dbSet.AsNoTracking().CountAsync();
            IQueryable<TEntity> query = this.dbSet.Where(filter).AsNoTracking();
            if (tracked)
            {
                query = this.dbSet.Where(filter);
            }

            return await this.GetPagedResponseAsync(query, numberOfRecords, pageNumber, totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieves a record from the database based on the given filtering expression.
        /// </summary>
        /// <param name="filter">he filter being applied</param>
        /// <returns>null if no recordis found return null. Otherwise returns the first record that matches the filter expression</returns>
        public virtual TEntity? GetRecord(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = this.dbSet.Where(filter);
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a record from the database based on the given filtering expression asynchronously.
        /// </summary>
        /// <param name="filter">he filter being applied</param>
        /// <returns>null if no recordis found return null. Otherwise returns the first record that matches the filter expression</returns>
        public virtual async Task<TEntity?> GetRecordAsync(Expression<Func<TEntity, bool>> expression)
        {
            IQueryable<TEntity> query = this.dbSet.Where(expression);
            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a record from the database based on the given id.
        /// </summary>
        /// <param name="id">the id of the record</param>
        /// <returns>null if no recordis found return null. Otherwise returns the record retrieved</returns>
        public virtual TEntity? GetRecordById(object id)
        {
            return this.dbSet.Find(id);
        }

        /// <summary>
        /// Retrieves a record from the database based on the given id.
        /// </summary>
        /// <param name="id">the id of the record</param>
        /// <returns>null if no recordis found return null. Otherwise returns the record retrieved</returns>
        public virtual async Task<TEntity?> GetRecordByIdAsync(object id)
        {
            return await this.dbSet.FindAsync(id);
        }

        /// <summary>
        /// Creates a new record in the database.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Create(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        /// <summary>
        /// Updates a record in the database.
        /// </summary>
        /// <param name="entityToUpdate">the entity to update</param>
        public virtual void Update(TEntity entityToUpdate)
        {
            this.dbSet.Attach(entityToUpdate);
            this.context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a record from the database if there is a record with the given id.
        /// </summary>
        /// <param name="id">the id of the record </param>
        public virtual void Delete(object id)
        {
            TEntity? entityToDelete = this.dbSet.Find(id);
            if (entityToDelete != null)
            {
                this.Delete(entityToDelete);
            }
        }

        /// <summary>
        /// Deletes a record from the database.
        /// </summary>
        /// <param name="entityToDelete">the record to delete</param>
        public virtual void Delete(TEntity entityToDelete)
        {
            EntityState entityState = this.context.Entry(entityToDelete).State;
            if (entityState == EntityState.Detached)
            {
                this.dbSet.Attach(entityToDelete);
            }

            this.dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Applies pagination to the given query based on the provided pagination parameters.
        /// </summary>
        /// <param name="numberOfRecords">The number of records to retrieve per page.</param>
        /// <param name="pageNumber">The current page number (1-based).</param>
        /// <param name="query">The IQueryable query to be paginated.</param>
        /// <returns>The paginated IQueryable query.</returns>
        protected IQueryable<TEntity> ApplyPagination(int numberOfRecords, int pageNumber, IQueryable<TEntity> query)
        {

            int recordsToSkip = (pageNumber - 1) * numberOfRecords;
            query = query.Skip(recordsToSkip).Take(numberOfRecords);
            return query;
        }

        /// <summary>
        /// Retrieves a paged response asynchronously based on the specified query and pagination parameters.
        /// </summary>
        /// <param name="query">The query to retrieve the records.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="totalNumberOfRecords">The total number of records in the database.</param>
        /// <returns>A task representing the asynchronous operation that returns the paged response object.</returns>

        private async Task<PagedResponseOffset<TEntity>> GetPagedResponseAsync(IQueryable<TEntity> query, int pageSize, int currentPage, int totalNumberOfRecords)
        {
            query = this.ApplyPagination(pageSize, currentPage, query);
            List<TEntity> response = await query.ToListAsync();

            return new PagedResponseOffset<TEntity>(response, currentPage, pageSize, totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieves a paged response synchronously based on the specified query and pagination parameters.
        /// </summary>
        /// <param name="query">The query to retrieve the records.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="totalNumberOfRecords">The total number of records in the database.</param>
        /// <returns>The paged response object.</returns>
        private PagedResponseOffset<TEntity> GetPagedResponse(IQueryable<TEntity> query, int pageSize, int currentPage, int totalNumberOfRecords)
        {
            query = this.ApplyPagination(pageSize, currentPage, query);
            List<TEntity> response = query.ToList();
            return new PagedResponseOffset<TEntity>(response, currentPage, pageSize, totalNumberOfRecords);
        }
    }
}