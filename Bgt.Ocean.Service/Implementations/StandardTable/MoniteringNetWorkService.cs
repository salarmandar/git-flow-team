using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.StandardTable.MonitoringNetwork;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{
    public interface IMoniteringNetWorkService
    {
        IEnumerable<MasterMonitoringNetworkView> GetMasterMonitoringNetWorkList(GetMonitoringNetworkRequest request);
        ResponseCreateMonitoringNetwork CreateMonitoringNetwork(MasterMonitoringNetworkView request);
        ResponseCreateMonitoringNetwork UpdateMonitoringNetwork(MasterMonitoringNetworkView request);
        SystemMessageView EnableAndDisableMonitoringNetwork(EnableDisableNetworkRequest request);
    }

    public class MoniteringNetWorkService : IMoniteringNetWorkService
    {
        private readonly ISFOMasterMonitoringNetworkRepository _SFOMasterMonitoringNetworkRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ISystemService _systemService;

        private readonly IObjectComparerService _ObjectComparerService;
        private readonly ISFOTblSystemLogProcessRepository _LogProcessRepository;
        private readonly ISFOTblSystemLogCategoryRepository _LogCategoryRepository;
        private readonly IGenericLogService _genericLogService;

        private readonly IBaseRequest _baseRequest;
        private readonly IUnitOfWork<OceanDbEntities> _uow;

        public MoniteringNetWorkService(
            ISFOMasterMonitoringNetworkRepository SFOMasterMonitoringNetworkRepository,
            ISystemMessageRepository systemMessageRepository,
            ISystemService systemService,
            IGenericLogService genericLogService,
            IObjectComparerService objectComparerService,
            ISFOTblSystemLogProcessRepository logProcessRepository,
            ISFOTblSystemLogCategoryRepository logCategoryRepository,
            IBaseRequest baseRequest,
            IUnitOfWork<OceanDbEntities> uow
            )
        {
            _SFOMasterMonitoringNetworkRepository = SFOMasterMonitoringNetworkRepository;
            _systemMessageRepository = systemMessageRepository;
            _systemService = systemService;
            _genericLogService = genericLogService;
            _ObjectComparerService = objectComparerService;
            _LogProcessRepository = logProcessRepository;
            _LogCategoryRepository = logCategoryRepository;
            _baseRequest = baseRequest;
            _uow = uow;
        }

        public IEnumerable<MasterMonitoringNetworkView> GetMasterMonitoringNetWorkList(GetMonitoringNetworkRequest request)
        {
            var userData = _baseRequest.Data;

            var result = _SFOMasterMonitoringNetworkRepository.FindAll(e => (request.flagDisable || e.FlagDisable == false)
            && e.MasterCountry_Guid == request.countryGuid
            && (request.monitoringNetworkName == "" || e.MonitoringNetworkName.ToLower().Contains(request.monitoringNetworkName.ToLower()))).Select(e => new MasterMonitoringNetworkView
            {
                guid = e.Guid,
                masterCountry_Guid = e.MasterCountry_Guid.GetValueOrDefault(),
                monitoringNetworkName = e.MonitoringNetworkName,
                phoneNumber = e.PhoneNumber,
                flagDisable = e.FlagDisable.GetValueOrDefault(),
                userCreated = e.UserCreated,
                datetimeCreated = e.DatetimeCreated.ToDateFormat(userData.UserFormatDateTime),
                userModified = e.UserModified??"",
                datetimeModified = e.DatetimeModified.ToDateFormat(userData.UserFormatDateTime)
            }).OrderBy(e=> e.monitoringNetworkName);
            return result;
        }

        public ResponseCreateMonitoringNetwork CreateMonitoringNetwork(MasterMonitoringNetworkView request)
        {
            var userData = _baseRequest.Data;
            var respone = new ResponseCreateMonitoringNetwork();
            try
            {
                var NewMoniterNetworkGuid = Guid.NewGuid();
                var MessageReturn = new SystemMessageView();
                #region Check duplicate MonitoringNetwork
                var CheckName = _SFOMasterMonitoringNetworkRepository.FindAll(e => e.MonitoringNetworkName == request.monitoringNetworkName
                                                               && e.MasterCountry_Guid == request.masterCountry_Guid).FirstOrDefault();
                if (CheckName != null)
                {
                    if (CheckName.FlagDisable == true)
                    {
                        MessageReturn = _systemMessageRepository.FindByMsgId(-855, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();                        
                        respone.systemMessageView = MessageReturn;
                        return respone;
                    }
                    else
                    {
                        MessageReturn = _systemMessageRepository.FindByMsgId(-854, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();                        
                        respone.systemMessageView = MessageReturn;
                        return respone;
                    }
                }
                #endregion

                using (var transection = _uow.BeginTransaction())
                {

                    DateTime UTCdatetime = DateTime.UtcNow;
                    SFOTblMasterMonitoringNetwork insertNew = new SFOTblMasterMonitoringNetwork();                     
                    insertNew.Guid = NewMoniterNetworkGuid;
                    insertNew.MasterCountry_Guid = request.masterCountry_Guid;
                    insertNew.MonitoringNetworkName = request.monitoringNetworkName;
                    insertNew.PhoneNumber = request.phoneNumber;
                    insertNew.FlagDisable = false;
                    insertNew.UserCreated = userData.UserName;
                    insertNew.DatetimeCreated = userData.ClientDateTime;
                    insertNew.UniversalDatetimeCreated = UTCdatetime;
                    _SFOMasterMonitoringNetworkRepository.Create(insertNew);

                    #region Audit log                 
                    var LogProcessGuid = _LogProcessRepository.FindAll(e => e.ProcessCode == "STD_MNW").FirstOrDefault().Guid;
                    var LogCategoryGuid = _LogCategoryRepository.FindAll(e => e.CategoryCode == "STD_MNW_DETAIL").FirstOrDefault().Guid;

                    var genericLogList = new List<TransactionGenericLogModel>();
                    genericLogList.Add(new TransactionGenericLogModel
                    {
                        DateTimeCreated = userData.ClientDateTime.DateTime,
                        JSONValue = SystemHelper.GetJSONStringByArray<string>("Monitoring Network Name", request.monitoringNetworkName), //**not complete
                        LabelIndex = SystemHelper.GetJSONStringByArray<int>(0),
                        ReferenceValue = NewMoniterNetworkGuid.ToString(),
                        SystemLogCategory_Guid = LogCategoryGuid,
                        SystemLogProcess_Guid = LogProcessGuid,
                        SystemMsgID = "112",
                        UserCreated = userData.UserName
                    });
                    _genericLogService.BulkInsertTransactionGenericLog(genericLogList);
                    #endregion

                    _uow.Commit();
                    transection.Complete();
                }
                MessageReturn = _systemMessageRepository.FindByMsgId(992, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                MessageReturn.IsSuccess = true;
                respone.systemMessageView = MessageReturn;
                respone.monitoringNetwork_Guid = NewMoniterNetworkGuid;
                return respone;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                respone.systemMessageView = _systemMessageRepository.FindByMsgId(-184, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                return respone;
            }
        }

        public ResponseCreateMonitoringNetwork UpdateMonitoringNetwork(MasterMonitoringNetworkView request)
        {
            var userData = _baseRequest.Data;
            var respone = new ResponseCreateMonitoringNetwork();
            try
            {
                var MessageReturn = new SystemMessageView();
                #region Check duplicate MonitoringNetwork
                var CheckName = _SFOMasterMonitoringNetworkRepository.FindAll(e => e.MonitoringNetworkName == request.monitoringNetworkName
                                                                   && e.MasterCountry_Guid == request.masterCountry_Guid
                                                                   && e.Guid != request.guid).FirstOrDefault();
                if (CheckName != null)
                {
                    if (CheckName.FlagDisable == true)
                    {
                        MessageReturn = _systemMessageRepository.FindByMsgId(-855, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();                        
                        respone.systemMessageView = MessageReturn;
                        return respone;
                    }
                    else
                    {
                        MessageReturn = _systemMessageRepository.FindByMsgId(-854, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();                        
                        respone.systemMessageView = MessageReturn;
                        return respone;
                    }
                }
                #endregion

                using (var transection = _uow.BeginTransaction())
                {

                    DateTime UTCdatetime = DateTime.UtcNow;
                    var updateRow = _SFOMasterMonitoringNetworkRepository.FindById(request.guid);

                    #region Keep old Data
                    MasterMonitoringNetworkView oldRequest = new MasterMonitoringNetworkView();
                    oldRequest.phoneNumber = updateRow.PhoneNumber;
                    oldRequest.monitoringNetworkName = updateRow.MonitoringNetworkName;
                    #endregion

                    updateRow.PhoneNumber = request.phoneNumber;
                    updateRow.MonitoringNetworkName = request.monitoringNetworkName;
                    updateRow.UserModified = userData.UserName;
                    updateRow.DatetimeModified = userData.ClientDateTime;
                    updateRow.UniversalDatetimeModified = UTCdatetime;
                    _SFOMasterMonitoringNetworkRepository.Modify(updateRow);

                    #region Auditlog
                    var genericLogList = new List<TransactionGenericLogModel>();
                    var msgId = "663";
                    CompareResult ResultCompare;
                    ResultCompare = _ObjectComparerService.GetCompareResult(oldRequest, request, "STD_MNW_DETAIL");

                    foreach (var ChangeItem in ResultCompare.ChangeInfoList)
                    {
                        ChangeItem.OldValue = ChangeItem.OldValue == null ? "" : ChangeItem.OldValue;
                        ChangeItem.NewValue = ChangeItem.NewValue == null ? "" : ChangeItem.NewValue;

                        genericLogList.Add(new TransactionGenericLogModel
                        {
                            DateTimeCreated = userData.ClientDateTime.DateTime,
                            JSONValue = SystemHelper.GetJSONStringByArray<string>("Monitoring Network"
                                                     , ChangeItem.LabelKey
                                                     , ChangeItem.OldValue != "" ? ChangeItem.OldValue : "Empty"
                                                     , ChangeItem.NewValue != "" ? ChangeItem.NewValue : "Empty"),
                            LabelIndex = null,
                            ReferenceValue = request.guid.ToString(),
                            SystemLogCategory_Guid = ChangeItem.LogCategoryGuid.GetValueOrDefault(),
                            SystemLogProcess_Guid = ChangeItem.LogProcessGuid.GetValueOrDefault(),
                            SystemMsgID = msgId,
                            UserCreated = userData.UserName
                        });
                    }
                    _genericLogService.BulkInsertTransactionGenericLog(genericLogList);
                    #endregion
                    _uow.Commit();
                    transection.Complete();
                }

                MessageReturn = _systemMessageRepository.FindByMsgId(993, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                MessageReturn.IsSuccess = true;
                respone.systemMessageView = MessageReturn;
                respone.monitoringNetwork_Guid = request.guid.Value;
                return respone;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                respone.systemMessageView = _systemMessageRepository.FindByMsgId(-184, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                return respone;
            }
        }

        public SystemMessageView EnableAndDisableMonitoringNetwork(EnableDisableNetworkRequest request)
        {
            var userData = _baseRequest.Data;
            try
            {
                using (var transection = _uow.BeginTransaction())
                {

                    DateTime UTCdatetime = DateTime.UtcNow;
                    var updateRow = _SFOMasterMonitoringNetworkRepository.FindById(request.guid);
                    updateRow.FlagDisable = request.flagDisable;
                    updateRow.UserModified = userData.UserName;
                    updateRow.DatetimeModified = userData.ClientDateTime;
                    updateRow.UniversalDatetimeModified = UTCdatetime;

                    #region Audit log                                 
                    var LogProcessGuid = _LogProcessRepository.FindAll(e => e.ProcessCode == "STD_MNW").FirstOrDefault().Guid;
                    var LogCategoryGuid = _LogCategoryRepository.FindAll(e => e.CategoryCode == "STD_MNW_DETAIL").FirstOrDefault().Guid;
                    var genericLogList = new List<TransactionGenericLogModel>();
                    genericLogList.Add(new TransactionGenericLogModel
                    {
                        DateTimeCreated = userData.ClientDateTime.DateTime,
                        JSONValue = SystemHelper.GetJSONStringByArray<string>("Monitoring Network", updateRow.MonitoringNetworkName),
                        LabelIndex = SystemHelper.GetJSONStringByArray<int>(0),
                        ReferenceValue = request.guid.ToString(),
                        SystemLogCategory_Guid = LogCategoryGuid,
                        SystemLogProcess_Guid = LogProcessGuid,
                        SystemMsgID = request.flagDisable ? "116" : "115",
                        UserCreated = userData.UserName
                    });
                    _genericLogService.BulkInsertTransactionGenericLog(genericLogList);
                    #endregion

                    _uow.Commit();
                    transection.Complete();
                }

                var MessageReturn = _systemMessageRepository.FindByMsgId(0, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                MessageReturn.IsSuccess = true;
                return MessageReturn;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                return _systemMessageRepository.FindByMsgId(-184, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
            }
        }


    }
}
