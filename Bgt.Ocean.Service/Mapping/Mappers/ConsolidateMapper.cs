using Bgt.Ocean.Models.Consolidation;
using System.Collections.Generic;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class ConsolidateMapper
    {
        public static IEnumerable<LocationConsolidateInfoView> ConvertToLocationInfoView(this IEnumerable<PreVaultConsolidateInfoResult> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<PreVaultConsolidateInfoResult>, IEnumerable<LocationConsolidateInfoView>>(model);
        }
        public static IEnumerable<RouteConsolidateInfoView> ConvertToRouteInfoView(this IEnumerable<PreVaultConsolidateInfoResult> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<PreVaultConsolidateInfoResult>, IEnumerable<RouteConsolidateInfoView>>(model);
        }
        public static IEnumerable<InterBranchConsolidateInfoView> ConvertToInterBranchInfoView(this IEnumerable<PreVaultConsolidateInfoResult> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<PreVaultConsolidateInfoResult>, IEnumerable<InterBranchConsolidateInfoView>>(model);
        }
        public static IEnumerable<MultiBranchConsolidateInfoView> ConvertToMultiBranchInfoView(this IEnumerable<PreVaultConsolidateInfoResult> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<PreVaultConsolidateInfoResult>, IEnumerable<MultiBranchConsolidateInfoView>>(model);
        }
        public static IEnumerable<ConsolidateItemView> ConvertToItemView(this IEnumerable<ConAvailableItemView> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<ConAvailableItemView>, IEnumerable<ConsolidateItemView>>(model);
        }
    }
}
