using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Service.Implementations.RunControl;
using Bgt.Ocean.Service.Messagings.RunControlService;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_RunController : ApiControllerBase
    {
        #region Objects & Variables
        private readonly IRunControlService _runControlService;
        #endregion

        #region Constructor
        public v1_RunController(
            IRunControlService runControlService)
        {
            _runControlService = runControlService;
        }
        #endregion

        #region For dolphin
        /// <summary>
        /// POST : api/v1/in/Run/TruckToTruckTransferReady
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public TruckToTruckTransferResponse TruckToTruckTransferReady(TruckToTruckRequest request)
        {
            return _runControlService.TruckToTruckTransferReady(request, Request.RequestUri);
        }


        /// <summary>
        /// POST : api/v1/in/Run/TruckToTruckHoldoverJob
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public TruckToTruckHoldoverJobResponse TruckToTruckHoldoverJob(TruckToTruckRequest request)
        {
            return _runControlService.TruckToTruckHoldoverJob(request, Request.RequestUri);
        }

        /// <summary>
        /// POST : api/v1/in/Run/TruckToTruckTransferJob
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public TruckToTruckTransferResponse TruckToTruckTransferJob(TruckToTruckRequest request)
        {
            return _runControlService.TruckToTruckTransferJob(request, Request.RequestUri);
        }

        /// <summary>
        /// POST : api/v1/in/Run/ValidateTruckLiabilityLimit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ValidateTruckLiabilityLimitResponse ValidateTruckLiabilityLimit(ValidateTruckLiabilityLimitRequest request)
        {
            return _runControlService.ValidateTruckLiabilityLimit(request);
        }
        #endregion

        #region Validate Jobs InRun
        [HttpPost]
        public ValidateAssignJobsToRunResponse ValidateAssignJobsToRun(ValidateAssignJobsToRunRequest request)
        {
            return _runControlService.ValidateAssignJobsToRun(request);
        }
        #endregion
    }
}