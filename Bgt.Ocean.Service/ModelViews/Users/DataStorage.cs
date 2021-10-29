using Bgt.Ocean.Service.Messagings;
using System;

namespace Bgt.Ocean.Service.ModelViews.Users
{
    public class DataStorage : BaseResponse
    {
        public Guid UserGuid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool FlagCustomer { get; set; }
        public Guid? GuidKeyID { get; set; }
        public bool? FlagChangePWDNextLogIn { get; set; }
        public bool? FlagLock { get; set; }
        public bool FlagDisable { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? FlagSupervisorVerification { get; set; }
        public bool FlagApprove { get; set; }
        public DateTime? LastDateTimeLogin { get; set; }
        public Guid? SystemDomain_Guid { get; set; }
        public string SystemDomain { get; set; }
        public string PersonImage { get; set; }
        public byte RoleType { get; set; }
        public string FormatDate { get; set; }
        public string FormatNumberCurrency { get; set; }
        public Guid? MasterSite_Guid { get; set; }
        public string LogoCustomer { get; set; }
        public Guid? UserLanguageGuid { get; set; }
        public Guid? MasterCountry_Guid { get; set; }
        public string MasterCountryName { get; set; }
        public string FlagPwdExpiry { get; set; }
        public string UserHomeCurrency { get; set; }
        public string UserHomeCurrencyGuid { get; set; }
        public string PasswordDecrypted { get; set; }
        public string SiteOnlyName
        {
            get
            {
                try
                {
                    return !string.IsNullOrEmpty(SiteName) ? SiteName.Split('-')[1]?.Trim() : string.Empty;
                }
                catch
                {

                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Site Code + Site Name
        /// </summary>
        public string SiteName { get; set; }
        public string SiteCode
        {
            get
            {
                try
                {
                    return !string.IsNullOrEmpty(SiteName) ? SiteName.Split('-')[0]?.Trim() : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public Guid? BrinksCompanyGuid { get; set; }
        public string BrinksCompanyName { get; set; }

        /// <summary>
        /// default is 'false' -> no limit access time (can access all the time)
        /// </summary>
        public bool? FlagLimitedTimeAccess { get; set; }
        public string DefaultMasterCurrencyAbbreviation { get; set; }
        public Guid DefaultMasterCurrencyGuid { get; set; }
        public Guid MasterPrevault_Guid { get; set; }
        public bool FlagCanApprove { get; set; }
        public Guid? FormatDateGuid { get; set; }
        public Guid? FormatNumberCurrencyGuid { get; set; }
        public bool FlagUseAmPm { get; set; }
        public int MaxRowSearch { get; set; }
        public int MaxRowSystemSearch { get; set; }
        public bool FlagStarFish { get; set; }

        /// <summary>
        /// this will return '{FormatDate} {HH:mm or hh:mm tt}' base on FlagUseAmPm
        /// </summary>
        public string FormatDateTime
        {
            get
            {
                return FormatDate + " " + FormatTime;
            }
        }

        public string FormatDateTimeDefault
        {
            get
            {
                return FormatDate + " HH:mm";
            }
        }

        /// <summary>
        /// return time format of user base on FlagUseAmPm
        /// </summary>
        public string FormatTime
        {
            get
            {
                return FlagUseAmPm ? "hh:mm tt" : "HH:mm";
            }
        }

        public decimal PercentDiscount { get; set; }
        public string LevelName { get; set; }
        public string SessionConfig { get; set; }
        public bool FlagLoadBankCleanOut { get; set; }
        public bool FlagAllUpdateHistoricJob { get; set; }
        public string[] RestrictedActionList { get; set; }
        public bool IsVPNRestricted { get; set; }
        public string UserEmail { get; set; }
        public string DefaultView { get; set; }

        /// <summary>
        /// Format number currency
        /// </summary>
        public string NumberCurrencyCultureCode { get; set; }
        public Guid? SystemDomainAlies_Guid { get; set; }
        public bool IsCheckDomain { get; set; }
        public bool FlagSkipSSO { get; set; }
        public string EmployeeID { get; set; }
        public Guid ApplicationGuid { get; set; }
    }
}
