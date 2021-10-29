using System;
using System.Web.Http;
using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.Messagings.CustomerLocationService;
using Bgt.Ocean.WebAPI.Controllers._Base;
using Bgt.Ocean.Service.Implementations.AuditLog;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_AdhocController : AdhocControllerBase
    {
        public v2_AdhocController(
            IAdhocService adhocService,
            IMachineService machineService
            ) : base(adhocService, machineService)
        {
        }        

        #region Job Pickup

        [HttpPost]
        public CreateJobAdHocResponse CreateJobPickUpMultiBranch(CreateJobAdHocRequest request)
        {
            return ValidateAndProcessRequest(
                    request,
                    () => new Guid[] { request.ServiceStopLegPickup.RunResourceGuid.GetValueOrDefault() },
                    () => _adhocService.CreateJobPickUpMultiBranch(request)
                );
        }
        
        #endregion       

        [HttpPost]
        public CreateJobAdHocResponse CreateJobTransferVaultMultiBranch(CreateJobAdHocRequest request)
        {
            return ValidateAndProcessRequest(
                    request,
                    () => new Guid[] { request.ServiceStopLegPickup.RunResourceGuid.GetValueOrDefault(), request.ServiceStopLegDelivery.RunResourceGuid.GetValueOrDefault() },
                    () => _adhocService.CreateJobTransferVaultMultiBranch(request)
                );
        }
            
        
        [HttpGet]
        public DetailDestinationForMultibranchReponseJobP GetDetailDestinationForDelivery_MultiBranchJobP(Guid? siteGuid, Guid? locationGuid)
        {
            return _adhocService.GetDetailDestinationForDelivery_MultiBranchJobP(siteGuid, locationGuid);
        }

        [HttpPost]
        public MultiBrDetailResponse GetAdhocDestinationByOriginLocation(GetMultiBrDestinationDetailRequest request)
        {
            return _adhocService.GetAdhocMultiBrDeliveryDetailByOriginLocation(request);
        }

        [HttpPost]
        public GetAdhocAllCustomerAndLocationResponse GetAdhocAllCustomerBySite(GetAdhocAllCustomerAndLocationRequest request)
        {
            return _adhocService.GetAdhocAllCustomerBySite(request);
        }

        [HttpPost]
        public GetAdhocAllCustomerAndLocationResponse GetAdhocAllLocationByCustomer(GetAdhocAllCustomerAndLocationRequest request)
        {
            return _adhocService.GetAdhocAllLocationByCustomer(request);
        }
    }
}
