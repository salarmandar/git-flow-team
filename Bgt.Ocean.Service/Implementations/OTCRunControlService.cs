using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.OTCManagement;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.RunControls;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumOTC;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Service.Implementations
{

    #region Interface

    public interface IOTCRunControlService
    {
        SystemMessageView UpdateJobHeaderOTCAfterSave(JobOtcRequest JobOtcRequest);
        SystemMessageView CreateHeaderOtcByJobGuids(IEnumerable<Guid> jobGuids);
    }

    #endregion

    public class OTCRunControlService : IOTCRunControlService
    {
        private readonly IMasterHistoryActualJobRepository _masterHistoryActualJobRepository;
        private readonly IMasterCountryRepository _masterCountryRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterActualJobHeaderOTCRepository _masterActualJobHeaderOTCRepository;
        private readonly ISFOMasterMachineRepository _sfoMasterMachineRepository;
        private readonly ISFOMasterMachineLockTypeRepository _sfoMasterMachineLockTypeRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemCustomerLocationTypeRepository _systemCustomerLocationTypeRepository;
        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegsRepository;
        private readonly IOTCManagementService _otcManagementService;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;
        private readonly IBaseRequest _baseRequest;
        private readonly ISFOMasterOTCLockModeRepository _sfoMasterOTCLockModeRepository;
        public OTCRunControlService(
                IMasterHistoryActualJobRepository masterHistoryActualJobRepository,
                IMasterCountryRepository masterCountryRepository,
                IMasterCustomerLocationRepository masterCustomerLocationRepository,
                IMasterCustomerRepository masterCustomerRepository,
                IMasterActualJobHeaderOTCRepository masterActualJobHeaderOTCRepository,
                ISFOMasterMachineRepository sfoMasterMachineRepository,
                ISFOMasterMachineLockTypeRepository SFOTblMasterMachine_LockTypeRepository,
                IMasterSiteRepository masterSiteRepository,
                ISystemMessageRepository systemMessageRepository,
                IUnitOfWork<OceanDbEntities> uow,
                ISystemCustomerLocationTypeRepository systemCustomerLocationTypeRepository,
                IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegsRepository,
                IOTCManagementService otcManagementService,
                IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
                ISystemService systemService,
                ISystemServiceJobTypeRepository systemServiceJobTypeRepository,
                IBaseRequest baseRequest,
                ISFOMasterOTCLockModeRepository sfoMasterOTCLockModeRepository
            )
        {
            _masterHistoryActualJobRepository = masterHistoryActualJobRepository;
            _masterCountryRepository = masterCountryRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _masterCustomerRepository = masterCustomerRepository;
            _masterActualJobHeaderOTCRepository = masterActualJobHeaderOTCRepository;
            _sfoMasterMachineRepository = sfoMasterMachineRepository;
            _sfoMasterMachineLockTypeRepository = SFOTblMasterMachine_LockTypeRepository;
            _masterSiteRepository = masterSiteRepository;
            _systemMessageRepository = systemMessageRepository;
            _uow = uow;
            _systemCustomerLocationTypeRepository = systemCustomerLocationTypeRepository;
            _masterActualJobServiceStopLegsRepository = masterActualJobServiceStopLegsRepository;
            _otcManagementService = otcManagementService;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _systemService = systemService;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _baseRequest = baseRequest;
            _sfoMasterOTCLockModeRepository = sfoMasterOTCLockModeRepository;
        }

        public SystemMessageView CreateHeaderOtcByJobGuids(IEnumerable<Guid> jobGuids)
        {
            SystemMessageView result = _systemMessageRepository.FindByMsgId(0, Guid.Empty).ConvertToMessageView(true);
            try
            {
                using (var tran = _uow.BeginTransaction())
                {
                    #region ## Related Jobs

                    var allJobs = from head in _masterActualJobHeaderRepository.FindAllAsQueryable()
                                  join type in _systemServiceJobTypeRepository.FindAllAsQueryable() on head.SystemServiceJobType_Guid equals type.Guid
                                  join leg in _masterActualJobServiceStopLegsRepository.FindAllAsQueryable() on head.Guid equals leg.MasterActualJobHeader_Guid
                                  where jobGuids.Any(j => j == head.Guid)
                                  select new { head, type, leg };
                    #endregion

                    #region ## Create OTC MCS

                    var allMSCjobs = allJobs.Where(o => o.type.ServiceJobTypeID == IntTypeJob.MCS);
                    if (allMSCjobs != null && allMSCjobs.Any())
                    {
                        var allMSCCusLegs = (from job in allMSCjobs
                                             join loc in _masterCustomerLocationRepository.FindAllAsQueryable() on job.leg.MasterCustomerLocation_Guid equals loc.Guid
                                             join cus in _masterCustomerRepository.FindAllAsQueryable() on loc.MasterCustomer_Guid equals cus.Guid
                                             join site in _masterSiteRepository.FindAllAsQueryable() on job.leg.MasterSite_Guid equals site.Guid
                                             where (bool)cus.FlagChkCustomer && jobGuids.Any(j => j == job.head.Guid)
                                             select new { leg = job.leg, site, type = job.type }).ToList();

                        foreach (var obj in allMSCCusLegs)
                        {
                            CreateJobHeaderOtcRequest otcrequest = new CreateJobHeaderOtcRequest()
                            {
                                CusLocationGuid = obj.leg.MasterCustomerLocation_Guid.GetValueOrDefault(), //must have value if it's job P
                                MasterActualJobHeaderGuid = obj.leg.MasterActualJobHeader_Guid.GetValueOrDefault(),
                                MaserActualJobLegGuid = obj.leg.Guid,
                                UserCreated = _baseRequest.Data.UserName,
                                DatetimeCreated = _baseRequest.Data.LocalClientDateTime,
                                UniversalDatetimeCreated = _baseRequest.Data.UniversalDatetime,
                                OtcBranchName = obj.site?.SiteName,
                                OtcLockUser = _baseRequest.Data.UserName,
                                OtcLockMode = GetLockModeByServiceJobTypeId(obj.type.ServiceJobTypeID.GetValueOrDefault(), obj.site?.MasterCountry_Guid),
                            };
                            CreateActualJobHeaderOTCSupportCashAdd(otcrequest);
                        }
                    }


                    #endregion

                    #region ## Create OTC not MCS
                    var notMCSJobs = allJobs.Where(o => o.type.ServiceJobTypeID != IntTypeJob.MCS)
                                                .Select(o => (Guid)o.leg.MasterActualJobHeader_Guid).Distinct();
                    if (notMCSJobs != null && notMCSJobs.Any())
                    {
                        var otcList = _masterActualJobHeaderOTCRepository.GetOTCLegsByJobGuids(notMCSJobs);
                        if (otcList != null && otcList.Any())
                        {
                            //remove
                            var oldOtcGuids = otcList.Select(o => o.MasterActualJobHeader_Guid).Distinct();
                            var oldOtc = _masterActualJobHeaderOTCRepository.FindAllAsQueryable(o => oldOtcGuids.Any(j => j == o.MasterActualJobHeader_Guid));
                            if (oldOtc != null && oldOtc.Any())
                            {
                                _masterActualJobHeaderOTCRepository.RemoveRange(oldOtc);
                            }
                            //add
                            _masterActualJobHeaderOTCRepository.CreateRange(otcList);
                            //log
                            foreach (var jobGuid in otcList.Select(o=> o.MasterActualJobHeader_Guid).Distinct())
                            {
                                CreateMessageInHistory(jobGuid, string.Empty);
                            }
                        }
                    }
                    #endregion

                    _uow.Commit();
                    tran.Complete();
                }
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result = _systemMessageRepository.FindByMsgId(-186, Guid.Empty).ConvertToMessageView();
            }
            return result;
        }
        
        public void CreateMessageInHistory(Guid masterActualJobHeaderGuid, string Error_Msg)
        {
            TblMasterHistory_ActualJob insertHistory = new TblMasterHistory_ActualJob();
            insertHistory.Guid = Guid.NewGuid();
            insertHistory.MasterActualJobHeader_Guid = masterActualJobHeaderGuid;
            insertHistory.UserCreated = _baseRequest.Data.UserName;
            insertHistory.DatetimeCreated = _baseRequest.Data.LocalClientDateTime;
            insertHistory.UniversalDatetimeCreated = _baseRequest.Data.UniversalDatetime;

            if (string.IsNullOrEmpty(Error_Msg))
            {
                insertHistory.MsgID = 2060; //Create JobHeader_OTC success
            }
            else
            {
                insertHistory.MsgID = -2081; //Cannot create JobHeader_OTC: Error_Msg
                insertHistory.MsgParameter = Error_Msg;
            }
            _masterHistoryActualJobRepository.Create(insertHistory);
        }

        /// <summary>
        /// => TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> UpdateJobHeaderOTCAfterSave
        /// </summary>
        /// <param name="JobOtcRequest"></param>
        /// <returns></returns>
        public SystemMessageView UpdateJobHeaderOTCAfterSave(JobOtcRequest JobOtcRequest)
        {
            SystemMessageView response = null;
            //ทำเลยไม่ต้องเช็ค
            var legsJobdetail = _masterActualJobServiceStopLegsRepository.GetJobDetailForUpdateOtc(JobOtcRequest.JobHeadGuid).Select(s => new GetJobListByRunRequest
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = s.JobGuid,
                MasterSite_Guid = JobOtcRequest.MasterSite_Guid,
                LocationName = s.CustomerLocateionName,
                LegGuid = s.JobLegGuid
            });
            if (legsJobdetail.Any())
            {
                JobOtcRequest.JobListRequest = legsJobdetail;
                response = _otcManagementService.GetOtcRequestToUpdateOtcTransactionForOcean(JobOtcRequest);
            }
            else
            {
                response = _systemMessageRepository.FindByMsgId(0, JobOtcRequest.UserLangquege).ConvertToMessageView(true);
            }

            return response;
        }

        /// <summary>
        /// OTC Support Cash add 
        /// Otc per job leg.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private void CreateActualJobHeaderOTCSupportCashAdd(CreateJobHeaderOtcRequest request)
        {
            var asso = _sfoMasterMachineRepository.GetAssociateMachine(request.CusLocationGuid);
            var deleteOldOtcHeader = _masterActualJobHeaderOTCRepository.FindByJob(request.MasterActualJobHeaderGuid);
            _masterActualJobHeaderOTCRepository.RemoveRange(deleteOldOtcHeader);
            var countryHandle = GetCountryByCustomerLocation(request.CusLocationGuid);
            var mainMachine = _sfoMasterMachineRepository
                .FindAll(f => f.Guid == request.CusLocationGuid)
                .Union(asso)
                .Select(s => s.Guid).ToList();
            var data = _sfoMasterMachineLockTypeRepository.FindAll(e => mainMachine.Contains(e.SFOMasterMachine_Guid))
              .Join(GetMachineOTCType(), mc => mc.SFOTblMasterMachine.SystemMachineType_Guid, o => o.Guid, (mc, o) => new TblMasterActualJobHeader_OTC
              {
                  Guid = Guid.NewGuid(),
                  MasterActualJobHeader_Guid = request.MasterActualJobHeaderGuid,
                  MasterActualJobServiceStopLegs_Guid = request.MaserActualJobLegGuid,
                  Lock = GetLockSerailNumber(mc.SFOTblSystemLockType.LockTypeID, mc.SerailNumber, mc.ReferenceCode, mc.MachineLockID), //mc.SerailNumber,//get from 
                  Country = countryHandle.MasterCountryAbbreviation,// get from customer
                  LockIndex = mc.LockSeq.GetValueOrDefault() - 1,
                  LockMode = request.OtcLockMode,
                  LockUser = request.OtcLockUser,
                  Branch = request.OtcBranchName,
                  LockType = GetLockType(mc.SFOTblSystemLockType.LockTypeID),
                  MachineType = o.OtcTypeId,
                  CombinationCode = mc.SFOTblSystemLockType.LockTypeID.Equals(LockTypeID.SpinDial) ? mc.CombinationCode : ""
              });
            _masterActualJobHeaderOTCRepository.CreateRange(data);
            /*=========== Set flag FlagUseTranferSafe ============ */
            var updateJobHead = _masterActualJobHeaderRepository.FindById(request.MasterActualJobHeaderGuid);
            updateJobHead.FlagUseTranferSafe = _sfoMasterMachineRepository.CheckAssociateMachine(request.CusLocationGuid);

            CreateMessageInHistory(request.MasterActualJobHeaderGuid, "");
        }
        private string GetLockType(string strLock)
        {
            string result = "";
            switch (strLock)
            {
                case LockTypeID.SG: { result = LockType.SG; break; }
                case LockTypeID.Cencon: { result = LockType.Cencon; break; }
                case LockTypeID.SpinDial: { result = LockType.SpinDial; break; }
                case LockTypeID.BrinksBox: { result = LockType.BrinksBox; break; }
            }
            return result;
        }
        private string GetLockSerailNumber(string strLock, string serailNumber, string referenceCode, string machineLockID)
        {
            string result = "";
            switch (strLock)
            {
                case LockTypeID.SG: { result = machineLockID; break; }
                case LockTypeID.Cencon: { result = serailNumber; break; }
                case LockTypeID.SpinDial: { result = referenceCode; break; }
                case LockTypeID.BrinksBox: { result = serailNumber; break; }
            }
            return result;
        }
        private TblMasterCountry GetCountryByCustomerLocation(Guid guidCustLoc)
        {
            var cusLo = _masterCustomerLocationRepository.FindById(guidCustLoc);
            var countryGuid = _masterCustomerRepository.FindById(cusLo.MasterCustomer_Guid)?.MasterCountry_Guid;
            return _masterCountryRepository.FindById(countryGuid);
        }
        private IEnumerable<OTCCustomerLocationTypeResult> GetMachineOTCType()
        {
            var dic = new Dictionary<int, int>()
            {
                [(int)CustomerLocationType.ATM_Machine] = 0,
                [(int)CustomerLocationType.CompuSafe_Machine] = 0,
                [(int)CustomerLocationType.Transfer_Safe] = 1,
                [(int)CustomerLocationType.Key_Safe] = 2
            };
            var machineOtc = _systemCustomerLocationTypeRepository.FindAll(f => f.CustomerLocationTypeID.HasValue);
            var result = machineOtc.Join(dic, m => m.CustomerLocationTypeID, d => d.Key, (m, d) => new OTCCustomerLocationTypeResult
            {
                Guid = m.Guid,
                CustomerLocationTypeId = m.CustomerLocationTypeID.Value,
                OtcTypeId = d.Value.ToString()

            });
            return result;

        }
        private string GetLockModeByServiceJobTypeId(int serviceJobTypeID, Guid? countryGuid)
        {
            string lockMode = "R";
            var lockTb = _sfoMasterOTCLockModeRepository.FindAll(f => f.TblSystemServiceJobType.ServiceJobTypeID == serviceJobTypeID && f.SFOTblSystemOTCLockMode.MasterCountry_Guid == countryGuid).ToList();
            if (lockTb.Any())
            {
                lockMode = lockTb.FirstOrDefault().SFOTblSystemOTCLockMode.LockMode;
            }
            return lockMode;
        }
    }
}

