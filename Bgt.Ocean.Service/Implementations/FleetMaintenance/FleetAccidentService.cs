using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Models.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Helpers;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;

using Bgt.Ocean.Service.ModelViews.FleetMaintenanceViewModel.FleetAccidentViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.FleetMaintenance
{
    public interface IFleetAccidentService
    {
        #region Get for DDL
        IEnumerable<DropdownViewModel<VehicleTypeView>> GetRunResourceTypeDdl(Guid? customer_Guid);
        IEnumerable<DropdownViewModel<VehicleBrandView>> GetRunResourceBrandDdl(Guid? customer_Guid);
        IEnumerable<DropdownViewModel<AccidentBrinksDriverView>> GetBrinksDriverList(Guid siteGuid);
        IEnumerable<DropdownViewModel<SystemTitleNameView>> GetTitleNameList();
        #endregion

        FleetAccidentResponse GetRunResourceAccidentInfo(AccidentInfoViewRequest request);
        AccidentResponse CreateFleetMaintenanceAccident(AccidentRequest request);
        AccidentResponse UpdateFleetMaintenanceAccident(AccidentRequest request);
        AccidentResponse DisableFleetMaintenanceAccident(DisableAccidentRequest modelRequest);

        #region Get Detail For Update Data
        AccidentDetailResponse GetAccidentDetail(Guid accidentGuid);
        List<AccidentListDetailDamagedModelView> GetAccidentDamageList_Brinks(Guid accidentGuid);
        List<AccidentListDetailDamagedModelView> GetAccidentDamageList_CounterParty(Guid accidentGuid);
        IEnumerable<AccidentImageRespone> GetAccidentImageList(Guid accidentGuid);
        #endregion
    }


    public class FleetAccidentService : IFleetAccidentService
    {
        private readonly IRunResourceTypeRepository _masterRunResourceType;
        private readonly IMasterRunResourceAccidentRepository _masterRunResourceAccidentRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly IMasterAccidentListDetailDamagedRespository _masterAccidentListDetailDamagedRespository;
        private readonly IMasterAccidentImagesRepository _masterAccidentImagesRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IBaseRequest _baseRequest;
        private readonly IMasterRunResourceRepository _masterRunResourceRepository;
        private readonly IEmployeeRepository _masterEmployeeRepository;
        private readonly ISystemTitleNameRepository _systemTitleNameRepository;
        private readonly IMasterImageTempRepository _masterImageTempRepository;

        public FleetAccidentService(
            IRunResourceTypeRepository masterRunResourceType,
            IMasterRunResourceAccidentRepository masterRunResourceAccidentRepository,
            IMasterAccidentListDetailDamagedRespository masterAccidentListDetailDamagedRespository,
            IMasterAccidentImagesRepository masterAccidentImagesRepository,
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IBaseRequest baseRequest,
            IUnitOfWork<OceanDbEntities> uow,
            IMasterRunResourceRepository masterRunResourceRepository,
            IEmployeeRepository masterEmployeeRepository,
            ISystemTitleNameRepository systemTitleNameRepository,
            IMasterImageTempRepository masterImageTempRepository

           )
        {
            _masterRunResourceType = masterRunResourceType;
            _masterRunResourceAccidentRepository = masterRunResourceAccidentRepository;
            _masterAccidentListDetailDamagedRespository = masterAccidentListDetailDamagedRespository;
            _masterAccidentImagesRepository = masterAccidentImagesRepository;
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _baseRequest = baseRequest;
            _uow = uow;
            _masterRunResourceRepository = masterRunResourceRepository;
            _masterEmployeeRepository = masterEmployeeRepository;
            _systemTitleNameRepository = systemTitleNameRepository;
            _masterImageTempRepository = masterImageTempRepository;
        }
        public IEnumerable<DropdownViewModel<VehicleTypeView>> GetRunResourceTypeDdl(Guid? customer_Guid)
        {
            if (customer_Guid.HasValue)
            {
                return _masterRunResourceType.GetRunResourceTypeDdl(customer_Guid.Value)
                    .Select(e => new DropdownViewModel<VehicleTypeView>()
                    {
                        Value = e.Guid.ToString(),
                        Text = e.MasterRunResourceTypeName
                    }).OrderBy(o => o.Text);
            }
            else
            {
                return Enumerable.Empty<DropdownViewModel<VehicleTypeView>>();
            }
        }
        public IEnumerable<DropdownViewModel<VehicleBrandView>> GetRunResourceBrandDdl(Guid? customer_Guid)
        {
            return _masterRunResourceType.GetRunResourceBrandDdl(customer_Guid)
                      .Select(e => new DropdownViewModel<VehicleBrandView>()
                      {
                          Value = e.Guid.ToString(),
                          Text = e.MasterRunResourceBrandName
                      }).OrderBy(o => o.Text);
        }
        public IEnumerable<DropdownViewModel<AccidentBrinksDriverView>> GetBrinksDriverList(Guid siteGuid)
        {
            return _masterEmployeeRepository.GetEmployeeBySite(siteGuid)
                      .Select(e => new DropdownViewModel<AccidentBrinksDriverView>()
                      {
                          Value = e.Guid.ToString(),
                          Text = $"{e.FirstName} {e.LastName}"
                      }).OrderBy(o => o.Text);
        }
        public IEnumerable<DropdownViewModel<SystemTitleNameView>> GetTitleNameList()
        {
            return _systemTitleNameRepository.GetTitleNameListByLanguageGuid(_baseRequest.Data.UserLanguageGuid.GetValueOrDefault())
                .Select(e => new DropdownViewModel<SystemTitleNameView>()
                {
                    Value = e.Guid.ToString(),
                    Text = e.TitleName
                }).OrderBy(o => o.Text);
        }

        public FleetAccidentResponse GetRunResourceAccidentInfo(AccidentInfoViewRequest request)
        {
            FleetAccidentResponse resp = new FleetAccidentResponse();
            var viewData = _masterRunResourceAccidentRepository.GetRunResourceAccidentInfo(request);
            var result = PaginationHelper.ToPagination(viewData, request);
            resp.AccidentViewInfo = result.Data;
            resp.Total = result.Total;
            return resp;
        }
        public AccidentResponse CreateFleetMaintenanceAccident(AccidentRequest request)
        {
            var response = new AccidentResponse();
            var userData = _baseRequest.Data;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    request.AccidentGuid = Guid.NewGuid();
                    #region insert fleet maintenance accident
                    var insert = new TblMasterRunResource_Accident()
                    {
                        Guid = request.AccidentGuid.Value,
                        MasterSite_Guid = request.SiteGuid,
                        MasterRunResource_Guid = request.RunresourceGuid,
                        //Accident detail
                        DateOfAccident = request.AccidentDetail.DateOfAccident,
                        TimeOfAccident = request.AccidentDetail.TimeOfAccident,
                        DescriptionOfAccident = request.AccidentDetail.DescriptionOfAccident,
                        BrinksEmployee_Guid = request.AccidentDetail.EmployeeGuid,
                        FlagBrinksIsFault = request.AccidentDetail.FlagBrinksIsFault,
                        FlagPersonalInjury = request.AccidentDetail.FlagPersonalInjury,
                        ClaimExpense = request.AccidentDetail.ClaimExpense,
                        ClaimReference = request.AccidentDetail.ClaimReference,
                        Indemnity = request.AccidentDetail.Indemnity,
                        CurrencyIndemnity_Guid = request.AccidentDetail.CurrencyIndemnityGuid,
                        CurrencyClaimExpense_Guid = request.AccidentDetail.CurrencyClaimExpenseGuid,

                        //Counterparty Detail
                        SystemTitleName_Guid = request.CounterpartyDetail.TitleNameGuid,
                        Parties_FirstName = request.CounterpartyDetail.PartiesFirstName,
                        Parties_MiddleName = request.CounterpartyDetail.PartiesMiddleName,
                        Parties_LastName = request.CounterpartyDetail.PartiesLastName,
                        Parties_RegistrationID = request.CounterpartyDetail.EmployeeRegistrationID,
                        Parties_RunResourceBrand_Guid = request.CounterpartyDetail.RunResourceBrandGuid,
                        Parties_RunResourceType_Guid = request.CounterpartyDetail.RunResourceTypeGuid,
                        Parties_RunResourceModel = request.CounterpartyDetail.RunResourceModel,
                        Parties_InsuranceCompanyName = request.CounterpartyDetail.InsuranceCompanyName,
                        Parties_InsuranceReferenceIDNumber = request.CounterpartyDetail.InsuranceReferenceIDNumber,
                        Parties_InsuranceValidityDate = request.CounterpartyDetail.InsuranceValidityDate,
                        Parties_DriverLicenseID = request.CounterpartyDetail.DriverLicenseID,
                        Parties_CarRegistrationDocumentNumber = request.CounterpartyDetail.CarRegistrationDocumentNumber,
                        FlagDisable = false,
                        UserCreated = userData.UserName,
                        DatetimeCreated = userData.LocalClientDateTime,
                        UniversalDatetimeCreated = userData.UniversalDatetime
                    };
                    _masterRunResourceAccidentRepository.Create(insert);
                    #endregion

                    // Insert Brinks Details List   
                    AddListDetailDamaged(request.BrinksDetailList, request.AccidentGuid.Value, true);

                    // Insert Counter Details List
                    AddListDetailDamaged(request.CounterDetailList, request.AccidentGuid.Value, false);

                    // Insert Accident image.
                    AddAccidentImage(request.ImageList.Where(w => !w.IsActualImage), request.AccidentGuid.Value);
                    response.Msg = _systemMessageRepository.FindByMsgId(552, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true);
                    var runNo = _masterRunResourceRepository.FindById(request.RunresourceGuid);
                    response.Msg.MessageTextContent = string.Format(response.Msg.MessageTextContent, runNo.VehicleNumber);
                    response.Guid = insert.Guid;
                    _uow.Commit();
                    transection.Complete();
                    return response;
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response.Msg = _systemMessageRepository.FindByMsgId(-184, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                    return response;
                }
            }
        }

        public AccidentResponse UpdateFleetMaintenanceAccident(AccidentRequest request)
        {
            var response = new AccidentResponse();
            var userData = _baseRequest.Data;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    #region update Accident
                    var update = _masterRunResourceAccidentRepository.FindById(request.AccidentGuid);
                    if (update != null)
                    {
                        update.MasterSite_Guid = request.SiteGuid;
                        update.MasterRunResource_Guid = request.RunresourceGuid;
                        // Accident Detail
                        update.DateOfAccident = request.AccidentDetail.DateOfAccident;
                        update.TimeOfAccident = request.AccidentDetail.TimeOfAccident;
                        update.DescriptionOfAccident = request.AccidentDetail.DescriptionOfAccident;
                        update.BrinksEmployee_Guid = request.AccidentDetail.EmployeeGuid;
                        update.FlagBrinksIsFault = request.AccidentDetail.FlagBrinksIsFault;
                        update.FlagPersonalInjury = request.AccidentDetail.FlagPersonalInjury;
                        update.ClaimExpense = request.AccidentDetail.ClaimExpense;
                        update.ClaimReference = request.AccidentDetail.ClaimReference;
                        update.Indemnity = request.AccidentDetail.Indemnity;
                        update.CurrencyIndemnity_Guid = request.AccidentDetail.CurrencyIndemnityGuid;
                        update.CurrencyClaimExpense_Guid = request.AccidentDetail.CurrencyClaimExpenseGuid;

                        //CounterpartyDetail
                        update.SystemTitleName_Guid = request.CounterpartyDetail.TitleNameGuid;
                        update.Parties_FirstName = request.CounterpartyDetail.PartiesFirstName;
                        update.Parties_MiddleName = request.CounterpartyDetail.PartiesMiddleName;
                        update.Parties_LastName = request.CounterpartyDetail.PartiesLastName;
                        update.Parties_RegistrationID = request.CounterpartyDetail.EmployeeRegistrationID;
                        update.Parties_RunResourceBrand_Guid = request.CounterpartyDetail.RunResourceBrandGuid;
                        update.Parties_RunResourceType_Guid = request.CounterpartyDetail.RunResourceTypeGuid;
                        update.Parties_RunResourceModel = request.CounterpartyDetail.RunResourceModel;
                        update.Parties_InsuranceCompanyName = request.CounterpartyDetail.InsuranceCompanyName;
                        update.Parties_InsuranceReferenceIDNumber = request.CounterpartyDetail.InsuranceReferenceIDNumber;
                        update.Parties_InsuranceValidityDate = request.CounterpartyDetail.InsuranceValidityDate;
                        update.Parties_DriverLicenseID = request.CounterpartyDetail.DriverLicenseID;
                        update.Parties_CarRegistrationDocumentNumber = request.CounterpartyDetail.CarRegistrationDocumentNumber;



                        update.UserModifed = userData.UserName;
                        update.DatetimeModified = userData.LocalClientDateTime;
                        update.UniversalDatetimeModified = userData.UniversalDatetime;
                    }
                    _masterRunResourceAccidentRepository.Modify(update);

                    #endregion

                    // Update Brinks Details List                   
                    MergeListDetailDamaged(request.BrinksDetailList, request.AccidentGuid.Value, true);

                    // Update Counter Details List   
                    MergeListDetailDamaged(request.CounterDetailList, request.AccidentGuid.Value, false);

                    // Insert Images List   
                    MergeAccidentImage(request.ImageList, request.AccidentGuid.Value);

                    response.Msg = _systemMessageRepository.FindByMsgId(553, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true);
                    var runNo = _masterRunResourceRepository.FindById(request.RunresourceGuid);
                    response.Msg.MessageTextContent = string.Format(response.Msg.MessageTextContent, runNo.VehicleNumber);
                    response.Guid = request.AccidentGuid.Value;

                    _uow.Commit();
                    transection.Complete();
                    return response;
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response.Msg = _systemMessageRepository.FindByMsgId(-184, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                    return response;
                }
            }
        }
        public void MergeListDetailDamaged(List<AccidentListDetailDamagedModelView> model, Guid accidentGuid, bool IsBrink)
        {

            var allItems = _masterAccidentListDetailDamagedRespository.FindAll(x => x.MasterRunResource_Accident_Guid == accidentGuid && x.FlagItemIsBrinks == IsBrink)?.ToList();
            if (allItems.Any())
            {
                // Remove
                var itemsToDelete = allItems.Where(w => !model.Any(a => a.Guid == w.Guid && a.Guid.HasValue));
                if (itemsToDelete.Any())
                {
                    _masterAccidentListDetailDamagedRespository.RemoveRange(itemsToDelete);
                }

                //Update
                foreach (var item in model.Where(w => w.Guid.HasValue && allItems.Any(a => a.Guid == w.Guid && a.ItemsDetail != w.AccidentItemsDetail)))
                {
                    var updateItem = allItems.Find(w => w.Guid == item.Guid);
                    if (updateItem != null)
                    {
                        updateItem.ItemsDetail = item.AccidentItemsDetail;
                    }
                }
            }
            //Insert
            AddListDetailDamaged(model.Where(w => !(w.Guid.HasValue))?.ToList(), accidentGuid, IsBrink);

        }
        private void AddListDetailDamaged(List<AccidentListDetailDamagedModelView> model, Guid accidentGuid, bool IsBrink)
        {
            if (model != null)
            {

                var detail = new List<TblMasterRunResource_Accident_ListDetailDamaged>();
                foreach (var item in model)
                {
                    var insertDetail = new TblMasterRunResource_Accident_ListDetailDamaged()
                    {
                        Guid = Guid.NewGuid(),
                        MasterRunResource_Accident_Guid = accidentGuid,
                        ItemsDetail = item.AccidentItemsDetail,
                        FlagItemIsBrinks = IsBrink
                    };
                    detail.Add(insertDetail);
                }
                _masterAccidentListDetailDamagedRespository.CreateRange(detail);
            }

        }

        public void MergeAccidentImage(IEnumerable<AccidentImageModelView> model, Guid accidentGuid)
        {
            var allImage = _masterAccidentImagesRepository.FindAll(e => e.MasterRunResource_Accident_Guid == accidentGuid)?.ToList();
            if (allImage != null)
            {
                var removeImage = allImage.Where(w => !model.Any(a => a.ImageGuid == w.Guid && a.IsActualImage));
                _masterAccidentImagesRepository.RemoveRange(removeImage);
            }
            AddAccidentImage(model.Where(w => !w.IsActualImage), accidentGuid);

        }
        private void AddAccidentImage(IEnumerable<AccidentImageModelView> model, Guid accidentGuid)
        {
            if (model.Any())
            {
                var tblTmpImage = _masterImageTempRepository.FindByGuids(model.Select(s => s.ImageGuid))?.ToList();
                if (tblTmpImage.Any())
                {
                    var image = tblTmpImage
                              .Select(s => new TblMasterRunResource_Accident_Images
                              {
                                  Guid = Guid.NewGuid(),
                                  ImageName = s.FileName,
                                  FileType = s.FileType,
                                  AccidentImage = s.Stream,
                                  MasterRunResource_Accident_Guid = accidentGuid,
                                  PathImage = ""
                              });
                    _masterAccidentImagesRepository.CreateRange(image);
                    _masterImageTempRepository.RemoveRange(tblTmpImage);
                }

            }
        }

        public AccidentResponse DisableFleetMaintenanceAccident(DisableAccidentRequest modelRequest)
        {
            var response = new AccidentResponse();
            var userData = _baseRequest.Data;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    var accidentItem = _masterRunResourceAccidentRepository.FindById(modelRequest.AccidentGuid);
                    if (accidentItem != null)
                    {
                        accidentItem.FlagDisable = modelRequest.IsDisabled;
                        accidentItem.UserModifed = userData.UserName;
                        accidentItem.DatetimeModified = userData.LocalClientDateTime;
                        accidentItem.UniversalDatetimeModified = userData.UniversalDatetime;
                        _masterRunResourceAccidentRepository.Modify(accidentItem);
                    }

                    response.Msg = _systemMessageRepository.FindByMsgId(0, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true);
                    _uow.Commit();
                    transection.Complete();
                    return response;
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response.Msg = _systemMessageRepository.FindByMsgId(-184, userData.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView();
                    return response;
                }
            }

        }

        #region Get Detail For Update Data
        public AccidentDetailResponse GetAccidentDetail(Guid accidentGuid)
        {
            AccidentDetailResponse accidentDetail = new AccidentDetailResponse();
            AccidentDetailModelView accidentTab = accidentDetail.AccidentDetailTab;
            CounterpartyDetailModelView counterpartyTab = accidentDetail.CounterPartyDetailTab;

            string userFormatDate = _baseRequest.Data.UserFormatDate;
            var data = _masterRunResourceAccidentRepository.FindById(accidentGuid);
            try
            {
                #region Accident Detail Tab
                accidentTab.DateOfAccident = data.DateOfAccident;
                accidentTab.StrDateOfAccident = data.DateOfAccident.ChangeFromDateToString(userFormatDate);
                accidentTab.TimeOfAccident = data.TimeOfAccident;
                accidentTab.EmployeeGuid = data.BrinksEmployee_Guid.GetValueOrDefault();
                accidentTab.FlagBrinksIsFault = data.FlagBrinksIsFault;
                accidentTab.FlagPersonalInjury = data.FlagPersonalInjury;
                //Insurance Covered Amount
                accidentTab.Indemnity = data.Indemnity;
                accidentTab.CurrencyIndemnityGuid = data.CurrencyIndemnity_Guid;
                //Total Liability
                accidentTab.ClaimExpense = data.ClaimExpense;
                accidentTab.CurrencyClaimExpenseGuid = data.CurrencyClaimExpense_Guid;
                accidentTab.DescriptionOfAccident = data.DescriptionOfAccident;
                accidentTab.ClaimReference = data.ClaimReference;
                #endregion

                #region Counterparty Detail Tab
                counterpartyTab.TitleNameGuid = data.SystemTitleName_Guid;
                counterpartyTab.PartiesFirstName = data.Parties_FirstName;
                counterpartyTab.PartiesMiddleName = data.Parties_MiddleName;
                counterpartyTab.PartiesLastName = data.Parties_LastName;
                counterpartyTab.DriverLicenseID = data.Parties_DriverLicenseID;
                counterpartyTab.EmployeeRegistrationID = data.Parties_RegistrationID;
                counterpartyTab.CarRegistrationDocumentNumber = data.Parties_CarRegistrationDocumentNumber;
                counterpartyTab.RunResourceTypeGuid = data.Parties_RunResourceType_Guid.GetValueOrDefault();
                counterpartyTab.RunResourceBrandGuid = data.Parties_RunResourceBrand_Guid.GetValueOrDefault();
                counterpartyTab.RunResourceModel = data.Parties_RunResourceModel;
                counterpartyTab.InsuranceCompanyName = data.Parties_InsuranceCompanyName;
                counterpartyTab.InsuranceReferenceIDNumber = data.Parties_InsuranceReferenceIDNumber;
                counterpartyTab.InsuranceValidityDate = data.Parties_InsuranceValidityDate;
                counterpartyTab.StrInsuranceValidityDate = data.Parties_InsuranceValidityDate.ChangeFromDateToString(userFormatDate);
                #endregion
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return accidentDetail;
        }
        public List<AccidentListDetailDamagedModelView> GetAccidentDamageList_Brinks(Guid accidentGuid)
        {
            List<AccidentListDetailDamagedModelView> result = new List<AccidentListDetailDamagedModelView>();
            try
            {
                result = _masterAccidentListDetailDamagedRespository
                        .FindAllAsQueryable(x => x.MasterRunResource_Accident_Guid == accidentGuid && x.FlagItemIsBrinks)
                        .Select(e => new AccidentListDetailDamagedModelView
                        {
                            Guid = e.Guid,
                            AccidentGuid = e.MasterRunResource_Accident_Guid,
                            AccidentItemsDetail = e.ItemsDetail
                        }).ToList();
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        public List<AccidentListDetailDamagedModelView> GetAccidentDamageList_CounterParty(Guid accidentGuid)
        {
            List<AccidentListDetailDamagedModelView> result = new List<AccidentListDetailDamagedModelView>();
            try
            {
                result = _masterAccidentListDetailDamagedRespository
                    .FindAllAsQueryable(x => x.MasterRunResource_Accident_Guid == accidentGuid && !x.FlagItemIsBrinks)
                    .Select(e => new AccidentListDetailDamagedModelView
                    {
                        Guid = e.Guid,
                        AccidentGuid = e.MasterRunResource_Accident_Guid,
                        AccidentItemsDetail = e.ItemsDetail
                    }).ToList();
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        public IEnumerable<AccidentImageRespone> GetAccidentImageList(Guid accidentGuid)
        {
            var result = _masterAccidentImagesRepository.FindAllAsQueryable(x => x.MasterRunResource_Accident_Guid == accidentGuid)
                .AsEnumerable().Select(e => new AccidentImageRespone
                {
                    ImageGuid = e.Guid,
                    ImageContent = e.AccidentImage != null ? Convert.ToBase64String(e.AccidentImage) : null,
                    IsActualImage = true
                });
            return result;
        }

        #endregion

    }
}