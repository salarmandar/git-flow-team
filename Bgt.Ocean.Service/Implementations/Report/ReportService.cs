using Bgt.Ocean.Models.Reports;
using Bgt.Ocean.Models.Reports.DailyPlan;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.Report
{
    public interface IReportService
    {
        #region Daily Plan
        IEnumerable<DailyPlanCustomerResponse> GetCustomerList(DailyPlanCustomerRequest request);
        IEnumerable<DailyPlanRouteGroupResponse> GetRouteGroupList(DailyPlanRouteGroupRequest request);
        IEnumerable<DailyPlanRouteGroupDetailResponse> GetRouteGroupDetailList(DailyPlanRouteGroupDetailRequest request);
        IEnumerable<DailyPlanDataResponse> GetDailyPlanDataList(DailyPlanDataRequest request);
        IEnumerable<DailyPlanEmailResponse> GetDailyPlanEmailList(List<DailyPlanEmailRequest> request);
        #endregion

        #region Vault Balance
        ExportFileResponse GetReportVaultBalance(Guid VaultBalanceHeaderGuid,  string PreVault, string UserFormatDateTime);
        #endregion
    }

    public class ReportService : IReportService
   { 
        #region Object & Variables
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly IVaultBalanceHeaderRepository _vaultBalanceHeaderRepository;
        #endregion

        #region Constuctor
        public ReportService(IMasterDailyRunResourceRepository masterDailyRunResourceRepository ,
                             IVaultBalanceHeaderRepository vaultBalanceHeaderRepository)
        {
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _vaultBalanceHeaderRepository = vaultBalanceHeaderRepository;
        }
        #endregion

        #region Functions
        public IEnumerable<DailyPlanCustomerResponse> GetCustomerList(DailyPlanCustomerRequest request)
        {
            return _masterDailyRunResourceRepository.GetDailyPlanCustomer(request);
        }

        public IEnumerable<DailyPlanRouteGroupResponse> GetRouteGroupList(DailyPlanRouteGroupRequest request)
        {
            return _masterDailyRunResourceRepository.GetDailyPlanRouteGroup(request);
        }

        public IEnumerable<DailyPlanRouteGroupDetailResponse> GetRouteGroupDetailList(DailyPlanRouteGroupDetailRequest request)
        {
            return _masterDailyRunResourceRepository.GetDailyPlanRouteDetail(request);
        }

        public IEnumerable<DailyPlanDataResponse> GetDailyPlanDataList(DailyPlanDataRequest request)
        {
            return _masterDailyRunResourceRepository.GetDailyPlanDataList(request);
        }

        public IEnumerable<DailyPlanEmailResponse> GetDailyPlanEmailList(List<DailyPlanEmailRequest> request)
        {
            return _masterDailyRunResourceRepository.GetDailyPlanEmailList(request);
        }

        public ExportFileResponse GetReportVaultBalance(Guid VaultBalanceHeaderGuid, string PreVault , string UserFormatDateTime)
        {
            ExportFileResponse response = new ExportFileResponse();

            try
            {
                LocalReport report = new LocalReport();
                byte[] buffer;
                
                report.ReportPath = System.Web.Hosting.HostingEnvironment.MapPath($"~/bin/Report/VaultBalanceReport.rdlc");

                var dataQuery = _vaultBalanceHeaderRepository.GetValueBalanceReport(VaultBalanceHeaderGuid , UserFormatDateTime);
                report.SetParameters(new List<ReportParameter>()
                {
                    new ReportParameter("pm_PrintDate", DateTime.Now.ToString(UserFormatDateTime))
                  , new ReportParameter("pm_UserVerify", dataQuery.UserVerify)
                  , new ReportParameter("pm_DatetimeVerify", dataQuery.datetimeVerify)
                  , new ReportParameter("pm_PreVault", PreVault)
                });          
                            
                ReportDataSource dtSourceDetail = new ReportDataSource("VaultBalance_Main", dataQuery.Header.OrderBy(e=>e.datetimeCreate));
                report.DataSources.Add(dtSourceDetail);
                dtSourceDetail = new ReportDataSource("BalanceNonBarcode", dataQuery.ListBalance);
                report.DataSources.Add(dtSourceDetail);
                dtSourceDetail = new ReportDataSource("DiscNonBarcode", dataQuery.ListDiscrepency);
                report.DataSources.Add(dtSourceDetail);
                report.DisplayName = "Vault_Balance_ENG_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";               

                buffer = report.Render("PDF");                         
                response.ReportData = buffer;
                response.FileName = "Vault_Balance_ENG_"+ DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessge = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }
        #endregion
    }
}
