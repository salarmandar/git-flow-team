using Bgt.Ocean.Models;
using System;
using System.Configuration;
using System.Data.Entity;

namespace Bgt.Ocean.Repository.EntityFramework
{
    public interface IDbFactory<out TDbContext>
    {
        TDbContext GetCurrentDbContext { get; }
        void ForceDispose();
        OceanDbEntities GetNewDataContext { get; }
    }

    public abstract class DbFactory<TDbContext> : Disposable, IDbFactory<TDbContext>
        where TDbContext : DbContext, new()
    {
        TDbContext dbContext;
        public TDbContext GetCurrentDbContext
        {
            get
            {
                dbContext = dbContext ?? NewDbContext();
                if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                    dbContext.Database.Connection.Open();
#if DEBUG
                SetLog(dbContext);
#endif

                return dbContext;
            }
        }

        protected abstract TDbContext NewDbContext();

        protected bool FlagUseSTG
        {
            get
            {
                string staging = ConfigurationManager.AppSettings["EnvSTG"];
                return Convert.ToBoolean(staging);
            }
        }

        public void ForceDispose()
        {
            DisposeCore();
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
                dbContext = null;
            }
        }

        private void SetLog(DbContext db)
            => db.Database.Log = l => System.Diagnostics.Debug.Write(l);

        /// <summary>
        /// get new dbContext
        /// </summary>
        public OceanDbEntities GetNewDataContext
        {
            get
            {
                var db = new OceanDbEntities(FlagUseSTG);
                SetLog(db);
                return db;
            }
        }
    }
}
