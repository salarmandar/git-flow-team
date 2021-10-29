using Bgt.Ocean.Models;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.PreVault
{
    public class CheckOutNonbarcodeModelView
    {
        public List<PrevaultDepartmentBarcodeScanOutResult> CheckOutNonbarcodeList { get; set; } = new List<PrevaultDepartmentBarcodeScanOutResult>();
        public int QtySumNonbarcode { get; set; }
    }
}
