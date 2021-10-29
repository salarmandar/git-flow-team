using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterCountry_StateRepository : IRepository<TblMasterCountry_State>
    {
        IEnumerable<TblMasterCountry_State> FindByCountry(Guid countryGuid);
    }

    public class MasterCountry_StateRepository : Repository<OceanDbEntities, TblMasterCountry_State>, IMasterCountry_StateRepository
    {
        public MasterCountry_StateRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterCountry_State> FindByCountry(Guid countryGuid)
        {
            return DbContext.TblMasterCountry_State.Where(x => x.MasterCountry_Guid == countryGuid && !x.FlagDisable).OrderBy(e => e.MasterStateName);
        }
    }
}
