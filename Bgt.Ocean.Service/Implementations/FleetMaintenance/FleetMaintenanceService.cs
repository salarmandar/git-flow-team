using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Helpers;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Service.Implementations.FleetMaintenance
{

    public interface IFleetMaintenanceService
    {
        int GetMaintenanceID(Guid? siteGuid);
        int GetCurrentOdometerByRunResource(Guid? runGuid);
        FleetMaintenanceFilterResponse GetFleetMaintenanceByRunResource(FleetMaintenanceFilterRequest req);
        FleetMaintenanceDetailBaseResponse GetFleetMaintenanceDetail(FleetMaintenanceDetailRequest req);
        FleetMaintenanceCategoryDetailResponse GetFleetMaintenanceCategoryDetail(FleetMaintenanceCategoryDetailRequest req);

        IEnumerable<DropdownViewModel<MaintenanceCategoryView>> GetMaintenanceCategory(Guid? brinksCompanyGuid);
        IEnumerable<MaintenanceCategoryDetailView> GetMaintenanceCategoryItems(FleetMaintenanceCategoryItemsRequest req);
        IEnumerable<DropdownViewModel<VendorView>> GetVendorBySite(Guid? siteGuid);
        IEnumerable<DropdownViewModel<VendorView>> GetVendorByBrinksCompany(Guid? brinksCompanyGuid);
        IEnumerable<DropdownViewModel<MaintenanceStatusView>> GetMaintenanceStatus();

        FleetMaintenanceResponse InsertOrUpdateFleetMaintenance(FleetMaintenanceRequest req);
        FleetMaintenanceCancelResponse UpdateCancelFleetMaintenance(Guid? maintenanceGuid);
    }

    public class FleetMaintenanceService : IFleetMaintenanceService
    {
        private readonly IMasterRunResourcePeriodicMaintenanceHistoryRepository _masterRunResourcePeriodicMaintenanceHistoryRepository;
        private readonly IMasterRunResourceMaintenanceDetailRepository _masterRunResourceMaintenanceDetailRepository;
        private readonly IMasterRunResourceMaintenanceRepository _masterRunResourceMaintenanceRepository;
        private readonly IMasterMaintenanceCategoryRepository _masterMaintenanceCategoryRepository;
        private readonly ISystemRunningValueGlobalRepository _systemRunningVaule_GlobalRepository;
        private readonly ISystemMaintenanceStatusRepository _systemMaintenanceStatusRepository;
        private readonly IMasterRunResourceRepository _masterRunResourceRepository;
        private readonly IMasterCurrencyRepository _masterCurrencyRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterVendorRepository _masterVendorRepository;

        private readonly IMasterSiteRepository _masterSiteRepository;

        private readonly IBaseRequest _req;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemService _systemService;



        public FleetMaintenanceService(
            IMasterRunResourcePeriodicMaintenanceHistoryRepository masterRunResourcePeriodicMaintenanceHistoryRepository
            , IMasterRunResourceMaintenanceDetailRepository masterRunResourceMaintenanceDetailRepository
            , IMasterRunResourceMaintenanceRepository masterRunResourceMaintenanceRepository
            , IMasterMaintenanceCategoryRepository masterMaintenanceCategoryRepository
            , ISystemRunningValueGlobalRepository systemRunningVaule_GlobalRepository
            , ISystemMaintenanceStatusRepository systemMaintenanceStatusRepository
            , IMasterRunResourceRepository masterRunResourceRepository
            , IMasterMenuDetailRepository masterMenuDetailRepository
            , IMasterCurrencyRepository masterCurrencyRepository
            , ISystemMessageRepository systemMessageRepository
            , IMasterVendorRepository masterVendorRepository
            , IMasterSiteRepository masterSiteRepository
            , IBaseRequest req
            , IUnitOfWork<OceanDbEntities> uow
            , ISystemService systemService)
        {
            _masterRunResourcePeriodicMaintenanceHistoryRepository = masterRunResourcePeriodicMaintenanceHistoryRepository;
            _masterRunResourceMaintenanceDetailRepository = masterRunResourceMaintenanceDetailRepository;
            _masterRunResourceMaintenanceRepository = masterRunResourceMaintenanceRepository;
            _masterMaintenanceCategoryRepository = masterMaintenanceCategoryRepository;
            _systemRunningVaule_GlobalRepository = systemRunningVaule_GlobalRepository;
            _systemMaintenanceStatusRepository = systemMaintenanceStatusRepository;
            _masterRunResourceRepository = masterRunResourceRepository;
            _masterCurrencyRepository = masterCurrencyRepository;
            _systemMessageRepository = systemMessageRepository;
            _masterVendorRepository = masterVendorRepository;
            _masterSiteRepository = masterSiteRepository;
            _req = req;
            _uow = uow;
            _systemService = systemService;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetFleetMaintenanceByRunResource
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public FleetMaintenanceFilterResponse GetFleetMaintenanceByRunResource(FleetMaintenanceFilterRequest req)
        {
            var result = new FleetMaintenanceFilterResponse();
            var pagingBase = (PagingBase)req.Filters;
            try
            {
                var maintenanceList = _masterRunResourceRepository.GetFleetMaintenance(req.Filters);
                if (pagingBase != null)
                {
                    var response = PaginationHelper.ToPagination(maintenanceList, pagingBase);
                    result.MaintenanceList = response.Data;
                    result.Total = response.Total;
                }
                else
                {
                    result.MaintenanceList = maintenanceList;
                    result.Total = maintenanceList.Count();
                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetFleetMaintenanceDetail
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public FleetMaintenanceDetailBaseResponse GetFleetMaintenanceDetail(FleetMaintenanceDetailRequest req)
        {
            var result = new FleetMaintenanceDetailBaseResponse();
            try
            {
                var model = new FleetMaintenanceModel();

                var fleet = _masterRunResourceMaintenanceRepository.FindById(req.MaintenanceGuid);
                if (fleet != null)
                {
                    model.MaintenanceGuid = req.MaintenanceGuid;
                    model.MaintenanceID = fleet.MaintenanceID;
                    model.MasterSite_Guid = fleet.MasterSite_Guid;
                    model.MasterRunResource_Guid = fleet.MasterRunResource_Guid;
                    model.MasterVendor_Guid = fleet.MasterVendor_Guid;
                    model.MaintenanceStatusID = (EnumMaintenanceStatus)fleet.MaintenanceStatusID;
                    model.DocumentRef_No = fleet.DocumentRef_No;
                    model.Remarks = fleet.Remarks;
                    model.DiscountValue = fleet.DiscountValue;
                    model.Discount_Type = (EnumDiscountType)fleet.Discount_Type;
                    model.Cost_Actual = fleet.CostActual;
                    model.Cost_Estimate = fleet.CostEstimate;

                    //cost
                    if (model.State == EnumMaintenanceState.Actual)
                    {
                        model.Cost = fleet.CostActual;
                        model.CurrencyGuid = fleet.CurrencyActual_Guid;
                    }

                    if (model.State == EnumMaintenanceState.Estimate)
                    {

                        model.Cost = fleet.CostEstimate;
                        model.CurrencyGuid = fleet.CurrencyEstimate_Guid;
                    }

                    //vendor
                    var vendorName = _masterVendorRepository.FindById(model.MasterVendor_Guid)?.VendorName;
                    model.VendorName = vendorName;

                    //currency
                    var currencyAbb = _masterCurrencyRepository.FindById(model.CurrencyGuid)?.MasterCurrencyAbbreviation;
                    model.CurrencyAbb = currencyAbb;

                    //open
                    model.Open_OdoMeter = fleet.Open_OdoMeter;
                    model.strOpen_DateServiceFrom = fleet.Open_DateServiceFrom.ChangeFromDateToString(_req.Data.UserFormatDate);
                    model.strOpen_TimeServiceFrom = fleet.Open_TimeServiceFrom.ChangeFromDateToTimeString();
                    model.strOpen_DateServiceTo = fleet.Open_DateServiceTo.ChangeFromDateToString(_req.Data.UserFormatDate);
                    model.strOpen_TimeServiceTo = fleet.Open_TimeServiceTo.ChangeFromDateToTimeString();

                    //close
                    model.Close_OdoMeter = fleet.Close_OdoMeter;
                    model.strClose_DateService = fleet.Close_DateService.ChangeFromDateToString(_req.Data.UserFormatDate);
                    model.strClose_TimeService = fleet.Close_TimeService.ChangeFromDateToTimeString();

                    result.MaintenanceModel = model;
                    result.SetMessageView(_systemMessageRepository.FindByMsgId(0, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(true));
                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetFleetMaintenanceCategoryDetail
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public FleetMaintenanceCategoryDetailResponse GetFleetMaintenanceCategoryDetail(FleetMaintenanceCategoryDetailRequest req)
        {
            var result = new FleetMaintenanceCategoryDetailResponse();
            try
            {
                var category = new FleetMaintenanceCategoryModel();
                category.MaintenanceCategoryDetailList = _masterRunResourceMaintenanceRepository.GetMaintenanceCategoryDetailByMaintenanceGuid(req.MaintenanceGuid, req.State);
                result.MaintenanceCategoryModel = category;
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
            }
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetMaintenanceID
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        public int GetMaintenanceID(Guid? siteGuid)
        {

            var maintenanceBrinksSiteData = _masterSiteRepository.FindById(siteGuid);
            var runningValue = _systemRunningVaule_GlobalRepository.FindAll(o => o.RunningKey.Equals(FixStringRoute.MaintenanceID)).FirstOrDefault();
            var maintenanceID = int.Parse(maintenanceBrinksSiteData.SiteCode) + runningValue?.RunningVaule1;

            // update running
            if (runningValue != null)
            {
                runningValue.RunningVaule1 = runningValue.RunningVaule1 + 1;
                _uow.Commit();
            }

            return maintenanceID ?? 0;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetCurrentOdometerByRunResource
        /// </summary>
        /// <param name="runGuid"></param>
        /// <returns></returns>
        public int GetCurrentOdometerByRunResource(Guid? runGuid)
        {
            return (_masterRunResourceRepository.FindById(runGuid)?.CurrentOdometer) ?? 0;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => InsertOrUpdateFleetMaintenance
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public FleetMaintenanceResponse InsertOrUpdateFleetMaintenance(FleetMaintenanceRequest req)
        {
            var result = new FleetMaintenanceResponse();
            var model = req.MaintenanceModel;
            using (var transaction = _uow.BeginTransaction())
            {
                try
                {
                    //TblMasterRunResource_Maintenance
                    var baseFeet = InsertOrUpdateMasterRunResource_Maintenance(model);

                    //TblMasterRunResource_Maintenance_Detail
                    var baseDetails = InsertOrUpdateMaintenance_Detail(req, baseFeet);

                    //TblMasterRunResource_PeriodicMaintenance_History
                    InsertPeriodicMaintenanceHistory(baseFeet, baseDetails);

                    //output
                    var msgID = GetMaintainanceMsgID(model);
                    var message = _systemMessageRepository.FindByMsgId(msgID, _req.Data.UserLanguageGuid.GetValueOrDefault());
                    message.MessageTextContent = string.Format(message.MessageTextContent, model.MaintenanceID.ToString());
                    result.SetMessageView(message.ConvertToMessageView(true));

                    _uow.Commit();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, _req.Data.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView());
                }
            }
            return result;
        }

        private int GetMaintainanceMsgID(FleetMaintenanceModel model)
        {
            var msgID = 0;
            switch (model.MaintenanceStatusID)
            {
                case EnumMaintenanceStatus.InProgress:
                    msgID = model.MaintenanceGuid == null ? 564 : 566;
                    break;
                case EnumMaintenanceStatus.Closed:
                    msgID = 565;
                    break;
            }
            return msgID;
        }


        private TblMasterRunResource_Maintenance InsertOrUpdateMasterRunResource_Maintenance(FleetMaintenanceModel model)
        {
            TblMasterRunResource_Maintenance baseFleet = null;
            baseFleet = _masterRunResourceMaintenanceRepository.FindById(model.MaintenanceGuid);
            var IsUpdate = baseFleet != null;
            baseFleet = IsUpdate ? baseFleet : new TblMasterRunResource_Maintenance();

            baseFleet.MasterSite_Guid = model.MasterSite_Guid.GetValueOrDefault();
            baseFleet.MasterRunResource_Guid = model.MasterRunResource_Guid.GetValueOrDefault();
            baseFleet.MasterVendor_Guid = model.MasterVendor_Guid.GetValueOrDefault();
            baseFleet.MaintenanceStatusID = (int)model.MaintenanceStatusID;
            baseFleet.DocumentRef_No = model.DocumentRef_No;
            baseFleet.Remarks = model.Remarks;
            baseFleet.DiscountValue = model.DiscountValue;
            baseFleet.Discount_Type = (int)model.Discount_Type;

            #region ### By Maintenance State
            if (model.State == EnumMaintenanceState.Estimate)
            {
                baseFleet.CostEstimate = model.Cost;
                baseFleet.CurrencyEstimate_Guid = model.CurrencyGuid;
            }

            if (model.State == EnumMaintenanceState.Actual)
            {
                baseFleet.CostActual = model.Cost;
                baseFleet.CurrencyActual_Guid = model.CurrencyGuid;
            }
            #endregion

            #region ### By Maintenance Status
            if (model.MaintenanceStatusID == EnumMaintenanceStatus.InProgress)
            {
                baseFleet.Open_OdoMeter = model.Open_OdoMeter;
                baseFleet.Open_DateServiceFrom = model.strOpen_DateServiceFrom.ChangeFromStrDateToDateTime();
                baseFleet.Open_TimeServiceFrom = model.strOpen_TimeServiceFrom.ChangeFromStrTimeToDateTime();
                baseFleet.Open_DateServiceTo = model.strOpen_DateServiceTo.ChangeFromStrDateToDateTime();
                baseFleet.Open_TimeServiceTo = model.strOpen_TimeServiceTo.ChangeFromStrTimeToDateTime();
            }
            if (model.MaintenanceStatusID == EnumMaintenanceStatus.Closed)
            {
                baseFleet.Close_OdoMeter = model.Close_OdoMeter;
                baseFleet.Close_DateService = model.strClose_DateService.ChangeFromStrDateToDateTime();
                baseFleet.Close_TimeService = model.strClose_TimeService.ChangeFromStrTimeToDateTime();
            }
            #endregion

            if (IsUpdate)
            {
                baseFleet.FlagDisable = model.FlagDisable;
                baseFleet.UserModifed = _req.Data.UserName;
                baseFleet.DatetimeModified = _req.Data.LocalClientDateTime;
                baseFleet.UniversalDatetimeCreated = _req.Data.UniversalDatetime;

                _masterRunResourceMaintenanceRepository.Modify(baseFleet);
            }
            else
            {
                baseFleet.Guid = Guid.NewGuid();
                baseFleet.FlagDisable = model.FlagDisable;
                baseFleet.MaintenanceID = model.MaintenanceID;
                baseFleet.FlagDisable = model.FlagDisable;
                baseFleet.UserCreated = _req.Data.UserName;
                baseFleet.DatetimeCreated = _req.Data.LocalClientDateTime;
                baseFleet.UniversalDatetimeModified = _req.Data.UniversalDatetime;

                _masterRunResourceMaintenanceRepository.Create(baseFleet);
            }

            return baseFleet;
        }
        private IEnumerable<TblMasterRunResource_Maintenance_Detail> InsertOrUpdateMaintenance_Detail(FleetMaintenanceRequest req, TblMasterRunResource_Maintenance baseFeet)
        {
            var result = Enumerable.Empty<TblMasterRunResource_Maintenance_Detail>();
            var FlagValidItems = false;
            var maintenanceDetailList = req.MaintenanceCategoryModel.MaintenanceCategoryDetailList;
            if (maintenanceDetailList != null && maintenanceDetailList.Any())
            {
                FlagValidItems = maintenanceDetailList.All(o => o.Validate());
                if (FlagValidItems)
                {
                    var updateItems = maintenanceDetailList.Where(o => o.ItemState != EnumState.Deleted);
                    var removeGuids = maintenanceDetailList.Where(o => o.ItemState == EnumState.Deleted).Select(o => o.MaintenanceDetailGuid);
                    var detailGuids = maintenanceDetailList.Where(o => o.ItemState != EnumState.Added).Select(o => o.MaintenanceDetailGuid);

                    var baseDetails = _masterRunResourceMaintenanceDetailRepository.FindAllAsQueryable().Where(o => detailGuids.Contains(o.Guid));

                    //remove
                    if (removeGuids != null && removeGuids.Any())
                    {
                        _masterRunResourceMaintenanceDetailRepository.RemoveRange(baseDetails.Where(o => removeGuids.Contains(o.Guid)));
                    }

                    //create/update
                    if (updateItems != null && updateItems.Any())
                    {
                        result = updateItems.Select(model =>
                       {
                           TblMasterRunResource_Maintenance_Detail baseDetail = null;
                           baseDetail = baseDetails.FirstOrDefault(b => b.Guid == model.MaintenanceDetailGuid);
                           var IsUpdate = baseDetail != null;
                           baseDetail = IsUpdate ? baseDetail : new TblMasterRunResource_Maintenance_Detail() { Guid = Guid.NewGuid() };

                           baseDetail.MasterMaintenanceCategory_Items_Guid = null; //srs required
                           baseDetail.MasterRunResource_Maintenance_Guid = baseFeet.Guid;// FK
                           baseDetail.MasterMaintenanceCategory_Guid = model.MasterMaintenanceCategory_Guid;
                           baseDetail.Description = model.Description;

                           #region ### Maintenance State
                           if (req.MaintenanceModel.State == EnumMaintenanceState.Actual)
                           {
                               baseDetail.Discount_Actual = model.Discount;
                               baseDetail.Discount_Type_Acutal = (int)model.DiscountType;
                               baseDetail.Part_Qty_Actual = model.PartQty;
                               baseDetail.Unite_Price_Actual = model.UnitPrice;
                               baseDetail.Total_Actual = Convert.ToDecimal(model.Total);
                           }
                           else
                           {
                               baseDetail.Discount_Estimate = model.Discount;
                               baseDetail.Discount_Type_Estimate = (int)model.DiscountType;
                               baseDetail.Part_Qty_Estimate = model.PartQty;
                               baseDetail.Unite_Price_Estimate = model.UnitPrice;
                               baseDetail.Total_Estimate = Convert.ToDecimal(model.Total);
                           }
                           #endregion

                           if (IsUpdate)
                               _masterRunResourceMaintenanceDetailRepository.Modify(baseDetail);
                           else
                               _masterRunResourceMaintenanceDetailRepository.Create(baseDetail);

                           return baseDetail;
                       }).ToList();
                    }
                }
            }

            return result;
        }

        private void InsertPeriodicMaintenanceHistory(TblMasterRunResource_Maintenance baseFeet, IEnumerable<TblMasterRunResource_Maintenance_Detail> baseDetails)
        {
            if ((EnumMaintenanceStatus)baseFeet.MaintenanceStatusID == EnumMaintenanceStatus.Closed)
            {
                var history = baseDetails.Select(baseDetail => new TblMasterRunResource_PeriodicMaintenance_History
                {
                    Guid = Guid.NewGuid(),
                    MaintenanceLastTimeOdoMeter = Convert.ToInt32(baseFeet.Close_OdoMeter),
                    MaintenanceLastTimeDate = DateTimeHelper.FromDateCombineWithTime(baseFeet.Close_DateService, baseFeet.Close_TimeService),
                    MasterRunResource_Guid = baseFeet.MasterRunResource_Guid,
                    MasterMaintenanceCategory_Guid = baseDetail.MasterMaintenanceCategory_Guid
                });
                _masterRunResourcePeriodicMaintenanceHistoryRepository.CreateRange(history);
            }
        }
        /// <summary>
        ///  => TFS#57836:Fleet Maintenance Revised => UpdateCancelFleetMaintenance
        /// </summary>
        /// <param name="maintenanceGuid"></param>
        /// <returns></returns>
        public FleetMaintenanceCancelResponse UpdateCancelFleetMaintenance(Guid? maintenanceGuid)
        {
            var result = new FleetMaintenanceCancelResponse();
            try
            {
                var baseFleet = _masterRunResourceMaintenanceRepository.FindById(maintenanceGuid);
                if (baseFleet != null)
                {
                    baseFleet.FlagDisable = true;
                    baseFleet.MaintenanceStatusID = (int)EnumMaintenanceStatus.Cancel;
                    baseFleet.UserModifed = _req.Data.UserName;
                    baseFleet.DatetimeModified = _req.Data.LocalClientDateTime;
                    baseFleet.UniversalDatetimeCreated = _req.Data.UniversalDatetime;

                    _masterRunResourceMaintenanceRepository.Modify(baseFleet);

                    _uow.Commit();

                    var message = _systemMessageRepository.FindByMsgId(563, _req.Data.UserLanguageGuid.GetValueOrDefault());
                    message.MessageTextContent = string.Format(message.MessageTextContent, baseFleet.MaintenanceID.ToString());
                    result.SetMessageView(message.ConvertToMessageView(true));
                }
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetMaintenanceCategoryItems
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public IEnumerable<MaintenanceCategoryDetailView> GetMaintenanceCategoryItems(FleetMaintenanceCategoryItemsRequest req)
        {
            req.CurrencyText = string.IsNullOrEmpty(req.CurrencyText) ? "" : req.CurrencyText;
            return _masterMaintenanceCategoryRepository.GetMaintenanceCategoryItems(req.BrinksCompanyGuid, req.MaintenanceCategoryGuid, req.CurrencyText);
        }
        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetMaintenanceCategory
        /// </summary>
        /// <param name="brinksCompanyGuid"></param>
        /// <returns></returns>
        public IEnumerable<DropdownViewModel<MaintenanceCategoryView>> GetMaintenanceCategory(Guid? brinksCompanyGuid)
        {
            IEnumerable<DropdownViewModel<MaintenanceCategoryView>> result = null;
            try
            {
                result = _masterMaintenanceCategoryRepository.GetMaintenanceCategory(brinksCompanyGuid).Select(o => new DropdownViewModel<MaintenanceCategoryView>
                {
                    //Obj = o,
                    Text = o.MaintenanceCategoryName,
                    Value = o.MasterMaintenanceCategoryGuid.ToString()
                }).OrderBy(o => o.Value);
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetVendorBySite
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        public IEnumerable<DropdownViewModel<VendorView>> GetVendorBySite(Guid? siteGuid)
        {
            var result = Enumerable.Empty<DropdownViewModel<VendorView>>();
            try
            {
                if (siteGuid.HasValue)
                {
                    result = _masterVendorRepository.FindAllAsQueryable(o => o.MasterSite_Guid == siteGuid && o.FlagDisable == false)
                                                        .Select(o => new DropdownViewModel<VendorView>
                                                        {
                                                            //Obj = o,
                                                            Text = o.VendorName,
                                                            Value = o.Guid.ToString()
                                                        }).OrderBy(o => o.Text);
                }

            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised => GetVendorByBrinksCompany
        /// </summary>
        /// <param name="brinksCompanyGuid"></param>
        /// <returns></returns>
        public IEnumerable<DropdownViewModel<VendorView>> GetVendorByBrinksCompany(Guid? brinksCompanyGuid)
        {
            var result = Enumerable.Empty<DropdownViewModel<VendorView>>();
            try
            {
                if (brinksCompanyGuid.HasValue)
                {
                    result = _masterVendorRepository.FindAllAsQueryable(o => o.MasterCustomer_Guid == brinksCompanyGuid && o.FlagDisable == false)
                                                        .Select(o => new DropdownViewModel<VendorView>
                                                        {
                                                            //Obj = o,
                                                            Text = o.VendorName,
                                                            Value = o.Guid.ToString()
                                                        }).OrderBy(o => o.Text);
                }

            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }
        /// <summary>
        /// => TFS#57836:Fleet Maintenance Revised=> GetMaintenanceStatus
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DropdownViewModel<MaintenanceStatusView>> GetMaintenanceStatus()
        {
            IEnumerable<DropdownViewModel<MaintenanceStatusView>> result = null;
            try
            {
                result = _systemMaintenanceStatusRepository.GetMaintenanceStatus().Select(o => new DropdownViewModel<MaintenanceStatusView>
                {
                    //Obj = o,
                    Text = o.MaintenanceStatusName,
                    Value = o.MaintenanceStatusID.ToString()
                }).OrderBy(o => o.Value);
            }
            catch (Exception ex)
            {
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
            }
            return result;
        }

    }
}