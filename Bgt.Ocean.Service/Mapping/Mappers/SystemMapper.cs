
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Systems;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.MasterService;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Service.ModelViews.Systems;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class SystemMapper
    {
        public static SystemMessageView ConvertToMessageView(this TblSystemMessage model, bool isSuccess = false)
        {
            var result = ServiceMapperBootstrapper.MapperService.Map<TblSystemMessage, SystemMessageView>(model);
            result.IsSuccess = isSuccess;
            return result;
        }

        public static SystemMessageView ReplaceTextContentStringFormatWithValue(this SystemMessageView msg, params object[] values)
        {
            if (msg == null) return msg;
            msg.MessageTextContent = string.Format(msg.MessageTextContent, values);
            return msg;
        }

        public static SystemApplicationResponse ConvertToSystemApplicationResponse(this TblSystemApplication model)
            => ServiceMapperBootstrapper.MapperService.Map<TblSystemApplication, SystemApplicationResponse>(model);

        public static AuthenLoginResponse ConvertToAuthenLoginResponse(this AuthenLoginResult model)
            => ServiceMapperBootstrapper.MapperService.Map<AuthenLoginResult, AuthenLoginResponse>(model);

        public static IEnumerable<SystemGlobalUnitView> ConvertToSystemGlobalUnitView(this IEnumerable<TblSystemGlobalUnit> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblSystemGlobalUnit>, IEnumerable<SystemGlobalUnitView>>(model);
        }

        //LineOfBusinessJobTypeByFlagAdhocJobResult, LineOfBusinessAndJobType
        public static IEnumerable<LineOfBusinessAndJobType> ConvertToLineOfBusinessJobTypeView(this IEnumerable<LineOfBusinessJobTypeByFlagAdhocJobResult> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<LineOfBusinessJobTypeByFlagAdhocJobResult>, IEnumerable<LineOfBusinessAndJobType>>(model);

        }
        public static IEnumerable<SystemDayOfWeekView> ConvertToSystemDayOfWeekView(this IEnumerable<TblSystemDayOfWeek> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblSystemDayOfWeek>, IEnumerable<SystemDayOfWeekView>>(model);
        }

        public static BaseResponse ConvertToBaseResponse(this SystemMessageView msgView, bool isSuccess = true)
        {
            var response = ServiceMapperBootstrapper.MapperService.Map<BaseResponse>(msgView);
            response.IsSuccess = isSuccess;

            return response;            
        }

        public static SystemApplicationView ConvertToSystemApplicationView(this TblSystemApplication model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<TblSystemApplication, SystemApplicationView>(model);
        }

        public static IEnumerable<SystemMaterRouteTypeOfWeekView> ConvertToSystemMaterRouteTypeOfWeekView(this IEnumerable<TblSystemMaterRouteTypeOfWeek> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblSystemMaterRouteTypeOfWeek>, IEnumerable<SystemMaterRouteTypeOfWeekView>>(model);
        }
    }
}
