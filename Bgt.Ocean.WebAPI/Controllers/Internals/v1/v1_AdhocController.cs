using System;
using System.Collections.Generic;
using System.Web.Http;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.ModelViews.Adhoc;
using Bgt.Ocean.Service.ModelViews;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.ModelViews.RunControls;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.WebAPI.Controllers._Base;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_AdhocController : AdhocControllerBase
    {
        private readonly IOTCRunControlService _otcRunControlService;

        public v1_AdhocController(
            IAdhocService adhocService,
            IOTCRunControlService otcRunControlService,
            IMachineService machineService
            ) : base(adhocService, machineService)
        {
            _otcRunControlService = otcRunControlService;
        }

        #region Get onward type for dropdown list
        /// <summary>
        /// Get onward destionation type for dropdown list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<OnwardDestinationTypeView> GetOnwardDestinationTypeList()
        {
            return _adhocService.GetOnwardDestinationType();
        }
        #endregion

        #region Get trip indicator for dropdown list
        /// <summary>
        /// Get trip indicator for dropdown list
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DisplayTextDropDownView> GetTripIndicator(Guid countryGuid)
        {
            return _adhocService.GetTripIndicator(countryGuid);
        }
        #endregion

        #region Get internal department for dropdown list
        /// <summary>
        /// Get internal department of the onward for dropdown list
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CustomerLocationInternalDepartmentView> GetInternalDepartmentOnward(Guid siteGuid)
        {
            return _adhocService.GetOnwarddestination_Internal(siteGuid);
        }

        #endregion

        #region Job Pickup
        [HttpPost]
        public CreateJobAdHocResponse CreateJobPickUp(CreateJobAdHocRequest request)
        {
            return ValidateAndProcessRequest(
                request, 
                () => new Guid[] { request.ServiceStopLegPickup.RunResourceGuid.GetValueOrDefault() },
                () => _adhocService.CreateJobPickUp(request)
            );
        }
        #endregion

        #region Job Delivery
        [HttpPost]
        public CreateJobAdHocResponse CreateJobDelivery(CreateJobAdHocRequest request)
        {
            return ValidateAndProcessRequest(
                request,
                () => new Guid[] { request.ServiceStopLegDelivery.RunResourceGuid.GetValueOrDefault() },
                () => _adhocService.CreateJobDelivery(request)
            );            
        }

        #endregion

        #region Job Tranfer
        [HttpPost]
        public CreateJobAdHocResponse CreateJobTransfer(CreateJobAdHocRequest request)
        {
            return ValidateAndProcessRequest(
                request,
                () => new Guid[] { request.ServiceStopLegPickup.RunResourceGuid.GetValueOrDefault() },
                () => _adhocService.CreateJobTransfer(request)
            );
        }
        #endregion

        #region Job Tranfer Vault
        [HttpPost]
        public CreateJobAdHocResponse CreateJobTransferVault(CreateJobAdHocRequest request)
        {
            return ValidateAndProcessRequest(
                request,
                () => new Guid[] { request.ServiceStopLegDelivery.RunResourceGuid.GetValueOrDefault(), request.ServiceStopLegPickup.RunResourceGuid.GetValueOrDefault() },
                () => _adhocService.CreateJobTransferVault(request)
            );
        }

        #endregion    

        [HttpGet]
        public string GenerateJobNo(Guid siteGuid)
        {

            return _adhocService.GenerateJobNo(siteGuid);
        }

        [HttpGet]
        public Adhoc_LOB_ServiceJobTypeView GetLineOfBusinessAndJobType(bool flagNotDelivery = false, bool flagAdhoc = false, bool flagFromMapping = false)
        {
            return _adhocService.GetLineOfBusinessAndJobType(flagNotDelivery, flagAdhoc, flagFromMapping);
        }

        [HttpGet]
        public IEnumerable<SpecialCommandView> GetSpecialCommandByCompany(Guid? companyGuid)
        {
            return _adhocService.GetSpecialCommandByCompany(companyGuid);
        }

        [HttpGet]
        public IEnumerable<SubJobTypeView> GetSubServiceType(Guid? companyGuid, Guid? LobGuid, int? runStatusID = null, bool pageMasterRoute = false)
        {
            return _adhocService.GetSubServiceType(companyGuid, LobGuid, runStatusID, pageMasterRoute);
        }

        [HttpPost]
        public AdhocJobResponse CheckDuplicateJobInDay(CreateJobAdHocRequest request)
        {
            return _adhocService.CheckDuplicateJobsInDay(request);
        }

        [HttpPost]
        public SystemMessageView IsThereEmployeeCanDoOTC(CheckEmployeeCanDoOTC modelEmp)
        {
            return _adhocService.IsThereEmployeeCanDoOTC(modelEmp);
        }

        [HttpPost]
        public SystemMessageView SetUpdateOtcJobHeader(JobOtcRequest Request)
        {
            return _otcRunControlService.UpdateJobHeaderOTCAfterSave(Request);
        }

        [HttpPost]
        public SystemMessageView CreateHeaderOtcByJobGuids(IEnumerable<Guid> jobGuids)
        {
            return _adhocService.CreateHeaderOtcByJobGuids(jobGuids);
        }
    }
}
