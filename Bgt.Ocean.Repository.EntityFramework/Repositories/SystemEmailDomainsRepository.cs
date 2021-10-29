using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public class SystemEmailDomainsRepository : Repository<OceanDbEntities, TblSystemDomainEmailWhiteList>, ISystemEmailDomainsRepository
    {
        public SystemEmailDomainsRepository(IDbFactory<OceanDbEntities> dbFactory)
            : base(dbFactory)
        {
        }

        public IEnumerable<TblSystemDomainEmailWhiteList> GetListSystemDomainEmailWhiteList()
        {
            return DbContext.TblSystemDomainEmailWhiteList.ToList();
        }
    }
}