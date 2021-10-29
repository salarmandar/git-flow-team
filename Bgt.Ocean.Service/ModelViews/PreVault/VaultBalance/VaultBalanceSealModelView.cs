using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance
{
    public class VaultBalanceSealModelView : LocationModelView
    {
        [Description("Guid of actual item seal or item master (BlindScanning : Front-end generate guid)")]
        public Guid? Guid { get; set; }
        public string SealNo { get; set; }
        public Guid? JobGuid { get; set; }
        public string JobNo { get; set; }
        public double STC { get; set; }
        public string Commodity { get; set; }
        public string Currency { get; set; }
        public string PickUpLocation { get; set; }
        public string DeliveryLocation { get; set; }
        public string ServiceType { get; set; }
        public DateTime? WorkDate { get; set; }
        public Guid? LiabilityGuid { get; set; }
        public EnumItemState ScanItemState { get; set; }

        //For Item Mapping
        public string ConsolidateType { get; set; }
        public bool IsMasterLoc { get; set; }
        public bool IsMasterRoute { get; set; }

       
    }

}
