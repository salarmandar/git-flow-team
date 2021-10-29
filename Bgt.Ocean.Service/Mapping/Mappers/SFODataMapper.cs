using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using Bgt.Ocean.Service.ModelViews.Problem;
using Bgt.Ocean.Service.ModelViews.ServiceRequest;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class SFODataMapper
    {
        public static ProblemView ConvertToProblemView(this SFOTblMasterProblem model)
            => ServiceMapperBootstrapper.MapperService.Map<SFOTblMasterProblem, ProblemView>(model);

        public static IEnumerable<ECashView> ConvertToEcashView(this IEnumerable<ECashAmount> model)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<ECashAmount>, IEnumerable<ECashView>>(model);

        public static TechMeetView ConvertToTechMeet(this SRCreateRequestTechMeet model)
            => ServiceMapperBootstrapper.MapperService.Map<SRCreateRequestTechMeet, TechMeetView>(model);

        public static BaseResponse ConvertToBaseResponse(this SRRescheduleResponse model)
            => ServiceMapperBootstrapper.MapperService.Map<SRRescheduleResponse, BaseResponse>(model);
    }
}
