using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation
{
    public interface ISystemConAndDeconsolidateStatusRepository : IRepository<TblSystemConAndDeconsolidateStatus>
    {
        Guid GetConsolidateStatus(int statusID);
    }

    public class SystemConAndDeconsolidateStatusRepository : Repository<OceanDbEntities, TblSystemConAndDeconsolidateStatus>, ISystemConAndDeconsolidateStatusRepository
    {
        public SystemConAndDeconsolidateStatusRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public Guid GetConsolidateStatus(int statusID )
        {
            return DbContext.TblSystemConAndDeconsolidateStatus.FirstOrDefault(o=>o.StatusID==statusID).Guid;
        }
    }

}
