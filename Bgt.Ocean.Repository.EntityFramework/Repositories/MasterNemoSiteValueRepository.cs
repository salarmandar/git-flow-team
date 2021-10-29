using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterNemoSiteValueRepository : IRepository<TblMasterNemoSiteValue>
    {
        TblMasterNemoSiteValue NemoSiteConfigGet(Guid countryGuid, Guid siteGuid);
        void NemoSiteConfigAddUpdate(TblMasterNemoSiteValue nemoSiteConfigModel);
    }

    public class MasterNemoSiteValueRepository : Repository<OceanDbEntities, TblMasterNemoSiteValue>, IMasterNemoSiteValueRepository
    {
        public MasterNemoSiteValueRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        #region Get data nemo site configuration
        public TblMasterNemoSiteValue NemoSiteConfigGet(Guid countryGuid, Guid siteGuid)
        {
            /* Fixed Async Issue */
            using (OceanDbEntities context = new OceanDbEntities())
            {
                return context.TblMasterNemoSiteValue.Where(o => (o.Guid == Guid.Empty) || (o.MasterCountry_Guid == countryGuid && o.MasterSite_Guid == siteGuid)).OrderByDescending(o => o.Guid).FirstOrDefault();
            }
        }
        #endregion

        #region Add & Update nemo site configuration
        public void NemoSiteConfigAddUpdate(TblMasterNemoSiteValue nemoSiteConfigModel)
        {
            var data = DbContext.TblMasterNemoSiteValue.FirstOrDefault(o => o.MasterCountry_Guid == nemoSiteConfigModel.MasterCountry_Guid && o.MasterSite_Guid == nemoSiteConfigModel.MasterSite_Guid);

            //Update site config
            if (data != null && nemoSiteConfigModel.Guid != Guid.Empty)
            {
                data.MaxBayLoadingVehicle = nemoSiteConfigModel.MaxBayLoadingVehicle;
                data.MaxBayLoadingTime = nemoSiteConfigModel.MaxBayLoadingTime;
                data.LunchTime = nemoSiteConfigModel.LunchTime;
                data.LunchDuration = nemoSiteConfigModel.LunchDuration;
                data.FlagTurnAround = nemoSiteConfigModel.FlagTurnAround;
                data.TurnAroundLeadTime = nemoSiteConfigModel.TurnAroundLeadTime;
                data.FlagAllowEarliestDispatchedTime = nemoSiteConfigModel.FlagAllowEarliestDispatchedTime;
                data.MaxOverlapDistance = nemoSiteConfigModel.MaxOverlapDistance;
                data.MaxServiceStop = nemoSiteConfigModel.MaxServiceStop;
                data.MaxLiability = nemoSiteConfigModel.MaxLiability;
                data.MaxDuration = nemoSiteConfigModel.MaxDuration;
                data.MaxCapacity = nemoSiteConfigModel.MaxCapacity;
                data.MaxVolumn = nemoSiteConfigModel.MaxVolumn;
                data.MaxDistanceBetweenStop = nemoSiteConfigModel.MaxDistanceBetweenStop;
                data.MaxRunTotalDistance = nemoSiteConfigModel.MaxRunTotalDistance;
                data.MaxWaitTime = nemoSiteConfigModel.MaxWaitTime;
                data.FlagOverlapZone = nemoSiteConfigModel.FlagOverlapZone;
                data.FlagZoneUsing = nemoSiteConfigModel.FlagZoneUsing;
                data.UserModifed = nemoSiteConfigModel.UserModifed;
                data.DatetimeModified = nemoSiteConfigModel.DatetimeModified;
                data.UniversalDatetimeModified = nemoSiteConfigModel.UniversalDatetimeModified;
            }
            //Add site config
            else
            {
                var newNemoSiteConfig = new TblMasterNemoSiteValue()
                {
                    Guid = Guid.NewGuid(),
                    MasterCountry_Guid = nemoSiteConfigModel.MasterCountry_Guid,
                    MasterSite_Guid = nemoSiteConfigModel.MasterSite_Guid,
                    MaxBayLoadingVehicle = nemoSiteConfigModel.MaxBayLoadingVehicle,
                    MaxBayLoadingTime = nemoSiteConfigModel.MaxBayLoadingTime,
                    LunchTime = nemoSiteConfigModel.LunchTime,
                    LunchDuration = nemoSiteConfigModel.LunchDuration,
                    FlagTurnAround = nemoSiteConfigModel.FlagTurnAround,
                    TurnAroundLeadTime = nemoSiteConfigModel.TurnAroundLeadTime,
                    FlagAllowEarliestDispatchedTime = nemoSiteConfigModel.FlagAllowEarliestDispatchedTime,
                    MaxOverlapDistance = nemoSiteConfigModel.MaxOverlapDistance,
                    MaxServiceStop = nemoSiteConfigModel.MaxServiceStop,
                    MaxLiability = nemoSiteConfigModel.MaxLiability,
                    MaxDuration = nemoSiteConfigModel.MaxDuration,
                    MaxCapacity = nemoSiteConfigModel.MaxCapacity,
                    MaxVolumn = nemoSiteConfigModel.MaxVolumn,
                    MaxDistanceBetweenStop = nemoSiteConfigModel.MaxDistanceBetweenStop,
                    MaxRunTotalDistance = nemoSiteConfigModel.MaxRunTotalDistance,
                    MaxWaitTime = nemoSiteConfigModel.MaxWaitTime,
                    UserCreated = nemoSiteConfigModel.UserCreated,
                    DatetimeCreated = nemoSiteConfigModel.DatetimeCreated,
                    UniversalDatetimeCreated = nemoSiteConfigModel.UniversalDatetimeCreated
                };
                DbContext.TblMasterNemoSiteValue.Add(newNemoSiteConfig);
            }
            DbContext.SaveChanges();
        }
        #endregion
    }
}
