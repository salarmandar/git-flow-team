using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemEnvironment_GlobalRepository : IRepository<TblSystemEnvironment_Global>
    {
        TblSystemEnvironment_Global FindByAppKey(string appKey);
        CountryOptionResult Func_CountryOption_Get(string appKey, Guid? siteGuid, Guid? countryGuid);
        int FindSystemLockCount();
        IEnumerable<TblSystemEnvironment_Global> GetListSystemEnviroment();

        Task<TblSystemEnvironment_Global> FindByAppKeyAsync(string appKey);
    }

    public class SystemEnvironment_GlobalRepository : Repository<OceanDbEntities, TblSystemEnvironment_Global>, ISystemEnvironment_GlobalRepository
    {
        public SystemEnvironment_GlobalRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemEnvironment_Global FindByAppKey(string appKey)
        {
            return DbContext.TblSystemEnvironment_Global.FirstOrDefault(o => o.AppKey == appKey);
        }

        public CountryOptionResult Func_CountryOption_Get(string appKey, Guid? siteGuid, Guid? countryGuid)
        {
            return DbContext.Up_OceanOnlineMVC_CountryOption_Get(appKey, siteGuid, countryGuid).FirstOrDefault();
        }

        public int FindSystemLockCount()
        {
            var lockedRecord = FindAllAsQueryable(e => e.AppKey == "InvalidAttempLogin").FirstOrDefault();
            return int.Parse(lockedRecord.AppValue1);
        }

        public IEnumerable<TblSystemEnvironment_Global> GetListSystemEnviroment()
        {
            return DbContext.TblSystemEnvironment_Global.ToList();
        }

        #region Async 
        public async Task<TblSystemEnvironment_Global> FindByAppKeyAsync(string appKey)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                return await context.TblSystemEnvironment_Global.FirstOrDefaultAsync(o => o.AppKey == appKey);
            }
        }
        #endregion
    }
}
