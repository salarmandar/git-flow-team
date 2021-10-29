using Bgt.Ocean.Models.Consolidation;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.Consolidation
{
    public class ConsolidateInfoResult
    {
        public IEnumerable<PreVaultConsolidateInfoResult> LocationConsolidation { get; set; } = new List<PreVaultConsolidateInfoResult>();
        public IEnumerable<PreVaultConsolidateInfoResult> RouteConsolidation { get; set; } = new List<PreVaultConsolidateInfoResult>();
        //public IEnumerable<PreVaultConsolidateInfoResult> LocationMaster_CanInRoute { get; set; } = new List<PreVaultConsolidateInfoResult>();
        public IEnumerable<PreVaultConsolidateInfoResult> InterBranchConsolidation { get; set; } = new List<PreVaultConsolidateInfoResult>();
        //public IEnumerable<PreVaultConsolidateInfoResult> RouteMaster_CanInConsolidate_InterBranch { get; set; } = new List<PreVaultConsolidateInfoResult>();
        public IEnumerable<PreVaultConsolidateInfoResult> MultiranchConsolidation { get; set; } = new List<PreVaultConsolidateInfoResult>();

    }
}
