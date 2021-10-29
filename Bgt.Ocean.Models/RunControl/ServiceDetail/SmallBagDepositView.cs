using Bgt.Ocean.Models.Masters;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    //Small Bag Deposit
    public class SvdSmallBagDepositSmallBag
    {
        public IEnumerable<MasterIDCollection> ItemsList { get; set; } = new List<MasterIDCollection>();
    }

    public class SvdSmallBagDepositSmallBagCollection
    {
        public IEnumerable<SmallBagAndBulkCashSealView> ItemsList { get; set; } = new List<SmallBagAndBulkCashSealView>();
    }
}
