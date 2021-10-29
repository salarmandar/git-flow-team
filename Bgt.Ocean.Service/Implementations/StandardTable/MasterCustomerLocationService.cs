using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.StringQuery.StandardTable;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.CustomerLocationService;
using Bgt.Ocean.Service.ModelViews.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{
    public interface IMasterCustomerLocationService
    {
        IEnumerable<AdhocCustomerLocationView> GetAdhocCustomerLocation(AdhocCustomerLocationRequest request);

        Task<List<LocationEmailActionView>> GetLocationsForEmailAction(LocationEmailRequest request);
        Task<List<LocationEmailActionView>> GetLocationsForEmailActionByCustomerGuid(Guid customerGuid);
        SystemMessageView SaveLocationEmailAction(IEnumerable<LocationEmailActionView> request);
        ChangeCustomerview GetChangeCustomer(Guid locationGuid);
        SystemMessageView SaveChangeCustomer(Guid locationGuid, Guid customerGuid);
        SystemMessageView ValidateBeForeSave(Guid locationGuid, Guid customerGuid);
    }
    public class MasterCustomerLocationService : IMasterCustomerLocationService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterCustomerLocation_EmailActionRepository _masterCustomerLocation_EmailAction;

        private readonly ISystemService _systemService;
        private readonly IBaseRequest _baseRequest;
        private readonly ISFOMasterMachineRepository _sfoMasterMachineRepository;

        public MasterCustomerLocationService(
            IUnitOfWork<OceanDbEntities> uow,
            ISystemMessageRepository systemMessageRepository,
            IMasterCustomerRepository masterCustomerRepository,
            IMasterCustomerLocationRepository masterCustomerLocationRepository,
            IMasterCustomerLocation_EmailActionRepository masterCustomerLocation_EmailAction,
            ISFOMasterMachineRepository sfoMasterMachineRepository,
            ISystemService systemService,
            IBaseRequest baseRequest

        )
        {
            _uow = uow;
            _systemMessageRepository = systemMessageRepository;
            _masterCustomerRepository = masterCustomerRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _masterCustomerLocation_EmailAction = masterCustomerLocation_EmailAction;
            _sfoMasterMachineRepository = sfoMasterMachineRepository;
            _systemService = systemService;
            _baseRequest = baseRequest;
        }

        public IEnumerable<AdhocCustomerLocationView> GetAdhocCustomerLocation(AdhocCustomerLocationRequest request)
        {
            var WorkDate = request.strWorkDate.ChangeFromStringToDate(request.DateTimeFormat);
            //DateTime? WorkDate = null; //These are not in use. Old version wanted to check for service hour
            Guid? DayOfWeek = null;
            return _masterCustomerLocationRepository.Func_AdhocLocationByCustomer_Get(request.CustomerGuid, request.SiteGuid, DayOfWeek, request.JobType_Guid, request.LineOfBusiness_Guid, WorkDate, request.FlagDestination, request.CustomerLocaitonPK, request.SiteGuid_Del, request.subServiceType_Guid).ConvertToAdhocCustomerLocationView().OrderBy(o => o.BranchName);
        }


        public async Task<List<LocationEmailActionView>> GetLocationsForEmailAction(LocationEmailRequest request)
        {
            return await _masterCustomerLocationRepository.GetLocationsForEmailAction(request);
        }

        public async Task<List<LocationEmailActionView>> GetLocationsForEmailActionByCustomerGuid(Guid customerGuid)
        {
            return await _masterCustomerLocationRepository.GetLocationsForEmailActionByCustomerGuid(customerGuid);
        }

        public SystemMessageView SaveLocationEmailAction(IEnumerable<LocationEmailActionView> request)
        {
            var saveData = request.Select(o
                            => new TblMasterCustomerLocation_EmailAction
                            {
                                Guid = o.Guid.HasValue ? o.Guid.Value : Guid.NewGuid(),
                                MasterCustomerLocation_Guid = o.LocationGuid,
                                SystemEmailAction_Guid = o.EmailActionGuid,
                                Email = o.Email
                            }).ToList();

            try
            {
                using (var transcope = _uow.BeginTransaction())
                {
                    var oldData = _masterCustomerLocation_EmailAction.GetLocationsEmail(saveData.Select(o => o.MasterCustomerLocation_Guid).Distinct());
                    if (oldData.Any())
                    {
                        _masterCustomerLocation_EmailAction.RemoveRange(oldData);
                    }

                    _masterCustomerLocation_EmailAction.CreateRange(saveData);
                    _uow.Commit();
                    transcope.Complete();
                }
                return new SystemMessageView { IsSuccess = true };
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                return _systemMessageRepository.FindByMsgId(-184, ApiSession.UserLanguage_Guid.GetValueOrDefault()).ConvertToMessageView();
            }
        }

        public ChangeCustomerview GetChangeCustomer(Guid locationGuid)
        {
            ChangeCustomerview result = null;
            var location = _masterCustomerLocationRepository.FindById(locationGuid);
            var currentCustomer = _masterCustomerRepository.FindById(location.MasterCustomer_Guid);
            var customerNew = _masterCustomerRepository.FindAllAsQueryable()
                       .Where(w => w.FlagDisable == false
                                   && w.FlagChkCustomer == true
                                   && w.MasterCountry_Guid == currentCustomer.MasterCountry_Guid
                                   && w.Guid != currentCustomer.Guid)
                       .Select(s => new CustomerView
                       {
                           Guid = s.Guid,
                           CustomerName = s.CustomerFullName
                       });
            result = new ChangeCustomerview()
            {
                LocationGuid = location.Guid,
                LocationName = location.BranchName,
                CurrentCustomerGuid = currentCustomer.Guid,
                CurrentCustomerName = currentCustomer.CustomerFullName,
                NewCustomer = customerNew
            };

            return result;
        }
        public SystemMessageView ValidateBeForeSave(Guid locationGuid, Guid customerGuid)
        {
            SystemMessageView validate = null;
            var currentLocation = _masterCustomerLocationRepository.FindById(locationGuid);

            var validateDuplicateLocation = _masterCustomerLocationRepository.GetLocationByCustomer(customerGuid).Any(a => a.BranchName == currentLocation.BranchName);
            if (validateDuplicateLocation)
            {
                var customerName = _masterCustomerRepository.FindById(customerGuid).CustomerFullName;
                validate = _systemMessageRepository.FindByMsgId(-17358, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                validate.IsWarning = true;
                validate.MessageTextContent = string.Format(validate.MessageTextContent, currentLocation.BranchName, customerName);
                return validate;
            }
            var machnie = _sfoMasterMachineRepository.FindById(locationGuid);
            if (machnie != null)
            {
                var validateMachineId = _sfoMasterMachineRepository.CheckDuplicateCustomerMachineIdByCustomer(customerGuid, machnie.CustomerMachineID);
                if (validateMachineId)
                {
                    var customerName = _masterCustomerRepository.FindById(customerGuid).CustomerFullName;
                    validate = _systemMessageRepository.FindByMsgId(-17359, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                    validate.IsWarning = true;
                    validate.MessageTextContent = string.Format(validate.MessageTextContent, machnie.CustomerMachineID,currentLocation.BranchName, customerName);
                    return validate;
                }
            }
            return validate;
        }
        public SystemMessageView SaveChangeCustomer(Guid locationGuid, Guid customerGuid)
        {
            SystemMessageView response = null;
            try
            {
                CustomerLocationQuery query = new CustomerLocationQuery();
                var userData = _baseRequest.Data;
                var querStr = query.GetQueryUpdateChangedCustomer;
                var dateTime = userData.ClientDateTime;
                var utcDate = DateTime.UtcNow;            
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@LocationGuid", locationGuid);
                param.Add("@NewCustomerGuid", customerGuid);
                param.Add("@UserCreated", userData.UserName);
                param.Add("@ClientDate", dateTime);
                param.Add("@UtcDate", utcDate);
                var executeResult = _masterCustomerLocationRepository.ExectueQuery<int>(querStr, param);
                if (executeResult.Any(a => a == 0))
                {
                    response = _systemMessageRepository.FindByMsgId(0, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true);
                }
                else
                {
                    response = _systemMessageRepository.FindByMsgId(-186, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                }

            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response = _systemMessageRepository.FindByMsgId(-186, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
            }
            return response;
        }
    }
}
