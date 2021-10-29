using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public abstract class Repository<TDbContext, TEntity>
            where TEntity : class
            where TDbContext : DbContext
    {
        protected IDbFactory<TDbContext> DbFactory
        {
            get;
            private set;
        }

        protected TDbContext DbContext
        {
            get
            {
                return DbFactory.GetCurrentDbContext;
            }
        }
        protected IDbConnection DbContextConnection {
            get { return DbContext.Database.Connection; }
        }

        protected Repository(IDbFactory<TDbContext> dbFactory)
        {
            DbFactory = dbFactory;
        }

        /// <summary>
        /// This will create another instance of OceanEntities
        /// </summary>
        protected OceanDbEntities NewDbContext() => DbFactory.GetNewDataContext;

        #region Create
        public virtual void Create(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
        }

        public async Task CreateAsync(TEntity entity)
        {
            using (var db = NewDbContext())
            {
                db.Set<TEntity>().Add(entity);
                await db.SaveChangesAsync();
            }
        }

        public virtual void CreateRange(IEnumerable<TEntity> entityList)
        {
            DbContext.Set<TEntity>().AddRange(entityList);
        }

        public void VirtualCreate(TEntity entity)
        {
            if (VirtualCreateList == null)
                VirtualCreateList = new List<TEntity>();

            VirtualCreateList.Add(entity);
        }

        public void VirtualCreateRange(IEnumerable<TEntity> entityList)
        {
            if (VirtualCreateList == null)
                VirtualCreateList = new List<TEntity>();

            VirtualCreateList.AddRange(entityList);
        }

        protected List<TEntity> VirtualCreateList { get; set; } = new List<TEntity>();

        public void CreateVirtualRangeToDbContext()
        {
            if (VirtualCreateList != null)
                DbContext.Set<TEntity>().AddRange(VirtualCreateList);
            VirtualCreateList = null;
        }
        #endregion

        #region Edit
        public virtual void Modify(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }
        #endregion

        #region Remove
        public virtual void Remove(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entityList)
        {
            DbContext.Set<TEntity>().RemoveRange(entityList);
        }

        public void VirtualRemove(TEntity entity)
        {
            if (VirtualRemoveList == null)
                VirtualRemoveList = new List<TEntity>();

            VirtualRemoveList.Add(entity);
        }

        public void VirtualRemoveRange(IEnumerable<TEntity> entityList)
        {
            if (VirtualRemoveList == null)
                VirtualRemoveList = new List<TEntity>();

            VirtualRemoveList.AddRange(entityList);
        }

        protected List<TEntity> VirtualRemoveList { get; set; } = new List<TEntity>();

        public void RemoveVirtualRangeToDbContext()
        {
            if (VirtualRemoveList != null)
                DbContext.Set<TEntity>().RemoveRange(VirtualRemoveList);
            VirtualRemoveList = null;
        }
        #endregion

        #region Select
        public virtual TEntity FindById(object id)
        {
            return DbContext.Set<TEntity>().Find(id);
        }

        public virtual IEnumerable<TEntity> FindAll()
        {
            return DbContext.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> FindAll(Func<TEntity, bool> predicate)
        {
            return DbContext.Set<TEntity>().Where(predicate);
        }

        public virtual IQueryable<TEntity> FindAllAsQueryable()
            => DbContext.Set<TEntity>().AsQueryable();

        public virtual IQueryable<TEntity> FindAllAsQueryable(Expression<Func<TEntity, bool>> predicate)
            => DbContext.Set<TEntity>().Where(predicate);

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> predicate)
            => DbContext.Set<TEntity>().FirstOrDefault(predicate);

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            if(predicate == null)
            {
                return DbContext.Set<TEntity>().Any();
            }
            else
            {
                return DbContext.Set<TEntity>().Any(predicate);
            }
            
        }
        #endregion
    }
}
