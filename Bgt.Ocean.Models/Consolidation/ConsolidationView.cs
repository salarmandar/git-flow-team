using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Consolidation
{
    public class ConsolidationView
    {
        public class ConsolidateInfoModel
        {
            public IEnumerable<LocationConsolidateInfoView> LocationConsolidation { get; set; } = new List<LocationConsolidateInfoView>();
            public IEnumerable<RouteConsolidateInfoView> RouteConsolidation { get; set; } = new List<RouteConsolidateInfoView>();
            public IEnumerable<InterBranchConsolidateInfoView> InterBranchConsolidation { get; set; } = new List<InterBranchConsolidateInfoView>();
            public IEnumerable<MultiBranchConsolidateInfoView> MultiBranchConsolidation { get; set; } = new List<MultiBranchConsolidateInfoView>();
        }

        #region Model Get Item
        public class ConsolidateAllItemView
        {
            /// <summary>
            /// Get New Available Items
            /// </summary>
            public IEnumerable<ConsolidateItemView> SealItems { get; set; } = new List<ConsolidateItemView>();

            public IEnumerable<SummarySealView> AvailableSummarySeal { get; set; } = new List<SummarySealView>();

            public IEnumerable<ConsolidatedSummarySealView> ConsolidatedSummarySeal { get; set; } = new List<ConsolidatedSummarySealView>();

            /// <summary>
            /// Get New Available Items
            /// </summary>
            public IEnumerable<ConsolidateItemView> CommodityItems { get; set; } = new List<ConsolidateItemView>();

            public IEnumerable<SummaryCommodityView> AvailableSummaryCommodity { get; set; } = new List<SummaryCommodityView>();

            public IEnumerable<ConsolidatedSummaryCommodityView> ConsolidatedSummaryCommodity { get; set; } = new List<ConsolidatedSummaryCommodityView>();

        }

        public class LiabilityView
        {
            public double Liability { get; set; }
            public string CurrencyNameAbb { get; set; }
        }

        public class SummayItemConsolidateView
        {
            public IEnumerable<SummarySealView> AvailableSummarySeal { get; set; } = new List<SummarySealView>();
            public IEnumerable<ConsolidatedSummarySealView> ConsolidatedSummarySeal { get; set; } = new List<ConsolidatedSummarySealView>();
            public IEnumerable<SummaryCommodityView> AvailableSummaryCommodity { get; set; } = new List<SummaryCommodityView>();
            public IEnumerable<ConsolidatedSummaryCommodityView> ConsolidatedSummaryCommodity { get; set; } = new List<ConsolidatedSummaryCommodityView>();
        }

        public class ConsolidateItemView
        {
            public Nullable<System.Guid> MasterID_Guid { get; set; }
            public Nullable<System.Guid> SealGuid { get; set; }
            public string SealNo { get; set; }
            public Nullable<double> Liability { get; set; }
            public Nullable<System.Guid> CommodityGuid { get; set; }
            public string Commodity { get; set; }
            public string JobNo { get; set; }
            public string RouteName { get; set; }
            public Nullable<System.Guid> MasterRouteGroupDetail_Guid { get; set; }
            public Nullable<System.Guid> OnwardDestinationGuid { get; set; }
            public Nullable<System.Guid> DailyRunGuid { get; set; }
            public string InterDepartmentName { get; set; }
            public string CurrencyNameAbb { get; set; }
            public string GroupScanName { get; set; }
            public Nullable<int> Qty { get; set; }
            public bool FlagCheckEdit { get; set; } = false;
            public bool FlagDisable { get; set; } = true;
            public Nullable<bool> FlagChkCustomer { get; set; }
            public string PickUp_Location { get; set; }
            public Nullable<System.Guid> CustomerLocationGuid { get; set; }
            public string Delivery_Location { get; set; }
            public List<LiabilityView> LiabilityList { get; set; } = new List<LiabilityView>();
            public string DataRecordType { get; set; }
        }

        public class SummarySealView
        {
            public string ItemsSummary { get; set; }
            public string GroupScanName { get; set; }
            public double Total { get; set; }
            public bool FlagCheckEdit { get; set; } = false;
        }

        public class ConsolidatedSummarySealView
        {
            public string ItemsSummary { get; set; }
            public string GroupScanName { get; set; }
            public double Total { get; set; }
            public bool FlagCheckEdit { get; set; } = false;
        }

        public class SummaryCommodityView
        {
            public string ItemsSummary { get; set; }
            public string GroupScanName { get; set; }
            public double Total { get; set; }
            public bool FlagCheckEdit { get; set; } = false;
        }

        public class ConsolidatedSummaryCommodityView
        {
            public string ItemsSummary { get; set; }
            public string GroupScanName { get; set; }
            public double Total { get; set; }
            public bool FlagCheckEdit { get; set; } = false;
        }

        #endregion

        #region Model Save
        public class ConSealView
        {
            public Guid SealGuid { get; set; }
            public Guid JobGuid { get; set; }
            public Guid CustomerInternalDepartmentGuid { get; set; }
            public string SealNo { get; set; }
            public Guid? MasterID_Guid { get; set; }
        }

        public class ConCommodityView
        {
            public Guid CommodityGuid { get; set; }
            public Guid JobGuid { get; set; }
        }
        #endregion
    }
}
