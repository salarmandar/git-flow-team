
using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Service.ModelViews.Users;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class UserMapper
    {
        public static IEnumerable<Messagings.UserService.MasterMenuDetailResponse> ConvertToMasterMenuDetailResult(this IEnumerable<Models.MasterMenuDetailResult> model)
            => ServiceMapperBootstrapper.MapperService
                .Map<IEnumerable<Models.MasterMenuDetailResult>, IEnumerable<Messagings.UserService.MasterMenuDetailResponse>>(model);


        public static SystemDomainDCResponse ConvertToSystemDomainDCResponse(this TblSystemDomainDC model)
            => ServiceMapperBootstrapper.MapperService.Map<TblSystemDomainDC, SystemDomainDCResponse>(model);

        public static SystemDomainAilesResponse ConvertToSystemDoaminAilesResponse(this TblSystemDomainAiles model)
            => ServiceMapperBootstrapper.MapperService.Map<TblSystemDomainAiles, SystemDomainAilesResponse>(model);

        public static DataStorage ConvertAuthenResponseToDataStorage(this AuthenLoginResponse model)
            => ServiceMapperBootstrapper.MapperService.Map<AuthenLoginResponse, DataStorage>(model);

        public static UserView ConvertToUserView(this TblMasterUser model)
            => ServiceMapperBootstrapper.MapperService.Map<TblMasterUser, UserView>(model);
    }
}
