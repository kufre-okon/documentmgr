using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace documentmgr.data.Repositories.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        #region Sync 
        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void Delete(object id);
        void Delete(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
        void Delete(Expression<Func<TEntity, bool>> where);
        bool Any(Expression<Func<TEntity, bool>> where);
        bool All(Expression<Func<TEntity, bool>> where);

        DbSet<TEntity> GetEntity();
        int Count();

        int Count(Expression<Func<TEntity, bool>> where);

        /// <summary>
        ///  Finds an entity with the given primary key values. If an entity with the given
        ///     primary key values is being tracked by the context, then it is returned immediately
        ///     without making a request to the database. Otherwise, a query is made to the database
        ///     for an entity with the given primary key values and this entity, if found, is
        ///     attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity GetById(params object[] keyValues);
        TEntity Single(Expression<Func<TEntity, bool>> predicate = null);
        TResult Single<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        TEntity Single(Expression<Func<TEntity, bool>> predicate,
           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           bool disableTracking = true);
      
        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null);

        IEnumerable<TResult> GetList<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null) where TResult : class;

        #endregion

        #region Async

        Task<TEntity> GetByIdAsync(params object[] keyValues);

        Task<TEntity> AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null,
           bool disableTracking = true);

        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null);

        Task<IEnumerable<TResult>> GetListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null) where TResult : class;

        #endregion
    }
}
