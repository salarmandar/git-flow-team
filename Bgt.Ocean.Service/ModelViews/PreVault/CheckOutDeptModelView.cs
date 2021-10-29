using Bgt.Ocean.Models;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.PreVault
{
    public class CheckOutDeptModelView
    {
        public IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> SealItemGet { get; set; }
        public CheckOutNonbarcodeModel NonbarcodeItemGet { get; set; }
    }

    public class CheckOutNonbarcodeModel
    {
        public IEnumerable<PrevaultDepartmentBarcodeScanOutResult> CheckOutNonbarcodeList { get; set; }
        public int QtySumNonbarcode { get; set; }
    }
}
