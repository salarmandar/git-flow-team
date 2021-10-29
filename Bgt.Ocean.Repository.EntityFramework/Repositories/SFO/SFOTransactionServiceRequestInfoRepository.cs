using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOTransactionServiceRequestInfoRepository : IRepository<SFOTblTransactionServiceRequest_Info>
    {
        void SnapMachineInServiceRequest(Guid serviceRequestGuid, int slaTime);
    }

    #endregion

    public class SFOTransactionServiceRequestInfoRepository : Repository<OceanDbEntities, SFOTblTransactionServiceRequest_Info>, ISFOTransactionServiceRequestInfoRepository
    {
        private readonly ISFOMasterMachineRepository _machineRepository;

        public SFOTransactionServiceRequestInfoRepository(
                IDbFactory<OceanDbEntities> dbFactory,
                ISFOMasterMachineRepository machineRepository
            ) : base(dbFactory)
        {
            _machineRepository = machineRepository;
        }


        public void SnapMachineInServiceRequest(Guid serviceRequestGuid, int slaTime)
        {
            var sr = DbContext.SFOTblTransactionServiceRequest.Find(serviceRequestGuid);
            var srInfo = new SFOTblTransactionServiceRequest_Info();
            var machineInfo = _machineRepository.GetMachineByGuid(sr.MasterCustomerLocation_Guid);
            var machineDb = sr.TblMasterCustomerLocation.SFOTblMasterMachine;
            Func<string> getLockTypeName = () =>
                machineDb.SFOTblMasterMachine_LockType.Count == 0 ? null : string.Join(", ", machineDb.SFOTblMasterMachine_LockType?.Select(e => e.SFOTblSystemLockType.LockTypeName));

            var ecashSetup = machineDb.SFOTblMasterMachine_ECash?.Select(GetEcashSetup);

            srInfo.TransactionServiceRequest_Guid = serviceRequestGuid;
            srInfo.AmoredBranch = machineInfo.SFOArmoredBranch_Name;
            srInfo.AssignedBranch = machineInfo.SFOAssignedBranch_Name;
            srInfo.MasterCustomer_Guid = machineInfo.CustomerGuid;
            srInfo.BillingType = machineInfo.SFOMasterBillingType_Name;
            srInfo.ContractHours = _machineRepository.GetWindowServiceHours(sr.MasterCustomerLocation_Guid);
            srInfo.CustomerEmail = machineInfo.CustomerEmail;
            srInfo.CustomerName = machineInfo.CustomerFullName;
            srInfo.CustomerPhone = machineInfo.TelephoneNo;
            srInfo.DistrictRegion = machineInfo.DistrictName;
            srInfo.ECashZone = machineInfo.SFOMasterECashZone_Name;
            srInfo.FlagDailyCredit = machineInfo.FlagDailyCreditIndicator;
            srInfo.FlagDeviceDashboard = machineInfo.FlagDeviceDashBoardIndicator.GetValueOrDefault();
            srInfo.FlagRemoteSafeManagement = machineInfo.FlagRemoteSafeManagementIndicator.GetValueOrDefault();
            srInfo.FLMBranch = machineInfo.SFOFLMBranch_Name;
            srInfo.FLMZone = machineInfo.SFOMasterFLMZone_Name;
            srInfo.LockType = getLockTypeName();
            srInfo.LockType_Guid = machineInfo.SFOSystemLockType_Guid;
            srInfo.MachineBrand = machineInfo.SFOMasterMachineBrand_Name;
            srInfo.MachineID = machineInfo.MachineID;
            srInfo.MachineModel = machineInfo.MachineModelTypeName;
            srInfo.MachinePhone = machineInfo.Phone;
            srInfo.MachineType = machineInfo.CustomerLocationTypeName;
            srInfo.Run = machineInfo.Run;
            srInfo.SerialNumber = machineInfo.SerialNumber;
            srInfo.ServiceSubType = machineInfo.MachineSubServiceTypeName;
            srInfo.ServiceType = machineInfo.MachineServiceTypeName;
            srInfo.ServicingBranch = machineInfo.SFOServicingBranch_Name;
            srInfo.SiteAddress = machineInfo.BranchAddress;
            srInfo.SiteID = machineInfo.SiteCode;
            srInfo.MachineBrinksSite_Guid = sr.MasterSite_Guid;
            srInfo.SLATime = slaTime;
            srInfo.SFOMasterCountryTimeZone_Code = machineInfo.CountryTimeZoneID;
            srInfo.SFOMasterCountryTimeZone_Guid = machineInfo.SFOMasterCountryTimeZone_Guid;
            srInfo.MachineTimeZoneID = sr.TblMasterCustomerLocation.SFOTblMasterMachine.SFOTblMasterCountryTimeZone.TblSystemTimezone.TimeZoneID;
            srInfo.Machine_Guid = machineInfo.Guid;
            srInfo.BrinksLocationGuid = machineInfo.BrinksLocation_Guid;
            srInfo.MachineType_Guid = machineInfo.CustomerLocationTypeGuid;
            srInfo.ECashSetup = ecashSetup != null && ecashSetup.Count() > 0 ? JsonConvert.SerializeObject(ecashSetup) : string.Empty;
            srInfo.FlagSupportsDaylightSavingTime = machineInfo.FlagDST;
            srInfo.CombinationCode = null;
            srInfo.LockTypeReferenceCode = null;
            srInfo.MachineCountryGuid = machineInfo.CountryGuid;
            srInfo.FlagRequirePACN = machineInfo.FlagRequirePACN;
            srInfo.CustomerSLATime = null;
            srInfo.FlagIntegrationFLMBranch = machineInfo.SFOFLMBranch_FlagIntegrationSite;
            srInfo.FlagIntegrationServiceBranch = machineInfo.SFOServicingBranch_FlagIntegrationSite;
            srInfo.ServiceBranchGuid = machineInfo.SFOServicingBranch_Guid;
            srInfo.FLMBranchGuid = machineInfo.SFOFLMBranch_Guid;
            srInfo.MachineRemark = machineInfo.MachineReason;
            srInfo.AlarmCode = machineInfo.AlarmCode;

            Create(srInfo);
        }

        private ECashSetup GetEcashSetup(SFOTblMasterMachine_ECash ecash)
        {
            return new ECashSetup
            {
                Guid = ecash.Guid,
                Amount = ecash.Amount,
                Country_Unit = ecash.UnitValue,
                Input_Value = ecash.InputValue,
                MasterCurrency_Guid = ecash.MasterCurrency_Guid,
                MasterCurrency_Name = ecash.TblMasterCurrency?.MasterCurrencyAbbreviation,
                MasterDenomination_Guid = ecash.MasterDenomination_Guid,
                MasterDenomination_Name = ecash.TblMasterDenomination?.DenominationText,
                MasterDenomination_Value = ecash.DenominationValue,
                SFOMasterMachine_Guid = ecash.SFOMasterMachine_Guid
            };
        }


        #region Private class

        private class ECashSetup
        {
            public Guid? Guid { get; set; }
            public Guid? SFOMasterMachine_Guid { get; set; }
            public Guid? MasterCurrency_Guid { get; set; }
            public string MasterCurrency_Name { get; set; }
            public Guid? MasterDenomination_Guid { get; set; }
            public string MasterDenomination_Name { get; set; }
            public double? MasterDenomination_Value { get; set; }
            public double? Input_Value { get; set; }
            public int Country_Unit { get; set; }
            public int? Amount { get; set; }
        }

        #endregion
    }


}
