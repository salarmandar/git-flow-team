using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.AuditLog.ServiceRequest
{
    public interface IServiceRequestEditorService
    {

        SRCancelResponse CancelSR(SRCancelRequest request);
        SRRescheduleResponse RescheduleSR(SRRescheduleRequest request);
        BaseResponse AddJournalToServiceRequest(Guid serviceRequestGuid, string journalDescription, bool flagInternal = false);

    }


    public class ServiceRequestEditorService : IServiceRequestEditorService
    {
        private readonly ISFOTransactionServiceRequestRepository _transactionServiceRequestRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IObjectComparerService _objectComparerService;
        private readonly ISystemJobStatusRepository _systemJobStatusRepository;
        private readonly ISystemLog_HistoryErrorRepository _systemLogHistoryErrorRepository;
        private readonly IGenericLogService _genericLogService;
        private readonly ISFOMasterMachineRepository _masterMachineRepository;
        private readonly ISFOTblTransactionOTCRepository _transactionOtcRepository;
        private readonly ISFOApiUserService _sfoApiUserService;
        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegsRepository;
        private readonly ISFOSystemFunctionRepository _systemFunctionRepository;
        private readonly ISFOMasterJournalRepository _masterJournalRepository;
        private readonly ISFOSystemEnvironmentGlobalRepository _sfoSystemEnvironmentGlobalRepository;

        public ServiceRequestEditorService(
                ISFOTransactionServiceRequestRepository transactionServiceRequestRepository,
                IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
                ISFOSystemFunctionRepository systemFunctionRepository,
                ISystemLog_HistoryErrorRepository systemLogHistoryErrorRepository,
                ISFOSystemDataConfigurationRepository sfoSystemDataConfigurationRepository,
                IMasterUserRepository masterUserRepository,
                IUnitOfWork<OceanDbEntities> uow,
                ISystemMessageRepository systemMessageRepository,
                IObjectComparerService objectComparerService,
                ISystemJobStatusRepository systemJobStatusRepository,
                IGenericLogService genericLogService,
                ISFOMasterMachineRepository masterMachineRepository,
                ISFOTblTransactionOTCRepository transactionOtcRepository,
                ISFOApiUserService sfoApiUserService,
                IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegsRepository,
                ISFOMasterJournalRepository masterJournalRepository,
                ISFOSystemEnvironmentGlobalRepository sfoSystemEnvironmentGlobalRepository
            )
        {
            _transactionServiceRequestRepository = transactionServiceRequestRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _uow = uow;
            _systemMessageRepository = systemMessageRepository;
            _objectComparerService = objectComparerService;
            _systemJobStatusRepository = systemJobStatusRepository;
            _systemLogHistoryErrorRepository = systemLogHistoryErrorRepository;
            _genericLogService = genericLogService;
            _masterMachineRepository = masterMachineRepository;
            _transactionOtcRepository = transactionOtcRepository;
            _sfoApiUserService = sfoApiUserService;
            _masterActualJobServiceStopLegsRepository = masterActualJobServiceStopLegsRepository;
            _systemFunctionRepository = systemFunctionRepository;
            _masterJournalRepository = masterJournalRepository;
            _sfoSystemEnvironmentGlobalRepository = sfoSystemEnvironmentGlobalRepository;
        }

        #region Cancel SR


        public SRCancelResponse CancelSR(SRCancelRequest request)
        {
            var sr = _transactionServiceRequestRepository.FindById(request.TicketGuid);
            var userInfo = _sfoApiUserService.GetUserByConfiguration(sr.MasterCountryGuid);
            Func<ResponseWithMsgID, SRCancelResponse> getResponse = responseWithMsgID =>
            {
                var msg = _systemMessageRepository.FindByMsgId(responseWithMsgID.MsgID, userInfo.UserLanguageGuid.Value);
                return new SRCancelResponse
                {
                    MsgID = responseWithMsgID.MsgID,
                    IsSuccess = responseWithMsgID.IsSuccess,
                    Message = msg.MessageTextContent,
                    Title = msg.MessageTextTitle
                };
            };

            try
            {
                _uow.ConfigAutoDetectChanges(false);
                var changeInfo = _objectComparerService.GetCompareResult(sr, request, "OO_WEB_API_CANCEL_SR");
                
                var responseValidateSr = IsAllowCancel(sr.Guid);
                if (!responseValidateSr.IsSuccess)
                    return getResponse(responseValidateSr);

                // update job data
                UpdateJobHeaderForCancel(sr.TblMasterActualJobHeader);

                // update service request
                sr.SetDateTimeAndUserModified(userInfo.UserName);
                sr.TicketStatus_Guid = JobStatusHelper.CancelledGuid.ToGuid();
                sr.CancellationReason = request.CancelDescription;
                sr.MasterReasonType_Guid = request.MasterReasonTypeGuid;

                _masterActualJobHeaderRepository.Modify(sr.TblMasterActualJobHeader);
                _transactionServiceRequestRepository.Modify(sr);

                _uow.Commit();

                // add log
                CreateLogCancelSR(sr.MasterActualJobHeader_Guid.Value, changeInfo, userInfo);

                // update job seq
                UpdateJobOrderByRun(sr.TicketStatus_Guid, sr.MasterDailyRunResource_Guid.GetValueOrDefault(), sr.MasterCustomerLocation_Guid, sr.MasterCustomerLocation_Guid, sr.MasterSite_Guid.Value, sr.TblMasterActualJobHeader.TransectionDate);

                var msgSuccess = _systemMessageRepository.FindByMsgId(450, userInfo.UserLanguageGuid.Value)
                                    .ConvertToMessageView()
                                    .ReplaceTextContentStringFormatWithValue(sr.TicketNumber);

                return new SRCancelResponse
                {
                    IsSuccess = true,
                    Message = msgSuccess.MessageTextContent,
                    MsgID = msgSuccess.MsgID,
                    Title = msgSuccess.MessageTextTitle
                };
            }
            catch (Exception err)
            {
                _systemLogHistoryErrorRepository.CreateHistoryError(err);
                return getResponse(new ResponseWithMsgID(false, -2095));
            }

        }

        private void CreateLogCancelSR(Guid jobHeaderGuid, CompareResult compareResult, DataStorage userInfo)
        {
            var sr = _transactionServiceRequestRepository
                .FindAllAsQueryable(e => e.MasterActualJobHeader_Guid == jobHeaderGuid)
                .Select(e => new { e.TicketNumber })
                .FirstOrDefault();

            // use this JobStatusHelper.Cancelled for logging only
            CreateLogSRChangeStatus(jobHeaderGuid, sr.TicketNumber, JobStatusHelper.Cancelled, userInfo.UserName);
            CreateLogSRChangeInformation(jobHeaderGuid, sr.TicketNumber, compareResult, userInfo.UserName);
        }

        private ResponseWithMsgID IsAllowCancel(Guid ticketGuid)
        {
            var sr = _transactionServiceRequestRepository.FindById(ticketGuid);

            #region Validate SR it self

            if (sr.IsDepartureToOnsite())
                return new ResponseWithMsgID(false, -2090);

            if (new Guid[] { JobStatusHelper.CancelledGuid.ToGuid(), JobStatusHelper.ClosedGuid.ToGuid(), JobStatusHelper.ReopenGuid.ToGuid() }.Contains(sr.TicketStatus_Guid))
                return new ResponseWithMsgID(false, -2128);

            #endregion

            #region Validate SR With lock

            if (IsLockAlreadyOpen(sr.MasterCustomerLocation_Guid, sr.MasterActualJobHeader_Guid.Value))
                return new ResponseWithMsgID(false, -2132);

            #endregion

            return new ResponseWithMsgID(true, 0);
        }

        private void UpdateJobHeaderForCancel(TblMasterActualJobHeader jobHeader)
        {

            if (new int[] { ConstFlagSyncToMobile.AlreadySyncToMobile }.Contains(jobHeader.FlagSyncToMobile.GetValueOrDefault()))
            {
                jobHeader.SystemStatusJobID = JobStatusHelper.WaitingforDolphinReceive;
            }
            else if (jobHeader.FlagSyncToMobile == 0)
            {
                jobHeader.SystemStatusJobID = JobStatusHelper.CancelledJob;
                jobHeader.FlagSyncToMobile = ConstFlagSyncToMobile.CancelBeforeMobileReceive;
            }
            else
            {
                jobHeader.SystemStatusJobID = JobStatusHelper.CancelledJob;
            }

            jobHeader.FlagCancelAll = true;
            jobHeader.FlagJobClose = true;

        }

        private void UpdateJobOrderByRun(Guid ticketStatusGuid, Guid dailyRunGuid, Guid oldCustomerLocationGuid, Guid newCustomerLocationGuid, Guid brinksSiteGuid, DateTime? jobHeaderTransactionDate)
        {
            if (new Guid[] { JobStatusHelper.DispatchedGuid.ToGuid(), JobStatusHelper.CancelledGuid.ToGuid() }.Contains(ticketStatusGuid) || oldCustomerLocationGuid != newCustomerLocationGuid)
            {
                if (JobStatusHelper.CancelledGuid.ToGuid() == ticketStatusGuid)
                {
                    _masterActualJobHeaderRepository.Func_CheckAndUpdateJobOrder(dailyRunGuid);
                }
                else
                {
                    _masterActualJobHeaderRepository.UpdateJobOrderByRun(dailyRunGuid, false, null, jobHeaderTransactionDate, brinksSiteGuid, null);
                }

            }
        }

        private bool IsLockAlreadyOpen(Guid machineGuid, Guid jobHeaderGuid)
        {
            var permittedLockStatus = new string[] { LockStatusCode.InActive, LockStatusCode.Close, LockStatusCode.ErrorFail };
            var machine = _masterMachineRepository.FindById(machineGuid);
            var lockSerialNumberList = machine.SFOTblMasterMachine_LockType?
                    .Where(e => !e.SerailNumber.IsEmpty())
                    .Select(l => l.SerailNumber);

            if (lockSerialNumberList?.Count() > 0)
            {
                foreach (var serialNumber in lockSerialNumberList)
                {
                    var resultLockStatus = _transactionOtcRepository
                        .GetOTCStatus(machine.SFOTblMasterCountryTimeZone.MasterCountry_Guid, serialNumber)
                        ?.Where(e => e.MasterActualJobHeader_Guid == jobHeaderGuid)
                        .Select(e => e.OTCStatusCode);

                    if (resultLockStatus == null || resultLockStatus.Any())
                        continue;

                    if (permittedLockStatus.Union(resultLockStatus).Count() != permittedLockStatus.Count())
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Reschedule SR

        public SRRescheduleResponse RescheduleSR(SRRescheduleRequest request)
        {
            var sr = _transactionServiceRequestRepository.FindById(request.TicketGuid);
            var userInfo = _sfoApiUserService.GetUserByConfiguration(sr.MasterCountryGuid);
            Func<ResponseWithMsgID, SRRescheduleResponse> getResponse = responseWithMsgID =>
            {
                var msg = _systemMessageRepository.FindByMsgId(responseWithMsgID.MsgID, userInfo.UserLanguageGuid.Value);
                return new SRRescheduleResponse
                {
                    MsgID = responseWithMsgID.MsgID,
                    IsSuccess = responseWithMsgID.IsSuccess,
                    Message = msg.MessageTextContent,
                    Title = msg.MessageTextTitle
                };
            };

            try
            {
                _uow.ConfigAutoDetectChanges(false);
                var changeInfo = _objectComparerService.GetCompareResult(sr, request, "OO_WEB_API_RESCHEDULE_SR");
                var oldStatusGuid = sr.TicketStatus_Guid;
                var oldDailyRunGuid = sr.MasterDailyRunResource_Guid;
                var oldWorkDate = sr.TblMasterActualJobHeader.TransectionDate;

                // validate
                var resultValidate = IsAllowReschedule(sr.Guid, request.RescheduleDateTime);
                if (!resultValidate.IsSuccess)
                    return getResponse(resultValidate);

                // update job data
                var legs = _masterActualJobServiceStopLegsRepository.FindByJobHeader(sr.MasterActualJobHeader_Guid.Value);
                UpdateJobAndLegsWhenReschedule(legs, sr, request.RescheduleDateTime);

                // update service request
                UpdateSRWhenReschedule(sr, request, userInfo.UserName);

                _uow.Commit();

                // add log
                CreateLogRescheduleSR(changeInfo, sr.MasterActualJobHeader_Guid.Value, oldStatusGuid, userInfo);

                // update job seq
                UpdateJobOrderByRun(oldStatusGuid, sr.MasterDailyRunResource_Guid.GetValueOrDefault(), sr.MasterCustomerLocation_Guid, sr.MasterCustomerLocation_Guid, sr.MasterSite_Guid.Value, sr.TblMasterActualJobHeader.TransectionDate);

                var msgSuccess = _systemMessageRepository.FindByMsgId(450, userInfo.UserLanguageGuid.Value)
                                    .ConvertToMessageView()
                                    .ReplaceTextContentStringFormatWithValue(sr.TicketNumber);

                return new SRRescheduleResponse
                {
                    IsSuccess = true,
                    Message = msgSuccess.MessageTextContent,
                    MsgID = msgSuccess.MsgID,
                    Title = msgSuccess.MessageTextTitle,
                    NewTicketStatusGuid = sr.TicketStatus_Guid,
                    OldDailyRunGuid = oldDailyRunGuid.GetValueOrDefault(),
                    OldWorkDate = oldWorkDate.Value
                };
            }
            catch (Exception err)
            {
                _systemLogHistoryErrorRepository.CreateHistoryError(err);
                return getResponse(new ResponseWithMsgID(false, -2095));
            }
        }



        private ResponseWithMsgID IsAllowReschedule(Guid ticketGuid, DateTime rescheduleDateTime)
        {
            var sr = _transactionServiceRequestRepository.FindById(ticketGuid);
            var validStatus = new Guid[]
            {
                JobStatusHelper.PlannedGuid.ToGuid(),
                JobStatusHelper.OpenGuid.ToGuid(),
                JobStatusHelper.DispatchedGuid.ToGuid()
            };

            if (!validStatus.Contains(sr.TicketStatus_Guid))
                return new ResponseWithMsgID(false, -2139);

            if (sr.IsDepartureToOnsite())
                return new ResponseWithMsgID(false, -2136);

            if (sr.IsOnHold())
                return new ResponseWithMsgID(false, -2137);

            if (rescheduleDateTime.Date < DateTime.UtcNow.Date)
                return new ResponseWithMsgID(false, -2138);


            return new ResponseWithMsgID(true, 1);
        }

        private void UpdateJobAndLegsWhenReschedule(IEnumerable<TblMasterActualJobServiceStopLegs> jobLegsGuidList, SFOTblTransactionServiceRequest serviceRequestDb, DateTime utcRescheduleDate)
        {
            var jobHeader = serviceRequestDb.TblMasterActualJobHeader;
            var rescheduleDateTimeSite = _systemFunctionRepository.Func_CalculateTime(utcRescheduleDate, SystemTimeZoneHelper.UTC, serviceRequestDb.BrinksSiteTimeZoneID.Value);
            var isRequireClearTeamAssignment = IsRescheduleToPlan(utcRescheduleDate.Date) || IsRescheduleToOpen(utcRescheduleDate.Date, serviceRequestDb.DateTimeServiceDate.Value.Date);

            if (IsRescheduleToPlan(utcRescheduleDate.Date))
            {
                jobHeader.SystemStatusJobIDPrevious = jobHeader.SystemStatusJobID;
                jobHeader.SystemStatusJobID = JobStatusHelper.Planned;
            }
            else if (IsRescheduleToOpen(utcRescheduleDate.Date, serviceRequestDb.DateTimeServiceDate.Value.Date))
            {
                jobHeader.SystemStatusJobIDPrevious = jobHeader.SystemStatusJobID;
                jobHeader.SystemStatusJobID = JobStatusHelper.Open;
            }

            jobHeader.TransectionDate = rescheduleDateTimeSite;
            _masterActualJobHeaderRepository.Modify(jobHeader);

            jobLegsGuidList.ToList().ForEach(item =>
            {
                if (isRequireClearTeamAssignment)
                {
                    item.MasterRouteGroupDetail_Guid = null;
                    item.MasterRunResourceDaily_Guid = null;
                }

                // set new date to legs
                item.ServiceStopTransectionDate = rescheduleDateTimeSite;
                item.WindowsTimeServiceTimeStart = rescheduleDateTimeSite.ToMinDateTime();

                _masterActualJobServiceStopLegsRepository.Modify(item);
            });


        }

        private void UpdateSRWhenReschedule(SFOTblTransactionServiceRequest serviceRequestDb, SRRescheduleRequest rescheduleRequest, string username)
        {
            var utcRescheduleDateTime = rescheduleRequest.RescheduleDateTime;
            var isRescheduleToPlan = IsRescheduleToPlan(utcRescheduleDateTime.Date);
            var isRescheduleToOpen = IsRescheduleToOpen(utcRescheduleDateTime.Date, serviceRequestDb.DateTimeServiceDate.Value.Date);

            Action clearTeamAssignment = () =>
            {
                // Clear Team Assignment
                serviceRequestDb.MasterDailyRunResource_Guid = null;
                serviceRequestDb.ResponderName = null;
                serviceRequestDb.ResponderEmail = null;
                serviceRequestDb.ResponderShift = null;
                serviceRequestDb.FlagFLM = null;
                serviceRequestDb.FlagATM = null;
                serviceRequestDb.FlagEcash = null;
                serviceRequestDb.FlagCompuSafe = null;
            };

            if (isRescheduleToPlan || isRescheduleToOpen)
            {
                clearTeamAssignment();

                if (isRescheduleToPlan)
                    serviceRequestDb.TicketStatus_Guid = JobStatusHelper.PlannedGuid.ToGuid();
                else 
                    serviceRequestDb.TicketStatus_Guid = JobStatusHelper.OpenGuid.ToGuid();               
            }

            // Update SR Data
            serviceRequestDb.SetDateTimeAndUserModified(username);
            serviceRequestDb.DateTimeServiceDate = utcRescheduleDateTime;
            serviceRequestDb.RescheduleReason = rescheduleRequest.ReasonName;
            serviceRequestDb.ReportedDescription = rescheduleRequest.ReportedIncidentDescription;
            serviceRequestDb.ReportedServiceRequestDescription = rescheduleRequest.ReportedIncidentDescription;

            _transactionServiceRequestRepository.Modify(serviceRequestDb);
        }

        private void CreateLogRescheduleSR(CompareResult compareResult, Guid jobHeaderGuid, Guid oldStatusGuid, DataStorage userInfo)
        {
            var sr = _transactionServiceRequestRepository
               .FindAllAsQueryable(e => e.MasterActualJobHeader_Guid == jobHeaderGuid)
               .FirstOrDefault();


            if (sr.TicketStatus_Guid != oldStatusGuid)
            {
                CreateLogSRChangeStatus(jobHeaderGuid, sr.TicketNumber, JobStatusHelper.GetStatusIDByGuid(sr.TicketStatus_Guid), userInfo.UserName);
            }

            CreateLogSRChangeInformation(jobHeaderGuid, sr.TicketNumber, compareResult, userInfo.UserName);
        }

        private bool IsRescheduleToPlan(DateTime utcRescheduleDate)
            => utcRescheduleDate.Date > DateTime.UtcNow.Date;

        private bool IsRescheduleToOpen(DateTime utcRescheduleDate, DateTime utcCurrentServiceDate)
            => (utcCurrentServiceDate.Date < DateTime.UtcNow.Date && DateTime.UtcNow.Date == utcRescheduleDate.Date) || // Reschedule from past
               (utcCurrentServiceDate.Date > DateTime.UtcNow.Date && DateTime.UtcNow.Date == utcRescheduleDate.Date);   // Reschedule from future

        #endregion

        #region Add Journal To SR

        public BaseResponse AddJournalToServiceRequest(Guid serviceRequestGuid, string journalDescription, bool flagInternal = false)
        {
            var srModel = _transactionServiceRequestRepository.FindById(serviceRequestGuid);
            var user = _sfoApiUserService.GetUserByConfiguration(srModel.TblMasterSite.MasterCountry_Guid);
            try
            {
                var journalGuid = _sfoSystemEnvironmentGlobalRepository.FindByAppKey("JournalEntryAPIGuid").AppValue1;
                var masterJournal = _masterJournalRepository.FindById(journalGuid.ToGuid());

                #region Add Journal

                srModel.SFOTblTransactionServiceRequest_Journal.Add(new SFOTblTransactionServiceRequest_Journal
                {
                    Guid = Guid.NewGuid(),
                    DatetimeCreated = DateTime.Now,
                    UniversalDatetimeCreated = DateTime.UtcNow,
                    MasterSite_Guid = srModel.MasterSite_Guid,
                    FlagInternal = flagInternal,
                    JournalDescription = journalDescription,
                    JournalName = masterJournal.JournalName,
                    SFOMasterJournal_Guid = masterJournal.Guid
                });

                _transactionServiceRequestRepository.Modify(srModel);
                _uow.Commit();

                #endregion

                #region Create Log

                var genericLogList = new TransactionGenericLogModel();
                genericLogList.DateTimeCreated = DateTime.UtcNow;
                genericLogList.JSONValue = SystemHelper.GetJSONStringByArray<string>(srModel.TicketNumber, journalDescription);
                genericLogList.LabelIndex = null;
                genericLogList.ReferenceValue = srModel.MasterActualJobHeader_Guid.ToString();
                genericLogList.SystemLogCategory_Guid = SFOLogCategoryHelper.ServiceRequestDetailsGuid.ToGuid();
                genericLogList.SystemLogProcess_Guid = SFOProcessHelper.ServiceRequestGuid.ToGuid();
                genericLogList.SystemMsgID = "456";
                genericLogList.UserCreated = user.UserName;
                _genericLogService.InsertTransactionGenericLog(genericLogList);

                #endregion

                var msgSuccess = _systemMessageRepository.FindByMsgId(450, user.UserLanguageGuid.Value)
                        .ConvertToMessageView()
                        .ReplaceTextContentStringFormatWithValue(srModel.TicketNumber);

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = msgSuccess.MessageTextContent,
                    MsgID = msgSuccess.MsgID,
                    Title = msgSuccess.MessageTextContent
                };
            }
            catch (Exception err)
            {
                var msgFail = _systemMessageRepository.FindByMsgId(450, user.UserLanguageGuid.Value)
                        .ConvertToMessageView()
                        .ReplaceTextContentStringFormatWithValue(srModel.TicketNumber);

                _systemLogHistoryErrorRepository.CreateHistoryError(err);

                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = msgFail.MessageTextContent,
                    MsgID = msgFail.MsgID,
                    Title = msgFail.MessageTextTitle
                };
            }
        }

        #endregion

        #region Private Function

        private void CreateLogSRChangeStatus(Guid jobHeaderGuid, string ticketNumber, int statusId, string username)
        {
            var statusName = _systemJobStatusRepository.FindByStatusID(statusId).StatusJobName;

            var genericLogList = new TransactionGenericLogModel();
            genericLogList.DateTimeCreated = DateTime.UtcNow;
            genericLogList.JSONValue = SystemHelper.GetJSONStringByArray<string>(ticketNumber, statusName);
            genericLogList.LabelIndex = null;
            genericLogList.ReferenceValue = jobHeaderGuid.ToString();
            genericLogList.SystemLogCategory_Guid = SFOLogCategoryHelper.ServiceRequestDetailsGuid.ToGuid();
            genericLogList.SystemLogProcess_Guid = SFOProcessHelper.ServiceRequestGuid.ToGuid();
            genericLogList.SystemMsgID = "2104";
            genericLogList.UserCreated = username;
            _genericLogService.InsertTransactionGenericLog(genericLogList);
        }

        private void CreateLogSRChangeInformation(Guid jobHeaderGuid, string ticketNumber, CompareResult compareResult, string username)
        {
            var genericLogList = new List<TransactionGenericLogModel>();
            foreach (var r in compareResult.ChangeInfoList)
            {
                genericLogList.Add(new TransactionGenericLogModel
                {
                    DateTimeCreated = DateTime.UtcNow,
                    JSONValue = SystemHelper.GetJSONStringByArray<string>(ticketNumber
                                    , r.LabelKey, (r.OldValue ?? "Empty"), r.NewValue),
                    LabelIndex = null,
                    ReferenceValue = jobHeaderGuid.ToString(),
                    SystemLogCategory_Guid = SFOLogCategoryHelper.ServiceRequestDetailsGuid.ToGuid(),
                    SystemLogProcess_Guid = SFOProcessHelper.ServiceRequestGuid.ToGuid(),
                    SystemMsgID = "673",
                    UserCreated = username
                });
            }
            _genericLogService.BulkInsertTransactionGenericLog(genericLogList);

        }

        #endregion

        #region Private class

        private class ResponseWithMsgID
        {
            public bool IsSuccess { get; }
            public int MsgID { get; }

            private ResponseWithMsgID()
            {

            }

            public ResponseWithMsgID(bool isSuccess, int msgId)
            {
                IsSuccess = isSuccess;
                MsgID = msgId;
            }
        }

        #endregion
    }
}
