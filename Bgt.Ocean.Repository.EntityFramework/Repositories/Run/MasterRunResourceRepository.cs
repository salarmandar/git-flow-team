

using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Run
{
    public interface IMasterRunResourceRepository : IRepository<TblMasterRunResource>
    {
        IEnumerable<FleetRunResourceView> GetRunResourcesBySite(Guid? siteGuid);
        IEnumerable<FleetMaintenanceView> GetFleetMaintenance(FleetMaintenanceFilter filters);


        IEnumerable<SummaryAccidentView> GetAccident(SummaryMaintenanceFilter filters);
        IEnumerable<SummaryGasolineView> GetGasolineExpense(SummaryMaintenanceFilter filters);
        IEnumerable<SummaryMaintenanceView> GetMaintenance(SummaryMaintenanceFilter filters);
    }
    public class MasterRunResourceRepository : Repository<OceanDbEntities, TblMasterRunResource>, IMasterRunResourceRepository
    {
        public MasterRunResourceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<FleetRunResourceView> GetRunResourcesBySite(Guid? siteGuid)
        {
            var languageGuid = ApiSession.UserLanguage_Guid;
            var inProgressStatus = (int)EnumMaintenanceStatus.InProgress;
            var inWalker = (int)EnumMOT.Walker;

            var runs = DbContext.TblMasterRunResource
                                .Where(r => r.MasterSite_Guid == siteGuid && r.Flag3Party == false && r.FlagDisable == false);

            var mots = (from s in DbContext.TblSystemModeOfTransport.Where(m => !m.FlagDisable && m.ModeOfTransportID != inWalker)
                        join l in DbContext.TblSystemDisplayTextControlsLanguage.Where(o => o.SystemLanguageGuid == languageGuid) on s.SystemDisplayTextControls_Guid equals l.Guid into DisplayText
                        from LDisplayText in DisplayText.DefaultIfEmpty()
                        select new
                        {
                            s.Guid,
                            s.ModeOfTransportID,
                            ModeOfTransport = LDisplayText.DisplayText == null ? s.ModeOfTransport : LDisplayText.DisplayText
                        }).AsEnumerable();

            var runGuids = runs.Select(o => o.Guid);

            var maintenRun = runGuids.Where(o => DbContext.TblMasterRunResource_Maintenance.Any(m => m.MasterRunResource_Guid == o && m.MaintenanceStatusID == inProgressStatus)).ToList();

            var runInSite = runs.Join(mots, r => r.SystemModeOfTransport_Guid, m => m.Guid, (run, mot) => new { run, mot }).AsEnumerable()
                                .Select(o =>
                                {
                                    var maintenanceStatusID = maintenRun.Any(m => m == o.run.Guid) ? EnumMaintenanceStatus.InProgress : EnumMaintenanceStatus.Normal;
                                    return new FleetRunResourceView
                                    {
                                        RunGuid = o.run.Guid,
                                        VehicleNumber = o.run.VehicleNumber,
                                        ModeOfTransportGuid = o.run.SystemModeOfTransport_Guid,
                                        ModeOfTransport = o.mot.ModeOfTransport,
                                        ModeOfTransportID = (EnumMOT)(o.mot.ModeOfTransportID ?? 0),
                                        MaintenanceStatusID = maintenanceStatusID,
                                        MaintenanceStatus = maintenanceStatusID.ToString()
                                    };
                                }).OrderBy(o => o.ModeOfTransport).ThenBy(o => o.VehicleNumber);

            return runInSite;
        }

        public IEnumerable<SummaryAccidentView> GetAccident(SummaryMaintenanceFilter filters)
        {
            var year = Convert.ToInt16(filters.Year);
            return DbContext.TblMasterRunResource_Accident
                            .Where(o => o.FlagDisable == false && o.MasterSite_Guid == filters.SiteGuid && o.MasterRunResource_Guid == filters.RunGuid && o.DateOfAccident.Year == year)
                            .Select(o => new
                            {
                                Accident = o.FlagBrinksIsFault == true ? "Brink’s is at Fault" : "Counterparty is at Fault",
                                FlagBrinksIsFault = (bool)o.FlagBrinksIsFault,
                                DateOfAccident = o.DateOfAccident
                            })
                            .GroupBy(o => new { o.DateOfAccident.Month, o.Accident, o.FlagBrinksIsFault })
                            .AsEnumerable()
                            .Select(o =>
                            {
                                var month = new DateTime(1, o.Key.Month, 1).ToString("MMMM", CultureInfo.InvariantCulture);
                                return new SummaryAccidentView
                                {
                                    Month = month,
                                    Accident = o.Key.Accident,
                                    TotalAccident = o.Count()
                                };
                            })
                            .OrderBy(o => o.Accident);
        }

        public IEnumerable<SummaryGasolineView> GetGasolineExpense(SummaryMaintenanceFilter filters)
        {
            //split get data is good performance
            var year = Convert.ToInt16(filters.Year);
            var gasolineExpense = DbContext.TblMasterRunResource_GasolineExpense
                                           .Where(o => o.FlagDisable == false && o.MasterSite_Guid == filters.SiteGuid && o.MasterRunResource_Guid == filters.RunGuid && o.TopUpDate.Year == year).AsEnumerable();
            var currencyGuids = gasolineExpense.Select(o => o.CurrencyAmount_Guid).Distinct();
            var mastercurrency = DbContext.TblMasterCurrency
                           .Where(o => !o.FlagDisable && currencyGuids.Contains(o.Guid)).AsEnumerable();

            return gasolineExpense.GroupBy(o => new { o.TopUpDate.Month, o.CurrencyAmount_Guid })
                            .Select(o =>
                            {
                                var month = new DateTime(1, o.Key.Month, 1).ToString("MMMM", CultureInfo.InvariantCulture);
                                var currency = mastercurrency.FirstOrDefault(c => c.Guid == o.Key.CurrencyAmount_Guid)?.MasterCurrencyAbbreviation;
                                var totalCost = decimal.ToDouble(o.Sum(s => s.TopUpAmount));

                                return new SummaryGasolineView
                                {
                                    Month = month,
                                    Currency = currency,
                                    TotalCost = totalCost
                                };
                            })
                            .OrderByDescending(o => o.Month);
        }

        public IEnumerable<SummaryMaintenanceView> GetMaintenance(SummaryMaintenanceFilter filters)
        {
            var year = Convert.ToInt16(filters.Year);
            var maintenance = DbContext.TblMasterRunResource_Maintenance
                               .Where(o => o.FlagDisable == false && o.MasterSite_Guid == filters.SiteGuid && o.MasterRunResource_Guid == filters.RunGuid && o.Open_DateServiceFrom.Year == year).AsEnumerable();

            var currencyGuids = maintenance.Select(o => o.CurrencyActual_Guid).Distinct();
            var mastercurrency = DbContext.TblMasterCurrency
                           .Where(o => !o.FlagDisable && currencyGuids.Contains(o.Guid)).AsEnumerable();

            var vendorGuids = maintenance.Select(o => o.MasterVendor_Guid).Distinct();
            var masterVendor = DbContext.TblMasterVendor
                           .Where(o => o.FlagDisable == false && vendorGuids.Contains(o.Guid)).AsEnumerable();

            return maintenance.GroupBy(o => new { o.MasterVendor_Guid, o.CurrencyActual_Guid })
                            .Select(o =>
                            {

                                var vendorName = masterVendor.FirstOrDefault(v => v.Guid == o.Key.MasterVendor_Guid)?.VendorName;
                                var currency = mastercurrency.FirstOrDefault(c => c.Guid == o.Key.CurrencyActual_Guid)?.MasterCurrencyAbbreviation;
                                var totalCost = decimal.ToDouble(o.Sum(s => s.CostActual ?? 0));

                                return new SummaryMaintenanceView
                                {
                                    VendorName = vendorName,
                                    Currency = currency,
                                    TotalCost = totalCost
                                };
                            })
                            .OrderBy(o => o.VendorName);
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetFleetMaintenanceByRunResource
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public IEnumerable<FleetMaintenanceView> GetFleetMaintenance(FleetMaintenanceFilter filters)
        {
            Expression<Func<TblMasterRunResource_Maintenance, bool>> run_maintenancelist_condition = o
                => filters.SiteGuid == o.MasterSite_Guid
                 && o.MasterRunResource_Guid == filters.RunGuid
                 // find all status || selected status
                 && (filters.MaintenanceStatusID == EnumMaintenanceStatus.Undefined || (int)filters.MaintenanceStatusID == o.MaintenanceStatusID)
                 // find all date || selected date
                 && (filters.FlagAllDate || (o.Open_DateServiceFrom >= filters.Open_DateServiceFrom && o.Open_DateServiceTo <= filters.Open_DateServiceTo));

            var dateFormat = ApiSession.UserFormatDate;
            var languageGuid = ApiSession.UserLanguage_Guid;
            var maintenance = DbContext.TblMasterRunResource_Maintenance.Where(run_maintenancelist_condition).ToList();

            var vendorGuids = maintenance.Select(o => o.MasterVendor_Guid);
            var currGuids = maintenance.Where(o => o.CurrencyActual_Guid.HasValue)
                                       .Select(o => o.CurrencyActual_Guid).Union(
                            maintenance.Where(o => o.CurrencyEstimate_Guid.HasValue)
                                       .Select(o => o.CurrencyEstimate_Guid));

            var vendor = DbContext.TblMasterVendor.Where(o => vendorGuids.Contains(o.Guid)).AsEnumerable();
            var currency = DbContext.TblMasterCurrency.Where(o => currGuids.Contains(o.Guid)).AsEnumerable();

            var maintenanceStatus = (from s in DbContext.TblSystemMaintenanceStatus
                                     join l in DbContext.TblSystemDisplayTextControlsLanguage.Where(o => o.SystemLanguageGuid == languageGuid) on s.SystemDisplayTextControls_Guid equals l.Guid into DisplayText
                                     from LDisplayText in DisplayText.DefaultIfEmpty()
                                     select new
                                     {
                                         s.MaintenanceStatusID,
                                         MaintenanceStatusName = LDisplayText.DisplayText == null ? s.MaintenanceStatusName : LDisplayText.DisplayText
                                     }).AsEnumerable();

            var result = (from m in maintenance
                          join v in vendor on m.MasterVendor_Guid equals v.Guid into V
                          from lv in V.DefaultIfEmpty()

                          join ca in currency on m.CurrencyActual_Guid equals ca.Guid into CA
                          from lca in CA.DefaultIfEmpty()

                          join ce in currency on m.CurrencyEstimate_Guid equals ce.Guid into CE
                          from lce in CE.DefaultIfEmpty()

                          join s in maintenanceStatus on m.MaintenanceStatusID equals s.MaintenanceStatusID into S
                          from ls in S.DefaultIfEmpty()

                          select new { m, lv, lca, lce, ls })
                        .Take(filters.MaxRow)
                        .Select(o =>
                                 {
                                     var isNotCloseMaintenance = (EnumMaintenanceStatus)o.m.MaintenanceStatusID != EnumMaintenanceStatus.Closed;
                                     var serviceFromDateTime = (DateTime?)DateTimeHelper.FromDateCombineWithTime(o.m.Open_DateServiceFrom, o.m.Open_TimeServiceFrom);
                                     var serviceToDateTime = (DateTime?)DateTimeHelper.FromDateCombineWithTime(o.m.Open_DateServiceTo, o.m.Open_TimeServiceTo);
                                     var receivedDateTime = isNotCloseMaintenance ? null : (DateTime?)DateTimeHelper.FromDateCombineWithTime(o.m.Close_DateService, o.m.Close_TimeService);

                                     return new FleetMaintenanceView
                                     {
                                         RunGuid = o.m.MasterRunResource_Guid,
                                         MaintenanceGuid = o.m.Guid,
                                         MaintenanceNo = o.m.MaintenanceID,
                                         MaintenanceStatusID = (EnumMaintenanceStatus)o.m.MaintenanceStatusID,
                                         MaintenanceStatusName = o.ls?.MaintenanceStatusName ?? string.Empty,
                                         DocRef = o.m.DocumentRef_No ?? string.Empty,
                                         OdometerBefore = o.m.Open_OdoMeter ?? string.Empty,
                                         OdometerAfter = o.m.Close_OdoMeter ?? string.Empty,
                                         CostEstimate = o.m.CostEstimate ?? 0,
                                         CostActual = o.m.CostActual ?? 0,
                                         CurrencyCostActual = o.lca?.MasterCurrencyAbbreviation ?? string.Empty,
                                         CurrencyCostEstimate = o.lce?.MasterCurrencyAbbreviation ?? string.Empty,
                                         VendorName = o.lv?.VendorName ?? string.Empty,
                                         Remarks = o.m.Remarks ?? string.Empty,
                                         UserCreated = o.m.UserCreated,
                                         UserModified = o.m.UserModifed,
                                         
                                         ReceivedDateTime = receivedDateTime,
                                         DateTimeCreated = o.m.DatetimeCreated,
                                         DateTimeModified = o.m.DatetimeModified,
                                         LastDateTimeModified = o.m.DatetimeModified ?? o.m.DatetimeCreated,
                                         ServiceDateRange = serviceFromDateTime.HasValue && serviceToDateTime.HasValue
                                            ? $"{serviceFromDateTime.ChangeFromDateToString(dateFormat)} - {serviceToDateTime.ChangeFromDateToString(dateFormat)}"
                                            : string.Empty
                                     };
                                 });


            return result.OrderByDescending(o => o.LastDateTimeModified);
        }
    }
}
