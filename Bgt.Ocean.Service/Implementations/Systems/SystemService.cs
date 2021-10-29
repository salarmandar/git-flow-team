using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models.MasterRoute;
using Bgt.Ocean.Models.Systems;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using Bgt.Ocean.Repository.EntityFramework.Core;

namespace Bgt.Ocean.Service.Implementations
{
    public interface ISystemService
    {
        SystemMessageView GetMessage(int msgId, Guid languageId, string[] textParams = null);
        int GetDefultMaxRow(Guid? countryGuid, Guid? siteGuid);
        IEnumerable<ServiceJobTypeView> GetServiceJobTypes(Guid? languageGuid, Guid? lobGuid = null);
        void CreateLogActivity(int activityId, string content, string userName, string clientIp, Guid? applicationGuid);
        string GetNumberCurrencyCultureCode(string formatNumberCurrency);
        void CreateHistoryError(Exception ex, string pageName, string clientIp, bool flagSendMail = false);
        void CreateHistoryError(Exception ex);
        IEnumerable<LanguageView> GetLanguageList();
        IEnumerable<SystemGlobalUnitView> GetSystemWeigthUnitList();
        IEnumerable<LobView> GetLOBList();
        IEnumerable<LineOfBusinessAndJobType> GetLineOfBusinessAndJobType();
        IEnumerable<SystemDayOfWeekView> GetSystemDayOfWeekList();
        SystemMessageView GetMessageByMsgId(int msgId);
        Task CreateLogActivityAsync(int activityId, string content, string userName, string clientIp, Guid applicationGuid);
        void CreateHistoryErrorAsync(Exception ex, string pageName, string ipAddress, string hostName);
        void CreateHistoryError_Force(Exception ex, string pageName);
        Task CreateAccessAPILogHistoryAsync(Guid? token, DateTime requestTime, string apiMethodName, string requestBody, DateTime responseTime, string responseBody);       

        SystemApplicationView GetSystemApplication(int appID);

        bool CheckExistData();
        SystemEnvironmentGlobalView GetAppKey(string appKey);

        IEnumerable<SystemMaterRouteTypeOfWeekView> GetSystemMasterRouteTypeOfWeek();
    }

    public class SystemService : ISystemService
    {
        private readonly ISystemApplicationRepository _systemApplicationRepository;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ISystemDisplayTextControlsLanguageRepository _systemDisplayTextControlsLanguageRepository;
        private readonly ISystemServiceJobTypeLOBRepository _systemServiceJobTypeLOBRepository;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;
        private readonly ISystemFormat_NumberCurrencyRepository _systemFormat_NumberCurrencyRepository;
        private readonly ISystemLog_HistoryErrorRepository _systemLog_HistoryErrorRepository;
        private readonly ISystemLanguageRepository _systemLanguageRepository;
        private readonly ISystemGlobalUnitRepository _systemGlobalUnitRepository;
        private readonly ISystemDayOfWeekRepository _systemDayOfWeekRepository;
        private readonly ISystemLineOfBusinessRepository _systemLineOfBusinessRepository;
        private readonly ISystemMaterRouteTypeOfWeekRepository _systemMaterRouteTypeOfWeekRepository;

        public SystemService(
            ISystemApplicationRepository systemApplicationRepository,
            ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository,
            ISystemMessageRepository systemMessageRepository,
            ISystemDisplayTextControlsLanguageRepository systemDisplayTextControlsLanguageRepository,
            ISystemServiceJobTypeLOBRepository systemServiceJobTypeLOBRepository,
            ISystemServiceJobTypeRepository systemServiceJobTypeRepository,
            ISystemLog_ActivityRepository systemLog_ActivityRepository,
            ISystemFormat_NumberCurrencyRepository systemFormat_NumberCurrencyRepository,
            ISystemLog_HistoryErrorRepository systemLog_HistoryErrorRepository,
            ISystemLanguageRepository systemLanguageRepository,
            ISystemGlobalUnitRepository systemGlobalUnitRepository,
            ISystemDayOfWeekRepository systemDayOfWeekRepository,
            ISystemLineOfBusinessRepository systemLineOfBusinessRepository,
            ISystemMaterRouteTypeOfWeekRepository systemMaterRouteTypeOfWeekRepository,
            IUnitOfWork<OceanDbEntities> uow)
        {
            _systemApplicationRepository = systemApplicationRepository;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
            _systemMessageRepository = systemMessageRepository;
            _systemDisplayTextControlsLanguageRepository = systemDisplayTextControlsLanguageRepository;
            _systemServiceJobTypeLOBRepository = systemServiceJobTypeLOBRepository;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _systemFormat_NumberCurrencyRepository = systemFormat_NumberCurrencyRepository;
            _systemLog_HistoryErrorRepository = systemLog_HistoryErrorRepository;
            _systemLanguageRepository = systemLanguageRepository;
            _systemGlobalUnitRepository = systemGlobalUnitRepository;
            _systemDayOfWeekRepository = systemDayOfWeekRepository;
            _systemLineOfBusinessRepository = systemLineOfBusinessRepository;
            _systemMaterRouteTypeOfWeekRepository = systemMaterRouteTypeOfWeekRepository;
        }

        public int GetDefultMaxRow(Guid? countryGuid, Guid? siteGuid)
        {
            try
            {
                var getCountryOprionMaxRow = _systemEnvironment_GlobalRepository.Func_CountryOption_Get(EnumAppKey.MaxDefaultRowSearch, siteGuid, countryGuid);
                var getGlobalMaxRow = _systemEnvironment_GlobalRepository.FindByAppKey(EnumAppKey.MaxRowSearch)?.AppValue1;

                int limitMaxRow = getGlobalMaxRow == null ? 500 : getGlobalMaxRow.ToInt();
                if (getCountryOprionMaxRow == null)
                    return limitMaxRow;
                else
                    return getCountryOprionMaxRow.AppValue1.ToInt() > limitMaxRow ? limitMaxRow : getCountryOprionMaxRow.AppValue1.ToInt();
            }
            catch
            {
                return 500;
            }
        }

        public SystemEnvironmentGlobalView GetAppKey(string appKey)
        {
            return _systemEnvironment_GlobalRepository.FindByAppKey(appKey).ConvertToSystemEnvironmentGlobalView();
        }

        public SystemMessageView GetMessage(int msgId, Guid languageId, string[] textParams = null)
        {
            var message = _systemMessageRepository.FindByMsgId(msgId, languageId).ConvertToMessageView();
            if (message != null && textParams != null)
            {
                message.MessageTextContent = string.Format(message.MessageTextContent, textParams);
            }
            return message;
        }


        public IEnumerable<ServiceJobTypeView> GetServiceJobTypes(Guid? languageGuid, Guid? lobGuid = null)
        {
            languageGuid = languageGuid ?? ApiSession.UserLanguage_Guid.Value;
            List<ServiceJobTypeView> response;
            if (lobGuid != null)
            {
                // service job type in lob
                var serviceTypeInLOB = _systemServiceJobTypeLOBRepository.FindAll(e => e.SystemLineOfBusinessGuid == lobGuid.Value);
                response = serviceTypeInLOB.Select(e => new ServiceJobTypeView()
                {
                    Guid = e.SystemServiceJobTypeGuid,
                    ServiceJobTypeID = e.TblSystemServiceJobType.ServiceJobTypeID,
                    ServiceJobTypeName = e.TblSystemServiceJobType.ServiceJobTypeName,
                    ServiceJobTypeNameAbb = e.TblSystemServiceJobType.ServiceJobTypeNameAbb,
                    TypeNameAbb_DisplayText = _systemDisplayTextControlsLanguageRepository.FindDisplayControlLanguage(e.TblSystemServiceJobType.SystemDisplayTextControlsAbb_Guid.GetValueOrDefault(), languageGuid.Value)?.DisplayText,
                    TypeName_DisplayText = _systemDisplayTextControlsLanguageRepository.FindDisplayControlLanguage(e.TblSystemServiceJobType.SystemDisplayTextControlsName_Guid.GetValueOrDefault(), languageGuid.Value)?.DisplayText,
                    SystemLineOfBusiness_Guid = e.SystemLineOfBusinessGuid
                }).ToList();
            }
            else
            {
                // all service job type
                response = _systemServiceJobTypeRepository.FindAll(e => e.FlagDisable == false).Select(e => new ServiceJobTypeView()
                {
                    Guid = e.Guid,
                    ServiceJobTypeID = e.ServiceJobTypeID,
                    ServiceJobTypeName = e.ServiceJobTypeName,
                    ServiceJobTypeNameAbb = e.ServiceJobTypeNameAbb,
                    TypeNameAbb_DisplayText = _systemDisplayTextControlsLanguageRepository.FindDisplayControlLanguage(e.SystemDisplayTextControlsAbb_Guid.GetValueOrDefault(), languageGuid.Value)?.DisplayText,
                    TypeName_DisplayText = _systemDisplayTextControlsLanguageRepository.FindDisplayControlLanguage(e.SystemDisplayTextControlsName_Guid.GetValueOrDefault(), languageGuid.Value)?.DisplayText,
                }).ToList();
            }

            return response.OrderBy(e => e.ServiceJobTypeID);
        }

        public void CreateLogActivity(int activityId, string content, string userName, string clientIp, Guid? applicationGuid)
        {
            using (var db = new OceanDbEntities())
            {
                // client computer name
                string clientName = SystemHelper.ClientHostName;
                TblSystemLog_Activity activity = new TblSystemLog_Activity();
                activity.Activity = content;
                activity.SystemActivity_ID = activityId;
                activity.SystemApplication_Guid = applicationGuid;
                activity.Guid = Guid.NewGuid();
                activity.UseID = userName;
                activity.DateTimeCreated = DateTime.Now;
                activity.ClientIP = clientIp;
                activity.ClientName = clientName;
                db.TblSystemLog_Activity.Add(activity);
                db.SaveChanges();
            }
        }


        public async Task CreateLogActivityAsync(int activityId, string content, string userName, string clientIp, Guid applicationGuid)
        {
            try
            {
                using (var db = new OceanDbEntities())
                {
                    TblSystemLog_Activity activity = new TblSystemLog_Activity();
                    activity.Activity = content;
                    activity.SystemActivity_ID = activityId;
                    activity.SystemApplication_Guid = applicationGuid;
                    activity.Guid = Guid.NewGuid();
                    activity.UseID = userName;
                    activity.DateTimeCreated = DateTime.Now;
                    activity.ClientIP = clientIp;

                    db.TblSystemLog_Activity.Add(activity);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception err)
            {
                CreateHistoryError(err);
            }
        }

        public void CreateHistoryErrorAsync(Exception ex, string pageName, string ipAddress, string hostName)
        {
            Task.Run(() => CreateHistoryError(ex, pageName, ipAddress, hostName));
        }
        private async Task CreateHistoryError(Exception ex, string pageName, string ipAddress, string hostName)
        {

            using (var db = new OceanDbEntities())
            {

                TblSystemLog_HistoryError newError = new TblSystemLog_HistoryError();
                newError.Guid = Guid.NewGuid();
                newError.ErrorDescription = ex.Message;
                newError.FunctionName = ex.TargetSite == null ? "" : ex.TargetSite.Name;
                newError.PageName = pageName;
                newError.InnerError = ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString();
                newError.ClientIP = ipAddress;
                newError.ClientName = hostName;
                newError.DatetimeCreated = DateTime.UtcNow;
                newError.FlagSendEmail = false;

                db.TblSystemLog_HistoryError.Add(newError);
                await db.SaveChangesAsync();

            }

        }

        public void CreateHistoryError_Force(Exception ex, string pageName)
        {
            using (var db = new OceanDbEntities())
            {
                var newError = new TblSystemLog_HistoryError();
                newError.Guid = Guid.NewGuid();
                newError.ErrorDescription = ex.Message;
                newError.FunctionName = ex.TargetSite == null ? "" : ex.TargetSite.Name;
                newError.PageName = pageName;
                newError.InnerError = ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString();
                newError.DatetimeCreated = DateTime.UtcNow;
                newError.FlagSendEmail = false;

                db.TblSystemLog_HistoryError.Add(newError);
                db.SaveChanges();
            }
        }

        public string GetNumberCurrencyCultureCode(string formatNumberCurrency)
        {
            var data = _systemFormat_NumberCurrencyRepository.FindAll(e => e.FormatNumberCurrency == formatNumberCurrency).FirstOrDefault();
            return data?.CultureInfoCode;
        }

        public void CreateHistoryError(Exception ex, string pageName, string clientIp, bool flagSendMail = false)
            => _systemLog_HistoryErrorRepository.CreateHistoryError(ex, pageName, clientIp, flagSendMail);

        public IEnumerable<LanguageView> GetLanguageList()
        {
            var result = _systemLanguageRepository.FindAll(e => e.FlagDisable == false && e.FlagAvaliableOnOO == true).Select(e => new LanguageView()
            {
                Guid = e.Guid,
                Abbreviation = e.Abbreviation,
                FlagDisable = e.FlagDisable.GetValueOrDefault(),
                LanguageCode = e.LanguageCode,
                LanguageName = e.LanaguageName
            });

            return result;
        }

        public void CreateHistoryError(Exception ex)
            => _systemLog_HistoryErrorRepository.CreateHistoryError(ex);

        public IEnumerable<SystemGlobalUnitView> GetSystemWeigthUnitList()
        {
            var languageGuid = ApiSession.UserLanguage_Guid.Value;
            var result = _systemGlobalUnitRepository.FindWeightUnit().Select(e => new SystemGlobalUnitView
            {
                Guid = e.Guid,
                UnitID = e.UnitID,
                UnitName = e.UnitName,
                UnitNameDisplayText = _systemDisplayTextControlsLanguageRepository.FindDisplayControlLanguage(e.SystemDisplayTextControls_Guid.GetValueOrDefault(), languageGuid)?.DisplayText,
                UnitNameAbbrevaition = e.UnitNameAbbrevaition,
                SystemGlobalUnitType_Guid = e.SystemGlobalUnitType_Guid
            }).ToList();
            return result.OrderBy(x => x.UnitNameDisplayText);
        }

        public IEnumerable<LobView> GetLOBList()
        {
            var allLOB = _systemLineOfBusinessRepository.FindAll().Where(e => !e.FlagDisable.GetValueOrDefault()).OrderBy(e => e.LOBFullName);
            var lobView = allLOB.ConvertToLOBView();
            return lobView;
        }

        public IEnumerable<LineOfBusinessAndJobType> GetLineOfBusinessAndJobType()
        {
            var languageGuid = ApiSession.UserLanguage_Guid.Value;
            IEnumerable<LineOfBusinessAndJobType> result = _systemServiceJobTypeLOBRepository.Func_LineOfBusinessJobTypeByFlagAdhocJob(languageGuid).ConvertToLineOfBusinessJobTypeView();
            return result;
        }

        public IEnumerable<SystemDayOfWeekView> GetSystemDayOfWeekList()
        {
            var languageGuid = ApiSession.UserLanguage_Guid.Value;
            var result = _systemDayOfWeekRepository.FindAll(x => !x.FlagDisable).Select(e => new SystemDayOfWeekView
            {
                Guid = e.Guid,
                MasterDayOfWeek_Name = e.MasterDayOfWeek_Name,
                MasterDayOfWeek_Sequence = e.MasterDayOfWeek_Sequence,
                SystemDisplayTextControls_Guid = e.SystemDisplayTextControls_Guid,
                MasterDayOfWeekNameDisplayText = _systemDisplayTextControlsLanguageRepository.FindDisplayControlLanguage(e.SystemDisplayTextControls_Guid.GetValueOrDefault(), languageGuid)?.DisplayText,
                FlagDisable = e.FlagDisable
            });
            return result.OrderBy(x => x.MasterDayOfWeek_Sequence);
        }

        public SystemMessageView GetMessageByMsgId(int msgId)
        {
            return _systemMessageRepository.FindByMsgId(msgId, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
        }

        public async Task CreateAccessAPILogHistoryAsync(Guid? token, DateTime requestTime, string apiMethodName, string requestBody, DateTime responseTime, string responseBody)
        {
            try
            {
                using (var db = OceanDbFactory.GetNewDbContext())
                {
                    TblSystemApiHistory history = new TblSystemApiHistory();
                    history.Guid = Guid.NewGuid();
                    history.TokenID = token;
                    history.DatetimeStop = responseTime;
                    history.DatetimeStart = requestTime;
                    history.ApiMethodName = apiMethodName;
                    history.ResponseBody = responseBody;
                    history.RequestBody = requestBody;

                    db.TblSystemApiHistory.Add(history);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception err)
            {
                CreateHistoryError(err);
            }
        }

        public SystemApplicationView GetSystemApplication(int appID)
        {
            return _systemApplicationRepository.FindByApplicationID(appID).ConvertToSystemApplicationView();
        }

        public bool CheckExistData()
        {
            return _systemApplicationRepository.Any();
        }

        public IEnumerable<SystemMaterRouteTypeOfWeekView> GetSystemMasterRouteTypeOfWeek()
        {
            return _systemMaterRouteTypeOfWeekRepository.GetMasterRouteTypeOfWeek().ConvertToSystemMaterRouteTypeOfWeekView();
        }
    }
}
