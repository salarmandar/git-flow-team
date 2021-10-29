using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface ISystemEnvironmentMasterCountryValueRepository : IRepository<TblSystemEnvironmentMasterCountryValue>
    {
        TblSystemEnvironmentMasterCountryValue GetSpecificKeyByCountryAndKey(Guid countryGuid, string appKey);
    }
    public class SystemEnvironmentMasterCountryValueRepository : Repository<OceanDbEntities, TblSystemEnvironmentMasterCountryValue>, ISystemEnvironmentMasterCountryValueRepository
    {
        public SystemEnvironmentMasterCountryValueRepository(IDbFactory<OceanDbEntities> dbFactory)
            : base(dbFactory)
        {
        }


        /// <summary>
        /// Get the value of a specific appKey by Country
        /// </summary>
        /// <param name="countryGuid">the unique identifier of a country.</param>
        /// <param name="appKey">the app key to search</param>
        /// <returns>the object with the value of the appKey</returns>
        public TblSystemEnvironmentMasterCountryValue GetSpecificKeyByCountryAndKey(Guid countryGuid, string appKey)
        {
            List<TblSystemEnvironmentMasterCountryValue> data = (from EMC in DbContext.TblSystemEnvironmentMasterCountryValue
                                                                 join EC in DbContext.TblSystemEnvironmentMasterCountry on EMC.SystemEnvironmentMasterCountry_Guid equals EC.Guid
                                                                 where EMC.MasterCountry_Guid == countryGuid && EC.AppKey == appKey
                                                                 select EMC).ToList();

            return data != null && data.Any() ? data.FirstOrDefault() : new TblSystemEnvironmentMasterCountryValue();
        }
    }
}
