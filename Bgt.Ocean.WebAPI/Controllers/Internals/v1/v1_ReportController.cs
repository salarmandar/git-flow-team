using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.Reports;
using Bgt.Ocean.Models.Reports.DailyPlan;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.Report;
using Bgt.Ocean.Service.Messagings.ExportExcelService;
using Bgt.Ocean.Service.ModelViews.Reports;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_ReportController : ApiController
    {
        #region Objects & Variables
        private readonly IExportExcelService _exportExcelService;
        private readonly IReportService _reportService;
        #endregion

        #region Constuctor
        public v1_ReportController(
            IExportExcelService exportExcelService,
            IReportService reportService)
        {
            _exportExcelService = exportExcelService;
            _reportService = reportService;
        }
        #endregion

        /// <summary>
        /// Download productivity Mexico file
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DownloadProductivityMexicoFile([FromUri]GetFileRequest request)
        {
            ReportProductivityCollection collection = new ReportProductivityCollection();
            collection.FileType = request.FileType;
            collection.BeginDate = request.BeginDate;
            collection.EndDate = request.EndDate;
            collection.BrinkSiteGuid = request.BrinkSiteGuid;
            collection.ReportStyleId = request.ReportStyleId;
            collection.UserName = request.UserName;

            try
            {
                var productivityBytes = _exportExcelService.ExcelProductivityCollection(collection);
                HttpResponseMessage result = new HttpResponseMessage();
                result.Content = new StreamContent(productivityBytes.ConvertToStream());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ProductivityMexico.xlsx"
                };

                return result;
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        #region Daily Plan Report       
        public IEnumerable<DailyPlanCustomerResponse> GetCustomerList([FromUri]DailyPlanCustomerRequest request)
        {
            return _reportService.GetCustomerList(request);
        }

        public IEnumerable<DailyPlanRouteGroupResponse> GetRouteGroupList([FromUri]DailyPlanRouteGroupRequest request)
        {
            return _reportService.GetRouteGroupList(request);
        }

        public IEnumerable<DailyPlanRouteGroupDetailResponse> GetRouteGroupDetailList([FromUri]DailyPlanRouteGroupDetailRequest request)
        {
            return _reportService.GetRouteGroupDetailList(request);
        }

        public IEnumerable<DailyPlanDataResponse> GetDailyPlanDataList([FromUri]DailyPlanDataRequest request)
        {
            return _reportService.GetDailyPlanDataList(request);
        }

        public IEnumerable<DailyPlanEmailResponse> GetDailyPlanEmailList([FromUri]List<DailyPlanEmailRequest> request)
        {
            return _reportService.GetDailyPlanEmailList(request);
        }
        #endregion

      
    }
}
