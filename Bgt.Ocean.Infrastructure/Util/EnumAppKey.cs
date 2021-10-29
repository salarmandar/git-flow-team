
namespace Bgt.Ocean.Infrastructure.Util
{
    #region Country Option
    public static class EnumAppKey
    {
        /// <summary>
        /// [EnumDescription("Flag request approve cancel job")]
        /// </summary>
        public const string FlagRequestCancelApprove = "FlagRequestCancelApprove";
        /// <summary>
        /// [EnumDescription("Calculate premise time per job on update job in progress (AR : arrival time , AC : actual time , DP : departure time)")]
        /// </summary>
        public const string CalculatePremiseTimePerJob = "CalculatePremiseTimePerJob";
        /// <summary>
        /// [EnumDescription("True,False")]
        /// </summary>
        public const string FlagReOrderWorkSeqWhenDispatch = "FlagReOrderWorkSeqWhenDispatch";
        /// <summary>
        /// [EnumDescription("Flag used BGS Pickup HK stlye")]
        /// </summary>
        public const string FlagUsedBgsPuHKStlye = "FlagUsedBgsPuHKStlye";
        /// <summary>
        /// [EnumDescription("Minimum seal digi")]
        /// </summary>
        public const string MinmumSealDigi = "MinmumSealDigi";
        /// <summary>
        /// [EnumDescription("Max Liability digi (STC)")]
        /// </summary>
        public const string MaxLiabilityDigit = "MaxLiabilityDigit";
        /// <summary>
        /// [EnumDescription("")]
        /// </summary>
        public const string RoleIDInRunResourceDailyDuplicate = "RoleIDInRunResourceDailyDuplicate";
        /// <summary>
        /// [EnumDescription("Default Home Currency")]
        /// </summary>
        public const string HomeCurrency = "HomeCurrency";
        /// <summary>
        /// [EnumDescription("1 : Time Inputed , 2 : In Vault")]
        /// </summary>
        public const string CloseRunForPUJobRule = "CloseRunForPUJobRule";
        /// <summary>
        /// [EnumDescription("Requite register employee return equipment")]
        /// </summary>
        public const string FlagRequiteRegisterEmployeeReturnEquipment = "FlagRequiteRegisterEmployeeReturnEquipment";
        /// <summary>
        /// [EnumDescription("Requite register employee return equipment")]
        /// </summary>
        public const string DefaultGenerateDailyJob = "DefaultGenerateDailyJob";
        /// <summary>
        /// [EnumDescription("Alert periodic maintenance when closed run")]
        /// </summary>
        public const string FlagAlertPMWhenClosedRun = "FlagAlertPMWhenClosedRun";
        /// <summary>
        /// [EnumDescription("Check role on run used mobile")]
        /// </summary>
        public const string RoleIDUsedMobile = "RoleIDUsedMobile";
        /// <summary>
        /// [EnumDescription("Dispatch run automatic when create job from WS")]
        /// </summary>
        public const string FlagDispatchAutoWhenCreateJobFromWS = "FlagDispatchAutoWhenCreateJobFromWS";
        /// <summary>
        /// [EnumDescription("Validate time sort on customize sort")]
        /// </summary>
        public const string FlagValidateTimeSortByCustomize = "FlagValidateTimeSortByCustomize";
        /// <summary>
        /// [EnumDescription("Unique equipment ID in all type")]
        /// </summary>
        public const string FlagUniqueEquipmentAllType = "FlagUniqueEquipmentAllType";
        /// <summary>
        /// [EnumDescription("Show location name  D leg for T,TV")]
        /// </summary>
        public const string FlagShowLocationDleg = "FlagShowLocationDleg";
        /// <summary>
        /// [EnumDescription("Equipment use RFID code")]
        /// </summary>
        public const string FlagEquipmetUseRFID = "FlagEquipmetUseRFID";
        /// <summary>
        /// [EnumDescription("Input time 24 hrs or 12 hrs.")]
        /// </summary>
        public const string FlagInputTime24Hrs = "FlagInputTime24Hrs";
        /// <summary>
        /// [EnumDescription("Allow create job without contract")]
        /// </summary>
        public const string FlagAllowCreateJobWithoutContract = "FlagAllowCreateJobWithoutContract";
        /// <summary>
        /// [EnumDescription("Flag request approve change schedule time")]
        /// </summary>
        public const string FlagRequestChangeScheduleTimeApprove = "FlagRequestChangeScheduleTimeApprove";
        /// <summary>
        /// [EnumDescription("It's Number Follow ( RoleInRunResoureID )")]
        /// </summary>
        public const string RoleAllowSendToPreVault = "RoleAllowSendToPreVault";
        /// <summary>
        /// [EnumDescription("It's Number Follow ( RoleInRunResoureID )")]
        /// </summary>
        public const string FlagSkipCheckInFromMP = "FlagSkipCheckInFromMP";
        /// <summary>
        /// [EnumDescription("LOB allow duplicate seal number")]
        /// </summary>
        public const string LobIdAllowDuplicateSeal = "FlagLobIdAllowDuplicateSeal";
        /// <summary>
        /// [EnumDescription("1 : Search only employee permission 2 : Search only employee have working on that day 3 : Search only employee have job with cusomter search.")]
        /// </summary>
        public const string CustomerSearchCrew = "CustomerSearchCrew";
        /// <summary>
        /// [EnumDescription("True : INJOb , False : IN SITE")]
        /// </summary>
        public const string FlagAllowDispatchWithoutRoleCheckIn = "FlagAllowDispatchWithoutRoleCheckIn";
        /// <summary>
        /// [EnumDescription("Validate duplicate seal per Country ON : Not allow duplicate seal in same country OFF : Not allow duplicate seal in same job")]
        /// </summary>
        public const string SealDuplicate = "SealDuplicate";
        /// <summary>
        /// [EnumDescription("Trip indicator required")]
        /// </summary>
        public const string FlagRequiredTripIndicator = "FlagRequiredTripIndicator";
        /// <summary>
        /// [EnumDescription("Days for Alert Password Before Expire")]
        /// </summary>
        public const string DaysAlertPasswordBeforeExpire = "DaysAlertPasswordBeforeExpire";
        /// <summary>
        /// [EnumDescription("Not Use PWD Within Time")]
        /// </summary>
        public const string NotUsePWDWithInTime = "NotUsePWDWithInTime";
        /// <summary>
        /// [EnumDescription("Key Expire")]
        /// </summary>
        public const string KeyExpire = "KeyExpire";
        /// <summary>
        /// [EnumDescription("Job duplicate validation: True = can't aprove for job duplicate data , False = aprove for job duplicate data")]
        /// </summary>
        public const string FlagJobDupValidate = "FlagJobDupValidate";
        /// <summary>
        /// [EnumDescription("Number of times input verify key invalid.")]
        /// </summary>
        public const string InvalidAttempKey = "InvalidAttempKey";
        /// <summary>
        /// [EnumDescription("Authen type by Active Directory Only.")]
        /// </summary>
        public const string FlagAuthenByAdOnly = "FlagAuthenByAdOnly";
        /// <summary>
        /// [EnumDescription("Start date and end date validate with contract.")]
        /// </summary>
        public const string FlagValidateStartEndDateWithContract = "FlagValidateStartEndDateWithContract";
        /// <summary>
        /// [EnumDescription("Allow withdraw equipment without crew on the run")]
        /// </summary>
        public const string FlagAllowWithdrawEquipmentWithoutRun = "FlagAllowWithdrawEquipmentWithoutRun";
        /// <summary>
        /// [EnumDescription("L2C check exceed with this percent")]
        /// </summary>
        public const string ExceedPercentage = "ExceedPercentage";
        /// <summary>
        /// [EnumDescription("Maximum row search")]
        /// </summary>
        public const string MaxDefaultRowSearch = "MaxDefaultRowSearch";
        /// <summary>
        /// FlagRequiredLocationCode : true = request
        /// </summary>
        public const string FlagRequiredLocationCode = "FlagRequiredLocationCode";
        /// <summary>
        /// [EnumDescription("Maximum Ditgit Location Code")]
        /// </summary>
        public const string MaximumDigitLocationCodeRef = "MaximumDigisLocationCodeRef";
        /// <summary>
        /// [EnumDescription("Numeric Only")]
        /// </summary>
        public const string FlagLocationCodeNumericOnly = "FlagLocationCodeNumericOnly";
        //added: 2018/05/04
        /// <summary>
        /// [EnumDescription("All Capital Letter")]
        /// </summary>
        public const string FlagLocationCodeAllCapitalLetter = "FlagLocationCodeAllCapitalLetter";
        //added: 2018/05/04
        /// <summary>
        /// [EnumDescription("Require Exact Length")]
        /// </summary>
        public const string FlagLocationCodeRequireExactLength = "FlagLocationCodeRequireExactLength";
        //added: 2018/05/04
        /// <summary>
        /// [EnumDescription("Show/hide select time for MX")]
        /// </summary>
        public const string InputTimeOnCrewManagement = "InputTimeOnCrewManagement";
        /// <summary>
        /// [EnumDescription("Allow edit job detail for delivery job"]
        /// </summary>
        public const string FlagAllowEditJobDetailForDLeg = "FlagAllowEditJobDetailForDLeg";
        public const string DaysForSendAlertAccessUserGroup = "daysForSendAlert";
        /// <summary>
        /// FlagAllowDuplicateCommercialRegId : true = allow
        /// </summary>
        public const string FlagAllowDuplicateCommercialRegId = "FlagAllowDuplicateCommercialRegId";
        /// <summary>
        /// EnumAppKey Global
        /// </summary>
        public const string MaxRowSearch = "MaxRowSearch";
        /// <summary>
        /// FlagAllowDolphinCloseRunWithoutCheckIn
        /// </summary>
        public const string FlagAllowDolphinCloseRunWithoutCheckIn = "FlagAllowDolphinCloseRunWithoutCheckIn";

        /// <summary>
        ///  Cash Add: Order denomination value in ascending order
        /// </summary>
        public const string FlagOrderMCSAscending = "FlagOrderMCSAscending";

        /// <summary>
        /// Crew Management
        /// </summary>
        public const string RoleAllowDolphinLogin = "RoleAllowDolphinLogin";

        /// <summary>
        /// Truck Limit Liability
        /// </summary>
        public const string FlagValidateRunLiabilityLimit = "FlagValidateRunLiabilityLimit";

        /// <summary>
        /// Truck Limit Liability
        /// </summary>
        public const string FlagAllowExceedLiabilityLimit = "FlagAllowExceedLiabilityLimit";


        /// <summary>
        /// Truck Limit Liability
        /// </summary>
        public const string PercentageLiabilityLimitAlert = "PercentageLiabilityLimitAlert";
        public const string DefaultCurrency = "DefaultCurrency";

        public const string RouteOptimizationDirectoryPath = "RouteOptimizationDirectoryPath";
        /// <summary>
        /// System Config (Hide Identity Fields)
        /// </summary>
        public const string ShowOrHideIdentifyFields = "ShowOrHideIdentifyFields";
    }
    #endregion

    #region Running 
    public static class EnumRunningKey
    {
        public const string AuditLogIDRunning = "AuditLogIDRunning";
        public const string OptimizeRunning = "OptimizeRunning";
        public const string MassRequestRunning = "MassRequestRunning";
        public const string JobNo = "JobNo";
        public const string ServiceRequestRunning = "ServiceRequestRunning";
        public const string RouteOptimizeRunning = "ROUTE_OPTIMIZE";
    }
    #endregion
}
