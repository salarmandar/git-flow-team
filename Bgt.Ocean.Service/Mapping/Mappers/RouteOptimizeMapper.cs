using Bgt.Ocean.Models.RouteOptimization;
using Bgt.Ocean.Service.Messagings.RouteOptimization;
using System.Collections.Generic;


namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class RouteOptimizeMapper
    {

        public static MasterRouteNameRequestModel ConvertToModelRequest(this MasterRouteNameRequest model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<MasterRouteNameRequest, MasterRouteNameRequestModel>(model);
        }
        public static MasterRouteOptimizationRequestListRequestModel ConvertRequestToModelRequest(this MasterRouteOptimizationRequestListRequest model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<MasterRouteOptimizationRequestListRequest, MasterRouteOptimizationRequestListRequestModel>(model);
        }

        public static IEnumerable<MasterRouteOptimizeView> ConvertToView(this IEnumerable<MasterRouteOptimizationViewModel> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<MasterRouteOptimizationViewModel>, IEnumerable<MasterRouteOptimizeView>>(model);
        }


    }
}
