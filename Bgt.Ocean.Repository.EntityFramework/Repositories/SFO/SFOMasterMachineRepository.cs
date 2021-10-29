using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Configuration;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterMachineRepository : IRepository<SFOTblMasterMachine>
    {
        bool IsMachineHasLock(Guid machineGuid);
        Up_OceanOnlineMVC_SFO_SearchMachine_Get_Result GetMachineByMachineID(string machineId, Guid masterCountryGuid);
        Up_OceanOnlineMVC_SFO_SearchMachine_Get_Result GetMachineByGuid(Guid machineGuid);
        string GetWindowServiceHours(Guid machineGuid);
        List<MachineView> GetMachineList(MachineView_Request request);
        List<MachineView_ServiceHour> GetServiceHourByMachineGuid(string machineGuid);
        IEnumerable<SFOTblMasterMachine> GetAssociateMachine(Guid custLocGuid);
        bool CheckAssociateMachine(Guid custLocGuid);
        bool CheckDuplicateCustomerMachineIdByCustomer(Guid customerGuid, string customerMachineId);
    }

    #endregion

    public class SFOMasterMachineRepository : Repository<OceanDbEntities, SFOTblMasterMachine>, ISFOMasterMachineRepository
    {
        public SFOMasterMachineRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public Up_OceanOnlineMVC_SFO_SearchMachine_Get_Result GetMachineByMachineID(string machineId, Guid masterCountryGuid)
        {
            var data = DbFactory.GetCurrentDbContext
                .Up_OceanOnlineMVC_SFO_SearchMachine_Get(
                    null,
                    null,
                    masterCountryGuid,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    machineId,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    false,
                    false
                );

            return data.FirstOrDefault();
        }

        public Up_OceanOnlineMVC_SFO_SearchMachine_Get_Result GetMachineByGuid(Guid machineGuid)
        {
            var data = DbFactory.GetCurrentDbContext
                .Up_OceanOnlineMVC_SFO_SearchMachine_Get(
                    machineGuid,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    false,
                    false
                );

            return data.FirstOrDefault();
        }

        public bool IsMachineHasLock(Guid machineGuid)
        {
            var isHasLock = FindById(machineGuid)
                .SFOTblMasterMachine_LockType
                .Any(e => e.FlagDisable == false);

            return isHasLock;
        }

        public string GetWindowServiceHours(Guid machineGuid)
        {
            var utcNow = DateTime.UtcNow;
            var serviceHours = FindById(machineGuid).TblMasterCustomerLocation.TblMasterCustomerLocation_ServiceHour
                .Select(e => new
                {
                    MasterDayOfWeek_Name = e.TblSystemDayOfWeek.MasterDayOfWeek_Name,
                    ServiceHourStart = e.ServiceHourStart,
                    ServiceHourStop = e.ServiceHourStop,
                    SystemServiceJobType_Name = e.TblSystemServiceJobType.ServiceJobTypeName
                })
                .ToList();
            Func<int, int, DateTime> getDateObj = (hour, minute) =>
            {
                var minDate = DateTime.MinValue;
                var date = new DateTime(minDate.Year, minDate.Month, minDate.Day, hour, minute, 0);

                return date;
            };

            if (serviceHours.Count == 0) return null;

            var windowPeriod = serviceHours.Where(e => e.MasterDayOfWeek_Name == utcNow.DayOfWeek.ToString() &&
                (
                    (getDateObj(e.ServiceHourStart.GetValueOrDefault().Hour, e.ServiceHourStart.GetValueOrDefault().Minute) <= getDateObj(utcNow.Hour, utcNow.Minute))
                    &&
                    (getDateObj(e.ServiceHourStop.GetValueOrDefault().Hour, e.ServiceHourStop.GetValueOrDefault().Minute) >= getDateObj(utcNow.Hour, utcNow.Minute))
                )
            );

            return windowPeriod.Any()
                ? string.Join(", ", windowPeriod.Select(e => $"{e.SystemServiceJobType_Name}{e.ServiceHourStart.Value.GetTime()} - {e.ServiceHourStop.Value.GetTime()}"))
                : "-";
        }

        public List<MachineView> GetMachineList(MachineView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<MachineView>
                (
                    "Up_OceanOnlineMVC_API_GetMachineList",
                    new
                    {
                        @MaxRow = WebConfigurationManager.AppSettings["MaxRow"],
                        @CountryAbb = request.countryAbb,
                        @CreatedFrom = request.createdDatetimeFrom,
                        @CreatedTo = request.createdDatetimeTo
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result;
        }

        public List<MachineView_ServiceHour> GetServiceHourByMachineGuid(string machineGuid)
        {
            var result = DbContext.Database.Connection
                .Query<MachineView_ServiceHour>
                (
                    "Up_OceanOnlineMVC_API_GetServiceHourByMachineGuid",
                    new
                    {
                        @MachineGuid = machineGuid
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result;
        }
        public IEnumerable<SFOTblMasterMachine> GetAssociateMachine(Guid custLocGuid)
        {
            return DbContext.SFOTblMasterMachine_AssociatedMachine.Where(w => w.SFOMasterMachine_Guid == custLocGuid).Select(s => s.SFOTblMasterMachine);
        }

        /// <summary>
        /// check transfer safe 
        /// </summary>
        /// <param name="custLocGuid"></param>
        /// <returns></returns>
        public bool CheckAssociateMachine(Guid custLocGuid)
        {
            var custType = DbContext.TblSystemCustomerLocationType.Where(w => w.CustomerLocationTypeID == (int)(CustomerLocationType.Transfer_Safe)).Select(s => s.Guid);
            return DbContext.SFOTblMasterMachine_AssociatedMachine.Any(a => a.SFOMasterMachine_Guid == custLocGuid && custType.Contains(a.SFOTblMasterMachine.TblMasterCustomerLocation.SystemCustomerLocationType_Guid ?? Guid.Empty));
        }

        public bool CheckDuplicateCustomerMachineIdByCustomer(Guid customerGuid, string customerMachineId)
        {
            var result = (from Loc in DbContext.TblMasterCustomerLocation.Where(w => w.MasterCustomer_Guid == customerGuid)
                          join Mc in DbContext.SFOTblMasterMachine.Where(w => w.CustomerMachineID == customerMachineId) on Loc.Guid equals Mc.Guid
                          select 1
                     ).Any();
            return result;
        }

    }
}
