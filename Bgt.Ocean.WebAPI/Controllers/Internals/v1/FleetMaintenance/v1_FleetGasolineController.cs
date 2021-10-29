using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_FleetGasolineController : ApiControllerBase
    {
        private readonly IFleetGasolineService _fleetGasolineService;

        public v1_FleetGasolineController(IFleetGasolineService fleetGasolineService, ISystemService systemService)
        {
            _fleetGasolineService = fleetGasolineService;
        }

        #region GET
        /// <summary>
        /// Get dropdown list of gasoline vendor by brinks company
        /// </summary>
        /// <param name="brinkCompanyGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<FleetGasolineVendorView>> GetGasolineVendorList(Guid? brinkCompanyGuid)
        {
            return _fleetGasolineService.GetGasolineVendorList(brinkCompanyGuid);
        }

        /// <summary>
        /// Get dropdown list of operator for search
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<FleetGasolineOperatorView>> GetGasolineOperatorList()
        {
            return _fleetGasolineService.GetGasolineOperatorList();
        }

        /// <summary>
        /// Get dropdown list of gasoline name by gasoline vendor
        /// </summary>
        /// <param name="gasolineVendorGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<FleetGasolineTypeView>> GetGasolineTypeList(Guid gasolineVendorGuid)
        {
            return _fleetGasolineService.GetGasolineTypeList(gasolineVendorGuid);
        }

        /// <summary>
        /// Get data of Gasoline by site and run
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public FleetGasolineResponse GasolineInfoList(FleetGasolineRequest request)
        {
            return _fleetGasolineService.GetGasolineInfoList(request);
        }

        /// <summary>
        /// Get datail of Gasoline for Edit
        /// </summary>
        /// <param name="runGasolineGuid"></param>
        /// <returns></returns>
        [HttpGet] 
        public FleetGasolineDataResponse GetGasolineDetial(Guid runGasolineGuid)
        {
            return _fleetGasolineService.GetGasolineDetail(runGasolineGuid);
        }

        /// <summary>
        /// Get default data of gasoline vendor by brinks company
        /// </summary>
        /// <param name="brinkCompanyGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public FleetGasolineVendorDefaultView GetGasolineVendorDefault(Guid? brinkCompanyGuid)
        {
            return _fleetGasolineService.GetGasolineVendorDefault(brinkCompanyGuid);
        }
        #endregion

        #region Create / Update Run Resource Gasoline
        /// <summary>
        /// Create run resource gasoline
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView CreateRunResourceGasoline(FleetGasolineDataRequest request)
        {
            return _fleetGasolineService.CreateRunResourceGasoline(request);
        }

        /// <summary>
        /// Update run resource gasoline
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView UpdateRunResourceGasoline(FleetGasolineDataRequest request)
        {
            return _fleetGasolineService.UpdateRunResourceGasoline(request);
        }

        /// <summary>
        /// Disable run resource gasoline
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView DisableRunResourceGasoline(FleetGasolineDisableRequest request)
        {
            return _fleetGasolineService.DisableRunResourceGasoline(request);
        }
        #endregion
    }
}