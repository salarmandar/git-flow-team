using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.PreVault
{
    public class CheckOutDepartmentRequest : RequestBase
    {
        public IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> SealList { get; set; }
        public IEnumerable<PrevaultDepartmentBarcodeScanOutResult> NonBarcodeList { get; set; }

        public IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> SealListNotScan { get; set; }
        public IEnumerable<PrevaultDepartmentBarcodeScanOutResult> NonBarcodeListNotScan { get; set; }

        public Guid InternalDepartmentGuid { get; set; }
        public string InternalDepartmentName { get; set; }
        public Guid PrevaultGuid { get; set; }
        public string PrevaultName { get; set; }
        public string BrinkSiteName { get; set; }
        public Guid BrinkSiteGuid { get; set; }
        public bool FlagGroupNonBarcode { get; set; } = false;
        public bool FlagCheckAllSeal { get; set; } = false;
        public bool FlagCheckAllNon { get; set; } = false;
        public Guid LanguageGuid { get; set; }
        public Guid? DailyRunGuid { get; set; }
        public bool FlagISA { get; set; }
    }
}
