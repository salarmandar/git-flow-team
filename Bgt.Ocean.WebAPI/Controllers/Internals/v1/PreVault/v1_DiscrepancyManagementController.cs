using Bgt.Ocean.Models.PreVault;
using Bgt.Ocean.Service.Implementations.PreVault;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_DiscrepancyManagementController : ApiControllerBase
    {
        private readonly IDiscrepancyManagementService _discrepancyManagementService;
        #region ### DEPENDENCY INJECTION ###
        public v1_DiscrepancyManagementController(IDiscrepancyManagementService discrepancyManagementService)
        {
            _discrepancyManagementService = discrepancyManagementService;
        }
        #endregion

        /// <summary>
        /// Close case Item from Discrepancy page 
        /// OO Web :: Scripts/Pages/Discrepancies/CloseCase.js
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView ClosecaseVaultBalance(DiscrepancyCloseCaseModel request)
        {
            return _discrepancyManagementService.ClosecaseVaultBalance(request);                        
        }

        /// <summary>
        /// Get Discrepancy Item Mode :: Vault balance
        /// OO Web :: Scripts/Pages/Discrepancies/DicrepanciesManagement.js
        /// </summary>
        /// <param name="prevaultGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDiscrepancyVaultBalanceItems(Guid prevaultGuid)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _discrepancyManagementService.GetDiscrepancyVaultBalanceItems(prevaultGuid));
        }

        /// <summary>
        /// Get Discrepancy Item Mode :: Check in Process
        /// OO Web :: Scripts/Pages/Discrepancies/DicrepanciesManagement.js
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <param name="prevaultGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDiscrepancyCheckInProcessItems(Guid siteGuid, Guid prevaultGuid)
        {
            return Request.CreateResponse(HttpStatusCode.OK,_discrepancyManagementService.GetDiscrepancyCheckInProcessItems(siteGuid, prevaultGuid));
        }

    }
}