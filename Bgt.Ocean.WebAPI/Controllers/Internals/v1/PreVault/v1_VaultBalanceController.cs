using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models.Reports;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.PreVault;
using Bgt.Ocean.Service.Implementations.Report;
using Bgt.Ocean.Service.Messagings.PreVault;
using Bgt.Ocean.Service.Messagings.PreVault.VaultBalance;
using Bgt.Ocean.Service.ModelViews;
using Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_VaultBalanceController : ApiControllerBase
    {
        private readonly IVaultBalanceService _vaultBalanceService;
        private readonly IReportService _reportService;
        private readonly IBrinksService _brinksService;
        public v1_VaultBalanceController(IVaultBalanceService vaultBalanceService,
                                         IReportService reportService,
                                         IBrinksService brinksService)
        {
            _vaultBalanceService = vaultBalanceService;
            _reportService = reportService;
            _brinksService = brinksService;
        }

       

        [HttpPost]
        public VaultBalanceStateResponse GetVaultBalanceState(VaultBalanceRequest req)
        {
            return _vaultBalanceService.GetVaultBalanceState(req);
        }


        /// <summary>
        /// Update state vault state to precess
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public StartAndValidateVaultBalanceReponse VaultBalanceStartScan(VaultBalanceDetailRequest request)
        {
            return _vaultBalanceService.StartScanVaultBalance(request);
        }

        /// <summary>
       /// Get vault balance items seal and nonbarcode
       /// </summary>
       /// <param name="request"></param>
       /// <returns></returns>
        [HttpPost]
        public VaultBalanceDetailReponse VaultBalanceItemsDetail(VaultBalanceDetailRequest request)
        {
            return _vaultBalanceService.GetItemsVaultBalanceDetail(request);
        }

        /// <summary>
        /// Save and close
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView SaveAndCloseVaultBalance(VaultBalanceItemsRequest request)
        {
            return _vaultBalanceService.SaveAndCloseVaultBalance(request);
        }

        /// <summary>
        /// Verify permission vault balance
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public SystemMessageView VerifyVaultBalance(string username, string password)
        {
            return _vaultBalanceService.VerifyVaultBalance(username, password);
        }

        /// <summary>
        /// Get seal dupplicate for mapping
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<VaultBalanceMappingModelView> GetVaultBalanceSealMapping(VaultBalanceItemsRequest request)
        {
            return _vaultBalanceService.GetVaultBalanceSealMapping(request);
        }

        /// <summary>
        /// Get summary for display item overage and shortage
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<VaultBalanceSummaryModelView> GetVaultBalanceSummary(VaultBalanceItemsRequest request)
        {
            return _vaultBalanceService.GetVaultBalanceSummary(request);
        }

        /// <summary>
        /// The system will cancel the vault balance for the current round and undo all scanned items for the current round.
        /// </summary>
        /// <param name="vaultBalanceGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public SystemMessageView CancelVaultBalance(Guid vaultBalanceGuid)
        {
            return _vaultBalanceService.CancelVaultBalance(vaultBalanceGuid);
        }

        /// <summary>
        /// Update data vault balance to state complete
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SystemMessageView SubmitVaultBalance(VaultBalanceItemsRequest request)
        {
            return _vaultBalanceService.SubmitVaultBalance(request);
        }


        /// <summary>
        /// Print Vault Balance Report
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetReportVaultBalance(Guid headerGuid , string preVault, string userFormatDateTime)
        {
            
            try
            {
                var result = _reportService.GetReportVaultBalance(headerGuid, preVault, userFormatDateTime);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            
        }

        #region For dolphin
        [HttpPost]
        public VaultBalanceApiResponse ValidateVaultBalanceBlocking(VaultBalanceApiRequest request)
        {
            return _vaultBalanceService.ValidateVaultBalanceBlocking(request, Request.RequestUri);
        }
        #endregion
    }
}