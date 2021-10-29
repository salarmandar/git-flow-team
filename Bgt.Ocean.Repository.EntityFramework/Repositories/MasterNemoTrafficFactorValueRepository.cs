using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterNemoTrafficFactorValueRepository : IRepository<TblMasterNemoTrafficFactorValue>
    {
        IEnumerable<TblMasterNemoTrafficFactorValue> NemoTrafficConfigGet(Guid countryGuid, Guid siteGuid);
        void NemoTrafficConfigAddUpdate(IEnumerable<TblMasterNemoTrafficFactorValue> nemoTrafficConfigModel, Guid countryGuid, Guid siteGuid, string userName, DateTime datetimeCreate, DateTimeOffset universalDatetime);
    }

    public class MasterNemoTrafficFactorValueRepository : Repository<OceanDbEntities,TblMasterNemoTrafficFactorValue>, IMasterNemoTrafficFactorValueRepository
    {
        public MasterNemoTrafficFactorValueRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterNemoTrafficFactorValue> NemoTrafficConfigGet(Guid countryGuid, Guid siteGuid)
        {
            return DbContext.TblMasterNemoTrafficFactorValue.Where(o => o.MasterCountry_Guid == countryGuid && o.MasterSite_Guid == siteGuid);         
        }

        public void NemoTrafficConfigAddUpdate(IEnumerable<TblMasterNemoTrafficFactorValue> nemoTrafficConfigModel,Guid countryGuid,Guid siteGuid,string userName,DateTime datetimeCreate,DateTimeOffset universalDatetime)
        {
            var trafficConfigList = DbContext.TblMasterNemoTrafficFactorValue.Where(o => o.MasterCountry_Guid == countryGuid && o.MasterSite_Guid == siteGuid);
            DbContext.TblMasterNemoTrafficFactorValue.RemoveRange(trafficConfigList);

            if (nemoTrafficConfigModel.Any())
            {
                var insertTrafficConfig = nemoTrafficConfigModel.Select(o => new TblMasterNemoTrafficFactorValue
                {
                    Guid = Guid.NewGuid(),
                    MasterCountry_Guid = countryGuid,
                    MasterSite_Guid = siteGuid,
                    DayofWeek_Guid = o.DayofWeek_Guid,
                    StartTime = o.StartTime,
                    EndTime = o.EndTime,
                    TrafficMultiplier = o.TrafficMultiplier,
                    UserCreated = userName,
                    DatetimeCreated = datetimeCreate,
                    UniversalDatetimeCreated = universalDatetime,
                    FlagTrafficCal = o.FlagTrafficCal
                });
                DbContext.TblMasterNemoTrafficFactorValue.AddRange(insertTrafficConfig);
            }
            DbContext.SaveChanges();
        }
    }
}
