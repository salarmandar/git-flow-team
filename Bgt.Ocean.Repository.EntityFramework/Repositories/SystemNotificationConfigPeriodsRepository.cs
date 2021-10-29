using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface ISystemNotificationConfigPeriodsRepository : IRepository<TblSystemNotificationConfigPeriods>
    {
        IEnumerable<SystemNotificationConfigPeriodsView> GetListSystemNotificationConfigPeriod(Guid countryGuid, bool? flagDisable);
        IEnumerable<SystemNotificationConfigPeriodsView> GetSystemNotificationConfigPeriodById(Guid notificationPeriodGuid);
    }

    public class SystemNotificationConfigPeriodsRepository : Repository<OceanDbEntities, TblSystemNotificationConfigPeriods>, ISystemNotificationConfigPeriodsRepository
    {

        public SystemNotificationConfigPeriodsRepository(IDbFactory<OceanDbEntities> dbFactory)
            : base(dbFactory)
        {

        }

        public IEnumerable<SystemNotificationConfigPeriodsView> GetListSystemNotificationConfigPeriod(Guid countryGuid, bool? flagDisable)
         {
             bool includeAll = (!flagDisable.HasValue | !flagDisable.Value) ? false : true;
             var notificationConfigPeriods = FindAll(e => e.MasterCountry_Guid == countryGuid && (includeAll | !e.FlagDisable));
             var systemEnvironmentMasterCountry = DbContext.TblSystemEnvironmentMasterCountry.ToList();

            var mappinTable = notificationConfigPeriods
               .Join(systemEnvironmentMasterCountry,
               tblnot => tblnot.SystemEnvironmentMasterCountry_Guid,
               tblEnvi => tblEnvi.Guid,
               (Left, Right) => new { Left, Right })
                   .Join(DbContext.TblSystemEnvironmentMasterCountryValue,
                   mapTbl => new { p = mapTbl.Right.Guid, q = mapTbl.Left.MasterCountry_Guid },
                   tblEnvCountry => new { p = tblEnvCountry.SystemEnvironmentMasterCountry_Guid, q = tblEnvCountry.MasterCountry_Guid },
                   (mapTbl, tblEnvCountry) => new SystemNotificationConfigPeriodsView()
                   {
                       NotificationConfigPeriodsGuid = mapTbl.Left.Guid,
                       PeriodTitle = mapTbl.Left.PeriodTitle,
                       InitialDate = mapTbl.Left.StartDate,
                       FinalDate = mapTbl.Left.EndDate,
                       NotifyBeforeDueDate = Convert.ToInt32(string.IsNullOrEmpty(tblEnvCountry.AppValue1) ? "0" : tblEnvCountry.AppValue1),
                       DaysBeforeDueDate = Convert.ToInt32(string.IsNullOrEmpty(tblEnvCountry.AppValue2) ? "0" : tblEnvCountry.AppValue2),
                       UserCreated = mapTbl.Left.UserCreated,
                       UserModifed = mapTbl.Left.UserModifed,
                       FlagDisable = mapTbl.Left.FlagDisable,
                       Country = tblEnvCountry.MasterCountry_Guid
                   });
            return mappinTable;
         }

        public IEnumerable<SystemNotificationConfigPeriodsView> GetSystemNotificationConfigPeriodById(Guid notificationPeriodGuid)
        {
            var notificationConfigPeriods = FindAll(e => e.Guid == notificationPeriodGuid);
            var systemEnvironmentMasterCountry = DbContext.TblSystemEnvironmentMasterCountry.ToList();
            var systemEnvironmentMasterCountryValue = DbContext.TblSystemEnvironmentMasterCountryValue;

            var mappinTable = notificationConfigPeriods.Join(systemEnvironmentMasterCountry,
                tblnot => tblnot.SystemEnvironmentMasterCountry_Guid,
                tblEnvi => tblEnvi.Guid,
                 (Left, Right) => new { Left, Right }).Join(systemEnvironmentMasterCountryValue,
                                        mapTbl => new { p = mapTbl.Right.Guid, q = mapTbl.Left.MasterCountry_Guid },
                                        tblEnvCountry => new { p = tblEnvCountry.SystemEnvironmentMasterCountry_Guid, q = tblEnvCountry.MasterCountry_Guid },
                                        (mapTbl, tblEnvCountry) => new SystemNotificationConfigPeriodsView()
                                        {
                                            NotificationConfigPeriodsGuid = mapTbl.Left.Guid,
                                            PeriodTitle = mapTbl.Left.PeriodTitle,
                                            InitialDate = mapTbl.Left.StartDate,
                                            FinalDate = mapTbl.Left.EndDate,
                                            NotifyBeforeDueDate = Convert.ToInt32(string.IsNullOrEmpty(tblEnvCountry.AppValue1) ? "0" : tblEnvCountry.AppValue1),
                                            DaysBeforeDueDate = Convert.ToInt32(string.IsNullOrEmpty(tblEnvCountry.AppValue2) ? "0" : tblEnvCountry.AppValue2),
                                            UserCreated = mapTbl.Left.UserCreated,
                                            UserModifed = mapTbl.Left.UserModifed,
                                            FlagDisable = mapTbl.Left.FlagDisable,
                                            Country = tblEnvCountry.MasterCountry_Guid,
                                            SystemEnvironmentMasterCountry_Guid = mapTbl.Left.SystemEnvironmentMasterCountry_Guid
                                        });
            return mappinTable;
        }
    }
}