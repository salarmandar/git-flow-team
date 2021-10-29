using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations.Monitoring;
using Bgt.Ocean.Service.Messagings.MonitorService;
using Bgt.Ocean.Service.ModelViews.Monitoring;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_MonitoringController : ApiControllerBase
    {
        private readonly IMonitoringService _monitoringService;

        public v1_MonitoringController(IMonitoringService monitoringService)
        {
            _monitoringService = monitoringService;
        }

        [HttpPost]
        public PodMonitoringView GetPodMonitorData(PodMonitorRequest request)
        {
            PodMonitoringView result = null;
            string dateUserFormat = request.DateFormat;
            request.DateFrom = request.StrDateFrom.ChangeFromStringToDate(dateUserFormat);
            request.DateTo = request.StrDateTo.ChangeFromStringToDate(dateUserFormat);
            request.DatetimeCreated = request.StrDatetimeCreated.ChangeFromStringToDate(dateUserFormat);
            var data = _monitoringService.GetPodMonitorData(request);
            result = data;
            return result;
        }

        [HttpGet]
        public PodMonitorErrorResponse GetPodMonitorErrorData(Guid LogPod_Guid)
        {
            PodMonitorErrorResponse result = null;
            result = _monitoringService.GetPodMonitorErrorData(LogPod_Guid);
            return result;
        }


        #region Smart Billing Monitoring
        [HttpGet]
        public SmartBillingConfigResponse SmartBillingConfigGet(Guid siteGuid)
        {
            return _monitoringService.SmartBillingConfigGet(siteGuid);
        }

        [HttpPost]
        public SmartBillingSubmitResponse SmartBillingConfigInsertUpdate(SmartBillingConfigRequest request)
        {
            var resp = _monitoringService.SaveUpdateSmartBillingConfig(request);
            return resp;
        }

        [HttpGet]
        public SmartBillingScheduleView SmartBillingSheduleDisplayGet(Guid siteGuid)
        {
            return _monitoringService.SmartBillingSheduleDisplayGet(siteGuid);
        }

        [HttpPost]
        public SmartBillingMonitorResponse SmartBillingAutoGenerateStatusGet(SmartBillingMonitorRequest request)
        {
            return _monitoringService.SmartBillingGenerateStatusGet(request);
        }

        [HttpPost]
        public string SmartBillingErrorMsgGet(SmartBillingErrorMsgRequest request)
        {
            return _monitoringService.SmartBillingErrorMsgGet(request);
        }
        #endregion


    }
}