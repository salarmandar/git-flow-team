using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.AlarmHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_AlarmHubController : ApiControllerBase
    {
        private readonly IAlarmHubService _alarmHubService;
        private readonly ISystemService _systemService;

        public v1_AlarmHubController(
                IAlarmHubService alarmHubService,
                ISystemService systemService
            )
        {
            _alarmHubService = alarmHubService;
            _systemService = systemService;
        }


        [HttpPost]
        public BaseResponse TriggerAlarm(AlarmHubCreateRequest alarmCreateModel)
        {
            try
            {
                _alarmHubService.TriggerAlarm(alarmCreateModel);
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Trigger alarm success",
                    MsgID = 870,
                    Title = "Alarm"
                };
            }
            catch(Exception err)
            {
                _systemService.CreateHistoryError(err);
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = $"Trigger alarm failed: {err.ToString()}",
                    MsgID = -1,
                    Title = "Alarm"
                };
            }

        }

        [HttpPost]
        public IEnumerable<AlarmHubTriggeredResponse> GetAlarmListByUser(Guid userGuid)
        {
            try
            {
                var response = _alarmHubService.GetCurrentAlarmTriggeredList(userGuid);
                return response;
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
                return Enumerable.Empty<AlarmHubTriggeredResponse>();
            }
        }

        [HttpPost]
        public BaseResponse AcknowledgeAlarm(AlarmHubAcknowledgeRequest request)
        {
            try
            {
                _alarmHubService.AcknowledgeAlarm(request.MasterDailyRunAlarmGuid, request.UserAcknowledged, request.DateTimeAcknowledged);
                var msg = _systemService.GetMessageByMsgId(871)?.ConvertToBaseResponse();                
                return msg;
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = $"Acknowledged alarm failed: {err.Message}",
                    MsgID = -1,
                    Title = "Alarm"
                };
            }
            
        }

        [HttpPost]
        public IEnumerable<Guid> IsHasAlarm(IEnumerable<Guid> masterDailyRunGuidList)
        {
            try
            {
                return _alarmHubService.IsHasAlarm(masterDailyRunGuidList);                
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
                return Enumerable.Empty<Guid>();
            }
        }
    }
}