
namespace Bgt.Ocean.Infrastructure.Util
{
    public static class EnumReport
    {
        public enum ReportStyleId
        {
            [EnumDescription("0")]
            None = 0,
            [EnumDescription("1")]
            DailyRouteCN = 1,
            [EnumDescription("2")]
            CustomerService = 2,
            [EnumDescription("3")]
            DailyCrewLux = 3,
            [EnumDescription("4")]
            DailyCrewCN = 4,
            [EnumDescription("5")]
            DailyRouteLux = 5,
            [EnumDescription("6")]
            VaultInventory = 6,
            [EnumDescription("7")]
            RouteExecutedCN = 7,
            [EnumDescription("8")]
            RouteExecutedLux = 8,
            [EnumDescription("9")]
            ProofofDelivery = 9,
            [EnumDescription("10")]
            ProductivityMex = 10,
            [EnumDescription("11")]
            Document_ID_Mex = 11,
            [EnumDescription("12")]
            ScanInFromCrew,
            [EnumDescription("13")]
            ScanInFromCrewNonPreAdvice,
            [EnumDescription("14")]
            Delivery_Note,
            [EnumDescription("17")] //CN
            WeeklyCrewPlan = 17,
            [EnumDescription("18")]
            WeeklyCrewPlan_EN = 18,
            [EnumDescription("19")]
            SmartBilling = 19,
            [EnumDescription("20")]
            iTrack = 20,
            [EnumDescription("21")]
            Consolidation = 21,
            [EnumDescription("22")]
            Four_Way = 22,
            [EnumDescription("23")]
            Same_Day = 23,
            [EnumDescription("25")]
            DailyCrewFR = 25,
            [EnumDescription("26")]
            Delivery_NoteFR,
            [EnumDescription("27")]
            VaultInventoryFR = 27,
            [EnumDescription("28")]
            ScanInFromCrewFR,
            [EnumDescription("31")]
            DailyRouteFR = 31,
            [EnumDescription("32")]
            RouteExecutedFR = 32,


            [EnumDescription("33")]
            DailyRouteLux_RDLC = 33,
            [EnumDescription("34")]
            DailyRouteCN_RDLC = 34,
            [EnumDescription("35")]
            DailyRouteHK_RDLC = 35,
            [EnumDescription("36")]
            DailyRouteFR_RDLC = 36,
            [EnumDescription("82")]
            DailyRouteRU_RDLC = 82,

            [EnumDescription("37")]
            DailyCrewLux_RDLC = 37,
            [EnumDescription("38")]
            DailyCrewCN_RDLC = 38,
            [EnumDescription("39")]
            DailyCrewFR_RDLC = 39,

            [EnumDescription("40")]
            WeeklyCrewPlan_RDLC = 40,
            [EnumDescription("41")]
            WeeklyCrewPlanCN_RDLC = 41,
            [EnumDescription("42")]

            RouteExecutedLux_RDLC = 42,
            [EnumDescription("43")]
            RouteExecutedCN_RDLC = 43,
            [EnumDescription("44")]
            RouteExecutedFR_RDLC = 44,
            [EnumDescription("80")]
            RouteExecutedRU_RDLC = 80,

            [EnumDescription("45")]
            CustomerService_RDLC = 45,
            [EnumDescription("46")]
            DeliveryNote_Job_EN = 46,
            [EnumDescription("47")]
            DeliveryNote_Job_FR = 47,
            [EnumDescription("48")]

            VaultInventory_RDLC = 48,
            [EnumDescription("49")]
            VaultInventoryFR_RDLC = 49,
            [EnumDescription("81")]
            VaultInventoryRU_RDLC = 81,

            [EnumDescription("51")]
            ProofofDelivery_RDLC = 51,
            [EnumDescription("55")]
            PushToSmart = 55,
            [EnumDescription("52")]
            ScanInFromCrew_RDLC = 52,
            [EnumDescription("53")]
            ScanInFromCrewNonPreAdvice_RDLC = 53,
            [EnumDescription("56")]
            DailyCrewMX = 56,
            [EnumDescription("57")]
            RouteBalance = 57,
            [EnumDescription("58")]
            PickUp = 58,
            [EnumDescription("59")]
            InterBranchSeal_RDLC = 59,
            [EnumDescription("60")]
            InterBranchNonBarcode_RDLC = 60,
            [EnumDescription("61")]
            PickUp_FR = 61,
            [EnumDescription("62")]
            DailyCrew_AllRoute = 62,
            [EnumDescription("63")]
            SmartItemResearch = 63,
            [EnumDescription("66")]
            DisabledUser = 66,
            [EnumDescription("67")]
            UnsuccessfulLogOnAttempts = 67,
            [EnumDescription("71")]
            SecurityReportUserNotLogonLongerThan = 71,
            [EnumDescription("68")]
            SuccessFullLogOnLogOff = 68,
            [EnumDescription("69")]
            ChangeUserProfile = 69,
            [EnumDescription("74")]
            ActiveUser = 74,
            [EnumDescription("70")]
            NewUser = 70,
            [EnumDescription("72")]
            EventSecurityReport = 72,
            [EnumDescription("76")]
            DriverGuideSheet = 76,
            [EnumDescription("84")]
            DailyCrewRU = 84,
            [EnumDescription("85")]
            DailyCrewCNWPB = 85,
            [EnumDescription("86")]
            DailyCrewFRWPB = 86,
            [EnumDescription("87")]
            DailyCrewCN_RDLCWPB = 87,
            [EnumDescription("88")]
            DailyCrewFR_RDLCWPB = 88,
            [EnumDescription("89")]
            DailyCrewMXWPB = 89,
            [EnumDescription("90")]
            DailySortingPurpose = 90,
            [EnumDescription("91")]
            DailyCrewRUWPB = 91,
            [EnumDescription("92")]
            DailyCrewLuxWPB = 92,
            [EnumDescription("93")]
            DailyCrewLux_RDLCWPB = 93,
            [EnumDescription("94")]
            DailyBranch = 94,
            [EnumDescription("95")]
            ConsolidatedRouteDelivery = 95,
            [EnumDescription("97")]
            CheckInFromInterbranchEN = 97,
            [EnumDescription("98")]
            CheckInFromInterbranchFR = 98,
            [EnumDescription("99")]
            CheckOutToInterbranchEN = 99,
            [EnumDescription("100")]
            CheckOutToInterbranchFR = 100,
            [EnumDescription("101")]
            CheckInFromCrewEN = 101,

            [EnumDescription("102")]
            CheckInFromCrewFR = 102,
            [EnumDescription("103")]
            CheckOutFromCrewEN = 103,
            [EnumDescription("104")]
            CheckOutFromCrewFR = 104,
            [EnumDescription("105")]
            CheckInFromInternalDepartmentEN = 105,
            [EnumDescription("106")]
            CheckInFromInternalDepartmentFR = 106,
            [EnumDescription("107")]
            CheckOutFromInternalDepartmentEN = 107,
            [EnumDescription("108")]
            CheckOutFromInternalDepartmentFR = 108,
            [EnumDescription("109")]
            CustomerAudit = 109,
            [EnumDescription("110")]
            EmployeeAudit = 110,
            [EnumDescription("111")]
            InboundCoin = 111,
            [EnumDescription("112")]
            OutboundCoin = 112,
            [EnumDescription("113")]
            LocationMasterRoute = 113,
            [EnumDescription("114")]
            VaultInventoryRoute = 114,
            [EnumDescription("115")]
            PODEmailReport = 115,
            [EnumDescription("116")]
            RouteKPI = 116,
            [EnumDescription("117")]
            QRCodeATMLocations = 117,
            [EnumDescription("118")]
            CheckInFromCrewCA = 118,
            [EnumDescription("119")]
            CheckOutFromCrewCA = 119,
            [EnumDescription("120")]
            CheckInFromInternalDepartmentCA = 120,
            [EnumDescription("121")]
            CheckOutFromInternalDepartmentCA = 121,
            [EnumDescription("122")]
            CheckInFromInterbranchCA = 122,
            [EnumDescription("123")]
            CheckOutToInterbranchCA = 123,
            [EnumDescription("124")]
            ConsolidationPortrait = 124,
            [EnumDescription("125")]
            UserSecurity = 125,
        }

        public enum ReportStyleName
        {
            [EnumDescription("English")]
            ENG,
            [EnumDescription("China")]
            CN,
            [EnumDescription("Mexico")]
            Mex,
            [EnumDescription("Hong Kong")]
            HK,
            [EnumDescription("French")]
            FR,
            [EnumDescription("Russian")]
            RU,
            [EnumDescription("Canada")]
            CA
        }

        public enum ReportName
        {
            [EnumDescription("1,5,31,33,34,35,36,82,85,86,87,88,89,91,92,93")]
            DailyRoute,
            [EnumDescription("2,45")]
            CustomerService,
            [EnumDescription("3,4,25,37,38,39,56,84")]
            DailyCrew,
            [EnumDescription("6,27,48,49,81")]
            VaultInventory,
            [EnumDescription("7,8,32,42,43,44,80")]
            RouteExecuted,
            [EnumDescription("9,51")]
            ProofofDelivery,
            [EnumDescription("10")]
            Productivity,
            [EnumDescription("11")]
            Document_ID_Mex,
            [EnumDescription("12,52")]
            PreVaultReport,
            [EnumDescription("13,53")]
            ScanInFromCrewNonPreAdvice,
            [EnumDescription("14,26,46,47")]
            DeliveryNote,
            [EnumDescription("17,18,40,41")]
            WeeklyCrewPlan,
            [EnumDescription("19")]
            SmartBilling,
            [EnumDescription("20")]
            iTrack,
            [EnumDescription("21")]
            Consolidation,
            [EnumDescription("22")]
            Four_Way,
            [EnumDescription("23")]
            Same_Day,
            [EnumDescription("55")]
            PushToSmart,
            [EnumDescription("57")]
            RouteBalance,
            [EnumDescription("58")]
            PickUp,
            [EnumDescription("59,60")]
            InterBranch,
            [EnumDescription("62")]
            DailyCrew_AllRoute,
            [EnumDescription("63")]
            SmartItemResearch,
            [EnumDescription("64")]
            RouteOtc,
            [EnumDescription("75,74,73,72,71,70,69,68,67,66")]
            Security,
            [EnumDescription("76")]
            DriverGuideSheet,
            [EnumDescription("90")]
            DailySortingPurpose,
            [EnumDescription("94")]
            DailyBranch,
            [EnumDescription("95")]
            ConsolidatedRouteDelivery,
            [EnumDescription("109,110,111,112,113,114")]
            Operational,
            [EnumDescription("116")]
            RouteKPI = 116,
            [EnumDescription("97,98,122")]
            ChkInFromInterBranch,
            [EnumDescription("99,100,123")]
            ChkOutToInterBranch,
            [EnumDescription("101,102,118")]
            CheckInFromCrew,
            [EnumDescription("103,104,119")]
            CheckOutFromCrew,
            [EnumDescription("105,106,120")]
            CheckInFromInternalDepartment,
            [EnumDescription("107,108,121")]
            CheckOutFromInternalDepartment,
            [EnumDescription("117")]
            QRCodeATMLocations,
            [EnumDescription("124")]
            ConsolidationPortrait,
            [EnumDescription("125")]
            UserSecurity,
        }

        public enum ReportFormat
        {
            [EnumDescription("Excel")]
            Excel,
            [EnumDescription("PDF")]
            Pdf,
            [EnumDescription("Zip")]
            Zip,
            [EnumDescription("Ibt")]
            Ibt
        }

        public static class SequenceStop
        {
            public const string TV_P = "1,2";
        }

        public static class ReportAction
        {
            /// <summary>
            /// Check in from crew
            /// </summary>
            public const string CrewToPV = "CrewToPV";
            /// <summary>
            /// Check out to crew
            /// </summary>
            public const string PVToCrew = "PVToCrew";
            /// <summary>
            /// Check in from crew  only non delivery
            /// </summary>
            public const string CrewToPVOnlyNondelivery = "CrewToPVOnlyNondelivery";
            /// <summary>
            /// Check in from crew Cancel
            /// </summary>
            public const string CrewToPVCancel = "CrewToPVCancel";
            /// <summary>
            /// Check in from crew non pre-advise
            /// </summary>
            public const string CrewToPVNon = "CrewToPVNon";
            /// <summary>
            /// Check out to internal dep.
            /// </summary>                             
            public const string PVToInt = "PVToInt";
            /// <summary>
            /// Check in from internal dep.
            /// </summary>
            public const string IntToPV = "IntToPV";
            /// <summary>
            /// Check in from inter Br.  
            /// </summary>
            public const string InterBranchToPV = "InterBranchToPV";
            /// <summary>
            /// check out to Inter Br.
            /// </summary>
            public const string PVToInterBranch = "PVToInterBranch";

        }

        public enum ReportStyle
        {
            [EnumDescription("Seal")]
            Seal = 12,
            [EnumDescription("SealIntBr")]
            SealIntBr = 59,
            [EnumDescription("Nonbarcode")]
            Nonbarcode = 16,
            [EnumDescription("NonbarcodeIntBr")]
            NonbarcodeIntBr = 60,
        }
    }
}
