using documentmgr.data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace documentmgr.data.Repositories.Infrastructure
{
    public class RepositoryBase<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryBase(DbContext context)
        {
            _dbContext = context ?? throw new ArgumentException(nameof(context));
            _dbSet = _dbContext.Set<TEntity>();
        }

        //var query2 = query;
        //var hmm = query2.Select(selector);
        //var q = orderBy != null ? orderBy(hmm) : hmm;
        //var resulte = hmm.ToSql();

        public DbSet<TEntity> GetEntity()
        {
            return _dbSet;
        }

        #region non async
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public void Delete(TEntity existing)
        {
            if (existing != null) _dbSet.Remove(existing);
        }

        public void Delete(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
                Delete(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = _dbSet.Where(predicate).ToList();
            entities.ForEach(e => _dbSet.Remove(e));
        }


        /// <summary>
        ///  Finds an entity with the given primary key values. If an entity with the given
        ///     primary key values is being tracked by the context, then it is returned immediately
        ///     without making a request to the database. Otherwise, a query is made to the database
        ///     for an entity with the given primary key values and this entity, if found, is
        ///     attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public TEntity GetById(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);

            return query.FirstOrDefault();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null && includes.Any())
            {
                query = handleIncludes(includes, query);
            }

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).FirstOrDefault();
            return query.FirstOrDefault();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public TResult Single<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var result = query.Select(selector);
            return result.FirstOrDefault();

        }
        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Any())
            {
                query = handleIncludes(includes, query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return (orderBy != null ? orderBy(query) : query).ToList();
        }

        public IEnumerable<TResult> GetList<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Any())
            {
                query = handleIncludes(includes, query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            var result = query.Select(selector);
            return orderBy != null ? orderBy(result).ToList() : result.ToList();
        }

        public bool Any(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Any(where);
        }

        public bool All(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.All(where);
        }

        public int Count(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where).Count();
        }

        #endregion

        #region async
        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Any())
            {
                query = handleIncludes(includes, query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            //  var query2 = query;
            //  var resulte = query2.ToSql();

            return await (orderBy != null ? orderBy(query).ToListAsync() : query.ToListAsync());
        }

        public async Task<IEnumerable<TResult>> GetListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Any())
            {
                query = handleIncludes(includes, query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            var result = query.Select(selector);
            return await (orderBy != null ? orderBy(result).ToListAsync() : result.ToListAsync());
        }

        private static IQueryable<TEntity> handleIncludes(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes, IQueryable<TEntity> query)
        {
            foreach (var include in includes)
                query = include(query);
            return query;
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null && includes.Any())
            {
                query = handleIncludes(includes, query);
            }

            if (predicate != null) query = query.Where(predicate);

            // var query2 = query;
            // var resulte = query2.ToSql();

            if (orderBy != null)
                return orderBy(query).FirstOrDefault();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetByIdAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }
        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbSet.Where(where).CountAsync();
        }

        #endregion
    }
}