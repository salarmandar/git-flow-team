using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Models.Systems;
using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using Bgt.Ocean.Service.ModelViews.FleetMaintenanceViewModel.FleetAccidentViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_FleetAccidentController : ApiControllerBase
    {
        private readonly IFleetAccidentService _fleetAccidentService;

        public v1_FleetAccidentController(IFleetAccidentService fleetAccidentService)
        {
            _fleetAccidentService = fleetAccidentService;
        }

        #region GET

        /// <summary>
        /// Get Data Run Resource Type
        /// </summary>
        /// <param name="customerGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<VehicleTypeView>> GetRunResourceTypeList(Guid customerGuid)
        {
            return _fleetAccidentService.GetRunResourceTypeDdl(customerGuid);
        }

        [HttpGet]
        public IEnumerable<DropdownViewModel<VehicleBrandView>> GetRunResourceBrandList(Guid customerGuid)
        {
            return _fleetAccidentService.GetRunResourceBrandDdl(customerGuid);
        }

        /// <summary>
        /// Get brinks driver for dropdownlist
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<AccidentBrinksDriverView>> GetBrinksDriverList(Guid siteGuid)
        {
            return _fleetAccidentService.GetBrinksDriverList(siteGuid);
        }

        /// <summary>
        /// Get title name for dropdownlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<SystemTitleNameView>> GetTitleNameList()
        {
            return _fleetAccidentService.GetTitleNameList();
        }

        /// <summary>
        /// Get Grid Accident Info
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public FleetAccidentResponse RunResourceAccidentInfo(AccidentInfoViewRequest request)
        {
            return _fleetAccidentService.GetRunResourceAccidentInfo(request);
        }


        #region Get detail for edit
        /// <summary>
        /// Get detail for accident tab and counterparty tab
        /// </summary>
        /// <param name="accidentGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public AccidentDetailResponse GetAccidentDetail(Guid accidentGuid)
        {
            return _fleetAccidentService.GetAccidentDetail(accidentGuid);
        }

        /// <summary>
        /// Get accident damage list (Brinks)
        /// </summary>
        /// <param name="accidentGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public List<AccidentListDetailDamagedModelView> GetAccidentDamageList_Brinks(Guid accidentGuid)
        {
            return _fleetAccidentService.GetAccidentDamageList_Brinks(accidentGuid);
        }

        /// <summary>
        /// Get accident damage list (Counterparty)
        /// </summary>
        /// <param name="accidentGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public List<AccidentListDetailDamagedModelView> GetAccidentDamageList_CounterParty(Guid accidentGuid)
        {
            return _fleetAccidentService.GetAccidentDamageList_CounterParty(accidentGuid);
        }

        /// <summary>
        /// Get accident image
        /// </summary>
        /// <param name="accidentGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<AccidentImageRespone> GetAccidentImageList(Guid accidentGuid)
        {
            return _fleetAccidentService.GetAccidentImageList(accidentGuid);
        }
        #endregion


        #endregion
        #region SET
        [HttpPost]
        public AccidentResponse CreateFleetMaintenanceAccident(AccidentRequest modelRequest)
        {
            return _fleetAccidentService.CreateFleetMaintenanceAccident(modelRequest);
        }

        [HttpPost]
        public AccidentResponse UpdateFleetMaintenanceAccident(AccidentRequest modelRequest)
        {
            return _fleetAccidentService.UpdateFleetMaintenanceAccident(modelRequest);
        }

        [HttpPost]
        public AccidentResponse DisableFleetMaintenanceAccident(DisableAccidentRequest modelRequest)
        {
            return _fleetAccidentService.DisableFleetMaintenanceAccident(modelRequest);
        }      

        #endregion SET
    }

}