﻿using documentmgr.data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace documentmgr.data.Repositories.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<Type, object> _repositories;
        public ApplicationDbContext dbContext { get; }
        public UnitOfWork(ApplicationDbContext context)
        {
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IDbContextTransaction transaction;

        /// <summary>
        /// Saves the underlying changes to the database
        /// </summary>
        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }
        /// <summary>
        /// Saves the underlying changes to the database asynchronously
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Initiate Database transaction on the current context
        /// </summary>
        public void BeginTransaction()
        {
            transaction = dbContext.Database.BeginTransaction();
        }
        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        public void Rollback()
        {
            if (transaction != null)
                transaction.Rollback();
            transaction = null;
        }

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        public void Commit()
        {
            if (transaction != null)
                transaction.Commit();
            transaction = null;
        }
        /// <summary>
        /// Get current transaction
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction GetCurrentTransaction()
        {
            return transaction ?? dbContext.Database.CurrentTransaction;
        }

        /// <summary>
        /// Dynamically build a repository of <typeparamref name="TEntity"/>
        /// </summary>
        /// <typeparam name="TEntity">The repository entity class</typeparam>
        /// <returns></returns>
        public IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
                _repositories[type] = new RepositoryBase<TEntity>(dbContext);
            return (IBaseRepository<TEntity>)_repositories[type];
        }
    }
}