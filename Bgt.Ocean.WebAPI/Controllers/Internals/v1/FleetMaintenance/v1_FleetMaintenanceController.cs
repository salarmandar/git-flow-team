using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_FleetMaintenanceController : ApiControllerBase
    {

        private readonly IFleetMaintenanceService _fleetMaintenanceService;
        public v1_FleetMaintenanceController(IFleetMaintenanceService fleetMaintenanceService)
        {
            _fleetMaintenanceService = fleetMaintenanceService;
        }

        #region GET
        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetFleetMaintenanceByRunResource
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public FleetMaintenanceFilterResponse GetFleetMaintenanceByRunResource(FleetMaintenanceFilterRequest req)
        {
            var result = _fleetMaintenanceService.GetFleetMaintenanceByRunResource(req);
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetFleetMaintenanceDetail
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public FleetMaintenanceDetailBaseResponse GetFleetMaintenanceDetail(FleetMaintenanceDetailRequest req)
        {
            var result = _fleetMaintenanceService.GetFleetMaintenanceDetail(req);
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetFleetMaintenanceCategoryDetail
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public FleetMaintenanceCategoryDetailResponse GetFleetMaintenanceCategoryDetail(FleetMaintenanceCategoryDetailRequest req)
        {
            var result = _fleetMaintenanceService.GetFleetMaintenanceCategoryDetail(req);
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetMaintenanceIDBySite
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public int GetMaintenanceIDBySite(Guid? siteGuid)
        {
            return _fleetMaintenanceService.GetMaintenanceID(siteGuid);
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetCurrentOdometerByRunResource
        /// </summary>
        /// <param name="runGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public int GetCurrentOdometerByRunResource(Guid? runGuid)
        {
            return _fleetMaintenanceService.GetCurrentOdometerByRunResource(runGuid);
        }
   

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetMaintenanceCategoryItems
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<MaintenanceCategoryDetailView> GetMaintenanceCategoryItems(FleetMaintenanceCategoryItemsRequest req)
        {
            var result = _fleetMaintenanceService.GetMaintenanceCategoryItems(req);
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetMaintenanceCategory
        /// </summary>
        /// <param name="brinksCompanyGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<MaintenanceCategoryView>> GetMaintenanceCategory(Guid? brinksCompanyGuid)
        {
            var result = _fleetMaintenanceService.GetMaintenanceCategory(brinksCompanyGuid);
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetVendorBySite
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<VendorView>> GetVendorBySite(Guid? siteGuid)
        {
            var result = _fleetMaintenanceService.GetVendorBySite(siteGuid);

            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetVendorByBrinksCompany
        /// </summary>
        /// <param name="brinksCompanyGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<VendorView>> GetVendorByBrinksCompany(Guid? brinksCompanyGuid)
        {
            var result = _fleetMaintenanceService.GetVendorByBrinksCompany(brinksCompanyGuid);

            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetMaintenanceStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DropdownViewModel<MaintenanceStatusView>> GetMaintenanceStatus()
        {
            var result = _fleetMaintenanceService.GetMaintenanceStatus();
            return result;
        }
        #endregion

        #region SET

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised=> InsertOrUpdateFleetMaintenance
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public FleetMaintenanceResponse InsertOrUpdateFleetMaintenance(FleetMaintenanceRequest req)
        {
            var result = _fleetMaintenanceService.InsertOrUpdateFleetMaintenance(req);
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => UpdateCancelFleetMaintenance
        /// </summary>
        /// <param name="maintenanceGuid"></param>
        /// <returns></returns>
        [HttpPost]
        public FleetMaintenanceCancelResponse UpdateCancelFleetMaintenance(Guid? maintenanceGuid)
        {
            var result = _fleetMaintenanceService.UpdateCancelFleetMaintenance(maintenanceGuid);
            return result;
        }

        #endregion





    }
}