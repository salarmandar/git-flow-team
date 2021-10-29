using Bgt.Ocean.Infrastructure.Configuration;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.SystemConfigurationAdditional
{
    public interface ISystemEmailDomainsRepository : IRepository<TblSystemDomainEmailWhiteList>
    {
        IEnumerable<TblSystemDomainEmailWhiteList> GetListSystemDomainEmailWhiteList();
    }
}
