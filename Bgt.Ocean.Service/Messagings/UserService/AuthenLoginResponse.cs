using System;

namespace Bgt.Ocean.Service.Messagings.UserService
{
    public class AuthenLoginResponse
    {
        public System.Guid GuidMasterUser { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool FlagCustomer { get; set; }
        public Nullable<System.Guid> GuidKeyID { get; set; }
        public Nullable<bool> FlagChangePWDNextLogIn { get; set; }
        public Nullable<bool> FlagLock { get; set; }
        public bool FlagDisable { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<bool> FlagSupervisorVerification { get; set; }
        public bool FlagApprove { get; set; }
        public Nullable<System.DateTime> LastDateTimeLogin { get; set; }
        public Nullable<System.Guid> SystemDomain_Guid { get; set; }
        public string SystemDomain { get; set; }
        public string PersonImage { get; set; }
        public byte RoleType { get; set; }
        public string FormatDate { get; set; }
        public string FormatNumberCurrency { get; set; }
        public Nullable<System.Guid> DefaultMasterCurrencyGuid { get; set; }
        public Nullable<System.Guid> MasterSite_Guid { get; set; }
        public string LogoCustomer { get; set; }
        public Nullable<System.Guid> SystemLanguage_Guid { get; set; }
        public System.Guid MasterCountry_Guid { get; set; }
        public string MasterCountryName { get; set; }
        public string FlagPwdExpiry { get; set; }
        public string UserHomeCurrency { get; set; }
        public string UserHomeCurrencyGuid { get; set; }
        public string DefaultView { get; set; }
        public Nullable<bool> FlagLimitedTimeAccess { get; set; }
        public Nullable<System.Guid> MasterPrevault_Guid { get; set; }
        public Nullable<bool> FlagCanApprove { get; set; }
        public string LevelName { get; set; }
        public Nullable<double> PercentDiscount { get; set; }
        public System.Guid BrinksCompanyGuid { get; set; }
        public string BrinksCompanyName { get; set; }
        public string SiteName { get; set; }
        public bool FlagAllUpdateHistoricJob { get; set; }
        public bool FlagSkipSSO { get; set; }
    }
}
