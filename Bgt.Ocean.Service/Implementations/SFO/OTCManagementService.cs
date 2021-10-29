using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Models.OTCManagement;
using Bgt.Ocean.Service.Messagings.OTCManagement;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.ModelViews.RunControls;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    public interface IOTCManagementService
    {
        IEnumerable<SFOJobListByRunResult> GetJobListByRun(JobListByRunRequest jobHeaderGuidList);
        IEnumerable<SFOOTCRequestResult> Ocean_GetJobListByRun(JobListByRunRequest jobHeaderGuidList);
        SystemMessageView GetOtcRequestToUpdateOtcTransactionForOcean(JobOtcRequest request);
    }
    public class OTCManagementService : IOTCManagementService
    {
        private readonly ISFOTblTransactionOTCRepository _sfoTblTransactionOTCRepository;
        private readonly IMasterActualJobHeaderOTCRepository _masterActualJobHeaderOTCRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uowOcean;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        public OTCManagementService(
            ISFOTblTransactionOTCRepository sfoTblTransactionOTCRepository,
            IMasterActualJobHeaderOTCRepository masterActualJobHeaderOTCRepository,
            IUnitOfWork<OceanDbEntities> uowOcean,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository
            )
        {
            _sfoTblTransactionOTCRepository = sfoTblTransactionOTCRepository;
            _masterActualJobHeaderOTCRepository = masterActualJobHeaderOTCRepository;
            _uowOcean = uowOcean;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;

        }

        public IEnumerable<SFOJobListByRunResult> GetJobListByRun(JobListByRunRequest jobHeaderGuidList)
        {
            var result = _sfoTblTransactionOTCRepository.Func_SFO_JobListByRun_Get(jobHeaderGuidList.JobListRequest.Select(e => new GetJobListByRunModel
            {
                Guid = Guid.NewGuid(),
                MasterSite_Guid = e.MasterSite_Guid,
                MasterActualJobHeader_Guid = e.MasterActualJobHeader_Guid,
                JobNo = e.JobNo,
                LOBAbbrevaitionName = e.LOBAbbrevaitionName,
                ServiceJobTypeNameAbb = e.ServiceJobTypeNameAbb,
                LocationName = e.LocationName
            }));
            result = result.Select(o =>
            {
                o.LastGeneratedStr = o.LastGenerated.HasValue ? o.LastGenerated.ChangeFromDateTimeOffsetToDate().Value.AddHours(Convert.ToDouble(o.LastGenerated.Value.Offset.Hours)).ChangeFromDateToString(jobHeaderGuidList.DateFormat + " HH:mm:ss") : string.Empty;
                o.ExpiredDateStr = o.ExpiredDate.ChangeFromDateTimeOffsetToDate().ChangeFromDateToString(jobHeaderGuidList.DateFormat + " HH:mm:ss");
                return o;
            });
            return result;
        }


        public IEnumerable<SFOOTCRequestResult> Ocean_GetJobListByRun(JobListByRunRequest jobHeaderGuidList)
        {
            var resultSp = _sfoTblTransactionOTCRepository.Func_SFO_JobListByRun_Get(jobHeaderGuidList.JobListRequest.Select(e => new GetJobListByRunModel
            {
                Guid = Guid.NewGuid(),
                MasterSite_Guid = e.MasterSite_Guid,
                MasterActualJobHeader_Guid = e.MasterActualJobHeader_Guid,
                JobNo = e.JobNo,
                LOBAbbrevaitionName = e.LOBAbbrevaitionName,
                ServiceJobTypeNameAbb = e.ServiceJobTypeNameAbb,
                LocationName = e.LocationName
            })).Select(o =>
            {
                o.LastGeneratedStr = o.LastGenerated.HasValue ? o.LastGenerated.ChangeFromDateTimeOffsetToDate().Value.AddHours(Convert.ToDouble(o.LastGenerated.Value.Offset.Hours)).ChangeFromDateToString(jobHeaderGuidList.DateFormat + " HH:mm:ss") : string.Empty;
                o.ExpiredDateStr = o.ExpiredDate.ChangeFromDateTimeOffsetToDate().ChangeFromDateToString(jobHeaderGuidList.DateFormat + " HH:mm:ss");
                return o;
            }).ToList();
            var otcRequest = resultSp.Select(e => new GetOTCRequestModel
            {
                MasterActualJobHeader_Guid = e.MasterActualJobHeader_Guid,
                MachineLockID = e.MachineLockID,
                SerialNumber = e.SerialNumber,
                ReferenceCode = e.ReferenceCode,
                MasterEmployee_Guid = e.MasterEmployee_Guid,
                MasterEmployee_Guid2 = e.MasterEmployee_Guid2,
                LockMode = e.LockMode,
                Date = e.WorkDate.ChangeFromStringToDate(jobHeaderGuidList.DateFormat),
                Hour = 0, // Change to get from ui in the future
                TimeBlock = 24, // Change to get from ui in the future

            }).ToList();
            return _sfoTblTransactionOTCRepository.Func_SFO_GetOTCRequest_Get_Result(otcRequest, Guid.NewGuid());
        }

        /// <summary>
        /// => TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> GetOtcRequestToUpdateOtcTransactionForOcean
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SystemMessageView GetOtcRequestToUpdateOtcTransactionForOcean(JobOtcRequest request)
        {
            SystemMessageView response = null;
            try
            {

                JobListByRunRequest jobHeaderList = new JobListByRunRequest()
                {
                    DateFormat = request.UserDateFormat,
                    JobListRequest = request.JobListRequest
                };

                // Get job detail with assigned employee
                var jobDetail = this.GetJobListByRun(jobHeaderList);

                // Prepair data to get otc request list
                var otcRequest = jobDetail.Select(e => new GetOTCRequestModel()
                {
                    MasterActualJobHeader_Guid = e.MasterActualJobHeader_Guid,
                    MachineLockID = e.MachineLockID,
                    SerialNumber = e.SerialNumber,
                    ReferenceCode = e.ReferenceCode,
                    MasterEmployee_Guid = e.MasterEmployee_Guid,
                    MasterEmployee_Guid2 = e.MasterEmployee_Guid2,
                    LockMode = e.LockMode,
                    Date = e.WorkDate.ChangeFromStringToDate(request.UserDateFormat),
                    Hour = 0, // Change to get from ui in the future
                    TimeBlock = 24, // Change to get from ui in the future
                });

                // Get otc request list
                var otcRequestResult = _sfoTblTransactionOTCRepository.Func_SFO_GetOTCRequest_Get_Result(otcRequest, Guid.NewGuid());

                foreach (var otcItem in otcRequestResult.Where(w => w.LockTypeName == EnumOTC.LockType.Cencon)) // Loop in case of multi lock
                {
                    var legGuid = request.JobListRequest.FirstOrDefault(o => o.MasterActualJobHeader_Guid == otcItem.MasterActualJobHeader_Guid)?.LegGuid;

                    var modelJobHeaderOtc = _masterActualJobHeaderOTCRepository.FindAll
                    (
                        e => e.MasterActualJobHeader_Guid == otcItem.MasterActualJobHeader_Guid // Same job
                        && e.LockType == otcItem.LockTypeName // Same lock type
                        && e.Lock == otcItem.SerialNumber // Same lock serial number
                    ).FirstOrDefault();

                    if (modelJobHeaderOtc != null) // Already has transaction row
                    {
                        modelJobHeaderOtc.Country = otcItem.MasterCountryAbbreviation;
                        modelJobHeaderOtc.Branch = otcItem.Branch;
                        modelJobHeaderOtc.LockUser = otcItem.User;
                        modelJobHeaderOtc.PIN = Sha256Encryption.Decrypt(otcItem.PIN, otcItem.MasterEmployee_Guid.ToString().ToUpper());
                        modelJobHeaderOtc.LockUser2 = otcItem.User2;
                        modelJobHeaderOtc.PIN2 = Sha256Encryption.Decrypt(otcItem.PIN2, otcItem.MasterEmployee_Guid2.ToString().ToUpper());
                        modelJobHeaderOtc.MasterActualJobServiceStopLegs_Guid = legGuid;
                        _masterActualJobHeaderOTCRepository.Modify(modelJobHeaderOtc);
                    }
                }
                _uowOcean.Commit();
                response = _systemMessageRepository.FindByMsgId(0, request.UserLangquege).ConvertToMessageView(true);
            }
            catch (Exception ex)
            {

                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response = _systemMessageRepository.FindByMsgId(-184, request.UserLangquege).ConvertToMessageView(false);
            }
            return response;
        }
    }


}
