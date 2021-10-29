using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemServiceJobTypeRepository : IRepository<TblSystemServiceJobType>
    {
        TblSystemServiceJobType FindByTypeId(int jobTypeId);

        IQueryable<TblSystemServiceJobType> FindAllAsQueryable();

        Task<List<TblSystemServiceJobType>> FindByLastNemoSyncAsync(DateTime dateTime);
    }

    public class SystemServiceJobTypeRepository : Repository<OceanDbEntities, TblSystemServiceJobType>, ISystemServiceJobTypeRepository
    {
        public SystemServiceJobTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemServiceJobType FindByTypeId(int jobTypeId)
        {
            return DbContext.TblSystemServiceJobType.FirstOrDefault(e => e.ServiceJobTypeID == jobTypeId);
        }

        public IQueryable<TblSystemServiceJobType> FindAllAsQueryable()
        {
            return DbContext.TblSystemServiceJobType;
        }

        public async Task<List<TblSystemServiceJobType>> FindByLastNemoSyncAsync(DateTime dateTime)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                return await context.TblSystemServiceJobType.Where(o => o.FlagDisable.HasValue && !o.FlagDisable.Value && (!o.LastNemoSync.HasValue || o.LastNemoSync < dateTime)).ToListAsync();
            }
        }
    }
}
