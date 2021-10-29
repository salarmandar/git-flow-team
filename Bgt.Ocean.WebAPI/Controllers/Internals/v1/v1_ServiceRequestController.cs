using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.PushToDolphin;
using Bgt.Ocean.Service.Implementations.AuditLog.ServiceRequest;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_ServiceRequestController : ApiControllerBase
    {
        private readonly IServiceRequestCreatorService _serviceRequestCreatorService;
        private readonly ISystemService _systemService;
        private readonly IServiceRequestEditorService _serviceRequestEditorService;
        private readonly IServiceRequestReaderService _serviceRequestReaderService;
        private readonly ISFOPushToDolphinService _sfoPushToDolphinService;

        public v1_ServiceRequestController(
                IServiceRequestCreatorService serviceRequestCreatorService,
                IServiceRequestEditorService serviceRequestEditorService,
                IServiceRequestReaderService serviceRequestReaderService,
                ISystemService systemService,
                ISFOPushToDolphinService sfoPushToDolphinService
            )
        {
            _serviceRequestCreatorService = serviceRequestCreatorService;
            _systemService = systemService;
            _serviceRequestEditorService = serviceRequestEditorService;
            _serviceRequestReaderService = serviceRequestReaderService;
            _sfoPushToDolphinService = sfoPushToDolphinService;            
        }

        #region Query Ticket

        [HttpPost]
        public ResponseQueryServiceRequest Query(RequestQueryServiceRequest request)
        {
            try
            {
                var result = _serviceRequestReaderService.GetServiceRequestList(request);
                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryServiceRequest
                {
                    responseCode = "-1",
                    responseMessage = "Error => Please contact administrator",
                    rows = 0
                };

                _systemService.CreateHistoryError(ex);

                return result;
            }
        }

        #endregion

        #region Create Ticket

        [HttpPost]
        public SRCreateResponse CreateFLM(SRCreateRequestFLM request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestFLM(request));

        [HttpPost]
        public SRCreateResponse CreateSLM(SRCreateRequestSLM request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestSLM(request));

        [HttpPost]
        public SRCreateResponse CreateECash(SRCreateRequestECash request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestECash(request));

        [HttpPost]
        public SRCreateResponse CreateTechMeet(SRCreateRequestTechMeet request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestTechMeet(request));

        private SRCreateResponse TryCreateTicket(Func<SRCreateResponse> fnCreate)
        {
            try
            {
                var result = fnCreate();
                return result;
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);

                return new SRCreateResponse
                {
                    IsSuccess = false,
                    Message = err.ToString(),
                    JobHeaderGuid = null,
                    TicketGuid = null,
                    TicketNumber = null
                };
            }
        }

        #endregion

        #region Update Ticket

        [HttpPost]
        public SRCancelResponse Cancel(SRCancelRequest request)
        {
            var resultCancel = _serviceRequestEditorService.CancelSR(request);
            
            if(resultCancel.IsSuccess)
            {
                _sfoPushToDolphinService.PushCancelSFOJobToDolphin(request.TicketGuid);
            }

            return resultCancel;
        }

        [HttpPost]
        public BaseResponse Reschedule(SRRescheduleRequest request)
        {
            var resultReschedule = _serviceRequestEditorService.RescheduleSR(request);

            if(resultReschedule.IsSuccess && resultReschedule.NewTicketStatusGuid == JobStatusHelper.PlannedGuid.ToGuid())
            {
                _sfoPushToDolphinService.PushRescheduleSFOJobToDolphin(request.TicketGuid, resultReschedule.OldDailyRunGuid, resultReschedule.OldWorkDate);
            }

            return resultReschedule.ConvertToBaseResponse();
        }

        #endregion

        #region Journal

        [HttpPost]
        public BaseResponse CreateSRExternalJournal(SRJournalCreateRequest request)
        {
            var addResult = _serviceRequestEditorService.AddJournalToServiceRequest(request.TicketGuid, request.JournalDescription, false);

            return addResult;
        }

        #endregion
    }
}
