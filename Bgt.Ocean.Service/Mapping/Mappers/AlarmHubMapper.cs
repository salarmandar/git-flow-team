using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings.AlarmHub;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class AlarmHubMapper
    {
        public static AlarmHubTriggeredResponse ConvertToAlarmHubTriggeredResponse(this TblMasterDailyRunResource_Alarm src)
            => ServiceMapperBootstrapper.MapperService.Map<AlarmHubTriggeredResponse>(src);
    }
}
