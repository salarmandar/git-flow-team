using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.PreVault;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobItemsSealRepository : IRepository<TblMasterActualJobItemsSeal>
    {
        IEnumerable<TblMasterActualJobItemsSeal> FindSeals(IEnumerable<Guid> seals);
        IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> Func_CheckOutDepartment_Seal_Consolidate_Get(Guid prevaultGuid, Guid? internalDepartmentGuid, bool flagTVD, bool flgWithOutOWD);
        void UpdateSeal(IEnumerable<Guid> sealGuid, Guid internalDeptGuid, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        IEnumerable<TblMasterActualJobItemsSeal> GetItemInCon(IEnumerable<Guid> conGuid);
        IEnumerable<TblMasterActualJobItemsSeal> GetItemInCon(Guid conGuid, bool isConR);
        IEnumerable<TblMasterActualJobItemsSeal> GetItemInCon(Guid conGuid);
        IEnumerable<TblMasterActualJobItemsSeal> GetItemInConL(Guid conGuid);
        IEnumerable<TblMasterActualJobItemsSeal> GetItemInConR(Guid conGuid);
        void UpdateSealWithMasterID(IEnumerable<Guid> seals, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        void UpdateSealWithMasterID_Route(IEnumerable<Guid> seals, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        void RemoveSealWithMasterID(IEnumerable<Guid> seals, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime, int flagConsolidate = 0);
        void RemoveSealWithMasterID_Route(IEnumerable<Guid> seals, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        IEnumerable<TblMasterActualJobItemsSeal> FindSealByJobList(IEnumerable<Guid> jobGuids);
        List<TblMasterActualJobItemsSeal> FindSealOnHandByJob(IEnumerable<Guid> JobGuids);
        IEnumerable<TblMasterActualJobItemsSeal> FindItemByPrevault(Guid prevaultGuid);
        IEnumerable<VaultBalanceSealModel> GetSealDetailVaultBalance(List<TblMasterActualJobItemsSeal> itemSeals, IEnumerable<VaultBalanceJobDetailModel> jobDetail);
    }

    public class MasterActualJobItemsSealRepository : Repository<OceanDbEntities, TblMasterActualJobItemsSeal>, IMasterActualJobItemsSealRepository
    {
        public MasterActualJobItemsSealRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterActualJobItemsSeal> FindSeals(IEnumerable<Guid> seals)
        {
            return DbContext.TblMasterActualJobItemsSeal.Where(w => seals.Contains(w.Guid));
        }

        public IEnumerable<TblMasterActualJobItemsSeal> FindSealByJobList(IEnumerable<Guid> jobGuids)
        {
            return DbContext.TblMasterActualJobItemsSeal.Where(s => s.FlagSealDiscrepancies == false)
              .Join(jobGuids,
              s => s.MasterActualJobHeader_Guid,
              j => j,
              (s, j) => s);
        }

        #region For check out department
        public IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> Func_CheckOutDepartment_Seal_Consolidate_Get(Guid prevaultGuid, Guid? internalDepartmentGuid, bool flagTVD, bool flgWithOutOWD)
        {
            return DbContext.Up_OceanOnlineMVC_PrevaultDepartment_Seal_SealConsolidate_ScanOut_Get(prevaultGuid, internalDepartmentGuid, flagTVD, flgWithOutOWD);
        }


        public void UpdateSeal(IEnumerable<Guid> sealGuid, Guid internalDeptGuid, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            #region Update List        
            var data = sealGuid.Select(o => new { SealGuid = o, clientDatetime = clientDatetime, universalDateTime = universalDateTime });

            string sql = $@" UPDATE TblMasterActualJobItemsSeal
                            SET  MasterCustomerLocation_InternalDepartment_Guid = '{internalDeptGuid}' ,
                                MasterCustomerLocation_InternalDepartmentArea_Guid = null,
                                MasterCustomerLocation_InternalDepartmentSubArea_Guid = null,
                                FlagSealDiscrepancies = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = @clientDatetime,
                                UniversalDatetimeModified = @universalDateTime
                            WHERE Guid = @SealGuid";

            DbContext.Database.Connection.Execute(sql, data);
            #endregion
        }

        public IEnumerable<TblMasterActualJobItemsSeal> GetItemInCon(IEnumerable<Guid> conGuid)
        {
            return DbContext.TblMasterActualJobItemsSeal.Where(o =>
                            conGuid.Any(c => o.MasterConAndDeconsolidateHeaderMasterID_Guid == c || o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == c));
        }
        #endregion

        #region ## CONSOLIDATION
        public IEnumerable<TblMasterActualJobItemsSeal> GetItemInCon(Guid conGuid)
        {
            return DbContext.TblMasterActualJobItemsSeal.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == conGuid
                                                                 || o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == conGuid);
        }

        public IEnumerable<TblMasterActualJobItemsSeal> GetItemInCon(Guid conGuid, bool isConR)
        {
            if (isConR)
            {
                return DbContext.TblMasterActualJobItemsSeal.Where(o => o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == conGuid);
            }
            else
            {
                return DbContext.TblMasterActualJobItemsSeal.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == conGuid);
            }
        }

        public IEnumerable<TblMasterActualJobItemsSeal> GetItemInConL(Guid conGuid)
        {
            return DbContext.TblMasterActualJobItemsSeal.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == conGuid
                                                                 && o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == null);
        }

        public IEnumerable<TblMasterActualJobItemsSeal> GetItemInConR(Guid conGuid)
        {
            return DbContext.TblMasterActualJobItemsSeal.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == null
                                                                 && o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == conGuid);
        }

        public void UpdateSealWithMasterID(IEnumerable<Guid> seals, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            var data = seals.Select(o => new { SealGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsSeal
                            SET Master_ID = '{masterID}',
                                Master_ID_DP = '{masterID}',
                                MasterConAndDeconsolidateHeaderMasterID_Guid = '{masterID_Guid}',
                                FlagConsolidate = 1,
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @SealGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }

        public void UpdateSealWithMasterID_Route(IEnumerable<Guid> seals, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            var data = seals.Select(o => new { SealGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsSeal
                            SET MasterID_Route = '{masterID}',
                                MasterID_Route_DP = '{masterID}',
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = '{masterID_Guid}',
                                FlagConsolidate = 1,
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @SealGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }

        public void RemoveSealWithMasterID(IEnumerable<Guid> seals, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime, int flagConsolidate)
        {
            var data = seals.Select(o => new { SealGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsSeal
                            SET Master_ID = NULL,
                                Master_ID_DP = NULL,
                                MasterConAndDeconsolidateHeaderMasterID_Guid = NULL,
                                FlagConsolidate = {flagConsolidate},
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @SealGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }

        public void RemoveSealWithMasterID_Route(IEnumerable<Guid> seals, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            var data = seals.Select(o => new { SealGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsSeal
                            SET MasterID_Route = NULL,
                                MasterID_Route_DP = NULL,
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = NULL,
                                FlagConsolidate = 0,
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @SealGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }
        #endregion

        #region On Hand
        public List<TblMasterActualJobItemsSeal> FindSealOnHandByJob(IEnumerable<Guid> JobGuids)
        {
            return DbContext.TblMasterActualJobItemsSeal.Where(e => e.FlagSealDiscrepancies != true)
                .Join(DbContext.TblMasterActualJobHeader.Where(e => JobGuids.Contains(e.Guid)),
                s => s.MasterActualJobHeader_Guid,
                j => j.Guid,
                (s, j) => new { s, j })
               .Where(o =>
                (o.j.SystemStatusJobID == IntStatusJob.ReturnToPreVault && o.j.FlagNonDelivery == false && o.s.FlagPartial == true) ||
                (o.j.SystemStatusJobID == IntStatusJob.ReturnToPreVault && o.j.FlagNonDelivery == true) ||
                (o.j.SystemStatusJobID != IntStatusJob.ReturnToPreVault))
                .Select(o => o.s).ToList();
        }
        #endregion

        #region Vault balance
        public IEnumerable<TblMasterActualJobItemsSeal> FindItemByPrevault(Guid prevaultGuid)
        {
            return DbContext.TblMasterActualJobItemsSeal
                   .Where(o => o.MasterCustomerLocation_InternalDepartment_Guid == prevaultGuid &&
                          o.FlagSealDiscrepancies == false && !o.FlagIntDisc);
        }
        public IEnumerable<VaultBalanceSealModel> GetSealDetailVaultBalance(List<TblMasterActualJobItemsSeal> itemSeals, IEnumerable<VaultBalanceJobDetailModel> jobDetail)
        {
            var jobsGuidList = jobDetail.Select(o => o.JobGuid);
            var sealLiaList = itemSeals.Where(e => e.MasterActualJobItemsCommodity_Guid.HasValue)
                              .GroupBy(g => g.MasterActualJobItemsCommodity_Guid)
                              .Select(o => new
                              {
                                  liaGuid = o.Key,
                                  sealGuid = o.OrderBy(r => r.SealNo).First().Guid
                              }).ToList();

            var liability = DbContext.TblMasterActualJobItemsLiability.Where(o => o.MasterActualJobHeader_Guid != null &&
                            jobsGuidList.Contains((Guid)o.MasterActualJobHeader_Guid)).ToList();

            var currencyGuidList = liability.Select(o => o.MasterCurrency_Guid);
            var currency = DbContext.TblMasterCurrency.Where(o => currencyGuidList.Contains(o.Guid)).ToList();

            var commodityGuidList = liability.Select(o => o.MasterCommodity_Guid);
            var commodity = DbContext.TblMasterCommodity.Where(o => commodityGuidList.Contains(o.Guid)).ToList();

            var result = itemSeals.Select(o =>
                {
                    var job = jobDetail.FirstOrDefault(e => e.JobGuid == o.MasterActualJobHeader_Guid);
                    var lia = liability.FirstOrDefault(e => e.Guid == o.MasterActualJobItemsCommodity_Guid);
                    var sealLia = sealLiaList.FirstOrDefault(e => e.sealGuid == o.Guid)?.liaGuid;
                    var commName = commodity.FirstOrDefault(e => lia != null && e.Guid == lia.MasterCommodity_Guid)?.CommodityName ?? string.Empty;
                    var currName = currency.FirstOrDefault(e => lia != null &&  e.Guid == lia.MasterCurrency_Guid)?.MasterCurrencyAbbreviation ?? string.Empty;
                    var liaValue = sealLia == null ? 0 : (decimal)(lia?.Liability ?? 0);
                    EnumItemState itemState = EnumItemState.NotScan;
                    return new VaultBalanceSealModel
                    {
                        Guid = o.Guid,
                        JobGuid = o.MasterActualJobHeader_Guid.GetValueOrDefault(),
                        SealNo = o.SealNo,
                        JobNo = job.JobNo,
                        STC = liaValue,
                        Commodity = commName,
                        Currency = currName,
                        PickUpLocation = job.PickUpLocation,
                        DeliveryLocation = job.DeliveryLocation,
                        ServiceType = job.ServiceType,
                        WorkDate = job.WorkDate,
                        LiabilityGuid = o.MasterActualJobItemsCommodity_Guid,
                        ScanItemState = itemState,
                        LocationPickupGuid = job.LocationPickupGuid,
                        LocationDeliveryGuid = job.LocationDeliveryGuid,
                        CustomerPickupGuid = job.CustomerPickupGuid,
                        CustomerDeliveryGuid = job.CustomerDeliveryGuid
                    };
                }).ToList();
            return result;
        }
        
        #endregion
    }
}