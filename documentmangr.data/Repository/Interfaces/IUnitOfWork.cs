using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace documentmgr.data.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves the underlying changes to the database asynchronously
        /// </summary>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Initiate Database transaction on the current context
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// Commits the current transaction
        /// </summary>
        void Commit();
        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        void Rollback();

        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        int SaveChanges();

        IDbContextTransaction GetCurrentTransaction();
    }
}
