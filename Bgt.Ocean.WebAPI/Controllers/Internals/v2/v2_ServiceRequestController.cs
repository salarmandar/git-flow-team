using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.AuditLog.ServiceRequest;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_ServiceRequestController : ApiControllerBase
    {
        private readonly IServiceRequestCreatorService _serviceRequestCreatorService;
        private readonly ISystemService _systemService;

        public v2_ServiceRequestController(
                IServiceRequestCreatorService serviceRequestCreatorService,
                ISystemService systemService
            )
        {
            _serviceRequestCreatorService = serviceRequestCreatorService;
            _systemService = systemService;
        }        

        #region Create Ticket

        [HttpPost]
        public SRCreateResponse CreateFLM(SRCreateRequestFLMWithSFI request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestFLM(request, request.SFIModel));

        [HttpPost]
        public SRCreateResponse CreateSLM(SRCreateRequestSLMWithSFI request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestSLM(request, request.SFIModel));

        [HttpPost]
        public SRCreateResponse CreateECash(SRCreateRequestECashWithSFI request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestECash(request, request.SFIModel));

        [HttpPost]
        public SRCreateResponse CreateTechMeet(SRCreateRequestTechMeetWithSFI request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestTechMeet(request, request.SFIModel));

        [HttpPost]
        public SRCreateResponse CreateMCSCashAdd(SRCreateRequestMCSCashAddWithSFI request)
            => TryCreateTicket(() => _serviceRequestCreatorService.CreateServiceRequestMCS(request, request.SFIModel));

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
    }
}
