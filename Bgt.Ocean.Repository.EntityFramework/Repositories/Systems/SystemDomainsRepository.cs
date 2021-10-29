using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemDomainsRepository : IRepository<TblSystemDomain>
    {
        TblSystemDomainDC FindDomainDCByAiles(Guid ailesGuid);
        TblSystemDomainAiles FindAilesByName(string ailesName);
    }

    public class SystemDomainsRepository : Repository<OceanDbEntities, TblSystemDomain>, ISystemDomainsRepository
    {
        public SystemDomainsRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemDomainAiles FindAilesByName(string ailesName)
        {
            return DbContext.TblSystemDomainAiles.FirstOrDefault(e => e.AilesName.ToLower().Equals(ailesName) && e.FlagDisable == false);
        }

        public TblSystemDomainDC FindDomainDCByAiles(Guid ailesGuid)
        {
            return DbContext.TblSystemDomainDC.FirstOrDefault(e => e.SystemDomain_Guid == ailesGuid);
        }
    }
}
