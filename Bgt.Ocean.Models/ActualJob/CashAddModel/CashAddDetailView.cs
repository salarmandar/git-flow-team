using System.Collections.Generic;

namespace Bgt.Ocean.Models.ActualJob.CashAddModel
{
  public  class CashAddDetailView
    {
        public IEnumerable< TblMasterActualJobSumCashAdd> CashAddHeader { get; set; }
        public IEnumerable<TblMasterActualJobCashAdd > CashAddDetail { get; set; }

    }
}
