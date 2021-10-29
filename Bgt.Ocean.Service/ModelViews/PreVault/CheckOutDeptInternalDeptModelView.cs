using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using static Bgt.Ocean.Infrastructure.Util.EnumPreVault;

namespace Bgt.Ocean.Service.ModelViews.PreVault
{
    public class CheckOutDeptInternalDeptModelView
    {
        public Guid Id { get; set; }
        public string InternalDeptName { get; set; }
        public bool FlagISA { get; set; }
    }
    public class CheckOutDeptItemModel
    {
        public IEnumerable<Guid> NonbarcodeAllGuid { get; set; } = new List<Guid>();
        public IEnumerable<Guid> SealAllGuid { get; set; } = new List<Guid>();
        public IEnumerable<Guid> JobAllGuid { get; set; } = new List<Guid>();

        public IEnumerable<TblMasterActualJobItemsSeal> SealInCon { get; set; }
        public IEnumerable<TblMasterActualJobItemsCommodity> CommInCon { get; set; }
    }

    public class CheckOutDeptSealHistoryModel
    {
        public Guid? JobGuid { get; set; }
        public Guid SealGuid { get; set; }
        public string SealNo { get; set; }
        public Guid? LocationGuid { get; set; }
        public string Location { get; set; }
        public Guid? MasterIDLocGuid { get; set; }
        public string MasterIDLocName { get; set; }
        public Guid? MasterIDRouteGuid { get; set; }
        public string MasterIDRouteName { get; set; }
        public bool FlagKeyIn { get; set; }
        public string ScanType { get { return this.FlagKeyIn ? ScanItemByType.KeyIn : ScanItemByType.BarcodeScan; } }
    }

    public class CheckOutDeptCommodityHistoryModel
    {
        public Guid? JobGuid { get; set; }
        public Guid ItemCommGuid { get; set; }
        public string CommodityName { get; set; }
    }
}
