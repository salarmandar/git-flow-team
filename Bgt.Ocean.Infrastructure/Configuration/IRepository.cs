using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bgt.Ocean.Infrastructure.Configuration
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        void Create(TEntity entity);
        void CreateRange(IEnumerable<TEntity> entityList);
        /// <summary>
        /// Please be aware, this method will create new Dbcontext and immediately save change
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync(TEntity entity);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entityList);
        void Modify(TEntity entity);


        #region virtual range management
        void VirtualCreate(TEntity entity);
        void VirtualCreateRange(IEnumerable<TEntity> entityList);
        void VirtualRemove(TEntity entity);
        void VirtualRemoveRange(IEnumerable<TEntity> entityList);
        void CreateVirtualRangeToDbContext();
        void RemoveVirtualRangeToDbContext();
        #endregion
    }
}
