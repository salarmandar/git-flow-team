using Bgt.Ocean.Models.Masters;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    //Other
    public class SvdCapturedCard
    {
        public IEnumerable<CapturedCard> CapturedCardList { get; set; } = new List<CapturedCard>();
        public IEnumerable<CitSealView> DelToMainBankBranchList { get; set; } = new List<CitSealView>();
        public IEnumerable<CitSealView> DelToBankBranchList { get; set; } = new List<CitSealView>();
        public bool FlagDeliverCardMainBank { get; set; }
        public bool FlagDeliverCardBankBranch { get; set; }
    }

    public class CapturedCard
    {
        public string CardNo { get; set; }
        public string HolderName { get; set; }
        public string BankName { get; set; }
    }
}
