
using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Models.RunControl.LiabilityLimitModel
{

    public class BankCleanOutJobView
    {
        public Guid? JobGuid { get; set; }
        public bool FlagTemp { get; set; }
    }

    public class RawJobDataView : JobDetailResult
    {
        public RawItemsView JobItems { get; set; } = new RawItemsView();
    }

    public class JobDetailResult
    {
        public Guid? DailyRunGuid { get; set; }
        public int JobTypeID { get; set; }
        public bool FlagDestination { get; set; }
        public int JobStatusID { get; set; }

        public Guid? Target_DailyRunGuid { get; set; }
        public Guid? Target_DailyRunSiteGuid { get; set; }
        public string Target_DailyRunSiteName { get; set; }
        public double? Target_DailyRunCurrentSTC { get; set; }
        public string Target_WorkDate { get; set; }
        public double? Target_RunLibilityLimit { get; set; }
        public string Target_RunNo { get; set; }
        public string Target_RouteDetail { get; set; }
        public Guid? Target_CurrencyGuid { get; set; }
        public string Target_CurrencyAbb { get; set; }

        public Guid? JobGuid { get; set; }
        public Guid? JobTypeGuid { get; set; }
        public string JobTypeName { get; set; }
        public string JobAction { get; set; }
        public string JobNo { get; set; }
        public Guid? LocationGuid { get; set; }
        public string Location { get; set; }
        public double TotalJobSTC { get; set; }
        public double TotalLiabilities_STC { get; set; }
        public double TotalCommodities_STC { get; set; }
        public bool FlagAllowSelect { get; set; }
        public bool FlagSelected { get; set; }
        public bool FlagDisplayChkBox { get; set; }
        public bool FlagNotConvertExchange { get; set; }
        public bool FlagJobInRun { get; set; }
        public bool FlagJobOutRun { get; set; }
        public bool FlagDisplayJob { get; set; }

        public IEnumerable<ItemSummaryView> SummaryLiability { get; set; }
    }

    public class TruckLibilityLimitResult
    {
        public IEnumerable<RunDetailResult> RunDetailList { get; set; }
        public IEnumerable<SiteDetailResult> SiteDetailList { get; set; }
        public IEnumerable<JobDetailResult> JobDetailList { get; set; }
        public bool FlagHasExceedJob { get { return JobDetailList != null && JobDetailList.Any(o => o.FlagDisplayJob); } }
    }

    public class RunDetailResult
    {
        public Guid? Target_DailyRunGuid { get; set; }
        public Guid? Target_DailyRunSiteGuid { get; set; }
        public string Target_WorkDate { get; set; }
        public string Target_RunNo { get; set; }
        public string Target_RouteDetail { get; set; }
        public double? Target_RunLibilityLimit { get; set; }
        public double? Target_DailyRunCurrentSTC { get; set; }
        public string Target_CurrencyAbb { get; set; }
    }
    public class SiteDetailResult
    {
        public Guid? Target_DailyRunSiteGuid { get; set; }
        public string Target_DailyRunSiteName { get; set; }
    }

    public class ItemSummaryView
    {
        public double ItemSTC { get; set; }
        public Guid? MasterActualJobHeader_Guid { get; set; }
        public Guid? RunCurrencyGuid { get; set; }
        public Guid? DailyRunGuid { get; set; }
        public Guid? CommodityGuid { get; set; }
        public bool IsConvert { get; set; }
        public Guid? SourceCurrencyGuid { get; set; }
    }

    public class RawExistJobView
    {
        public Guid? JobGuid { get; set; }
        public string JobAction { get; set; }
    }

    public class RawItemsView
    {
        public IEnumerable<ItemsLibilityView> Liabilities { get; set; } = new List<ItemsLibilityView>();
        public IEnumerable<ItemsCommodityView> Commodities { get; set; } = new List<ItemsCommodityView>();
    }
    public class ItemsLibilityView
    {
        public Guid? DailyRunGuid { get; set; }
        public bool FlagDestination { get; set; }
        public Guid? DocCurrencyGuid { get; set; }
        public double Liability { get; set; }
        public Guid? LibilityGuid { get; set; }
        public int JobStatusID { get; set; }
        public Guid? JobGuid { get; set; }
        public EnumState ItemState { get; set; }
        public Guid? CommodityGuid { get; set; }
        public bool FlagPartial { get; set; }

    };
    public class ItemsCommodityView
    {
        public Guid? ActualCommodityGuid { get; set; }
        public Guid? CommodityGuid { get; set; }
        public bool FlagCommodityDiscrepancies { get; set; }
        public int Quantity { get; set; }
        public int QuantityActual { get; set; }
        public int QuantityExpected { get; set; }
        public Guid? DailyRunGuid { get; set; }
        public Guid? JobGuid { get; set; }
        public EnumState ItemState { get; set; }
        public bool FlagPartial { get; set; }
    }
    public class PercentageLiabilityLimitAlertResult
    {
        public Guid? Target_CurrencyGuid { get; set; }
        public Guid? Target_DailyRunGuid { get; set; }
        public double? Target_RunLibilityLimit { get; set; }
        public double? Target_DailyRunSTC { get; set; }
        public double? Target_CurrentPercentageLiability { get; set; }
        public string Target_CurrencyAbbr { get; set; }
        public bool FlagPercentageLiabilityLimitAlert { get; set; }
    }

    public class CurrencyOfDailyRunResourceView
    {
        public Guid DailyRunGuid { get; set; }
        public string CurrencyAbb { get; set; }
        public Guid CurrencyGuid { get; set; }
    }
    public class CurrencyOfDailyRunView: CurrencyOfDailyRunResourceView
    {
        public bool ValidateRunLiabilityLimit { get; set; }
    }
    
}
