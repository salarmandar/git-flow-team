using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemTripIndicatorRepository : IRepository<TblSystemTripIndicator>
    {
        IEnumerable<TblSystemTripIndicator> GetTripIndicatorByCountryGuid(Guid countryGuid);
    }
    public class SystemTripIndicatorRepository : Repository<OceanDbEntities, TblSystemTripIndicator>, ISystemTripIndicatorRepository
    {
        public SystemTripIndicatorRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblSystemTripIndicator> GetTripIndicatorByCountryGuid(Guid countryGuid)
        {
            return DbContext.TblSystemTripIndicator.Where(o => o.MasterCountry_Guid == countryGuid && o.FlagDisable != true);
        }
    }
}
