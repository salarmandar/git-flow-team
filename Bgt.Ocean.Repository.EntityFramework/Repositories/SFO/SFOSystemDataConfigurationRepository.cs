using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region interface

    public interface ISFOSystemDataConfigurationRepository : IRepository<SFOTblSystemDataConfiguration>
    {
        Up_OceanOnlineMVC_SFO_SearchDataConfiguration_Get_Result GetByDataKey(string dataKey, Guid masterCountryGuid);
        IEnumerable<Up_OceanOnlineMVC_SFO_SearchDataConfiguration_Get_Result> GetByDataKey(string[] DataKey, Guid masterCountryGuid);
    }

    #endregion

    public class SFOSystemDataConfigurationRepository : Repository<OceanDbEntities, SFOTblSystemDataConfiguration>, ISFOSystemDataConfigurationRepository
    {
        public SFOSystemDataConfigurationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public Up_OceanOnlineMVC_SFO_SearchDataConfiguration_Get_Result GetByDataKey(string dataKey, Guid masterCountryGuid)
            => GetByDataKey(new string[] { dataKey }, masterCountryGuid).FirstOrDefault();

        public IEnumerable<Up_OceanOnlineMVC_SFO_SearchDataConfiguration_Get_Result> GetByDataKey(string[] DataKey, Guid masterCountryGuid)
        {
            string StrDataKey = "";
            foreach (var Item in DataKey)
            {
                if (StrDataKey != "")
                    StrDataKey += ",";
                StrDataKey += Item;
            }

            return DbContext.Up_OceanOnlineMVC_SFO_SearchDataConfiguration_Get(masterCountryGuid, StrDataKey);
        }
    }
}
