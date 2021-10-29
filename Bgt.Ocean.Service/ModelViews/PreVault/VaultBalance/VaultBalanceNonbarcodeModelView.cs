using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance
{
    public class VaultBalanceNonbarcodeModelView
    {
        public Guid CommodityGuid { get; set; }
        public string CommodityName { get; set; }
        public double ActualStc { get; set; }
        public double STC { get; set; }
        public double CommodityValue { get; set; }
        public double CommodityAmount { get; set; }
        public int PreAdviceQty { get; set; }
        public int ActualQty { get; set; }
        public EnumState ItemState { get; set; }
        public EnumItemState ScanItemState { get; set; }
        public bool FlagTemp { get; set; }
        public string ColumnInReport { get; set; }
        public string CommodityCode { get; set; }


    }
}
