
namespace Bgt.Ocean.Infrastructure.Util
{
    public enum EnumPageName
    {
        [EnumDescription("Run Control")]
        RunControl = 0,
        [EnumDescription("Cash Delivery Preparation")]
        CashDeliveryPreparation = 1,
        [EnumDescription("Fast Update Jobs")]
        FastUpdateJobs = 2,
        [EnumDescription("Bank Clean Out")]
        BankCleanOut = 3,
        [EnumDescription("Cash Delivery Import")]
        CashDeliveryImport = 4,
        [EnumDescription("Deconsolidation")]
        Deconsolidation = 5,
        [EnumDescription("Consolidation")]
        Consolidation = 6
    }

    public enum EnumVaultState
    {
        [EnumDescription("None/Close")]
        None = 0,
        [EnumDescription("In progress")]
        Process = 1,
        [EnumDescription("On Hold")]
        OnHold = 2,
        [EnumDescription("Completed")]
        Complete = 3
    }

    public enum EnumItemState
    {
        [EnumDescription("Not scan")]
        NotScan = 0,
        [EnumDescription("Scanned")]
        Scanned = 1,
        [EnumDescription("Overage")]
        Overage = 2,
        [EnumDescription("Shortage")]
        Shortage = 3,
    }

    public enum EnumConsolidateType
    {
        [EnumDescription("Individaul")]
        Individaul = 0,
        [EnumDescription("Location")]
        Location = 1,
        [EnumDescription("Route")]
        Route = 2,
        [EnumDescription("Interbranch")]
        Interbranch = 3,
        [EnumDescription("Multi-Branch")]
        MultiBranch = 4,
    }

    public enum EnumItemType
    {
        [EnumDescription("Seal")]
        Seal = 1,
        [EnumDescription("Nonbarcode")]
        Nonbarcode = 2,
        [EnumDescription("Consolidate")]
        Consolidate = 3,
    }

    public static class EnumPreVault
    {
        public static class StatusConsolidate
        {
            public const int Inprocessive = 1;
            public const int Sealed = 2;
            public const int Consolidated = 3;
            public const int Completed = 4;
            public const int Deconsolidated = 5;
            public const int InterBranchConsolidatedDone = 6;
            public const int InProcess_Deconsolidate = 7;
            public const int ConsolidateOutVault = 8;
        }

        public static class ConsolidateStatusName
        {
            public const string Inprocess = "In-Process";
            public const string Sealed = "Sealed";
        }

        public static class ConsolidateSource
        {
            public const int Internal = 1;
            public const int External = 2;
            public const int InterBranch = 3;
            public const int MultiBranch = 4;
        }

        public static class FixStringPrevault
        {
            public const string ConInnerLayer = "Con inner layer";
            public const string Seal = "Seal";
            public const string SealInLiability = "Seal In Liability";
            public const string SealItem = "Seal Item";
            public const string Liability = "Liability";
            public const string Commodity = "Commodity";
            public const string NonBarcode = "NonBarcode";
            public const string Non_Barcode = "Non Barcode";
            public const string Message = "Message";
            public const string Route_Balance = "Route Balance";
            public const string LocationLiability = "Location Liability";
            public const string LocationMaster = "Location Consolidated Seal";
            public const string InterBranchToPV = "InterBranchToPV";
            public const string PVToInterBranch = "PVToInterBranch";
            public const string RouteMaster = "Route Consolidated Inter-Branch Seal";
        }

        public static class ScanItemByType
        {
            public const string KeyIn = "Key-in";
            public const string BarcodeScan = "Barcode scan";
        }
    }
}
