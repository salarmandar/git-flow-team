using Bgt.Ocean.Service.Messagings.Consolidation;
using Bgt.Ocean.Service.ModelViews.Systems;
using System.Collections.Generic;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.WebAPI.Models.Consolidation
{
    public class ConMultiBranchModel
    {
        public ConsolidateMultiBranchView ConMultiBrModel { get; set; } = new ConsolidateMultiBranchView();
        public bool FlagSealed { get; set; } = false;
        public bool FlagUnsealed { get; set; } = false;

        #region Consolidate Item
        public List<ConSealView> ItemSeals { get; set; } = new List<ConSealView>();
        public List<ConCommodityView> ItemCommodity { get; set; } = new List<ConCommodityView>();
        #endregion

        public SystemMessageView Message { get; set; }
        public int MinimumSealDigi { get; set; }
    }
}