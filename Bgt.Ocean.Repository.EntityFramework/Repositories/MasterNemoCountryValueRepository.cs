using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.NemoConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterNemoCountryValueRepository : IRepository<TblMasterNemoCountryValue>
    {
        IEnumerable<GlobalUnitText> DistanceUnitGet(Guid languageGuid);
        TblMasterNemoCountryValue NemoCountryConfigGet(Guid countryGuid);
        IEnumerable<SiteForApplyView> SiteForApplyGet(Guid countryGuid);
        void NemoCountryConfigAddUpdate(TblMasterNemoCountryValue nemoCountryModel);
        void NemoCountryApplyToSite(NemoApplyToSiteView nemoApplyToSiteView);
    }
    public class MasterNemoCountryValueRepository : Repository<OceanDbEntities, TblMasterNemoCountryValue>, IMasterNemoCountryValueRepository
    {
        public MasterNemoCountryValueRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        #region Get data
        public IEnumerable<GlobalUnitText> DistanceUnitGet(Guid languageGuid)
        {
            Guid dimensionTypeGuid = Guid.Parse("6acc21f5-1d42-4fd7-87ae-41335ba17a1f");
            int?[] unitID = { 4, 6 };  // 4 : Mile , 6 : Kilometer
            var data = DbContext.TblSystemGlobalUnit.Where(o => !o.FlagDisable && o.SystemGlobalUnitType_Guid == dimensionTypeGuid && unitID.Contains(o.UnitID))
                       .Join(DbContext.TblSystemDisplayTextControlsLanguage,
                       gu => gu.SystemDisplayTextControls_Guid,
                       dl => dl.Guid,
                       (gu, dl) => new { gu, dl });

            var resultData = data.Where(o => o.dl.SystemLanguageGuid == languageGuid) ?? data.Where(o => o.dl.SystemLanguageGuid == Guid.Parse("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0"));
            return resultData.Select(o => new GlobalUnitText() { Guid = o.gu.Guid, DisplayTxt = o.dl.DisplayText });
        }

        public TblMasterNemoCountryValue NemoCountryConfigGet(Guid countryGuid)
        {
            /* Fixed Async Issue */
            using (OceanDbEntities context = new OceanDbEntities())
            {
                return context.TblMasterNemoCountryValue.Where(o => (o.Guid == Guid.Empty) || (o.MasterCountry_Guid == countryGuid)).OrderByDescending(o => o.Guid).FirstOrDefault();
            }
        }

        public IEnumerable<SiteForApplyView> SiteForApplyGet(Guid countryGuid)
        {
            var siteGuidList = DbContext.TblMasterNemoSiteValue.Where(o => o.MasterCountry_Guid == countryGuid).Select(o => o.MasterSite_Guid).Distinct();
            return DbContext.TblMasterSite.Where(x => x.MasterCountry_Guid == countryGuid && !x.FlagDisable)
                           .Select(o => new SiteForApplyView
                           {
                               SiteGuid = o.Guid,
                               SiteName = o.SiteCode + " - " + o.SiteName,
                               FlagSiteApply = siteGuidList.Any(x => x == o.Guid)
                           });
        }

        #endregion

        #region Add & Update Nemo country configuration
        public void NemoCountryConfigAddUpdate(TblMasterNemoCountryValue nemoCountryModel)
        {
            var data = DbContext.TblMasterNemoCountryValue.FirstOrDefault(o => o.Guid == nemoCountryModel.Guid);
            if (data != null && data.Guid != Guid.Empty)
            {
                data.SystemGlobalUnit_Guid = nemoCountryModel.SystemGlobalUnit_Guid;
                data.LunchTime = nemoCountryModel.LunchTime;
                data.LunchDuration = nemoCountryModel.LunchDuration;
                data.FlagZoneUsing = nemoCountryModel.FlagZoneUsing;
                data.FlagOverlapZone = nemoCountryModel.FlagOverlapZone;
                data.MaxOverlapDistance = nemoCountryModel.MaxOverlapDistance;
                data.MaxServiceStop = nemoCountryModel.MaxServiceStop;
                data.MaxLiability = nemoCountryModel.MaxLiability;
                data.MaxDuration = nemoCountryModel.MaxDuration;
                data.MaxCapacity = nemoCountryModel.MaxCapacity;
                data.MaxVolumn = nemoCountryModel.MaxVolumn;
                data.MaxDistanceBetweenStop = nemoCountryModel.MaxDistanceBetweenStop;
                data.MaxWaitTime = nemoCountryModel.MaxWaitTime;
                data.UserModifed = nemoCountryModel.UserModifed;
                data.DatetimeModified = nemoCountryModel.DatetimeModified;
                data.UniversalDatetimeModified = nemoCountryModel.UniversalDatetimeModified;
                data.FlagTurnAround = nemoCountryModel.FlagTurnAround;
                data.TurnAroundLeadTime = nemoCountryModel.TurnAroundLeadTime;
                data.FlagAllowEarliestDispatchedTime = nemoCountryModel.FlagAllowEarliestDispatchedTime;
                data.MaxRunTotalDistance = nemoCountryModel.MaxRunTotalDistance;
                data.MaxBayLoadingVehicle = nemoCountryModel.MaxBayLoadingVehicle;
                data.MaxBayLoadingTime = nemoCountryModel.MaxBayLoadingTime;
            }
            else
            {
                var newNemoCountryConfig = new TblMasterNemoCountryValue()
                {
                    Guid = Guid.NewGuid(),
                    MasterCountry_Guid = nemoCountryModel.MasterCountry_Guid,
                    SystemGlobalUnit_Guid = nemoCountryModel.SystemGlobalUnit_Guid,
                    LunchTime = nemoCountryModel.LunchTime,
                    LunchDuration = nemoCountryModel.LunchDuration,
                    FlagZoneUsing = nemoCountryModel.FlagZoneUsing,
                    FlagOverlapZone = nemoCountryModel.FlagOverlapZone,
                    MaxOverlapDistance = nemoCountryModel.MaxOverlapDistance,
                    MaxServiceStop = nemoCountryModel.MaxServiceStop,
                    MaxLiability = nemoCountryModel.MaxLiability,
                    MaxDuration = nemoCountryModel.MaxDuration,
                    MaxCapacity = nemoCountryModel.MaxCapacity,
                    MaxVolumn = nemoCountryModel.MaxVolumn,
                    MaxDistanceBetweenStop = nemoCountryModel.MaxDistanceBetweenStop,
                    MaxWaitTime = nemoCountryModel.MaxWaitTime,
                    UserCreated = nemoCountryModel.UserCreated,
                    DatetimeCreated = nemoCountryModel.DatetimeCreated,
                    UniversalDatetimeCreated = nemoCountryModel.UniversalDatetimeCreated,
                    FlagTurnAround = nemoCountryModel.FlagTurnAround,
                    TurnAroundLeadTime = nemoCountryModel.TurnAroundLeadTime,
                    FlagAllowEarliestDispatchedTime = nemoCountryModel.FlagAllowEarliestDispatchedTime,
                    MaxRunTotalDistance = nemoCountryModel.MaxRunTotalDistance,
                    MaxBayLoadingVehicle = nemoCountryModel.MaxBayLoadingVehicle,
                    MaxBayLoadingTime = nemoCountryModel.MaxBayLoadingTime
                };
                DbContext.TblMasterNemoCountryValue.Add(newNemoCountryConfig);
            }
            DbContext.SaveChanges();
        }
        public void NemoCountryApplyToSite(NemoApplyToSiteView nemoApplyToSiteView)
        {
            //Get country config data
            var nemoCountryConfigData = DbContext.TblMasterNemoCountryValue.FirstOrDefault(o => o.MasterCountry_Guid == nemoApplyToSiteView.CountryGuid) ??
                                        DbContext.TblMasterNemoCountryValue.FirstOrDefault(o => o.MasterCountry_Guid == Guid.Empty);
            //Get site config data
            var nemoSiteConfigData = DbContext.TblMasterNemoSiteValue.Where(o => o.MasterCountry_Guid == nemoApplyToSiteView.CountryGuid);

            //Get site config data in table 
            var siteConfigData = nemoSiteConfigData.Where(o => nemoApplyToSiteView.SiteGuids.Contains(o.MasterSite_Guid.Value));
            //Get site config not in table
            var exceptSiteConfig = nemoApplyToSiteView.SiteGuids.Except(siteConfigData.Select(o => o.Guid));
            //Get site config not in config
            var exceptSiteNotConfig = nemoSiteConfigData.Where(o => !nemoApplyToSiteView.SiteGuids.Contains(o.MasterSite_Guid.Value));

            if (siteConfigData.Any())
            {
                var updateSiteConfig = siteConfigData.Where(o => siteConfigData.Select(x => x.Guid).Contains(o.Guid));
                foreach (var item in updateSiteConfig)
                {
                    item.MaxBayLoadingVehicle = nemoCountryConfigData.MaxBayLoadingVehicle;
                    item.MaxBayLoadingTime = nemoCountryConfigData.MaxBayLoadingTime;
                    item.LunchTime = nemoCountryConfigData.LunchTime;
                    item.LunchDuration = nemoCountryConfigData.LunchDuration;
                    item.FlagTurnAround = nemoCountryConfigData.FlagTurnAround;
                    item.TurnAroundLeadTime = nemoCountryConfigData.TurnAroundLeadTime;
                    item.FlagAllowEarliestDispatchedTime = nemoCountryConfigData.FlagAllowEarliestDispatchedTime;
                    item.MaxOverlapDistance = nemoCountryConfigData.MaxOverlapDistance;
                    item.MaxServiceStop = nemoCountryConfigData.MaxServiceStop;
                    item.MaxLiability = nemoCountryConfigData.MaxLiability;
                    item.MaxDuration = nemoCountryConfigData.MaxDuration;
                    item.MaxCapacity = nemoCountryConfigData.MaxCapacity;
                    item.MaxVolumn = nemoCountryConfigData.MaxVolumn;
                    item.MaxDistanceBetweenStop = nemoCountryConfigData.MaxDistanceBetweenStop;
                    item.MaxRunTotalDistance = nemoCountryConfigData.MaxRunTotalDistance;
                    item.MaxWaitTime = nemoCountryConfigData.MaxWaitTime;
                    item.UserModifed = nemoApplyToSiteView.UserName;
                    item.DatetimeModified = nemoApplyToSiteView.ClientDateTime;
                    item.UniversalDatetimeModified = nemoApplyToSiteView.UniversalDateTime;
                    item.FlagZoneUsing = nemoCountryConfigData.FlagZoneUsing;
                    item.FlagOverlapZone = nemoCountryConfigData.FlagOverlapZone;
                }
            }

            if (exceptSiteConfig.Any())
            {
                var insertNemoSiteConfig = exceptSiteConfig.Select(siteGuid => new TblMasterNemoSiteValue
                {
                    Guid = Guid.NewGuid(),
                    MasterCountry_Guid = nemoApplyToSiteView.CountryGuid,
                    MasterSite_Guid = siteGuid,
                    MaxBayLoadingVehicle = nemoCountryConfigData.MaxBayLoadingVehicle,
                    MaxBayLoadingTime = nemoCountryConfigData.MaxBayLoadingTime,
                    LunchTime = nemoCountryConfigData.LunchTime,
                    LunchDuration = nemoCountryConfigData.LunchDuration,
                    FlagTurnAround = nemoCountryConfigData.FlagTurnAround,
                    TurnAroundLeadTime = nemoCountryConfigData.TurnAroundLeadTime,
                    FlagAllowEarliestDispatchedTime = nemoCountryConfigData.FlagAllowEarliestDispatchedTime,
                    MaxOverlapDistance = nemoCountryConfigData.MaxOverlapDistance,
                    MaxServiceStop = nemoCountryConfigData.MaxServiceStop,
                    MaxLiability = nemoCountryConfigData.MaxLiability,
                    MaxDuration = nemoCountryConfigData.MaxDuration,
                    MaxCapacity = nemoCountryConfigData.MaxCapacity,
                    MaxVolumn = nemoCountryConfigData.MaxVolumn,
                    MaxDistanceBetweenStop = nemoCountryConfigData.MaxDistanceBetweenStop,
                    MaxRunTotalDistance = nemoCountryConfigData.MaxRunTotalDistance,
                    MaxWaitTime = nemoCountryConfigData.MaxWaitTime,
                    UserCreated = nemoApplyToSiteView.UserName,
                    DatetimeCreated = nemoApplyToSiteView.ClientDateTime,
                    UniversalDatetimeCreated = nemoApplyToSiteView.UniversalDateTime,
                    FlagZoneUsing = nemoCountryConfigData.FlagZoneUsing,
                    FlagOverlapZone = nemoCountryConfigData.FlagOverlapZone
                });
                DbContext.TblMasterNemoSiteValue.AddRange(insertNemoSiteConfig);
            }

            if (exceptSiteNotConfig.Any())
            {
                DbContext.TblMasterNemoSiteValue.RemoveRange(exceptSiteNotConfig);
            }

            DbContext.SaveChanges();
        }
        #endregion
    }
}
