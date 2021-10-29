using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Helpers;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Bgt.Ocean.Service.Implementations.FleetMaintenance
{
    public interface IFleetGasolineService
    {
        IEnumerable<DropdownViewModel<FleetGasolineVendorView>> GetGasolineVendorList(Guid? brinkCompanyGuid);
        IEnumerable<DropdownViewModel<FleetGasolineOperatorView>> GetGasolineOperatorList();
        FleetGasolineResponse GetGasolineInfoList(FleetGasolineRequest request);
        SystemMessageView CreateRunResourceGasoline(FleetGasolineDataRequest request);
        SystemMessageView UpdateRunResourceGasoline(FleetGasolineDataRequest request);
        SystemMessageView DisableRunResourceGasoline(FleetGasolineDisableRequest request);
        IEnumerable<DropdownViewModel<FleetGasolineTypeView>> GetGasolineTypeList(Guid gasolineVendorGuid);
        FleetGasolineDataResponse GetGasolineDetail(Guid runGasolineGuid);
        FleetGasolineVendorDefaultView GetGasolineVendorDefault(Guid? brinkCompanyGuid);
    }

    public class FleetGasolineService : IFleetGasolineService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterGasolineVendorRepository _masterGasolineVendorRepository;
        private readonly IMasterRunResourceGasolineExpenseRepository _masterRunResourceGasolineExpenseRepository;
        private readonly IMasterGasolineRepository _masterGasolineRepository;
        private readonly IMasterCurrencyRepository _masterCurrencyRepository;
        private readonly ISystemGlobalUnitRepository _systemGlobalUnitRepository;
        private readonly IBaseRequest _baseRequest;

        public FleetGasolineService(
            IUnitOfWork<OceanDbEntities> uow,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IMasterGasolineVendorRepository masterGasolineVendorRepository,
            IMasterRunResourceGasolineExpenseRepository masterRunResourceGasolineExpenseRepository,
            IMasterGasolineRepository masterGasolineRepository,
            IMasterCurrencyRepository masterCurrencyRepository,
            ISystemGlobalUnitRepository systemGlobalUnitRepository,
            IBaseRequest baseRequest
            )
        {
            _uow = uow;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _masterGasolineVendorRepository = masterGasolineVendorRepository;
            _masterRunResourceGasolineExpenseRepository = masterRunResourceGasolineExpenseRepository;
            _masterGasolineRepository = masterGasolineRepository;
            _masterCurrencyRepository = masterCurrencyRepository;
            _systemGlobalUnitRepository = systemGlobalUnitRepository;
            _baseRequest = baseRequest;
        }

        public IEnumerable<DropdownViewModel<FleetGasolineVendorView>> GetGasolineVendorList(Guid? brinkCompanyGuid)
        {
            if (brinkCompanyGuid.HasValue)
            {
                return _masterGasolineVendorRepository.GetGasolineVendorByBrinkCompany(brinkCompanyGuid.Value)
                 .Select(e => new DropdownViewModel<FleetGasolineVendorView>()
                 {
                     Value = e.Guid.ToString(),
                     Text = e.GasolineVendorName
                 }).OrderBy(o => o.Text);
            }
            return Enumerable.Empty<DropdownViewModel<FleetGasolineVendorView>>();
        }
        
        public IEnumerable<DropdownViewModel<FleetGasolineOperatorView>> GetGasolineOperatorList()
        {
            var operatorlist = Enum.GetValues(typeof(EnumFleetOperator))
                               .Cast<EnumFleetOperator>()
                               .Select(e => new DropdownViewModel<FleetGasolineOperatorView>()
                               {
                                   Value = ((int)e).ToString(),
                                   Text = EnumHelper.GetDescription(e)
                               }).OrderBy(o => o.Value);
            return operatorlist;
        }

        public IEnumerable<DropdownViewModel<FleetGasolineTypeView>> GetGasolineTypeList(Guid gasolineVendorGuid)
        {
            return _masterGasolineRepository.FindAllAsQueryable(x => x.MasterGasloineVendor_Guid == gasolineVendorGuid && x.FlagDisable != true)
                 .Select(e => new DropdownViewModel<FleetGasolineTypeView>()
                 {
                     Value = e.Guid.ToString(),
                     Text = e.GasolineName
                 }).OrderBy(o => o.Text);
        }

        #region Get default
        public FleetGasolineVendorDefaultView GetGasolineVendorDefault(Guid? brinkCompanyGuid)
        {
            var result = new FleetGasolineVendorDefaultView();
            Guid? gasolineVendorGuid = _masterGasolineVendorRepository.GetGasolineVendorDefaultByBrinkCompany(brinkCompanyGuid.Value)?.Guid;
            if (gasolineVendorGuid != null)
            {
                var gasolineType = _masterGasolineRepository.FindOne(x => x.MasterGasloineVendor_Guid == (Guid)gasolineVendorGuid && x.FlagDisable != true && x.FlagDefault);
                result.GasolineVendorGuid = gasolineVendorGuid;
                result.GasolineTypeGuid = gasolineType?.Guid;
                result.UnitPrice = gasolineType?.UnitPrice;
            }
            return result;
        }
        #endregion

        public FleetGasolineResponse GetGasolineInfoList(FleetGasolineRequest request)
        {
            FleetGasolineResponse response = new FleetGasolineResponse();
            string docRef = request.DocumentRef.Trim().ToLower();
            try
            {
                IEnumerable<TblMasterRunResource_GasolineExpense> runResourceGasoline =
                _masterRunResourceGasolineExpenseRepository.GetGasolineInfoList(request.MasterRunResource_Guid, request.MasterSite_Guid)
                .Where(x => ((!request.flagShowAll && !(bool)x.FlagDisable) || request.flagShowAll)
                            && (x.TopUpDate.Date >= request.DateFrom.Date && x.TopUpDate.Date <= request.DateTo.Date)
                            && ((request.CurrencyUnit_Guid.HasValue && x.CurrencyUnit_Guid == request.CurrencyUnit_Guid) || !request.CurrencyUnit_Guid.HasValue)
                            && (((!string.IsNullOrEmpty(docRef) && !string.IsNullOrEmpty(x.DocumentRef)) 
                                && x.DocumentRef.ToLower().Contains(docRef)) || string.IsNullOrEmpty(docRef))
                             );

                IEnumerable<Guid> gasolineGuids = runResourceGasoline.Select(x => x.MasterGasloine_Guid).Distinct();
                IEnumerable<TblMasterGasloine> gasoline = _masterGasolineRepository.FindAllAsQueryable(x => gasolineGuids.Contains(x.Guid) &&((request.GasolineVendorGuid.HasValue && x.MasterGasloineVendor_Guid == request.GasolineVendorGuid) || !request.GasolineVendorGuid.HasValue));

                if (request.TopUpAmount.HasValue)
                {
                    string strOperator = EnumHelper.GetDescription(request.Operator);
                    runResourceGasoline = runResourceGasoline.AsQueryable()
                        .Where($"o => o.TopUpAmount {strOperator} {request.TopUpAmount}")
                        .ToDynamicList<TblMasterRunResource_GasolineExpense>();
                }

                var pagingBase = (PagingBase)request;
                response.FleetGasolineList = runResourceGasoline
                             .Join(gasoline,
                             run => run.MasterGasloine_Guid,
                             gas => gas.Guid,
                             (run, gas) => new { run, gas })
                             .Join(_masterGasolineVendorRepository.FindAllAsQueryable(),
                             rg => rg.gas.MasterGasloineVendor_Guid,
                             ven => ven.Guid,
                             (rg, ven) => new { rg, ven })
                             .Join(_masterCurrencyRepository.FindAllAsQueryable(),
                             rgv => rgv.rg.run.CurrencyAmount_Guid,
                             cur => cur.Guid,
                             (rgv, cur) => new { rgv, cur })
                             .Join(_systemGlobalUnitRepository.FindAllAsQueryable(),
                             rgvc => rgvc.rgv.rg.run.TopupQtyUnit_Guid,
                             unit => unit.Guid,
                             (rgvc, unit) => new FleetGasolineView()
                             {
                                 RunResourceGasolineGuid = rgvc.rgv.rg.run.Guid,
                                 TopUpDate = rgvc.rgv.rg.run.TopUpDate,
                                 TopUpAmount = rgvc.rgv.rg.run.TopUpAmount,
                                 GasolineVendorName = rgvc.rgv.ven.GasolineVendorName,
                                 GasolineName = rgvc.rgv.rg.gas.GasolineName,
                                 CurrencyAmount = rgvc.cur.MasterCurrencyAbbreviation,
                                 UserCreated = rgvc.rgv.rg.run.UserCreated,
                                 DatetimeCreated = rgvc.rgv.rg.run.DatetimeCreated,
                                 UserModifed = rgvc.rgv.rg.run.UserModifed,
                                 DatetimeModified = rgvc.rgv.rg.run.DatetimeModified,
                                 FlagDisable = rgvc.rgv.rg.run.FlagDisable.GetValueOrDefault(),
                                 TopUpQty = rgvc.rgv.rg.run.TopUpQty,
                                 TopUpQtyUnit = unit.UnitName,

                             }).Take(pagingBase.MaxRow).ToList();

                var result = PaginationHelper.ToPagination(response.FleetGasolineList, pagingBase);
                response.FleetGasolineList = result.Data;
                response.Total = result.Total;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _baseRequest.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return response;
        }

        public SystemMessageView CreateRunResourceGasoline(FleetGasolineDataRequest request)
        {
            Guid LanguageGuid = _baseRequest.Data.UserLanguageGuid.GetValueOrDefault();
            SystemMessageView responseMsg = null;
            using (var trans = _uow.BeginTransaction())
            {
                try
                {
                    var gasolineInsert = request.ConvertToTblMasterRunResourceGasoline();
                    gasolineInsert.UserCreated = request.UserName;
                    gasolineInsert.DatetimeCreated = request.LocalClientDateTime;
                    gasolineInsert.UniversalDatetimeCreated = request.UniversalDatetime;
                    _masterRunResourceGasolineExpenseRepository.Create(gasolineInsert);

                    responseMsg = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView();
                    _uow.Commit();
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
                }
            }
            return responseMsg;
        }

        public SystemMessageView UpdateRunResourceGasoline(FleetGasolineDataRequest request)
        {
            Guid LanguageGuid = _baseRequest.Data.UserLanguageGuid.GetValueOrDefault();
            SystemMessageView responseMsg = null;
            using (var trans = _uow.BeginTransaction())
            {
                try
                {
                    TblMasterRunResource_GasolineExpense gasolineUpdate = _masterRunResourceGasolineExpenseRepository.FindById(request.RunResourceGasolineGuid);
                    if (gasolineUpdate != null)
                    {
                        gasolineUpdate.MasterSite_Guid = request.MasterSite_Guid;
                        gasolineUpdate.TopUpDate = request.TopUpDate;
                        gasolineUpdate.TopUpAmount = request.TopUpAmount;
                        gasolineUpdate.CurrencyAmount_Guid = request.CurrencyUnit_Guid;  //It's equal currency unit
                        gasolineUpdate.TopUpQty = request.TopUpQty;
                        gasolineUpdate.Unit_Price = request.Unit_Price;
                        gasolineUpdate.CurrencyUnit_Guid = request.CurrencyUnit_Guid;
                        gasolineUpdate.MasterGasloine_Guid = request.MasterGasloine_Guid;
                        gasolineUpdate.DocumentRef = request.DocumentRef;
                        gasolineUpdate.OdoMeter = request.OdoMeter;
                        gasolineUpdate.TopupQtyUnit_Guid = request.TopupQtyUnit_Guid;
                        gasolineUpdate.UserModifed = request.UserName;
                        gasolineUpdate.DatetimeModified = request.LocalClientDateTime;
                        gasolineUpdate.UniversalDatetimeModified = request.UniversalDatetime;

                        _masterRunResourceGasolineExpenseRepository.Modify(gasolineUpdate);
                    }

                    responseMsg = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView();
                    _uow.Commit();
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
                }
            }
            return responseMsg;
        }

        public SystemMessageView DisableRunResourceGasoline(FleetGasolineDisableRequest request)
        {
            Guid LanguageGuid = _baseRequest.Data.UserLanguageGuid.GetValueOrDefault();
            SystemMessageView responseMsg = null;
            using (var trans = _uow.BeginTransaction())
            {
                try
                {
                    TblMasterRunResource_GasolineExpense gasolineUpdate = _masterRunResourceGasolineExpenseRepository.FindById(request.RunResourceGasolineGuid);
                    if (gasolineUpdate != null)
                    {
                        gasolineUpdate.FlagDisable = request.IsDisable;
                        gasolineUpdate.UserModifed = request.UserName;
                        gasolineUpdate.DatetimeModified = request.LocalClientDateTime;
                        gasolineUpdate.UniversalDatetimeModified = request.UniversalDatetime;

                        _masterRunResourceGasolineExpenseRepository.Modify(gasolineUpdate);
                    }

                    responseMsg = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView();
                    _uow.Commit();
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    responseMsg = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
                }
            }
            return responseMsg;
        }

        public FleetGasolineDataResponse GetGasolineDetail(Guid runGasolineGuid)
        {
            TblMasterRunResource_GasolineExpense data = _masterRunResourceGasolineExpenseRepository.FindById(runGasolineGuid);
            var result = new FleetGasolineDataResponse();
            try
            {
                if (data != null)
                {
                    TblMasterGasloine fuelType = _masterGasolineRepository.FindById(data.MasterGasloine_Guid);
                    Guid fuelStationGuid = _masterGasolineVendorRepository.FindById(fuelType.MasterGasloineVendor_Guid).Guid;

                    result.RunResourceGasolineGuid = data.Guid;
                    result.StrTopUpDate = data.TopUpDate.ChangeFromDateToString(_baseRequest.Data.UserFormatDate);
                    result.StrTopUpTime = data.TopUpDate.ChangeFromDateToTimeString();
                    result.OdoMeter = data.OdoMeter;
                    result.MasterGasloine_Guid = data.MasterGasloine_Guid;
                    result.FuelStationGuid = fuelStationGuid;
                    result.DocumentRef = data.DocumentRef;
                    result.Unit_Price = data.Unit_Price;
                    result.CurrencyUnit_Guid = data.CurrencyUnit_Guid;
                    result.TopUpQty = data.TopUpQty;
                    result.TopupQtyUnit_Guid = data.TopupQtyUnit_Guid;
                    result.TopUpAmount = data.TopUpAmount;
                    result.CurrencyAmount_Guid = data.CurrencyAmount_Guid;
                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
    }
}