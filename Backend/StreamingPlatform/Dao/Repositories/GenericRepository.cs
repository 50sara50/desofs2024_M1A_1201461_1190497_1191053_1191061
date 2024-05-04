using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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
        /// Get all records from the database.
        /// </summary>
        /// <param name="tracked">indicates if changes detected in the entity are persisted to the database during SaveChanges</param>
        /// <returns>the list of records</returns>
        public virtual IEnumerable<TEntity> GetAllRecords(bool tracked = false)
        {
            if (tracked)
            {
                return this.dbSet.ToList();
            }

            return this.dbSet.AsNoTracking().ToList();
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
        /// Get records from the database based on the given filtering expression.
        /// </summary>
        /// <param name="filter">he filter being applied</param>
        /// <param name="tracked">indicates if changes detected in the entity are persisted to the database during SaveChanges</param>
        /// <returns>the list of records retrieved from the database</returns>
        public virtual List<TEntity> GetRecords(Expression<Func<TEntity, bool>> filter, bool tracked = false)
        {
            if (tracked)
            {
                return this.dbSet.Where(filter).ToList();
            }

            return this.dbSet.Where(filter).AsNoTracking().ToList();
        }

        /// <summary>
        /// Get records from the database based on the given filtering expression asynchronously.
        /// </summary>
        /// <param name="filter">he filter being applied</param>
        /// <param name="tracked">indicates if changes detected in the entity are persisted to the database during SaveChanges</param>
        /// <returns>the list of records retrieved from the database</returns>
        public virtual async Task<List<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> filter, bool tracked = false)
        {
            if (tracked)
            {
                return await this.dbSet.Where(filter).ToListAsync();
            }

            return await this.dbSet.Where(filter).AsNoTracking().ToListAsync();
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
    }
}