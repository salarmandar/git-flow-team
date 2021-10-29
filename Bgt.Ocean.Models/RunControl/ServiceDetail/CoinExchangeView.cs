using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{

    //Coin Exchange
    public class SvdCoinExchangeMachineBalance
    {
        public string Title { get; set; } = "Machin Balance";
        public decimal TotalReport { get; set; }
        public decimal TotalCounted { get; set; }
        public decimal TotalDiff { get; set; }
        public decimal BeginningAmount { get; set; }
        public string CurrencyAbbr { get; set; }
        public IEnumerable<MBCassetteTransectionView> MachineBalanceHopperList { get; set; } = new List<MBCassetteTransectionView>();
        public IEnumerable<MBCassetteTransectionView> MachineBalanceNoteList { get; set; } = new List<MBCassetteTransectionView>();
        public IEnumerable<MBCassetteTransectionView> MachineBalanceCoinList { get; set; } = new List<MBCassetteTransectionView>();
    }
    public class SvdCoinExchangeCashAdd
    {
        public string Title { get; set; } = "Cash Add";
        public decimal TotalLoadded { get; set; }
        public decimal TotalATM { get; set; }
        public decimal TotalSurplus { get; set; }
        public decimal PrevSurplusAmount { get; set; }
        public decimal DeliveryAmount { get; set; }
        public decimal TotalAmountAvailable { get; set; }
        public string CurrencyAbbr { get; set; }
        public string CoinOrderNo { get; set; } = "";
        public string NoteOrderNo { get; set; } = "";
        public IEnumerable<CACassetteTransectionView> CashAddHopperList { get; set; } = new List<CACassetteTransectionView>();
        public IEnumerable<CACassetteTransectionView> CashAddNoteList { get; set; } = new List<CACassetteTransectionView>();
        public IEnumerable<CACassetteTransectionView> CashAddCoinList { get; set; } = new List<CACassetteTransectionView>();
    }
    public class SvdCoinExchangeCashReturn
    {
        public string Title { get; set; } = "Cash Return";
        public decimal TotalReturn { get; set; }
        public decimal TotalStay { get; set; }
        public decimal TotalDiff { get; set; }
        public IEnumerable<CRCXTransectionView> CashReturnHopperList { get; set; } = new List<CRCXTransectionView>();
        public IEnumerable<CRCXTransectionView> CashReturnNoteList { get; set; } = new List<CRCXTransectionView>();
        public IEnumerable<CRCXTransectionView> CashReturnCoinList { get; set; } = new List<CRCXTransectionView>();
        public IEnumerable<SealBagView> ReturnBagList { get; set; } = new List<SealBagView>();
        public IEnumerable<SealBagView> StayBagList { get; set; } = new List<SealBagView>();

    }
    public class SvdCoinExchangeBulkNote
    {
        public string Title { get; set; } = "Bulk Note Collection";
        public decimal TotalAmount { get; set; }
        public IEnumerable<BNCXTransectionView> BulkNoteCollectionList { get; set; } = new List<BNCXTransectionView>();
        public IEnumerable<SealBagView> BulkNoteBagList { get; set; }
    }
    public class SvdCoinExchangeSuspectFake
    {
        public string Title { get; set; } = "Suspect Fake";
        public decimal TotalReport { get; set; }
        public decimal TotalDiff { get; set; }
        public decimal TotalCounted { get; set; }
        public IEnumerable<SuspectFakeTransectionView> SuspectFakeList { get; set; } = new List<SuspectFakeTransectionView>();
        public IEnumerable<SuspectFakeDetailView> SuspectFakeDetailList { get; set; } = new List<SuspectFakeDetailView>();
        public IEnumerable<SealBagView> SuspectFakeBagList { get; set; } = new List<SealBagView>();
    }

    public class MBCassetteTransectionView : CassetteBase
    {
        public int Report { get; set; }
        public int Counted { get; set; }
        public int Diff { get; set; }
    }
    public class CACassetteTransectionView : CassetteBase
    {
        public decimal Loaded { get; set; }
        public decimal TotalATM { get; set; }
        public decimal Surplus { get; set; }
    }
    public class CRCXTransectionView : CassetteBase
    {
        public int Return { get; set; }
        public int Stay { get; set; }
        public int Diff { get; set; }
    }
    public class BNCXTransectionView : DenoBase
    {
        public decimal AllIn { get; set; }
        public decimal Amount { get; set; }
    }

}
