using Bgt.Ocean.Service.ModelViews.Systems;
using System.Collections.Generic;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.Service.ModelViews.Consolidation
{
    public class ConsolidateModel
    {
        public SystemMessageView Message { get; set; }
        //public ConAndDeconsolidate_HeaderView MasterID_Detail { get; set; } = new ConAndDeconsolidate_HeaderView();

        /// <summary>
        /// true: Click Seal (Sealed), false: Click Save (In-Process)
        /// </summary>
        public bool FlagSealed { get; set; } = false;
        public bool FlagUnsealed { get; set; } = false;
        public int MinimumSealDigi { get; set; }


        #region Consolidate Item
        public List<ConSealView> ItemSeals { get; set; } = new List<ConSealView>();
        public List<ConCommodityView> ItemCommodity { get; set; } = new List<ConCommodityView>();
        #endregion
    }
}
