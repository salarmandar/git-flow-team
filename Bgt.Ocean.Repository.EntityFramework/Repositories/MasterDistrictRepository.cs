using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterDistrictRepository : IRepository<TblMasterDistrict>
    {
        IEnumerable<TblMasterDistrict> FindByProvinceState(Guid provinceStateGuid);
    }

    public class MasterDistrictRepository : Repository<OceanDbEntities, TblMasterDistrict>, IMasterDistrictRepository
    {
        public MasterDistrictRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterDistrict> FindByProvinceState(Guid provinceStateGuid)
        {
            return DbContext.TblMasterDistrict
                .Where(e => (e.MasterCity_Guid == provinceStateGuid || e.MasterCountry_State_Guid == provinceStateGuid) && !e.FlagDisable)
                .OrderBy(x => x.MasterDistrictName);
        }
    }
}
