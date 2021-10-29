using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.PreVault;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.PreVault.VaultBalance;
using Bgt.Ocean.Service.ModelViews;
using Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance;
using Bgt.Ocean.Service.ModelViews.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bgt.Ocean.Infrastructure.Util.EnumPreVault;
using static Bgt.Ocean.Infrastructure.Util.EnumUser;

namespace Bgt.Ocean.Service.Implementations.PreVault
{
    public partial interface IVaultBalanceService
    {
        StartAndValidateVaultBalanceReponse StartScanVaultBalance(VaultBalanceDetailRequest request);
        VaultBalanceDetailReponse GetItemsVaultBalanceDetail(VaultBalanceDetailRequest request);
        SystemMessageView SaveAndCloseVaultBalance(VaultBalanceItemsRequest request);
        SystemMessageView VerifyVaultBalance(string username, string password);
        IEnumerable<VaultBalanceMappingModelView> GetVaultBalanceSealMapping(VaultBalanceItemsRequest request);
        IEnumerable<VaultBalanceSummaryModelView> GetVaultBalanceSummary(VaultBalanceItemsRequest request);
        SystemMessageView CancelVaultBalance(Guid vaultBalanceGuid);
        SystemMessageView SubmitVaultBalance(VaultBalanceItemsRequest request);

        #region For dolphin
        VaultBalanceApiResponse ValidateVaultBalanceBlocking(VaultBalanceApiRequest request, Uri requestUri);
        #endregion
    }
    public partial class VaultBalanceService : RequestBase, IVaultBalanceService
    {
        #region Validate vault balance
        private TblVaultBalanceHeader GetVaultBalance(Guid siteGuid, Guid prevaultGuid)
        {
            int[] vautState = new int[] { (int)EnumVaultState.Process, (int)EnumVaultState.OnHold };
            return _vaultBalanceHeaderRepository.FindOne(o => o.MasterSite_Guid == siteGuid &&
                    o.MasterCustomerLocation_InternalDepartment_Guid == prevaultGuid &&
                    vautState.Any(x => x == o.StateID));
        }
        private bool IsExistDiscrepanciesItem(Guid prevualtGuid)
        {
            var actualDisc = _masterActualJobItemDiscrapenciesRepository
                            .FindAllAsQueryable(o => o.MasterCustomerLocation_InternalDepartment_Guid == prevualtGuid &&
                                      o.FlagCloseCase != true).Any();
            var vaultDisc = _vaultBalanceDiscrepancyRepository
                            .FindAllAsQueryable(o => o.MasterCustomerLocation_InternalDepartment_Guid == prevualtGuid &&
                                      !o.FlagCloseCase && !o.FlagTempDiscrepancy).Any();
            var itemUnknow = _masterActualJobItemUnknowRepository
                            .FindAllAsQueryable(o => o.MasterCustomerLocation_InternalDepartment_Guid == prevualtGuid && o.FlagMatchDone != true).Any();
            return actualDisc || vaultDisc || itemUnknow;
        }
        #endregion

        #region Start scan
        public StartAndValidateVaultBalanceReponse StartScanVaultBalance(VaultBalanceDetailRequest request)
        {
            Guid siteGuid = request.SiteGuid;
            Guid prevaultGuid = request.PrevaultGuid;
            bool flagModeBlind = request.FlagBlindScanning;

            StartAndValidateVaultBalanceReponse response = new StartAndValidateVaultBalanceReponse
            {
                FlagBlindScanning = request.FlagBlindScanning
            };
            Guid languageGuid = UserLanguageGuid.GetValueOrDefault();

            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    #region 1.Validate discrepancies
                    bool isExistDisc = IsExistDiscrepanciesItem(prevaultGuid);
                    if (isExistDisc)
                    {
                        response.SystemMessageView = _systemMessageRepository.FindByMsgId(-17375, languageGuid).ConvertToMessageView(false);
                        response.SystemMessageView.IsWarning = true;
                        return response;
                    }
                    #endregion

                    var vaultBalanceHeader = GetVaultBalance(siteGuid, prevaultGuid);

                    #region 2.Validate vault state is process
                    if (vaultBalanceHeader != null)
                    {
                        var vaultBalanceUsing = _vaultBalanceDetailRepository
                                            .FindAll(o => o.VaultBalanceHeader_Guid == vaultBalanceHeader.Guid)
                                            .OrderByDescending(x => x.DatetimeBalance).FirstOrDefault().UserBalance;


                        if (vaultBalanceHeader.StateID == (int)EnumVaultState.Process && vaultBalanceUsing != UserName)
                        {
                            response.SystemMessageView = _systemMessageRepository.FindByMsgId(-17376, languageGuid).ConvertToMessageView(false);
                            response.SystemMessageView.IsWarning = true;
                            return response;
                        }
                    }
                    #endregion

                    #region 3.Validate mode vault balance state
                    if (vaultBalanceHeader != null && vaultBalanceHeader.FlagModeBlind != flagModeBlind)
                    {
                        response.SystemMessageView = _systemMessageRepository.FindByMsgId(-180012, languageGuid).ConvertToMessageView(false);
                        response.SystemMessageView.IsWarning = true;
                        response.FlagBlindScanning = vaultBalanceHeader.FlagModeBlind;
                        return response;
                    }
                    #endregion

                    response.VaultBalanceGuid = UpdateVaultStartScan(vaultBalanceHeader, siteGuid, prevaultGuid, request.FlagBlindScanning);
                    response.SystemMessageView = _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView();
                    _uow.Commit();
                    transection.Complete();
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response.SystemMessageView = _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
                }
            }
            return response;
        }
        private Guid UpdateVaultStartScan(TblVaultBalanceHeader vaultBalanceHeader, Guid siteGuid, Guid prevaultGuid, bool flagModeBlind)
        {
            //For not hasn't vault balance
            if (vaultBalanceHeader == null)
            {
                return InsertVaultBalance(siteGuid, prevaultGuid, flagModeBlind);
            }
            //For has vault balance
            return UpdateVaultBalanceStart(vaultBalanceHeader, EnumVaultState.Process);
        }
        private Guid InsertVaultBalance(Guid siteGuid, Guid prevaultGuid, bool flagModeBlind)
        {
            var insertVaultBalance = new TblVaultBalanceHeader()
            {
                Guid = Guid.NewGuid(),
                MasterSite_Guid = siteGuid,
                MasterCustomerLocation_InternalDepartment_Guid = prevaultGuid,
                StateID = (int)EnumVaultState.Process,
                FlagModeBlind = flagModeBlind
            };
            _vaultBalanceHeaderRepository.Create(insertVaultBalance);
            InsertVautDetail(insertVaultBalance.Guid);

            return insertVaultBalance.Guid;
        }
        private Guid UpdateVaultBalanceStart(TblVaultBalanceHeader vaultBalanceHeader, EnumVaultState vaultStateID)
        {
            UpdateVaultBalance(vaultBalanceHeader, vaultStateID);
            InsertVautDetail(vaultBalanceHeader.Guid);
            return vaultBalanceHeader.Guid;
        }
        private void UpdateVaultBalance(TblVaultBalanceHeader vaultBalanceHeader, EnumVaultState vaultStateID, string userSuperVerify = null)
        {
            vaultBalanceHeader.PreviousStateID = vaultBalanceHeader.StateID ?? 0;
            vaultBalanceHeader.StateID = (int)vaultStateID;
            if (!string.IsNullOrEmpty(userSuperVerify))
            {
                vaultBalanceHeader.UsernameSupervisorVerify = userSuperVerify;
                vaultBalanceHeader.DatetimeSupervisorVerify = LocalClientDateTime;
            }
            _vaultBalanceHeaderRepository.Modify(vaultBalanceHeader);
        }
        private void InsertVautDetail(Guid vaultBalanceGuid)
        {
            var insertVaultDetail = new TblVaultBalanceDetail()
            {
                Guid = Guid.NewGuid(),
                UserBalance = UserName,
                VaultBalanceHeader_Guid = vaultBalanceGuid,
                DatetimeBalance = LocalClientDateTime
            };
            _vaultBalanceDetailRepository.Create(insertVaultDetail);
        }
        #endregion

        #region Get Items
        public VaultBalanceDetailReponse GetItemsVaultBalanceDetail(VaultBalanceDetailRequest request)
        {
            VaultBalanceDetailReponse response = new VaultBalanceDetailReponse();

            //Get temp items
            List<VaultBalanceSealModelView> sealsTemp = GetTempVaultBalanceSeal(request.VaultBalanceGuid.GetValueOrDefault(), request.FlagBlindScanning).ToList();
            List<VaultBalanceNonbarcodeModelView> nonbsTemp = GetTempVaultBalanceNonbarcode(request.VaultBalanceGuid.GetValueOrDefault(), request.SiteGuid).ToList();

            //Get actual items
            if (!request.FlagBlindScanning)
            {
                var result = GetDetailVaultBalance(request);
                sealsTemp = sealsTemp.Select(o =>
                                      {
                                          var temp = result.SealList.FirstOrDefault(e => e.Guid == o.Guid);
                                          if (temp != null)
                                          {
                                              o.CustomerPickupGuid = temp.CustomerPickupGuid;
                                              o.CustomerDeliveryGuid = temp.CustomerDeliveryGuid;
                                              o.LocationPickupGuid = temp.LocationPickupGuid;
                                              o.LocationDeliveryGuid = temp.LocationDeliveryGuid;
                                          }
                                          return o;
                                      }).ToList();

                var sealsGuidExcept = result.SealList.Select(o => o.Guid).Except(sealsTemp.Select(o => o.Guid));
                var sealsExcept = result.SealList.Where(o => sealsGuidExcept.Contains(o.Guid));

                var nonbsGuidExcept = result.NonbarcodeList.Select(o => o.CommodityGuid).Except(nonbsTemp.Select(o => o.CommodityGuid));
                var nonbExcept = result.NonbarcodeList.Where(o => nonbsGuidExcept.Contains(o.CommodityGuid));

                sealsTemp = sealsTemp.Union(sealsExcept).ToList();
                nonbsTemp = nonbsTemp.Union(nonbExcept).ToList();
            }

            response.SealList = sealsTemp.OrderBy(o => o.ScanItemState == EnumItemState.Scanned || o.ScanItemState == EnumItemState.Overage ? 0 : 1)
                                         .ThenBy(o => o.WorkDate)
                                         .ThenBy(o => o.JobNo)
                                         .ThenBy(o => o.LiabilityGuid)
                                         .ThenBy(o => o.SealNo).ToList();
            response.NonbarcodeList = nonbsTemp;
            return response;
        }
        private VaultBalanceDetailReponse GetDetailVaultBalance(VaultBalanceDetailRequest request, bool isSealMapping = false)
        {
            VaultBalanceDetailReponse result = new VaultBalanceDetailReponse();
            var seals = _masterActualJobItemsSealRepository.FindItemByPrevault(request.PrevaultGuid).ToList();
            var nonbs = _masterActualJobItemsCommodityRepository.FindItemByPrevault(request.PrevaultGuid).ToList();
            var itemJobs = seals.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault())
                           .Union(nonbs.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault()));

            var jobs = _masterActualJobHeaderRepository.GetJobDetailVaultBalance(itemJobs).ToList();
            result.SealList = GetItemVaultBalanceSealAndMaster(seals, nonbs, jobs, request.SiteGuid);
            if (!isSealMapping)
            {
                var jobsGuidInVault = jobs.Select(o => o.JobGuid);
                var nonbInJob = nonbs.Where(o => o.MasterActualJobHeader_Guid != null && jobsGuidInVault.Contains((Guid)o.MasterActualJobHeader_Guid)).ToList();
                result.NonbarcodeList = GetItemVaultBalanceNonbarcode(nonbInJob, request.SiteGuid);
            }
            return result;
        }
        public IEnumerable<VaultBalanceSealModelView> GetTempVaultBalanceSeal(Guid vaultBalanceGuid, bool flagBlind)
        {
            var result = _vaultBalanceSealAndMasterRepository.FindAll(e => e.VaultBalanceHeader_Guid == vaultBalanceGuid)
                .Select(o =>
                {
                    bool isActualItem = o.MasterActualJobItemsSeal_Guid.HasValue || o.MasterConAndDeconsolidate_Header_Guid.HasValue;
                    Guid? sealGuid = o.MasterActualJobItemsSeal_Guid.HasValue ? o.MasterActualJobItemsSeal_Guid : o.MasterConAndDeconsolidate_Header_Guid;

                    return new VaultBalanceSealModelView()
                    {
                        Guid = isActualItem ? sealGuid : o.Guid,
                        SealNo = o.SealNo,
                        JobGuid = o.MasterActualJobHeader_Guid,
                        JobNo = o.JobNo,
                        STC = o.STC,
                        Commodity = o.CommodityName,
                        Currency = o.Currency,
                        PickUpLocation = o.MasterCustomer_Location_Pickup,
                        DeliveryLocation = o.MasterCustomer_Location_Delivery,
                        ServiceType = o.SystemServiceJobTypeName,
                        WorkDate = o.WorkDate,
                        ScanItemState = isActualItem || flagBlind ? EnumItemState.Scanned : EnumItemState.Overage
                    };
                }).OrderBy(o => o.SealNo);
            return result;
        }
        public IEnumerable<VaultBalanceNonbarcodeModelView> GetTempVaultBalanceNonbarcode(Guid vaultBalanceGuid, Guid siteGuid)
        {
            var tblVaultNonb = _vaultBalanceNonbarcodeRepository.FindAll(e => e.VaultBalanceHeader_Guid == vaultBalanceGuid);
            var commList = tblVaultNonb.Select(o => o.Commodity_Guid).Distinct();
            var templateComm = _masterCommodityRepository.GetAllCommodityBySite(siteGuid, flagIncludeDisable: true)
                               .Where(o => commList.Contains(o.MasterCommodity_Guid)).ToList();

            var result = tblVaultNonb
                .Select(o =>
                {
                    var comm = templateComm.FirstOrDefault(e => e.MasterCommodity_Guid == o.Commodity_Guid);
                    EnumItemState itemState = GetItemState(o.PreAdviceQty, o.ActualQty);
                    var actualSTC = (comm.CommodityAmount * comm.CommodityValue) * o.ActualQty;
                    var preSTC = (comm.CommodityAmount * comm.CommodityValue) * o.PreAdviceQty;
                    return new VaultBalanceNonbarcodeModelView()
                    {
                        CommodityGuid = o.Commodity_Guid.GetValueOrDefault(),
                        CommodityName = comm?.CommodityName ?? string.Empty,
                        ActualStc = actualSTC,
                        STC = preSTC,
                        CommodityValue = comm.CommodityValue,
                        CommodityAmount = comm.CommodityAmount,
                        PreAdviceQty = o.PreAdviceQty,
                        ActualQty = o.ActualQty,
                        ItemState = EnumState.Added,
                        ScanItemState = itemState,
                        FlagTemp = o.PreAdviceQty == 0,
                        ColumnInReport = comm?.ColumnInReport ?? string.Empty,
                        CommodityCode = comm?.CommodityCode ?? string.Empty
                    };
                }).OrderBy(o => o.CommodityCode == "CX" ? 0 : 1).ThenBy(e => e.ColumnInReport).ThenBy(j => j.CommodityName).ToList();
            return result;
        }

        private EnumItemState GetItemState(int preadviceQty, int actualQty)
        {
            EnumItemState result = EnumItemState.Scanned;
            if (preadviceQty > actualQty)
            {
                result = EnumItemState.Shortage;
            }

            if (preadviceQty < actualQty)
            {
                result = EnumItemState.Overage;
            }

            return result;
        }
        public IEnumerable<VaultBalanceSealModelView> GetItemVaultBalanceSealAndMaster(List<TblMasterActualJobItemsSeal> seals, List<TblMasterActualJobItemsCommodity> nonbs, IEnumerable<VaultBalanceJobDetailModel> jobDetail, Guid siteGuid)
        {
            List<VaultBalanceSealModelView> itemsDetail = new List<VaultBalanceSealModelView>();
            #region Get individual seals
            var jobsGuidInVault = jobDetail.Select(o => o.JobGuid);
            var sealInJob = seals.Where(o => o.MasterActualJobHeader_Guid != null && jobsGuidInVault.Contains((Guid)o.MasterActualJobHeader_Guid)).ToList();
            var sealItems = sealInJob.Where(e => !e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue &&
                                             !e.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue).ToList();
            var sealDetail = _masterActualJobItemsSealRepository.GetSealDetailVaultBalance(sealItems, jobDetail).ConvertToVaultBalanceSealView();
            itemsDetail.AddRange(sealDetail);
            #endregion

            #region Get detail of consolidate
            //Master Location
            var sealLoItems = seals.Where(e => !e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue && e.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue);
            var nonbLoItems = nonbs.Where(e => !e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue && e.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue);

            var masterLocGuid = sealLoItems.Select(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid.GetValueOrDefault()).Union(nonbLoItems.Select(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid.GetValueOrDefault()));

            //Master Route
            var sealRoItems = seals.Where(e => e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue);
            var nonbRoItems = nonbs.Where(e => e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue);

            var masterRouteGuid = sealRoItems.Select(o => o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.GetValueOrDefault()).Union(nonbRoItems.Select(o => o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.GetValueOrDefault()));

            //Master All
            var masterGuids = masterLocGuid.Union(masterRouteGuid).ToList();
            if (masterGuids.Any())
            {
                var masterDetailList = GetConsolidateType(masterGuids, siteGuid).Select(o =>
                 {
                     bool isLoc = masterLocGuid.Any(e => e == o.MasterGuid);
                     bool isRoute = masterRouteGuid.Any(e => e == o.MasterGuid);
                     return new VaultBalanceSealModelView
                     {
                         Guid = o.MasterGuid,
                         SealNo = o.MasterID,
                         STC = 0,
                         ConsolidateType = o.ConsolidateType,
                         IsMasterLoc = isLoc,
                         IsMasterRoute = isRoute
                     };
                 });
                itemsDetail.AddRange(masterDetailList);
            }
            #endregion

            return itemsDetail;
        }
        public IEnumerable<VaultBalanceNonbarcodeModelView> GetItemVaultBalanceNonbarcode(IEnumerable<TblMasterActualJobItemsCommodity> nonbs, Guid siteGuid)
        {

            var itemNonbAll = nonbs.Where(e => !e.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue && !e.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue).ToList();
            var commList = itemNonbAll.Select(o => o.MasterCommodity_Guid).Distinct();
            var templateComm = _masterCommodityRepository.GetAllCommodityBySite(siteGuid, flagIncludeDisable: true)
                               .Where(o => commList.Contains(o.MasterCommodity_Guid)).ToList();

            var itemNonb = itemNonbAll.Join(templateComm,
                           n => n.MasterCommodity_Guid,
                           t => t.MasterCommodity_Guid,
                           (n, t) => n);
            var result = itemNonb.GroupBy(g => g.MasterCommodity_Guid).Select(o =>
            {
                var comm = templateComm.FirstOrDefault(e => e.MasterCommodity_Guid == o.Key);
                var commQty = o.Sum(x => x.Quantity) ?? 0;
                var preQty = (comm.CommodityAmount * comm.CommodityValue) * commQty;
                return new VaultBalanceNonbarcodeModelView
                {
                    CommodityGuid = o.Key.GetValueOrDefault(),
                    CommodityName = comm?.CommodityName ?? string.Empty,
                    ActualStc = 0,
                    STC = preQty,
                    CommodityValue = comm?.CommodityValue ?? 0,
                    CommodityAmount = comm?.CommodityAmount ?? 0,
                    PreAdviceQty = commQty,
                    ActualQty = 0,
                    ItemState = EnumState.Unchanged,
                    ScanItemState = EnumItemState.NotScan,
                    FlagTemp = false,
                    ColumnInReport = comm?.ColumnInReport ?? string.Empty,
                    CommodityCode = comm?.CommodityCode ?? string.Empty
                };
            }).OrderBy(o => o.CommodityCode == "CX" ? 0 : 1).ThenBy(e => e.ColumnInReport).ThenBy(j => j.CommodityName).ToList();
            return result;
        }
        #endregion

        #region Save and close
        public SystemMessageView SaveAndCloseVaultBalance(VaultBalanceItemsRequest request)
        {
            Guid languageGuid = UserLanguageGuid.GetValueOrDefault();
            SystemMessageView response = new SystemMessageView();
            var vaultBalanceHeader = _vaultBalanceHeaderRepository.FindById(request.VaultBalanceGuid);
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    if (vaultBalanceHeader != null)
                    {
                        //Update vault state to on hold
                        UpdateVaultBalance(vaultBalanceHeader, EnumVaultState.OnHold);

                        //Insert Item Scan
                        Guid vaultBalGuid = request.VaultBalanceGuid.GetValueOrDefault();
                        RemoveTempSeals(vaultBalGuid);
                        InsertTempSeals(request.SealList, vaultBalGuid, vaultBalanceHeader.FlagModeBlind);

                        RemoveTempNonbarcode(vaultBalGuid);
                        InsertTempNonbarcode(request.NonbarcodeList, vaultBalGuid);
                        _uow.Commit();

                        response = _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView();
                        transection.Complete();
                    }
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response = _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
                }
            }
            return response;
        }
        private void InsertTempSeals(IEnumerable<VaultBalanceSealModelView> sealList, Guid vaultBalanceGuid, bool flagModeBlind)
        {
            var insertSeal = new List<TblVaultBalanceSealAndMaster>();
            var itemSeal = sealList.Where(o => o.ScanItemState == EnumItemState.Scanned || o.ScanItemState == EnumItemState.Overage);
            foreach (var o in itemSeal)
            {
                var row = new TblVaultBalanceSealAndMaster
                {
                    Guid = Guid.NewGuid(),
                    VaultBalanceHeader_Guid = vaultBalanceGuid,
                    SealNo = o.SealNo,
                    UserCreated = UserName,
                    DatetimeCreated = LocalClientDateTime,
                    UniversalDatetimeCreated = UniversalDatetime
                };

                if (!flagModeBlind)
                {
                    if (o.JobGuid.HasValue)
                    {
                        row.MasterActualJobHeader_Guid = o?.JobGuid;
                        row.MasterActualJobItemsSeal_Guid = o?.Guid;
                        if (o.LiabilityGuid.HasValue)
                        {
                            row.MasterActualJobItemsLiability_Guid = o?.LiabilityGuid;
                        }
                        row.JobNo = o?.JobNo;
                    }

                    if (o.IsMasterLoc || o.IsMasterRoute)
                    {
                        row.MasterConAndDeconsolidate_Header_Guid = o?.Guid;
                    }

                    row.CommodityName = o?.Commodity;
                    row.Currency = o?.Currency;
                    row.STC = o.STC;

                    row.MasterCustomer_Location_Pickup = o?.PickUpLocation;
                    row.MasterCustomer_Location_Delivery = o?.DeliveryLocation;
                    row.SystemServiceJobTypeName = o?.ServiceType;
                    row.WorkDate = o?.WorkDate;
                }
                insertSeal.Add(row);
            }
            _vaultBalanceSealAndMasterRepository.CreateRange(insertSeal);
        }
        private void InsertTempNonbarcode(IEnumerable<VaultBalanceNonbarcodeModelView> nonbList, Guid vaultBalanceGuid)
        {
            var insertNonbarcode = nonbList.Select(o => new TblVaultBalanceNonbarcode
            {
                Guid = Guid.NewGuid(),
                VaultBalanceHeader_Guid = vaultBalanceGuid,
                Commodity_Guid = o.CommodityGuid,
                PreAdviceQty = o.PreAdviceQty,
                ActualQty = o.ActualQty,
                UserCreated = UserName,
                DatetimeCreated = LocalClientDateTime
            });
            _vaultBalanceNonbarcodeRepository.CreateRange(insertNonbarcode);
        }
        private void RemoveTempSeals(Guid vaultBalanceGuid)
        {
            var tempSeals = _vaultBalanceSealAndMasterRepository.FindAll(o => o.VaultBalanceHeader_Guid == vaultBalanceGuid);
            if (tempSeals != null && tempSeals.Any())
            {
                _vaultBalanceSealAndMasterRepository.RemoveRange(tempSeals);
            }
        }
        private void RemoveTempNonbarcode(Guid vaultBalanceGuid)
        {
            var tempNonbs = _vaultBalanceNonbarcodeRepository.FindAll(o => o.VaultBalanceHeader_Guid == vaultBalanceGuid);
            if (tempNonbs != null && tempNonbs.Any())
            {
                _vaultBalanceNonbarcodeRepository.RemoveRange(tempNonbs);
            }
        }
        #endregion

        #region Verify
        public SystemMessageView VerifyVaultBalance(string username, string password)
        {
            int msgId = HasPermission(username, password) ? 0 : -64;
            var resMsg = _systemMessageRepository.FindByMsgId(msgId, UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(msgId == 0);
            resMsg.IsWarning = msgId == -64;
            return resMsg;
        }
        private bool HasPermission(string userName, string passWord)
        {
            var authenUser = _userService.GetAuthenLocal(userName, passWord, SystemHelper.Ocean_ApplicationId);
            if (authenUser == null)
            {
                return false;
            }

            var verifyUser = _userService.GetMenuCommandInUserByCommandId(authenUser.GuidMasterUser, SystemHelper.Ocean_ApplicationId, EnumMenuCommandId.VerifyVaultBalance);
            if (verifyUser == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Mapping
        public IEnumerable<VaultBalanceMappingModelView> GetVaultBalanceSealMapping(VaultBalanceItemsRequest request)
        {
            IEnumerable<VaultBalanceMappingModelView> result = Enumerable.Empty<VaultBalanceMappingModelView>();
            EnumItemState[] stateItem = new EnumItemState[] { EnumItemState.Scanned, EnumItemState.Overage };
            var sealNoScanList = request.SealList.Where(e => stateItem.Contains(e.ScanItemState)).Select(o => o.SealNo.ToLower()).ToList();
            if (request.FlagBlindScanning)
            {
                var sealDetailList = GetDetailVaultBalance(request, isSealMapping: true).SealList;

                var groupSeal = sealDetailList.Where(e => sealNoScanList.Contains(e.SealNo.ToLower()))
                                .GroupBy(g => g.SealNo.ToLower())
                                .Select(o =>
                                {
                                    int countSealScan = sealNoScanList.Count(e => e.ToLower() == o.Key);
                                    return new
                                    {
                                        sealNo = o.Key.ToLower(),
                                        display = o.Count() > countSealScan
                                    };
                                })
                                .Where(x => x.display)
                                .Select(o => o.sealNo).ToList();

                result = sealDetailList.Where(e => groupSeal.Contains(e.SealNo.ToLower()))
                                  .Select(o =>
                                  {
                                      return new VaultBalanceMappingModelView
                                      {
                                          Guid = o.Guid.GetValueOrDefault(),
                                          JobGuid = o.JobGuid,
                                          JobNo = o.JobNo,
                                          ConsolidateType = o.ConsolidateType,
                                          PickUpLocation = o.PickUpLocation,
                                          DeliveryLocation = o.DeliveryLocation,
                                          SealNo = o.SealNo
                                      };
                                  });
            }

            return result;
        }
        private IEnumerable<ConsolidateDetailModel> GetConsolidateType(List<Guid> masterGuid, Guid siteGuid)
        {
            List<int> conStatus = new List<int> { 1, 2, 3 };
            var tblConStatus = _systemConAndDeconsolidateStatusRepository.FindAllAsQueryable(o => conStatus.Contains(o.StatusID)).Select(o => o.Guid).ToList();
            var masterCon = _masterConAndDeconsolidate_HeaderRepository.FindByMasterID_Guids(masterGuid)
                            .Where(o => o.MasterSite_Guid == siteGuid && tblConStatus.Contains(o.SystemCoAndDeSolidateStatus_Guid)).ToList();

            var siteGuidList = masterCon.Where(e => e.Destination_MasterSite_Guid != null).Select(o => o.Destination_MasterSite_Guid.GetValueOrDefault()).ToList();
            var masterSite = _masterSiteRepository.FindAllAsQueryable(e => siteGuidList.Contains(e.Guid)).ToList();
            var masterRouteGrpDetail = masterCon.Where(e => e.MasterRouteGroup_Detail_Guid != null).Select(e => e.TblMasterRouteGroup_Detail).ToList();
            var routeGrpGuidList = masterRouteGrpDetail.Select(e => e.MasterRouteGroup_Guid).ToList();
            var masterRouteGrp = _masterRouteGroupRepository.FindAll(e => routeGrpGuidList.Contains(e.Guid)).ToList();
            var cusGuidList = masterCon.Where(e => e.TblMasterCustomerLocation != null).Select(e => e.TblMasterCustomerLocation.MasterCustomer_Guid).ToList();
            var masterCus = _masterCustomerRepository.FindAllAsQueryable(e => cusGuidList.Contains(e.Guid)).ToList();
            var result = masterCon.Select(o =>
            {
                string site = masterSite.FirstOrDefault(s => s.Guid == o.Destination_MasterSite_Guid)?.SiteName ?? string.Empty;
                string routeGrp = masterRouteGrp.FirstOrDefault(r => o.TblMasterRouteGroup_Detail != null && r.Guid == o.TblMasterRouteGroup_Detail.MasterRouteGroup_Guid)?.MasterRouteGroupName ?? string.Empty;
                string customer = masterCus.FirstOrDefault(c => o.TblMasterCustomerLocation != null && c.Guid == o.TblMasterCustomerLocation.MasterCustomer_Guid)?.CustomerFullName ?? string.Empty;
                string consolidateType = GetConsolidateType(o, site, routeGrp, customer);
                return new ConsolidateDetailModel()
                {
                    MasterGuid = o.Guid,
                    MasterID = o.MasterID,
                    ConsolidateType = consolidateType
                };
            }).ToList();
            return result;
        }
        private string GetConsolidateType(TblMasterConAndDeconsolidate_Header consolidate, string site, string routeGrp, string customer)
        {
            bool isInterBr = consolidate.SystemConsolidateSourceID == 3;
            bool isMultiBr = consolidate.SystemConsolidateSourceID == 4;
            bool isRoute = consolidate.MasterRouteGroup_Detail_Guid.HasValue;
            bool isLoc = !consolidate.MasterRouteGroup_Detail_Guid.HasValue && consolidate.MasterCustomerLocation_Guid.HasValue;

            string conType = string.Empty;
            string conDetail = string.Empty;
            if (isInterBr)
            {
                conType = EnumHelper.GetDescription(EnumConsolidateType.Interbranch);
                conDetail = site;
            }
            if (isMultiBr)
            {
                conType = EnumHelper.GetDescription(EnumConsolidateType.MultiBranch);
                conDetail = site;
            }
            if (isRoute)
            {
                conType = EnumHelper.GetDescription(EnumConsolidateType.Route);
                conDetail = routeGrp + "-" + consolidate.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName;
            }
            if (isLoc)
            {
                conType = EnumHelper.GetDescription(EnumConsolidateType.Location);
                conDetail = customer + "-" + consolidate.TblMasterCustomerLocation.BranchName;
            }
            return conType + "-" + conDetail;
        }
        #endregion

        #region Summary
        public IEnumerable<VaultBalanceSummaryModelView> GetVaultBalanceSummary(VaultBalanceItemsRequest request)
        {
            List<VaultBalanceSummaryModelView> summaryResult = new List<VaultBalanceSummaryModelView>();
            if (request.FlagBlindScanning)
            {
                var nonbsSummary = SummaryNonbarcode_Blind(request);
                summaryResult.AddRange(nonbsSummary);
                var sealsSummary = SummarySeal_Blind(request);
                summaryResult.AddRange(sealsSummary);
            }
            else
            {
                var nonbsSummary = SummaryNonbarcode(request.NonbarcodeList);
                summaryResult.AddRange(nonbsSummary);
                var sealsSummary = SummarySeal(request.SealList);
                summaryResult.AddRange(sealsSummary);
            }
            return summaryResult;
        }

        private IEnumerable<VaultBalanceSummaryModelView> SummaryNonbarcode_Blind(VaultBalanceItemsRequest request)
        {
            var nonbs = _masterActualJobItemsCommodityRepository.FindItemByPrevault(request.PrevaultGuid)
                        .Where(o => o.MasterCommodity_Guid.HasValue)
                        .GroupBy(g => g.MasterCommodity_Guid)
                        .Select(o => new { commodityGuid = o.Key.GetValueOrDefault(), commodityQty = o.Sum(s => s.Quantity.GetValueOrDefault()) }).ToList();

            var mergeCommo = nonbs.Select(o => o.commodityGuid).Union(request.NonbarcodeList.Select(o => o.CommodityGuid));
            var commodityList = _masterCommodityRepository.FindAllAsQueryable(o => mergeCommo.Contains(o.Guid)).ToList();

            // expectQty: Qty in system current , actualQty: Qty scanned
            var result = mergeCommo.Select(o =>
            {
                var comm = commodityList.FirstOrDefault(e => e.Guid == o);
                var commName = comm?.CommodityName ?? string.Empty;
                var actualQty = request.NonbarcodeList.FirstOrDefault(e => e.CommodityGuid == o)?.ActualQty ?? 0;
                var expectQty = nonbs.FirstOrDefault(e => e.commodityGuid == o)?.commodityQty ?? 0;
                var isShort = actualQty < expectQty;
                var isOver = actualQty > expectQty;
                return new VaultBalanceSummaryModelView
                {
                    ItemGuid = o,
                    ItemName = commName,
                    Shortage = isShort ? expectQty - actualQty : 0,
                    Overage = isOver ? actualQty - expectQty : 0,
                    ColumnInReport = comm?.ColumnInReport,
                    CommodityCode = comm?.CommodityCode,
                    ItemType = EnumItemType.Nonbarcode

                };
            }).Where(o => !(o.Shortage == 0 && o.Overage == 0))
            .OrderBy(o => o.CommodityCode.Contains("CX") ? 0 : 1).ThenBy(e => e.ColumnInReport).ThenBy(j => j.ItemName).ToList();

            return result;
        }

        private IEnumerable<VaultBalanceSummaryModelView> SummarySeal_Blind(VaultBalanceItemsRequest request)
        {
            var seals = _masterActualJobItemsSealRepository.FindItemByPrevault(request.PrevaultGuid).ToList();
            var nonbs = _masterActualJobItemsCommodityRepository.FindMasterByPrevault(request.PrevaultGuid).ToList();
            var itemJobs = seals.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault())
                          .Union(nonbs.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault())).ToList();

            var jobs = _masterActualJobHeaderRepository.GetJobDetailVaultBalance(itemJobs);
            var sealAndMaster = GetItemVaultBalanceSealAndMaster(seals, nonbs, jobs, request.SiteGuid)
                                .OrderBy(o => o.JobGuid.HasValue).ThenBy(o => o.SealNo).ToList();

            var result = CalSealSummary_Blind(request, sealAndMaster);
            return result;
        }
        private List<VaultBalanceSummaryModelView> CalSealSummary_Blind(VaultBalanceItemsRequest request, List<VaultBalanceSealModelView> sealAndMaster)
        {
            List<VaultBalanceSummaryModelView> result = new List<VaultBalanceSummaryModelView>();
            var itemSeal = sealAndMaster.Select(o => o.SealNo);
            var sealsScan = request.SealList.Where(o => o.ScanItemState == EnumItemState.Scanned).Select(o => o.SealNo).OrderBy(o => o).ToList();

            #region Shortage
            List<ShortageSealModelView> sealShortList = new List<ShortageSealModelView>();

            var sealsShortScan = sealAndMaster.Where(e => !string.IsNullOrEmpty(e.SealNo) && !sealsScan.Contains(e.SealNo, StringComparer.OrdinalIgnoreCase))
                .Select(o => new ShortageSealModelView { Guid = o.Guid, SealNo = o.SealNo }).ToList();
            sealShortList.AddRange(sealsShortScan);

            var sealsMapping = request.SealItemsMapping.Where(e => !e.FlagScan).OrderByDescending(o => o.JobGuid).ThenBy(o => o.SealNo)
                .Select(o => new ShortageSealModelView { Guid = o.Guid, SealNo = o.SealNo }).ToList();
            sealShortList.AddRange(sealsMapping);

            List<Guid> alreadyStampShort = new List<Guid>();
            var summaryShortList = sealShortList.OrderBy(r => r.SealNo).Select(o =>
            {
                var actualSeal = sealAndMaster.Where(s => !alreadyStampShort.Contains(s.Guid.GetValueOrDefault())).FirstOrDefault(e => e.Guid == o.Guid);
                var itemGuid = actualSeal?.Guid ?? Guid.NewGuid();
                bool isMaster = actualSeal?.JobGuid == null;
                alreadyStampShort.Add(itemGuid);
                return new VaultBalanceSummaryModelView
                {
                    ItemGuid = itemGuid,
                    ItemName = o.SealNo,
                    JobGuid = actualSeal?.JobGuid,
                    Shortage = 1,
                    Overage = 0,
                    ItemType = isMaster ? EnumItemType.Consolidate : EnumItemType.Seal
                };
            });
            result.AddRange(summaryShortList);
            #endregion

            #region Overage
            List<string> sealOverList = new List<string>();
            var sealOverScan = sealsScan.Where(e => !itemSeal.Contains(e, StringComparer.OrdinalIgnoreCase));
            sealOverList.AddRange(sealOverScan);

            var sealOverMapping = sealsScan.Where(e => !sealOverScan.Contains(e));
            var sealDistinct = sealOverMapping.Distinct(StringComparer.OrdinalIgnoreCase);
            foreach (var item in sealDistinct)
            {
                var sealActualList = itemSeal.Count(e => e.ToLower() == item.ToLower());
                var sealScanList = sealOverMapping.Where(e => e.ToLower() == item.ToLower());
                if (sealScanList.Count() > sealActualList)
                {
                    sealOverList.AddRange(sealScanList.Skip(sealActualList));
                }
            }
            var summaryOverList = sealOverList
                .Select(o => new VaultBalanceSummaryModelView
                {
                    ItemGuid = Guid.NewGuid(),
                    ItemName = o,
                    Shortage = 0,
                    Overage = 1,
                    ItemType = EnumItemType.Seal
                });
            result.AddRange(summaryOverList);
            #endregion

            return result.OrderBy(o => o.ItemName).ToList();
        }

        private IEnumerable<VaultBalanceSummaryModelView> SummaryNonbarcode(IEnumerable<VaultBalanceNonbarcodeModelView> nonbsList)
        {
            var commoGuidList = nonbsList.Select(o => o.CommodityGuid);
            var commodityList = _masterCommodityRepository.FindAllAsQueryable(o => commoGuidList.Contains(o.Guid)).ToList();
            var nonbsGrpSummary = nonbsList.GroupBy(e => e.CommodityGuid)
                           .Select(o =>
                           {
                               var preAdviceQty = o.Sum(s => s.PreAdviceQty);
                               var actualQty = o.Sum(s => s.ActualQty);
                               bool isShort = preAdviceQty > actualQty;
                               bool isOver = preAdviceQty < actualQty;
                               var comm = commodityList.FirstOrDefault(e => e.Guid == o.Key);
                               return new VaultBalanceSummaryModelView
                               {
                                   ItemGuid = o.Key,
                                   ItemName = o.FirstOrDefault()?.CommodityName ?? string.Empty,
                                   Shortage = isShort ? preAdviceQty - actualQty : 0,
                                   Overage = isOver ? actualQty - preAdviceQty : 0,
                                   ColumnInReport = comm?.ColumnInReport ?? string.Empty,
                                   CommodityCode = comm?.CommodityCode ?? string.Empty,
                                   ItemType = EnumItemType.Nonbarcode
                               };
                           })
                           .Where(o => !(o.Overage == 0 && o.Shortage == 0))
                           .OrderBy(o => o.CommodityCode.Contains("CX") ? 0 : 1).ThenBy(e => e.ColumnInReport).ThenBy(j => j.ItemName).ToList();

            return nonbsGrpSummary;
        }
        private IEnumerable<VaultBalanceSummaryModelView> SummarySeal(IEnumerable<VaultBalanceSealModelView> sealsList)
        {
            List<VaultBalanceSummaryModelView> sealDiscrepancy = new List<VaultBalanceSummaryModelView>();
            var sealShort = sealsList.Where(e => e.ScanItemState == EnumItemState.NotScan)
                            .Select(o => new VaultBalanceSummaryModelView()
                            {
                                ItemGuid = o.Guid,
                                ItemName = o.SealNo,
                                Shortage = 1,
                                Overage = 0,
                                ItemType = o.JobGuid == null ? EnumItemType.Consolidate : EnumItemType.Seal
                            });
            sealDiscrepancy.AddRange(sealShort);

            var sealOver = sealsList.Where(e => e.ScanItemState == EnumItemState.Overage)
                            .Select(o => new VaultBalanceSummaryModelView()
                            {
                                ItemGuid = o.Guid,
                                ItemName = o.SealNo,
                                Shortage = 0,
                                Overage = 1,
                                ItemType = EnumItemType.Seal
                            });
            sealDiscrepancy.AddRange(sealOver);
            return sealDiscrepancy.OrderBy(o => o.ItemName);
        }
        #endregion

        #region Submit
        public SystemMessageView SubmitVaultBalance(VaultBalanceItemsRequest request)
        {
            Guid languageGuid = UserLanguageGuid.GetValueOrDefault();
            SystemMessageView response = new SystemMessageView();

            Guid vaultBalGuid = request.VaultBalanceGuid.GetValueOrDefault();
            var vaultBalanceHeader = _vaultBalanceHeaderRepository.FindById(vaultBalGuid);
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    if (vaultBalanceHeader != null)
                    {
                        if (request.FlagBlindScanning)
                        {
                            var detailReq = new VaultBalanceDetailRequest()
                            {
                                SiteGuid = request.SiteGuid,
                                PrevaultGuid = request.PrevaultGuid,
                                FlagBlindScanning = request.FlagBlindScanning,
                                FlagDiscrepancyVaultBalance = request.FlagDiscrepancyVaultBalance,
                                VaultBalanceGuid = request.VaultBalanceGuid
                            };
                            var detailItemInvault = GetDetailVaultBalance(detailReq);
                            UpdateVaultBalanceHeaderWithBlind(request, detailItemInvault, vaultBalanceHeader);
                            UpdateVaultBalanceNonbarcodeWithBlind(request, detailItemInvault);
                        }
                        else
                        {
                            UpdateVaultBalanceHeader(request, vaultBalanceHeader);
                            UpdateVaultBalanceNonbarcode(request);
                        }

                        #region Insert Discrepancies
                        if (request.SummaryList.Any())
                        {
                            UpdateVaultDiscrepanciesSeal(request);
                        }
                        #endregion
                    }
                    _uow.Commit();

                    response = _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView(true);
                    response.IsWarning = false;
                    transection.Complete();
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response = _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
                    response.IsWarning = true;
                }
            }
            return response;
        }

        #region Submit for data blind
        private void UpdateVaultBalanceHeaderWithBlind(VaultBalanceItemsRequest request, VaultBalanceDetailReponse valtBalanceDetail, TblVaultBalanceHeader vaultBalanceHeader)
        {
            var sealsScanList = request.SealList.Where(o => o.ScanItemState == EnumItemState.Scanned).ToList();
            var sealsInVaultList = valtBalanceDetail.SealList;
            var summarySealList = request.SummaryList.Where(o => o.ItemType != EnumItemType.Nonbarcode);

            var sealsInVault = sealsInVaultList.Where(o => !o.IsMasterLoc && !o.IsMasterRoute).OrderBy(e => e.SealNo).ToList();
            var masterLocInVault = sealsInVaultList.Where(o => o.IsMasterLoc).OrderBy(e => e.SealNo).ToList();
            var masterRouteInVault = sealsInVaultList.Where(o => o.IsMasterRoute).OrderBy(e => e.SealNo).ToList();

            vaultBalanceHeader.Seal_PreAdviceQty = sealsInVault.Count;
            vaultBalanceHeader.LocationMasterSeal_PreAdviceQty = masterLocInVault.Count;
            vaultBalanceHeader.RouteMasterSeal_PreAdviceQty = masterRouteInVault.Count;

            var sealNoInVault = sealsInVault.Select(o => o.SealNo.ToLower()).ToList();
            var locMasterInVault = masterLocInVault.Select(o => o.SealNo.ToLower()).ToList();
            var routeMasterInVault = masterRouteInVault.Select(o => o.SealNo.ToLower()).ToList();

            var sealNoScan = sealsScanList.Select(o => o.SealNo.ToLower());
            var countItemOver = summarySealList.Count(o => o.Overage > 0);

            int sealQty = 0;
            int locQty = 0;
            int routeQty = 0;

            foreach (var itemScan in sealNoScan)
            {
                bool isSeal = sealNoInVault.Any(e => e == itemScan);
                if (isSeal)
                {
                    sealQty += 1;
                    int i = sealNoInVault.ConvertAll(o => o.ToLower()).IndexOf(itemScan.ToLower());
                    sealNoInVault.RemoveAt(i);
                    continue;
                }

                bool isLoc = locMasterInVault.Any(e => e == itemScan);
                if (isLoc)
                {
                    locQty += 1;
                    int i = locMasterInVault.ConvertAll(o => o.ToLower()).IndexOf(itemScan.ToLower());
                    locMasterInVault.RemoveAt(i);
                    continue;
                }

                bool isRoute = routeMasterInVault.Any(e => e == itemScan);
                if (isRoute)
                {
                    routeQty += 1;
                    int i = routeMasterInVault.ConvertAll(o => o.ToLower()).IndexOf(itemScan.ToLower());
                    routeMasterInVault.RemoveAt(i);
                }
            }

            vaultBalanceHeader.Seal_ActualQty = sealQty + countItemOver;
            vaultBalanceHeader.LocationMasterSeal_ActualQty = locQty;
            vaultBalanceHeader.RouteMasterSeal_ActualQty = routeQty;

            if (request.SummaryList.Any())
            {
                var sealNoShortList = summarySealList.Where(e => e.Shortage > 0).Select(o => o.ItemName).OrderBy(r => r).ToList();
                vaultBalanceHeader.Seal_ShortageQty = summarySealList.Sum(o => o.Shortage);
                vaultBalanceHeader.Seal_ShortageList = string.Join(",", sealNoShortList);

                var sealNoOverList = summarySealList.Where(e => e.Overage > 0).Select(o => o.ItemName).OrderBy(r => r).ToList();
                vaultBalanceHeader.Seal_OverageQty = summarySealList.Sum(o => o.Overage);
                vaultBalanceHeader.Seal_OverageList = string.Join(",", sealNoOverList);
            }

            //Update vault state to completed
            UpdateVaultBalance(vaultBalanceHeader, EnumVaultState.Complete, request.UsernameVerify);
        }
        private void UpdateVaultBalanceNonbarcodeWithBlind(VaultBalanceItemsRequest request, VaultBalanceDetailReponse valtBalanceDetail)
        {
            var nonbScanList = request.NonbarcodeList;
            var nonbInVaultList = valtBalanceDetail.NonbarcodeList;
            var nonbsAll = request.NonbarcodeList.Select(o => o.CommodityGuid).Union(nonbInVaultList.Select(o => o.CommodityGuid));

            var nonbsList = nonbsAll.Select(o =>
            {
                var preAdviceQty = nonbInVaultList.Where(e => e.CommodityGuid == o).Sum(s => s.PreAdviceQty);
                var actualQty = nonbScanList.Where(e => e.CommodityGuid == o).Sum(s => s.ActualQty);
                return new VaultBalanceNonbarcodeModelView
                {
                    CommodityGuid = o,
                    PreAdviceQty = preAdviceQty,
                    ActualQty = actualQty,
                };
            });
            //Remove temp nonbarcode
            RemoveTempNonbarcode(request.VaultBalanceGuid.GetValueOrDefault());
            //Add qty actual nonbarcode
            InsertTempNonbarcode(nonbsList, request.VaultBalanceGuid.GetValueOrDefault());
        }

        #endregion
        private void UpdateVaultBalanceHeader(VaultBalanceItemsRequest request, TblVaultBalanceHeader vaultBalanceHeader)
        {
            var sealList = request.SealList.Where(o => o.ScanItemState == EnumItemState.Scanned || o.ScanItemState == EnumItemState.NotScan);
            var sealScan = request.SealList.Where(o => o.ScanItemState == EnumItemState.Scanned || o.ScanItemState == EnumItemState.Overage);

            vaultBalanceHeader.Seal_PreAdviceQty = sealList.Count(o => !o.IsMasterLoc && !o.IsMasterRoute);
            vaultBalanceHeader.LocationMasterSeal_PreAdviceQty = sealList.Count(o => o.IsMasterLoc);
            vaultBalanceHeader.RouteMasterSeal_PreAdviceQty = sealList.Count(o => o.IsMasterRoute);

            vaultBalanceHeader.Seal_ActualQty = sealScan.Count(o => !o.IsMasterLoc && !o.IsMasterRoute);
            vaultBalanceHeader.LocationMasterSeal_ActualQty = sealScan.Count(o => o.IsMasterLoc);
            vaultBalanceHeader.RouteMasterSeal_ActualQty = sealScan.Count(o => o.IsMasterRoute);

            if (request.SummaryList.Any())
            {
                var sealSummary = request.SummaryList.Where(e => e.ItemType == EnumItemType.Seal || e.ItemType == EnumItemType.Consolidate);
                var sealNoShortList = sealSummary.Where(e => e.Shortage > 0).Select(o => o.ItemName).OrderBy(r => r).ToList();
                vaultBalanceHeader.Seal_ShortageQty = sealSummary.Sum(o => o.Shortage);
                vaultBalanceHeader.Seal_ShortageList = string.Join(",", sealNoShortList);

                var sealNoOverList = sealSummary.Where(e => e.Overage > 0).Select(o => o.ItemName).OrderBy(r => r).ToList();
                vaultBalanceHeader.Seal_OverageQty = sealSummary.Sum(o => o.Overage);
                vaultBalanceHeader.Seal_OverageList = string.Join(",", sealNoOverList);
            }

            //Update vault state to completed
            UpdateVaultBalance(vaultBalanceHeader, EnumVaultState.Complete, request.UsernameVerify);
        }
        private void UpdateVaultBalanceNonbarcode(VaultBalanceItemsRequest request)
        {
            //Remove temp nonbarcode
            RemoveTempNonbarcode(request.VaultBalanceGuid.GetValueOrDefault());
            //Add qty actual nonbarcode
            InsertTempNonbarcode(request.NonbarcodeList, request.VaultBalanceGuid.GetValueOrDefault());
        }

        private void UpdateVaultDiscrepanciesSeal(VaultBalanceItemsRequest request)
        {
            List<TblVaultBalance_Discrepancy> listDiscrepancy = new List<TblVaultBalance_Discrepancy>();
            var sealSummary = request.SummaryList.Where(o => o.ItemType == EnumItemType.Seal || o.ItemType == EnumItemType.Consolidate).ToList();

            var insertSealOver = sealSummary.Where(o => o.Overage > 0)
                .Select(o =>
                 new TblVaultBalance_Discrepancy
                 {
                     Guid = Guid.NewGuid(),
                     MasterSite_Guid = request.SiteGuid,
                     MasterCustomerLocation_InternalDepartment_Guid = request.PrevaultGuid,
                     SealNo = o.ItemName,
                     QtyOverage = o.Overage,
                     MasterReasonType_Guid = o.ReasonTypeGuid,
                     Remarks = o.Remark,
                     UsernameSupervisorVerify = request.UsernameVerify,
                     DatetimeSupervisorVerify = LocalClientDateTime,
                     ClientHostNameScan = _dnsWrapper.ClientHostName,
                     UserCreated = UserName,
                     DatetimeCreated = LocalClientDateTime,
                     UniversalDatetimeCreated = UniversalDatetime

                 });
            listDiscrepancy.AddRange(insertSealOver);

            foreach (var item in sealSummary.Where(o => o.Shortage > 0))
            {
                var sealDetail = request.SealList.FirstOrDefault(e => e.Guid == item.ItemGuid);
                bool isMaster = item.ItemType == EnumItemType.Consolidate;

                var r = new TblVaultBalance_Discrepancy
                {
                    Guid = Guid.NewGuid(),
                    MasterSite_Guid = request.SiteGuid,
                    MasterCustomerLocation_InternalDepartment_Guid = request.PrevaultGuid,
                    UserCreated = UserName,
                    DatetimeCreated = LocalClientDateTime,
                    UniversalDatetimeCreated = UniversalDatetime,
                    SealNo = item.ItemName,
                    QtyOverage = item.Overage,
                    QtyShortage = item.Shortage,
                    MasterReasonType_Guid = item.ReasonTypeGuid,
                    Remarks = item.Remark,
                    UsernameSupervisorVerify = request.UsernameVerify,
                    DatetimeSupervisorVerify = LocalClientDateTime,
                    ClientHostNameScan = _dnsWrapper.ClientHostName
                };

                if (isMaster)
                {
                    r.MasterConAndDeconsolidateHeader_Guid = item.ItemGuid;
                }
                else
                {
                    r.MasterActualJobHeader_Guid = sealDetail?.JobGuid;
                    r.MasterActualJobItemsSeal_Guid = item.ItemGuid;
                }

                r.MasterCustomer_Pickup_Guid = sealDetail?.CustomerPickupGuid == Guid.Empty ? null : sealDetail?.CustomerPickupGuid;
                r.MasterCustomerLocation_Pickup_Guid = sealDetail?.LocationPickupGuid == Guid.Empty ? null : sealDetail?.LocationPickupGuid;
                r.MasterCustomer_Delivery_Guid = sealDetail?.CustomerDeliveryGuid == Guid.Empty ? null : sealDetail?.CustomerDeliveryGuid;
                r.MasterCustomerLocation_Delivery_Guid = sealDetail?.LocationDeliveryGuid == Guid.Empty ? null : sealDetail?.LocationDeliveryGuid;

                listDiscrepancy.Add(r);
            }
            _vaultBalanceDiscrepancyRepository.CreateRange(listDiscrepancy);
        }
        private void UpdateVaultDiscrepanciesNonbarcode(VaultBalanceItemsRequest request)
        {
            var insertNonbarcodeDisc = request.SummaryList.Where(o => o.ItemType == EnumItemType.Nonbarcode)
                .Select(o =>
                {
                    return new TblVaultBalance_Discrepancy
                    {
                        Guid = Guid.NewGuid(),
                        MasterSite_Guid = request.SiteGuid,
                        MasterCustomerLocation_InternalDepartment_Guid = request.PrevaultGuid,
                        SealNo = o.ItemName,
                        MasterCommodity_Guid = o.ItemGuid,
                        QtyShortage = o.Shortage,
                        QtyOverage = o.Overage,
                        MasterReasonType_Guid = o.ReasonTypeGuid,
                        Remarks = o.Remark,
                        UsernameSupervisorVerify = request.UsernameVerify,
                        DatetimeSupervisorVerify = LocalClientDateTime,
                        ClientHostNameScan = _dnsWrapper.ClientHostName,
                        UserCreated = UserName,
                        DatetimeCreated = LocalClientDateTime,
                        UniversalDatetimeCreated = UniversalDatetime
                    };
                });
            _vaultBalanceDiscrepancyRepository.CreateRange(insertNonbarcodeDisc);
        }
        #endregion

        #region Cancel
        public SystemMessageView CancelVaultBalance(Guid vaultBalanceGuid)
        {
            Guid languageGuid = UserLanguageGuid.GetValueOrDefault();
            SystemMessageView response = new SystemMessageView();
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    var vaultBalanceHeader = _vaultBalanceHeaderRepository.FindById(vaultBalanceGuid);
                    if (vaultBalanceHeader != null)
                    {
                        if (vaultBalanceHeader.PreviousStateID == (int)EnumVaultState.OnHold)
                        {
                            UpdateVaultBalance(vaultBalanceHeader, EnumVaultState.OnHold);
                        }
                        else
                        {
                            UpdateVaultBalance(vaultBalanceHeader, EnumVaultState.None);
                        }
                    }
                    _uow.Commit();

                    response = _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView();
                    transection.Complete();
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    response = _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
                }
            }
            return response;
        }
        #endregion

        #region For dolphin
        public VaultBalanceApiResponse ValidateVaultBalanceBlocking(VaultBalanceApiRequest request, Uri requestUri)
        {
            ValidateVaultBalanceAPIActivityRequest(request, requestUri);

            VaultBalanceApiResponse response = new VaultBalanceApiResponse
            {
                IsValid = false
            };

            //True: pass , False: blocking
            var jobsGuid = _masterActualJobServiceStopLegsRepository.FindByDailyRunAndSite(request.DailyRunGuid)
                           .Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault());

            #region Find vault in seal
            var vaultsGuid_Seal = _masterActualJobItemsSealRepository.FindSealByJobList(jobsGuid)
                           .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid.GetValueOrDefault()).Distinct();

            response.IsValid = !(_vaultBalanceHeaderRepository.FindAllAsQueryable(o => o.MasterSite_Guid == request.SiteGuid &&
                                o.StateID == (int)EnumVaultState.Process)
                                .Join(vaultsGuid_Seal,
                                h => h.MasterCustomerLocation_InternalDepartment_Guid,
                                v => v,
                                (h, v) => h).Any());
            #endregion

            #region Find vault in nonbarcode
            if (response.IsValid)
            {
                var vaultsGuid_Nonb = _masterActualJobItemsCommodityRepository.FindCommodityByListJob(jobsGuid)
                          .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid.GetValueOrDefault()).Distinct();

                response.IsValid = !(_vaultBalanceHeaderRepository.FindAllAsQueryable(o => o.MasterSite_Guid == request.SiteGuid &&
                                    o.StateID == (int)EnumVaultState.Process)
                                    .Join(vaultsGuid_Nonb,
                                    h => h.MasterCustomerLocation_InternalDepartment_Guid,
                                    v => v,
                                    (h, v) => h).Any());
            }
            #endregion

            ValidateVaultBalanceAPIActivityResponse(response, requestUri, request.UserCreated);

            return response;
        }

        #region -- API Activity
        private void ValidateVaultBalanceAPIActivityRequest(VaultBalanceApiRequest request, Uri requestUri)
        {
            var msg = $"Request: Get request from Dolphin EE with url: {requestUri}, request:  {JsonConvert.SerializeObject(request)}";
            _systemService.CreateLogActivity(SystemActivityLog.DPOOAPIActivity, msg, request.UserCreated, SystemHelper.CurrentIpAddress, ApiSession.Application_Guid.GetValueOrDefault());
        }
        private void ValidateVaultBalanceAPIActivityResponse(VaultBalanceApiResponse response, Uri requestUri, string userCreated)
        {
            var msg = $"Response: Send response to Dolphin EE with url: {requestUri}, response: {JsonConvert.SerializeObject(response)}";
            _systemService.CreateLogActivity(SystemActivityLog.DPOOAPIActivity, msg, userCreated, SystemHelper.CurrentIpAddress, ApiSession.Application_Guid.GetValueOrDefault());
        }
        #endregion

        #endregion
    }
}
