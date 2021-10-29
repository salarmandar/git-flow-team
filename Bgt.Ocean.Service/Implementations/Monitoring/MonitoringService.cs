using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Helpers;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.MonitorService;
using Bgt.Ocean.Service.ModelViews.Monitoring;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static Bgt.Ocean.Infrastructure.Util.EnumMonitoring;

namespace Bgt.Ocean.Service.Implementations.Monitoring
{
    public interface IMonitoringService
    {
        PodMonitoringView GetPodMonitorData(PodMonitorRequest request);
        PodMonitorErrorResponse GetPodMonitorErrorData(Guid LogPod_Guid);

        SmartBillingSubmitResponse SaveUpdateSmartBillingConfig(SmartBillingConfigRequest request);
        SmartBillingConfigResponse SmartBillingConfigGet(Guid siteGuid);
        SmartBillingScheduleView SmartBillingSheduleDisplayGet(Guid siteGuid);
        SmartBillingMonitorResponse SmartBillingGenerateStatusGet(SmartBillingMonitorRequest request);
        string SmartBillingErrorMsgGet(SmartBillingErrorMsgRequest request);
    }

    public class MonitoringService : IMonitoringService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ISmartBillingScheduleDayMappingRepository _smartBillingScheduleDayMappingRepository;
        private readonly IMasterSite_SmartBillingScheduleRepository _masterSite_SmartBillingScheduleRepository;
        private readonly ISystemDisplayTextControlsLanguageRepository _systemDisplayTextControlsLanguageRepository;
        private readonly IMasterHistoryReportPushToSmartRepository _masterHistoryReportPushToSmartRepository;

        private readonly ISystemService _systemService;
        private readonly ISystemDomainEmailService _systemDomainEmailService;
        public MonitoringService(
            IUnitOfWork<OceanDbEntities> uow,
            ISystemMessageRepository systemMessageRepository,
            ISmartBillingScheduleDayMappingRepository smartBillingScheduleDayMappingRepository,
            IMasterSite_SmartBillingScheduleRepository masterSite_SmartBillingScheduleRepository,
            ISystemDisplayTextControlsLanguageRepository systemDisplayTextControlsLanguageRepository,
            IMasterHistoryReportPushToSmartRepository masterHistoryReportPushToSmartRepository,

            ISystemService systemService,
            ISystemDomainEmailService systemDomainEmailService
            )
        {
            _uow = uow;
            _systemMessageRepository = systemMessageRepository;
            _smartBillingScheduleDayMappingRepository = smartBillingScheduleDayMappingRepository;
            _masterSite_SmartBillingScheduleRepository = masterSite_SmartBillingScheduleRepository;
            _systemDisplayTextControlsLanguageRepository = systemDisplayTextControlsLanguageRepository;
            _masterHistoryReportPushToSmartRepository = masterHistoryReportPushToSmartRepository;

            _systemService = systemService;
            _systemDomainEmailService = systemDomainEmailService;
        }

        #region POD
        public PodMonitoringView GetPodMonitorData(PodMonitorRequest request)
        {
            PodMonitoringView response = new PodMonitoringView();
            StringBuilder query = null;
            StringBuilder queryTotal = null;

            string queryStatus = null;
            string success = "success";
            string fail = "fail";

            if (String.IsNullOrWhiteSpace(request.StrSuccessStatus))
            {
                queryStatus = "IN (1,0)";
            }
            else if (success.Contains(request.StrSuccessStatus.Trim().ToLower()))
            {
                queryStatus = "= 1";
            }
            else if (fail.Contains(request.StrSuccessStatus.Trim().ToLower()))
            {
                queryStatus = "= 0";
            }
            else
            {
                queryStatus = "= 2";
            }

            //Select Date Format
            query = new StringBuilder(" DECLARE @IntDateFormatSQL int");
            query.Append(" SELECT @IntDateFormatSQL = TblSystemFormat_Date.FormatSQLCode");
            query.Append(" FROM TblMasterUser INNER JOIN TblSystemRoleType ON TblMasterUser.SystemUserRole_Guid = TblSystemRoleType.Guid");
            query.Append(" LEFT OUTER JOIN TblSystemFormat_Date ON TblMasterUser.SystemFormat_Date_Guid = TblSystemFormat_Date.Guid");
            query.Append(" WHERE TblMasterUser.Username = @UserName");

            //Select Data
            query.Append(" BEGIN SELECT Guid AS LogPod_Guid, ISNULL(JobNo,'') AS JobNo, ISNULL(LocationName,'') AS LocationName, ISNULL(ActionType,'') AS ActionType, ISNULL(ReportFileName,'') AS ReportFileName, ISNULL(PodTypeName,'') AS PodTypeName,");
            query.Append(" ISNULL(Destination,'') AS Destination, DatetimeCreated, SuccessStatus,");
            query.Append(" (CONVERT(nvarchar(20), DatetimeCreated, @IntDateFormatSQL)) AS StrDatetimeCreated,");
            query.Append(" (CASE WHEN SuccessStatus = 1 THEN 'Success' WHEN SuccessStatus = 0 THEN 'Fail' ELSE '' END) AS StrSuccessStatus");
            query.Append(" FROM dbo.TblMasterHistoryLogPodByService L WHERE L.MasterSite_Guid = @SiteGuid");
            query.Append(" AND L.DatetimeCreated BETWEEN @DateFrom AND @DateTo");
            query.Append(" AND (JobNo LIKE '%'+@JobNo+'%' OR (@JobNo IS NULL))");
            query.Append(" AND (LocationName LIKE '%'+@LocationName+'%' OR (@LocationName IS NULL))");
            query.Append(" AND (ActionType LIKE '%'+@ActionType+'%' OR (@ActionType IS NULL))");
            query.Append(" AND (ReportFileName LIKE '%'+@ReportFileName+'%' OR (@ReportFileName IS NULL))");
            query.Append(" AND (PodTypeName LIKE '%'+@PodTypeName+'%' OR (@PodTypeName IS NULL))");
            query.Append(" AND (Destination LIKE '%'+@Destination+'%' OR (@Destination IS NULL))");
            query.Append(" AND (DatetimeCreated = @DatetimeCreated OR (@DatetimeCreated IS NULL))");
            query.Append(" AND (SuccessStatus " + queryStatus + " )");
            query.Append(" ORDER BY " + request.SortBy + " " + request.SortWith);
            query.Append(" OFFSET(@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY; END");

            SqlParameter pJobNo = new SqlParameter("@JobNo", request.JobNo);
            SqlParameter pLocationName = new SqlParameter("@LocationName", request.LocationName);
            SqlParameter pActionType = new SqlParameter("@ActionType", request.ActionType);
            SqlParameter pReportFileName = new SqlParameter("@ReportFileName", request.ReportFileName);
            SqlParameter pPodTypeName = new SqlParameter("@PodTypeName", request.PodTypeName);
            SqlParameter pDestination = new SqlParameter("@Destination", request.Destination);
            SqlParameter pDatetimeCreated = new SqlParameter("@DatetimeCreated", request.DatetimeCreated);

            if (String.IsNullOrWhiteSpace(request.JobNo))
            {
                pJobNo.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.LocationName))
            {
                pLocationName.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.ActionType))
            {
                pActionType.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.ReportFileName))
            {
                pReportFileName.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.PodTypeName))
            {
                pPodTypeName.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.Destination))
            {
                pDestination.Value = DBNull.Value;
            }
            if (request.DatetimeCreated == null)
            {
                pDatetimeCreated.Value = DBNull.Value;
            }

            using (var dbContext = new OceanDbEntities())
            {
                response.Data = dbContext.Database.SqlQuery<PodMonitorResponse>(
                            query.ToString()
                            , new SqlParameter("@UserName", request.UserName)
                            , new SqlParameter("@SiteGuid", request.SiteGuid)
                            , new SqlParameter("@DateFrom", request.DateFrom)
                            , new SqlParameter("@DateTo", request.DateTo)
                            , new SqlParameter("@page", request.Page)
                            , new SqlParameter("@rows", request.Rows)
                            , pJobNo
                            , pLocationName
                            , pActionType
                            , pReportFileName
                            , pPodTypeName
                            , pDestination
                            , pDatetimeCreated
                            ).ToList();
            }

            //Select Count Total of Data
            queryTotal = new StringBuilder(" SELECT COUNT(1)");
            queryTotal.Append(" FROM dbo.TblMasterHistoryLogPodByService L WHERE L.MasterSite_Guid = @pSiteGuid");
            queryTotal.Append(" AND L.DatetimeCreated BETWEEN @pDateFrom AND @pDateTo");
            queryTotal.Append(" AND (JobNo LIKE '%'+@pJobNo+'%' OR (@pJobNo IS NULL))");
            queryTotal.Append(" AND (LocationName LIKE '%'+@pLocationName+'%' OR (@pLocationName IS NULL))");
            queryTotal.Append(" AND (ActionType LIKE '%'+@pActionType+'%' OR (@pActionType IS NULL))");
            queryTotal.Append(" AND (ReportFileName LIKE '%'+@pReportFileName+'%' OR (@pReportFileName IS NULL))");
            queryTotal.Append(" AND (PodTypeName LIKE '%'+@pPodTypeName+'%' OR (@pPodTypeName IS NULL))");
            queryTotal.Append(" AND (Destination LIKE '%'+@pDestination+'%' OR (@pDestination IS NULL))");
            queryTotal.Append(" AND (DatetimeCreated = @pDatetimeCreated OR (@pDatetimeCreated IS NULL))");
            queryTotal.Append(" AND (SuccessStatus " + queryStatus + " )");

            SqlParameter _pJobNo = new SqlParameter("@pJobNo", request.JobNo);
            SqlParameter _pLocationName = new SqlParameter("@pLocationName", request.LocationName);
            SqlParameter _pActionType = new SqlParameter("@pActionType", request.ActionType);
            SqlParameter _pReportFileName = new SqlParameter("@pReportFileName", request.ReportFileName);
            SqlParameter _pPodTypeName = new SqlParameter("@pPodTypeName", request.PodTypeName);
            SqlParameter _pDestination = new SqlParameter("@pDestination", request.Destination);
            SqlParameter _pDatetimeCreated = new SqlParameter("@pDatetimeCreated", request.DatetimeCreated);

            if (String.IsNullOrWhiteSpace(request.JobNo))
            {
                _pJobNo.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.LocationName))
            {
                _pLocationName.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.ActionType))
            {
                _pActionType.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.ReportFileName))
            {
                _pReportFileName.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.PodTypeName))
            {
                _pPodTypeName.Value = DBNull.Value;
            }
            if (String.IsNullOrWhiteSpace(request.Destination))
            {
                _pDestination.Value = DBNull.Value;
            }
            if (request.DatetimeCreated == null)
            {
                _pDatetimeCreated.Value = DBNull.Value;
            }

            using (var dbContext = new OceanDbEntities())
            {
                var total = dbContext.Database.SqlQuery<int>(
                            queryTotal.ToString()
                            , new SqlParameter("@pSiteGuid", request.SiteGuid)
                            , new SqlParameter("@pDateFrom", request.DateFrom)
                            , new SqlParameter("@pDateTo", request.DateTo)
                            , _pJobNo
                            , _pLocationName
                            , _pActionType
                            , _pReportFileName
                            , _pPodTypeName
                            , _pDestination
                            , _pDatetimeCreated
                            ).FirstOrDefault();
                response.Total = total;
            }

            return response;
        }
        public PodMonitorErrorResponse GetPodMonitorErrorData(Guid LogPod_Guid)
        {
            IEnumerable<PodMonitorErrorResponse> data = null;
            StringBuilder query = null;
            query = new StringBuilder("SELECT Guid AS ErrorPod_Guid, MasterHistoryLogPodByService_Guid AS LogPod_Guid, ErrorLogDetail");
            query.Append(" FROM TblMasterHistoryErrorPodByService E WHERE E.MasterHistoryLogPodByService_Guid = @LogPod_Guid");

            using (var dbContext = new OceanDbEntities())
            {
                data = dbContext.Database.SqlQuery<PodMonitorErrorResponse>(query.ToString(), new SqlParameter("@LogPod_Guid", LogPod_Guid)).ToList();
            }
            return data.FirstOrDefault();
        }
        #endregion

        #region Smart Billing Monitoring

        #region Configuration
        public SmartBillingConfigResponse SmartBillingConfigGet(Guid siteGuid)
        {
            SmartBillingConfigResponse result = null;
            var dataSchedule = _masterSite_SmartBillingScheduleRepository.FindAllAsQueryable(e => e.MasterSite_Guid == siteGuid).FirstOrDefault();
            if (dataSchedule != null)
            {
                var daysSchedule = _smartBillingScheduleDayMappingRepository.FindAllAsQueryable(e => e.SmartBillingSchedule_Guid == dataSchedule.Guid)
                              .Select(o => o.SystemDayOfWeek_Guid).ToList();
                result = new SmartBillingConfigResponse()
                {
                    ConfigGuid = dataSchedule.Guid,
                    Email = dataSchedule.Email,
                    DropFilePath = dataSchedule.DropFilePath,
                    ScheduleTime = dataSchedule.ScheduledTime.ChangFromDateToTimeString(),
                    DaysGuid = daysSchedule,
                    FlagAutoGenerate = dataSchedule.FlagAutoGenerated,
                    SiteGuid = dataSchedule.MasterSite_Guid
                };
            }
            return result;
        }

        public SmartBillingSubmitResponse SaveUpdateSmartBillingConfig(SmartBillingConfigRequest request)
        {
            DateTimeOffset dateNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            Guid languageGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault();
            DateTime clientDateTime = ApiSession.ClientDatetime.LocalDateTime;
            SmartBillingSubmitResponse response = new SmartBillingSubmitResponse();
            if (!string.IsNullOrEmpty(request.Email) && !IsValidateEmailDomainConfig(request.Email))
            {
                response.MessageVeiw = _systemMessageRepository.FindByMsgId(-239, languageGuid).ConvertToMessageView();
                return response;
            }

            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    Guid scheduleGuid = Guid.NewGuid();
                    var siteSchedule = _masterSite_SmartBillingScheduleRepository.FindOne(e => e.MasterSite_Guid == request.SiteGuid);
                    if (siteSchedule != null)
                    {
                        if (request.FlagAutoGenerate)
                        {
                            scheduleGuid = siteSchedule.Guid;
                            siteSchedule.MasterSite_Guid = request.SiteGuid;
                            siteSchedule.Email = request.Email;
                            siteSchedule.DropFilePath = request.DropFilePath;
                            siteSchedule.ScheduledTime = request._schduleTime;
                        }

                        siteSchedule.FlagAutoGenerated = request.FlagAutoGenerate;
                        siteSchedule.UserModified = ApiSession.UserName;
                        siteSchedule.DatetimeModified = clientDateTime;
                        siteSchedule.UniversalDatetimeModified = dateNow;
                        _masterSite_SmartBillingScheduleRepository.Modify(siteSchedule);

                        response.ScheduleSiteGuid = siteSchedule.Guid;
                    }
                    else
                    {
                        var scheduleInsert = new TblMasterSite_SmartBillingSchedule
                        {
                            Guid = scheduleGuid,
                            MasterSite_Guid = request.SiteGuid,
                            FlagAutoGenerated = request.FlagAutoGenerate,
                            Email = request.Email,
                            DropFilePath = request.DropFilePath,
                            ScheduledTime = request._schduleTime,
                            UserCreated = ApiSession.UserName,
                            DatetimeCreated = clientDateTime,
                            UniversalDatetimeCreated = dateNow
                        };
                        _masterSite_SmartBillingScheduleRepository.Create(scheduleInsert);

                        response.ScheduleSiteGuid = scheduleGuid;
                    }
                    if (request.FlagAutoGenerate)
                    {
                        UpdateSmartBillingDayConfig(scheduleGuid, request, clientDateTime, dateNow);
                    }

                    _uow.Commit();
                    response.MessageVeiw = _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView();
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response.MessageVeiw = _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
                }
            }
            return response;
        }

        private void UpdateSmartBillingDayConfig(Guid scheduleGuid, SmartBillingConfigRequest request, DateTime clientDateTime, DateTimeOffset dateNow)
        {
            var scheduleDays = _smartBillingScheduleDayMappingRepository
                               .FindAllAsQueryable(e => e.SmartBillingSchedule_Guid == scheduleGuid).AsEnumerable();
            if (scheduleDays != null && !scheduleDays.IsEmpty())
            {
                _smartBillingScheduleDayMappingRepository.RemoveRange(scheduleDays);
            }

            var daysInsert = request.DaysGuid.Select(o => new TblSmartBillingSchedule_Day_Mapping
            {
                Guid = Guid.NewGuid(),
                SmartBillingSchedule_Guid = scheduleGuid,
                SystemDayOfWeek_Guid = o,
                UserCreated = ApiSession.UserName,
                DatetimeCreated = clientDateTime,
                UniversalDatetimeCreated = dateNow
            });
            _smartBillingScheduleDayMappingRepository.CreateRange(daysInsert);
        }

        private bool IsValidateEmailDomainConfig(string strEmail)
        {
            char[] delimit = { ',', ';' };
            var domainList = _systemDomainEmailService.GetEmailDomainsInfo().Select(o => o.AllowedDomain.ToLower()).ToList();
            var emailList = strEmail.Split(delimit).Select(o => new MailAddress(o).Host.ToLower()).ToList();

            if (emailList.Except(domainList).Any())
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Generating Status
        public SmartBillingScheduleView SmartBillingSheduleDisplayGet(Guid siteGuid)
        {
            SmartBillingScheduleView response = new SmartBillingScheduleView();
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            var smartBillingSite = _masterSite_SmartBillingScheduleRepository.FindOne(e => e.MasterSite_Guid == siteGuid && e.FlagAutoGenerated);
            if (smartBillingSite != null)
            {
                var dayOfWeekList = smartBillingSite.TblSmartBillingSchedule_Day_Mapping.Select(e => e.TblSystemDayOfWeek).ToList();
                var daysDisplayTxt = _systemDisplayTextControlsLanguageRepository.FindDisplayControlLanguageList(dayOfWeekList.Select(e => e.SystemDisplayTextControls_Guid.GetValueOrDefault()), LanguageGuid);

                var daysTxt = dayOfWeekList.Join(daysDisplayTxt,
                        d => d.SystemDisplayTextControls_Guid,
                        t => t.Guid,
                        (d, t) => new { d.MasterDayOfWeek_Sequence, t.DisplayText })
                        .OrderBy(o => o.MasterDayOfWeek_Sequence)
                        .Select(e => e.DisplayText);

                response.TimeConfig = smartBillingSite.ScheduledTime.ChangFromDateToTimeString();
                response.DaysConfig = string.Join(",", daysTxt);
            }
            return response;
        }
        public SmartBillingMonitorResponse SmartBillingGenerateStatusGet(SmartBillingMonitorRequest request)
        {
            SmartBillingMonitorResponse response = new SmartBillingMonitorResponse();
            try
            {
                var data = _masterHistoryReportPushToSmartRepository.GetSmartBillingAutoGenBySite(request.SiteGuid)
                           .Where(o => o.DatetimeCreated.GetValueOrDefault().Date >= request._fromDate.Date
                                   && o.DatetimeCreated.GetValueOrDefault().Date <= request._toDate.Date)
                           .ConvertToSmartBillingGenerateStatusView();

                if (data != null && !data.IsEmpty())
                {
                    var resp = PaginationHelper.ToPagination(data, request);
                    response.GenerateStatusList = resp.Data;
                    response.Total = (int)resp.Total;
                }
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return response;
        }
        public string SmartBillingErrorMsgGet(SmartBillingErrorMsgRequest request)
        {
            var data = _masterHistoryReportPushToSmartRepository.FindOne(o => o.Guid == request.AutoGenGuid);
            string respErrorMsg = string.Empty;
            if (data != null)
            {
                switch (request.ErrorMsgTypeID)
                {
                    case EnumErrorMsgType.AutoGen:
                        respErrorMsg = data.AutoGenErrorMessage ?? string.Empty;
                        break;
                    case EnumErrorMsgType.EmailSending:
                        respErrorMsg = data.EmailSendingErrorMessage ?? string.Empty;
                        break;
                    case EnumErrorMsgType.FileDropping:
                        respErrorMsg = data.FileDropingErrorMessage ?? string.Empty;
                        break;
                }
            }
            return respErrorMsg;
        }
        #endregion

        #endregion
    }
}
