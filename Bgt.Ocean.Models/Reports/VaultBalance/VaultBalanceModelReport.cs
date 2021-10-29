using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Models.Reports.VaultBalance
{   

    public class VaultValanceModelReport_Main
    {
        public List<VaultBalanceModelReport> Header { get; set; }
        public List<VaultBalanceModelReport_BalanceNonBar> ListBalance { get; set; }
        public List<VaultBalanceModelReport_DisNonBar> ListDiscrepency { get; set; }
        public string UserVerify { get; set; }
        public string datetimeVerify { get; set; }
    }

    public class VaultBalanceModelReport
    {
        public int? BalanceSeal_Individual_PreAdvQuantity { get; set; }
        public int? BalanceSeal_Individual_ActualQuantity { get; set; }
        public int? BalanceSeal_LocationMaster_PreAdvQuantity { get; set; }
        public int? BalanceSeal_LocationMaster_ActualQuantity { get; set; }
        public int? BalanceSeal_RouteMaster_PreAdvQuantity { get; set; }
        public int? BalanceSeal_RouteMaster_ActualQuantity { get; set; }

        public int? DiscrepanciesSeal_Missing_Quantity { get; set; }
        public string DiscrepanciesSeal_Missing_SealNumber { get; set; }
        public int? DiscrepanciesSeal_Extra_Quantity { get; set; }
        public string DiscrepanciesSeal_Extra_SealNumber { get; set; }

        public string UserCreate { get; set; }
        public string datetimeCreate { get; set; }

    }

    public class VaultBalanceModelReport_BalanceNonBar
    {
        public string orderCommodity { get; set; }
        public string BalanceNonBar_Commodity { get; set; }
        public int? BalanceNonBar_PreAdvice { get; set; }
        public int? BalanceNonBar_Actual { get; set; }
    }

    public class VaultBalanceModelReport_DisNonBar
    {
        public string orderCommodity { get; set; }
        public string DisNonBar_Commodity { get; set; }
        public int? DisNonBar_Missing { get; set; }
        public int? DisNonBar_Extra { get; set; }
    }

}
