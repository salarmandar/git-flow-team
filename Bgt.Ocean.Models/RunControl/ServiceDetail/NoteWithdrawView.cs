using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    //Note Withdraw
    public class SvdNoteWithdrawMachineReport
    {
        public decimal SumDispense { get; set; }
        public decimal SumReject { get; set; }
        public decimal SumRemain { get; set; }
        public int BeginningAmount { get; set; }

        public string CurrencyAbbr { get; set; }
        public IEnumerable<MRNWTransectionView> CassetteList { get; set; } = new List<MRNWTransectionView>(); //cassette : dispense,reject,remain       
    }
    public class SvdNoteWithdrawActualCount
    {
        public decimal SumCount { get; set; }
        public decimal SumReject { get; set; }
        public decimal SumDiff { get; set; }

        public string CurrencyAbbr { get; set; }
        public IEnumerable<ACNWTransectionView> CassetteList { get; set; } = new List<ACNWTransectionView>();//cassette : counted,reject,deff.
    }
    public class SvdNoteWithdrawCashAdd
    {
        public decimal SumLoadded { get; set; }
        public decimal SumTotalATM { get; set; }
        public decimal SumSurplus { get; set; }
        public int PrevSurplusAmont { get; set; }
        public int DeliveryAmount { get; set; }
        public int TotalAmountAvaliable { get; set; }    
        public string CustomerOrderNo { get; set; }
        public decimal OtherMachineAmount { get; set; }
        public string CurrencyAbbr { get; set; }        
        public IEnumerable<CANWTransectionView> CassetteList { get; set; } = new List<CANWTransectionView>();//cassette : loaded,total atm,surplus
    }
    public class SvdNoteWithdrawCashReturn
    {
        public decimal SumReturn { get; set; }
        public decimal SumStay { get; set; }
        public decimal SumDiff { get; set; }
        public decimal ReturnAmount { get; set; }
        public IEnumerable<SealBagView> StayBagList { get; set; } = new List<SealBagView>();
        public IEnumerable<SealBagView> ReturnBagList { get; set; } = new List<SealBagView>();
        public string CurrencyAbbr { get; set; }
        public IEnumerable<CRNWTransectionView> CassetteList { get; set; } = new List<CRNWTransectionView>();//cassette : dispense,reject,remain

    }



    public class MRNWTransectionView : CassetteBase
    {
        public int Dispense { get; set; }
        public int Reject { get; set; }
        public int Remain { get; set; }
    }
    public class ACNWTransectionView : CassetteBase
    {
        public int Count { get; set; }
        public int Reject { get; set; }
        public int Diff { get; set; }
    }
    public class CANWTransectionView : CassetteBase
    {
        public int Loadded { get; set; }
        public int TotalATM { get; set; }
        public int Surplus { get; set; }
    }
    public class CRNWTransectionView : DenoBase
    {
        public int Return { get; set; }
        public int Stay { get; set; }
        public int Diff { get; set; }
    }
}
