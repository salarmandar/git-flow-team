using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{

    public class SvdBulkNoteDepositDepositReport
    {

        public string Title { get; set; } = "Deposit Collection Report";
        public decimal TotalAmount { get; set; }
        public IEnumerable<DCRBNDTransectionView> CassetteList { get; set; } = new List<DCRBNDTransectionView>();
        public IEnumerable<SealBagView> DepositReturnBagList { get; set; } = new List<SealBagView>();
    }
    
    public class SvdBulkNoteDepositRetract
    {
        public string Title { get; set; } = "Retract";
        public decimal TotalReport { get; set; }
        public decimal TotalCounted { get; set; }
        public decimal TotalDiff { get; set; }
        public int TotalDocument { get; set; }
        public IEnumerable<RBNDTransectionView> DenoList { get; set; } = new List<RBNDTransectionView>();
        public IEnumerable<SealBagView> RetractReturnBagList { get; set; } = new List<SealBagView>();
        public IEnumerable<SealBagView> DocumentT1BagList { get; set; } = new List<SealBagView>();
        public IEnumerable<RBNDDocumentT1View> DocumentT1DetailList { get; set; } = new List<RBNDDocumentT1View>();
    }

    public class SvdBulkNoteDepositSuspectFake
    {
        public string Title { get; set; } = "Suspect Fake";
        public decimal TotalReport { get; set; }
        public decimal TotalCounted { get; set; }
        public decimal TotalDiff { get; set; }
        public IEnumerable<SFBNDTransectionView> DenoList { get; set; } = new List<SFBNDTransectionView>();

    }

    public class SvdBulkNoteDepositSuspectFakeDetail
    {
        public string Title { get; set; } = "Suspect Fake";
        public IEnumerable<SuspectFakeDetailView> SuspectFakeDetailList { get; set; } = new List<SuspectFakeDetailView>();
        public IEnumerable<SealBagView> SuspectFakeBagList { get; set; } = new List<SealBagView>();
    }

    public class SvdBulkNoteDepositJammed
    {
        public string Title { get; set; } = "Jammed";
        public decimal TotalAmount { get; set; }
        public IEnumerable<JBNDTransectionView> DenoList { get; set; } = new List<JBNDTransectionView>();

    }

    public class SvdBulkNoteDepositJammedDetail
    {
        public string Title { get; set; } = "Jammed";
        public IEnumerable<JBNDDetailView> JammedDetailList { get; set; } = new List<JBNDDetailView>();
        public IEnumerable<SealBagView> JammedBagList { get; set; } = new List<SealBagView>();
    }
 
    public class DCRBNDTransectionView : CassetteBase
    {
        public int NumberOfNote { get; set; }
        public decimal Amount { get; set; }
    }
    public class RBNDTransectionView : DenoBase
    {
        public int Report { get; set; }
        public int Counted { get; set; }
        public int Diff { get; set; }
    }
    public class SFBNDTransectionView : DenoBase
    {
        public decimal Report { get; set; }
        public decimal Counted { get; set; }
        public decimal Diff { get; set; }
    }
    public class JBNDTransectionView : DenoBase
    {
        public int Jammed { get; set; }
        public decimal Amount { get; set; }
    }
    public class JBNDDetailView
    {
        public string JammedReason { get; set; }
        public int NoOfNote { get; set; }
    }

    public class RBNDDocumentT1View
    {
        public string ReasonName { get; set; }
        public int NumberOfNote { get; set; }
    }

}
