

using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.PreVault;
using Bgt.Ocean.Models.Reports.VaultBalance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance
{
    public interface IVaultBalanceHeaderRepository : IRepository<TblVaultBalanceHeader>
    {
        IEnumerable<VaultStateModel> GetVaultBalanceStateByInternalDepartment(IEnumerable<Guid?> InternalGuidList);
        VaultValanceModelReport_Main GetValueBalanceReport(Guid VaultBalanceHeaderGuid, string UserFormatDate);
        IEnumerable<Guid> GetInternalDepartmentBySiteGuid(Guid? siteGuid);
        IEnumerable<Guid> GetInternalDepartmentByItemsGuid(IEnumerable<Guid?> SealGuidList, IEnumerable<Guid?> CommodityGuidList, IEnumerable<Guid?> ConsolidateGuidList);
    }

    public class VaultBalanceHeaderRepository : Repository<OceanDbEntities, TblVaultBalanceHeader>, IVaultBalanceHeaderRepository
    {
        public VaultBalanceHeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<Guid> GetInternalDepartmentByItemsGuid(IEnumerable<Guid?> SealGuidList, IEnumerable<Guid?> CommodityGuidList, IEnumerable<Guid?> ConsolidateGuidList)
        {
            IQueryable<Guid?> poolingVaults = null;
            Action<IQueryable<Guid?>> AddPooling = (o) =>
            {
                poolingVaults = poolingVaults == null ? o : poolingVaults.Union(o);
            };

            if (SealGuidList != null && SealGuidList.Any())
            {
                var baseVault = DbContext.TblMasterActualJobItemsSeal.Where(o => SealGuidList.Contains(o.Guid) && o.MasterCustomerLocation_InternalDepartment_Guid != null)
                                         .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid);
                AddPooling(baseVault);
            }

            if (CommodityGuidList != null && CommodityGuidList.Any())
            {
                var baseVault = DbContext.TblMasterActualJobItemsCommodity.Where(o => CommodityGuidList.Contains(o.Guid) && o.MasterCustomerLocation_InternalDepartment_Guid != null)
                                        .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid);

                AddPooling(baseVault);
            }

            if (ConsolidateGuidList != null && ConsolidateGuidList.Any())
            {
                var sMasterID = DbContext.TblMasterActualJobItemsSeal.Where(o => ConsolidateGuidList.Contains(o.MasterConAndDeconsolidateHeaderMasterID_Guid) && o.MasterCustomerLocation_InternalDepartment_Guid != null)
                                       .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid);
                var sMasterIDRoute = DbContext.TblMasterActualJobItemsSeal.Where(o => ConsolidateGuidList.Contains(o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid) && o.MasterCustomerLocation_InternalDepartment_Guid != null)
                                       .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid);
                var cMasterID = DbContext.TblMasterActualJobItemsCommodity.Where(o => ConsolidateGuidList.Contains(o.MasterConAndDeconsolidateHeaderMasterID_Guid) && o.MasterCustomerLocation_InternalDepartment_Guid != null)
                                       .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid);
                var cMasterIDRoute = DbContext.TblMasterActualJobItemsCommodity.Where(o => ConsolidateGuidList.Contains(o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid) && o.MasterCustomerLocation_InternalDepartment_Guid != null)
                                     .Select(o => o.MasterCustomerLocation_InternalDepartment_Guid);
                var baseVault = sMasterID
                      .Union(sMasterIDRoute)
                      .Union(cMasterID)
                      .Union(cMasterIDRoute);

                AddPooling(baseVault);
            }

            return poolingVaults == null ? Enumerable.Empty<Guid>() : poolingVaults.Select(o => (Guid)o).ToList();
        }

        public IEnumerable<Guid> GetInternalDepartmentBySiteGuid(Guid? siteGuid)
        {

            //Get Internal Department (Cash Delivery Import)
            var result = (from cus in DbContext.TblMasterCustomer
                          join loc in DbContext.TblMasterCustomerLocation on cus.Guid equals loc.MasterCustomer_Guid
                          join Int in DbContext.TblMasterCustomerLocation_InternalDepartment on loc.Guid equals Int.MasterCustomerLocation_Guid
                          join dtype in DbContext.TblSystemInternalDepartmentType on Int.InternalDepartmentType equals dtype.Guid
                          where cus.FlagChkCustomer == false
                          && loc.MasterSite_Guid == siteGuid
                          && dtype.InternalDepartmentID == 2
                          && !dtype.FlagDisable
                          select Int.Guid);

            return result;
        }

        public IEnumerable<VaultStateModel> GetVaultBalanceStateByInternalDepartment(IEnumerable<Guid?> InternalGuidList)
        {
            var intVaultState = new[] { (int)EnumVaultState.Complete, (int)EnumVaultState.None };
            var result = DbContext.TblVaultBalanceHeader.Where(o => !intVaultState.Any(s => s == o.StateID) && InternalGuidList.Any(i => i == o.MasterCustomerLocation_InternalDepartment_Guid)).ToList();
            var vaultList = DbContext.TblMasterCustomerLocation_InternalDepartment.Where(o => InternalGuidList.Any(v => v == o.Guid)).ToList();
            var valutState = InternalGuidList.Select(o =>
            {
                var baseVaults = result.Where(e => e.MasterCustomerLocation_InternalDepartment_Guid == o);
                var baseInternal = vaultList.FirstOrDefault(i => i.Guid == o);
                var state = EnumVaultState.Complete;
                state = result.Any(e => e.StateID == (int)EnumVaultState.OnHold) ? EnumVaultState.OnHold : state;
                state = result.Any(e => e.StateID == (int)EnumVaultState.Process) ? EnumVaultState.Process : state;
                var baseVault = baseVaults.FirstOrDefault(s => s.StateID == (int)state);
                return new VaultStateModel
                {
                    VaultBalanceHeader_Guid = baseVault?.Guid,
                    InternalDepartment_Guid = o,
                    VaultState = state,
                    InternalFullName = (baseInternal?.InternalDepartmentReferenceCode ?? string.Empty) + " - " + (baseInternal?.InterDepartmentName ?? string.Empty)
                };
            }).ToList();

            return valutState;
        }

        public VaultValanceModelReport_Main GetValueBalanceReport(Guid VaultBalanceHeaderGuid, string UserFormatDate)
        {
            var ReturnModel = new VaultValanceModelReport_Main();

            var queryUsername = (from Head in DbContext.TblVaultBalanceHeader
                                 join Detail in DbContext.TblVaultBalanceDetail on Head.Guid equals Detail.VaultBalanceHeader_Guid
                                 where Head.Guid == VaultBalanceHeaderGuid
                                 select new
                                 {
                                     Detail.UserBalance,
                                     Detail.DatetimeBalance,
                                 }).ToList();

            var queryHeader = (from Head in DbContext.TblVaultBalanceHeader
                               where Head.Guid == VaultBalanceHeaderGuid
                               select new
                               {
                                   Head.LocationMasterSeal_ActualQty,
                                   Head.LocationMasterSeal_PreAdviceQty,

                                   Head.RouteMasterSeal_ActualQty,
                                   Head.RouteMasterSeal_PreAdviceQty,

                                   Head.Seal_ActualQty,
                                   Head.Seal_PreAdviceQty,

                                   Head.Seal_OverageList,
                                   Head.Seal_OverageQty,

                                   Head.Seal_ShortageList,
                                   Head.Seal_ShortageQty,

                                   Head.DatetimeSupervisorVerify,
                                   Head.UsernameSupervisorVerify
                               }).ToList().FirstOrDefault();

            var queryNonbar = (from Head in DbContext.TblVaultBalanceHeader
                               join NonBar in DbContext.TblVaultBalanceNonbarcode on Head.Guid equals NonBar.VaultBalanceHeader_Guid
                               into NonbarL
                               from NonBar in NonbarL.DefaultIfEmpty()
                               join commodity in DbContext.TblMasterCommodity on NonBar.Commodity_Guid equals commodity.Guid
                               into commodityL
                               from commodity in commodityL.DefaultIfEmpty()
                               where Head.Guid == VaultBalanceHeaderGuid
                               select new
                               {
                                   NonBar.Commodity_Guid,
                                   commodity.CommodityName,
                                   commodity.CommodityCode,
                                   commodity.ColumnInReport,

                                   // Balance
                                   PreAdviceQty = (int?)NonBar.PreAdviceQty,
                                   ActualQty = (int?)NonBar.ActualQty,                             
                               }).ToList();

            ReturnModel.datetimeVerify = queryHeader.DatetimeSupervisorVerify.HasValue? queryHeader.DatetimeSupervisorVerify.Value.ToString(UserFormatDate) : "";
            ReturnModel.UserVerify = queryHeader.UsernameSupervisorVerify;

            ReturnModel.Header = queryUsername.Select(e => new VaultBalanceModelReport()
            {
                BalanceSeal_Individual_ActualQuantity = queryHeader.Seal_ActualQty,
                BalanceSeal_Individual_PreAdvQuantity = queryHeader.Seal_PreAdviceQty,

                BalanceSeal_LocationMaster_ActualQuantity = queryHeader.LocationMasterSeal_ActualQty,
                BalanceSeal_LocationMaster_PreAdvQuantity = queryHeader.LocationMasterSeal_PreAdviceQty,

                BalanceSeal_RouteMaster_ActualQuantity = queryHeader.RouteMasterSeal_ActualQty,
                BalanceSeal_RouteMaster_PreAdvQuantity = queryHeader.RouteMasterSeal_PreAdviceQty,

                DiscrepanciesSeal_Extra_Quantity = queryHeader.Seal_OverageQty,
                DiscrepanciesSeal_Extra_SealNumber = queryHeader.Seal_OverageList,

                DiscrepanciesSeal_Missing_Quantity = queryHeader.Seal_ShortageQty,
                DiscrepanciesSeal_Missing_SealNumber = queryHeader.Seal_ShortageList,

                datetimeCreate = e.DatetimeBalance.Value.ToString(UserFormatDate),
                UserCreate = e.UserBalance
            }).ToList();

            ReturnModel.ListBalance = queryNonbar.Where(e => e.PreAdviceQty != 0).Select(e => new VaultBalanceModelReport_BalanceNonBar()
            {
                orderCommodity = ((e.CommodityCode == null) ? "1" : e.CommodityCode.Contains("CX")?"0":"1") + "_" + (e.ColumnInReport??"") + "_" + e.CommodityName,
                BalanceNonBar_Actual = e.ActualQty,
                BalanceNonBar_PreAdvice = e.PreAdviceQty,
                BalanceNonBar_Commodity = e.CommodityName
            }).OrderBy(o=>o.orderCommodity).ToList();

            ReturnModel.ListDiscrepency = queryNonbar.Where(e=>(e.ActualQty - e.PreAdviceQty) != 0).Select(e => new VaultBalanceModelReport_DisNonBar()
            {
                orderCommodity = ((e.CommodityCode == null)? "1" : e.CommodityCode.Contains("CX") ? "0" : "1") + "_" + (e.ColumnInReport ?? "") + "_" + e.CommodityName,
                DisNonBar_Commodity = e.CommodityName,
                DisNonBar_Extra = e.ActualQty - e.PreAdviceQty,
                DisNonBar_Missing = e.PreAdviceQty - e.ActualQty
            }).OrderBy(o => o.orderCommodity).ToList();

            return ReturnModel;
        }

    }

}
