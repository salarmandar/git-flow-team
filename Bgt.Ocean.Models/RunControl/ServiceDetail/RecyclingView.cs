using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    //Recycling
    public class SvdRecyclingMachineReportWODispense
    {
        public string Title { get; set; } = "Machine Report w/o Dispense";
        public decimal TotalRemain { get; set; }
        public decimal TotalReject { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<MRDRCTransectionView> MachinReportDispenseList { get; set; } = new List<MRDRCTransectionView>();
    }
    public class SvdRecyclingActualCount
    {
        public string Title { get; set; } = "Actual Count";
        public decimal TotalAmount { get; set; }
        public decimal TotalReject { get; set; }
        public decimal TotalDiff { get; set; }
        public IEnumerable<ACRCTransectionView> ActualCountList { get; set; } = new List<ACRCTransectionView>();
    }
    public class SvdRecyclingCashRecycling
    {
        public string Title { get; set; } = "Cash Recycling";
        public decimal DeliveryAmount { get; set; }
        public decimal TotalLoaded { get; set; }
        public decimal TotalRecycled { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencyAbbr { get; set; }
        public string CustomerOrderNo { get; set; } = "";
        public IEnumerable<RRCTransectionView> CassetteList { get; set; } = new List<RRCTransectionView>();
        public IEnumerable<SealBagView> ReturnCashBagList { get; set; } = new List<SealBagView>();
    }

    public class MRDRCTransectionView : CassetteBase
    {
        public int Remain { get; set; }
        public int Reject { get; set; }
        public decimal Amount { get; set; }
    }
    public class ACRCTransectionView : CassetteBase
    {
        public int Counted { get; set; }
        public int Reject { get; set; }
        public int Diff { get; set; }
        public decimal Amount { get; set; }
    }

    public class RRCTransectionView : CassetteBase
    {
        public decimal Amount { get; set; }
        public decimal Loaded { get; set; }
        public decimal Recyled { get; set; }
    }


}
