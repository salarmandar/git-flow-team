using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.Prevault;
using Bgt.Ocean.Service.Messagings.PreVault;
using Bgt.Ocean.Service.ModelViews.PreVault;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_CheckOutDepartmentController : ApiControllerBase
    {
        private readonly ICheckOutDepartmentService _checkOutDepartmentService;

        public v1_CheckOutDepartmentController(ICheckOutDepartmentService checkOutDepartmentService, ISystemService systemService)
        {
            _checkOutDepartmentService = checkOutDepartmentService;
        }

        #region GET
        /// <summary>
        /// Get internal department
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>        
        [HttpGet]
        public IEnumerable<CheckOutDeptInternalDeptModelView> GetInternalDepartment(Guid siteGuid)
        {
            return _checkOutDepartmentService.InternalDepartmentGet(siteGuid);
        }

        /// <summary>
        /// POST : api/v1/in/CheckOutDepartment/CheckOutDepartmentItemSealGet
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> GetItemSealCheckOutDepartment(CheckOutDepartmentViewModel request)
        {
            return _checkOutDepartmentService.CheckOutDeptItemSealConsolidateGet(request);
        }

        /// <summary>
        /// POST : api/v1/in/CheckOutDepartment/CheckOutDepartmentItemNonbarcodeGet
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public CheckOutNonbarcodeModelView GetItemNonbarcodeCheckOutDepartment(CheckOutDepartmentViewModel request)
        {
            return _checkOutDepartmentService.CheckOutDeptItemNonbarcodeGet(request);
        }

        /// <summary>
        /// POST : api/v1/in/CheckOutDepartment/GetRouteGroupRunResourceList
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <param name="workDate"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<RouteGroupRunResourceModelView> GetRouteGroupRunResourceList(Guid siteGuid,string workDate)
        {
            return _checkOutDepartmentService.GetRouteGroupRunResourceList(siteGuid,workDate);
        }
        #endregion

        #region SUBMIT
        /// <summary>
        /// POST : api/v1/in/CheckOutDepartment/SubmitCheckOutDepartment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public CheckOutDepartmentResponse SubmitCheckOutDepartment(CheckOutDepartmentRequest request)
        {
            return _checkOutDepartmentService.CheckOutToDepartmentSubmit(request);
        }
        #endregion
    }
}