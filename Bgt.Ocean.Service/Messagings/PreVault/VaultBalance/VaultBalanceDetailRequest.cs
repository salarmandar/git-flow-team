using Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Messagings.PreVault.VaultBalance
{
    public class VaultBalanceDetailRequest : VaultBalanceHeaderModelView
    {
        public Guid SiteGuid { get; set; }
        public Guid PrevaultGuid { get; set; }
        [Description("On : True / Off : False")]
        public bool FlagBlindScanning { get; set; }
        public bool FlagDiscrepancyVaultBalance { get; set; }
    }

    public class VaultBalanceItemsRequest : VaultBalanceDetailRequest
    {
        public IEnumerable<VaultBalanceSealModelView> SealList { get; set; }
        public IEnumerable<VaultBalanceNonbarcodeModelView> NonbarcodeList { get; set; }
        public IEnumerable<VaultBalanceSummaryModelView> SummaryList { get; set; }
        public IEnumerable<VaultBalanceMappingModelView> SealItemsMapping { get; set; }

        public string UsernameVerify { get; set; }
    }

    public class VaultBalanceApiRequest
    {
        public Guid DailyRunGuid { get; set; }
        public Guid SiteGuid { get; set; }
        public String UserCreated { get; set; }
        public DateTime DatetimeCreated { get; set; }
    }


}
