using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Models.OnHandRoute;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class OnHandRouteResponse
    {
        public SystemMessageView MessageResponse { get; set; }
        public List<OnHandJobOnRunView> OnHandJobOnRun { get; set; } = new List<OnHandJobOnRunView>();
        public List<string> CurrencyNotConvert { get; set; }

        #region Summary job on run
        //Group job on run
        public List<GroupOnHandQtyOnRunView> GrpJobOnRunList { get; set; } = new List<GroupOnHandQtyOnRunView>();
        //Group job service done
        public List<GroupOnHandQtyOnRunView> GrpJobServiceDone { get; set; } = new List<GroupOnHandQtyOnRunView>();
        //Group item seal
        public List<GroupOnHandItem> GrpItemSeal { get; set; } = new List<GroupOnHandItem>();
        //Group commodiy
        public List<GroupOnHandCommodity> GrpItemCommodity { get; set; } = new List<GroupOnHandCommodity>();
        //Group job unable
        public List<GroupOnHandJobUnable> GrpJobUnable { get; set; } = new List<GroupOnHandJobUnable>();
        //Group liability
        public List<GroupOnHandLiablity> GrpItemLiability { get; set; } = new List<GroupOnHandLiablity>();
        #endregion
    }

    public class OnHandJobOnRunView : JobDetailOnRunView
    {
        public string Commodity { get; set; }
        public int Seal_Qty { get; set; }
        public int Non_Qty { get; set; }
        public double STC { get; set; }
        public bool FlagExConvertToRun { get; set; }
        public List<string> CurrencyNotConvertToRun { get; set; }
        public List<CurrencyValueView> LiabilityOnHandList { get; set; } = new List<CurrencyValueView>();
        public List<CommodityValueView> CommodityOnHandList { get; set; } = new List<CommodityValueView>();
        public string MasterLocationID { get; set; }
        public string MasterRouteID { get; set; }

        //Section
        [Description("Job service done section")]
        public bool FlagSecServiceDone { get; set; }
        [Description("Job unable to service section")]
        public bool FlagSecUnableToService { get; set; }
        [Description("Liability on hand section")]
        public bool FlagSecLiability { get; set; }
        [Description("Item on hand section")]
        public bool FlagSecItem { get; set; }
        [Description("Non-barcode on hand section")]
        public bool FlagSecNonbarcode { get; set; }
    }

    public class GroupOnHandQtyOnRunView
    {
        public string JobTypeName { get; set; }
        public int JobTypeIDGrp { get; set; }
        public int Qty { get; set; }
    }

    public class GroupOnHandItem
    {
        public string Seal { get; set; }
        public int Qty { get; set; }
    }

    public class GroupOnHandCommodity
    {
        public string CommodityName { get; set; }
        public int Qty { get; set; }
        public int GoldenRuleNo { get; set; }
    }

    public class GroupOnHandLiablity
    {
        public string CurrencyAbbr { get; set; }
        public double LiabilityValue { get; set; }
    }

    public class GroupOnHandJobUnable
    {
        public string JobTypeName { get; set; }
        public int JobTypeIDGrp { get; set; }
        public int SumQty { get; set; }
        public List<OnHandUnableItem> OnHandUnableItem { get; set; }
    }

    public class OnHandRouteSummaryResponse
    {
        public List<RunControlRunResourceDailyBySiteAndDateGetResult> DailyRunDetailList { get; set; }
        public int SumJobInRun { get; set; }
        public int SumJobServiceDone { get; set; }
        public int SumJobUnableToService { get; set; }
        public double SumLiabilityOnHand { get; set; }
        public double SumLiabilityOnHandUser { get; set; }
        public List<CurrencyValueView> SumLiabilityValue { get; set; }
        public bool FlagMixCurrency { get; set; }
        public int SumItemOnHand { get; set; }
        public int SumNonBarcodeOnHand { get; set; }

        public SystemMessageView MessageResponse { get; set; }
    }

    public class OnHandUnableItem
    {
        public int JobTypeIDGrp { get; set; }
        public int JobTypeID { get; set; }
        public string JobStatus { get; set; }
        public int Qty { get; set; }
    }

    public class MasterIDGroup
    {
        public Guid JobGuid { get; set; }
        public string MasterLocID { get; set; }
        public string MasterRouteID { get; set; }
    }
    
}
