using Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Messagings.PreVault.VaultBalance
{
    //Get grid
    public class VaultBalanceDetailReponse : VaultBalanceHeaderModelView
    {
        public IEnumerable<VaultBalanceSealModelView> SealList { get; set; }
        public IEnumerable<VaultBalanceNonbarcodeModelView> NonbarcodeList { get; set; }
    }

    //Validate response
    public class StartAndValidateVaultBalanceReponse : VaultBalanceHeaderModelView
    {
        public bool FlagBlindScanning { get; set; }
        public SystemMessageView SystemMessageView { get; set; }
    }

    public class VaultBalanceApiResponse
    {
        [Description("Pass: True/ Blocking: False")]
        public bool IsValid { get; set; }
    }

}
