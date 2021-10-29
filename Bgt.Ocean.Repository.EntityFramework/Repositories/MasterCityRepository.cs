using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterCityRepository : IRepository<TblMasterCity>
    {
        IEnumerable<TblMasterCity> FineByCountry(Guid contryGuid);
    }

    public class MasterCityRepository : Repository<OceanDbEntities, TblMasterCity>, IMasterCityRepository
    {
        public MasterCityRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterCity> FineByCountry(Guid contryGuid)
        {
            return DbContext.TblMasterCity.Where(x => x.MasterCountry_Guid == contryGuid && !x.FlagDisable).OrderBy(e => e.MasterCityName);
        }
    }
}
