using System.ComponentModel;

namespace Bgt.Ocean.Infrastructure.Util
{
    public static class EnumATMScreen
    {
        public const string Mr = "Mr";
        public const string Ca = "Ca";
        public const string Cr = "Cr";
        public const string Ac = "Ac";

    }
    public static class DenominationTypeID
    {
        public const int Note = 1;
        public const int Coin = 2;
    }
    public enum DenoUnit
    {
        [EnumDescription("2115D058-EFBD-4BA9-A0C3-6720546D1AD4")]
        BankNote = 1,
        [EnumDescription("AAD4968F-8A92-45CC-A70B-E75E8157668A")]
        Coin = 2,
    }

    public enum Capability
    {
        None = 0,
        BulkNoteDeposit = 1,
        NoteWithdraw = 2,
        SmallBagDeposit = 3,
        Recycling = 4,
        CoinExchange = 5
    }

    public enum SealTypeID
    {
        Other = 0, //CIT 
        NoteWithdraw = 1,
        DeliverToBankBranch = 2,
        DeliverToMainBankBranch = 3,
        BulkCashDeposit = 4,
        SmallBagDeposit = 5,

        ReturnBag = 6,
        StayBag = 7,
        ReturnCashBag = 8,
        DepositReturnBag = 9,
        RetrackBag = 10,
        SuspectFakeBag = 11,
        JammedBag = 12,
        BulkNoteBag = 13,

        T1Bag = 15,
        ReturnCoinBag = 16

    }
    public static class CustomerLocationTypeName
    {
        public const int eCash = 4;
        public const int Branch = 5;
        public const int KeySafe = 10;
        public const int ATMMachine = 1;
        public const int TransferSafe = 9;
        public const int CompuSafeMachine = 3;
        public const int Airport = 3;
        public const int Location = 2;
        public const int Railway = 4;
        public const int SeaPort = 5;

    }


    public enum CassetteType
    {
        None = 0,
        Normal = 1,
        Reject = 2,
        Retract = 3,
        AllIn = 4,
        Hopper = 5,
        Coin = 6
    }

    public enum CashAddPropertiesTab
    {
        None = 0,
        tabDetail = 100,
        tabLeg = 200,
        tabServiceDetail = 300, //SVD
        //CIT
        tabSVD_CITDelivery = 301,
        //Note Withdraw 
        tabSVD_NoteWithdraw_MachineReport_ActualCount = 302,
        tabSVD_NoteWithdraw_CashAdd_CashReturn = 303,
        //Recycling
        tabSVD_Recycling_MachineReportWODispense_ActualCount = 304,
        tabSVD_Recycling_CashRecycling = 305,
        //Bulk Note Deposit 
        tabSVD_BulkNoteDeposit_DepositReport_Retract = 306,
        tabSVD_BulkNoteDeposit_SuspectFake = 307,
        tabSVD_BulkNoteDeposit_Jammed = 308,
        //Coin Exchange 
        tabSVD_CoinExchange_MachineBalance_CashAdd = 309,
        tabSVD_CoinExchange_CashReturn_BulkNote = 310,
        tabSVD_CoinExchange_SuspectFake = 311,
        //Small Bag Deposit 
        tabSVD_SmallBagDeposit_SmallBag = 312,
        //Other
        tabSVD_CapturedCard = 313,
        tabSVD_Checklist = 314,
        tabHistory = 400,
    }

    public enum JobScreen
    {
        [Description("00000000-0000-0000-0000-000000000000")]
        None = 0,
        [Description("0E47D40D-AC41-4162-83F4-79FDD77D2581")]
        ActualCount = 1,
        [Description("4CDD2215-4115-474C-9DB8-FA008B57518B")]
        MachineReport = 2,
    }

    public enum JobField
    {
        [Description("00000000-0000-0000-0000-000000000000")]
        EntireScreen = 0,
        [Description("77F8502A-7544-442D-B0E4-500D2561E052")]
        Dispense_Beginning_TotalATM = 1,
        [Description("EEAF3532-B9F1-43D6-8626-ECA9EB184695")]
        None = 99,
    }

    public enum CommoditySealtype
    {
        [Description("Note")]
        Note = 8,
        [Description("Coin")]
        Coin = 16
    }

}
