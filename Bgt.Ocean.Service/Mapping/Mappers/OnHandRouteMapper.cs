using Bgt.Ocean.Models.OnHandRoute;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class OnHandRouteMapper
    {
        public static IEnumerable<OnHandJobOnRunView> ConvertToOnHandMasterRouteResponse(this IEnumerable<JobDetailOnRunView> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<JobDetailOnRunView>, IEnumerable<OnHandJobOnRunView>>(model);
        }
    }
}
