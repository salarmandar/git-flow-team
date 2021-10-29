using System;

namespace Bgt.Ocean.WebAPI.Models.AuditLog.CustomerLocation
{
    public class AuditLogCustomerLocationDetailModel : AuditLogUpdateModel<TblMasterCustomerLocation, TblMasterCustomerLocation>
    {
    }


    #region Source and Target Model

    public class TblMasterCustomerLocation
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterCustomer_Guid { get; set; }
        public Nullable<System.Guid> SystemCustomerOfType_Guid { get; set; }
        public Nullable<System.Guid> SystemCustomerLocationType_Guid { get; set; }
        public Nullable<System.Guid> MasterSite_Guid { get; set; }
        public Nullable<System.Guid> MasterCity_Guid { get; set; }
        public string StateName { get; set; }
        public Nullable<System.Guid> MasterDistrict_Guid { get; set; }
        public string CitryName { get; set; }
        public Nullable<System.Guid> MasterPlace_Guid { get; set; }
        public Nullable<System.Guid> MasterServiceHour_Guid { get; set; }
        public string BranchCodeReference { get; set; }
        public string BranchName { get; set; }
        public string BranchAbbrevaitionName { get; set; }
        public string BranchProgramName { get; set; }
        public string BranchReportName { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string ContractName { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public string Restrictions { get; set; }
        public string CardReturnBranch { get; set; }
        public byte[] QR_Code { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<int> ServiceDuration { get; set; }
        public Nullable<bool> FlagDefaultServiceHours { get; set; }
        public bool FlagSmokeBox { get; set; }
        public string EmergencyProcedures { get; set; }
        public string AccessRequirments { get; set; }
        public string AlarmActivationCode { get; set; }
        public string AlarmDuressCode { get; set; }
        public bool FlagOutOfTown { get; set; }
        public Nullable<double> Distance { get; set; }
        public Nullable<System.Guid> SystemGlobalUnit_Distance_Guid { get; set; }
        public Nullable<double> CeilingHeight { get; set; }
        public Nullable<System.Guid> SystemGlobalUnit_CeilingHeight_Guid { get; set; }
        public Nullable<int> WaitingMinute { get; set; }
        public Nullable<short> ReportOrderPriority { get; set; }
        public bool FlagExchangeMoney { get; set; }
        public Nullable<bool> FlagHaveAirport { get; set; }
        public string ExternalID { get; set; }
        public Nullable<System.DateTime> StartofService { get; set; }
        public Nullable<System.DateTime> EndofService { get; set; }
        public Nullable<int> PeriodofServiceDay { get; set; }
        public Nullable<int> PeriodofServiceMonth { get; set; }
        public bool FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<int> OnwardDestinationType { get; set; }
        public Nullable<System.Guid> OnwardDestination_Guid { get; set; }
        public string CustomerLocationExtrnalID { get; set; }
        public Nullable<System.Guid> MasterCountry_State_Guid { get; set; }
        public Nullable<bool> FlagPrintReceive { get; set; }
        public Nullable<bool> FlagSendPodEmail { get; set; }
        public Nullable<int> TimeZoneID { get; set; }
        public Nullable<System.Guid> SystemTimezone_Guid { get; set; }
        public Nullable<bool> FlagDefaultCommodity { get; set; }
        public string PdfPassword { get; set; }
        public string ExtBranchID { get; set; }
        public string ExtBranchName { get; set; }
        public Nullable<System.Guid> MasterNonBillable_Guid { get; set; }
        public Nullable<System.DateTime> StartServiceDate { get; set; }
        public Nullable<System.DateTime> EndServiceDate { get; set; }
        public Nullable<System.Guid> SFOCashBranch_Guid { get; set; }
        public Nullable<System.Guid> SFOServicingBranch_Guid { get; set; }
        public Nullable<System.Guid> SFOFLMBranch_Guid { get; set; }
        public Nullable<System.Guid> SFOMasterFLMZone_Guid { get; set; }
        public Nullable<System.Guid> SFOMasterECashZone_Guid { get; set; }
        public Nullable<System.Guid> SFOMasterCompuSafeZone_Guid { get; set; }
        public Nullable<System.Guid> SFOCountryTimeZone_Guid { get; set; }
        public bool FlagSFOBranch { get; set; }
        public bool FlagNonBillable { get; set; }
        public bool FlagCommentOnDPNotReq { get; set; }
        public Nullable<System.Guid> MasterLocationReportGroup_Guid { get; set; }
        public string Branch { get; set; }
        public string UpperLock { get; set; }
        public string LowerLock { get; set; }
        public string LockMode { get; set; }
        public string LockUser { get; set; }
        public Nullable<bool> FlagSendMissedStop { get; set; }
        public string EmailUnableServiceNotification { get; set; }
        public Nullable<decimal> PremiseTime { get; set; }
        public Nullable<int> RiskLevelRating { get; set; }
        public Nullable<System.Guid> MasterGeoZone_Guid { get; set; }
        public Nullable<System.Guid> SystemAirportType_Guid { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationAddress4 { get; set; }
        public bool FlagSftp_POD { get; set; }
        public bool FlagGenUnableToServiceReport { get; set; }
        public bool FlagUseSftpPubKey { get; set; }               
    }

    #endregion
}