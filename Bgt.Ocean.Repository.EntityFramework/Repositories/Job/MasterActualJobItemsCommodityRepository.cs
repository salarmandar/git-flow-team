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
    public interface IMasterActualJobItemsCommodityRepository : IRepository<TblMasterActualJobItemsCommodity>
    {
        IEnumerable<PrevaultDepartmentBarcodeScanOutResult> Func_CheckOutDepartment_NonBarcode_Get(Guid prevaultGuid, Guid internalDepartmentGuid, bool flagTVD, bool flgWithOutOWD);
        IEnumerable<TblMasterActualJobItemsCommodity> FindCommodities(IEnumerable<Guid> commodities);
        void UpdateNonbarcode(IEnumerable<Guid> nonGuid, Guid internalDeptGuid, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        IEnumerable<TblMasterActualJobItemsCommodity> GetItemInCon(IEnumerable<Guid> conGuid);
        IEnumerable<TblMasterActualJobItemsCommodity> GetItemInCon(Guid conGuid, bool isConR);
        IEnumerable<TblMasterActualJobItemsCommodity> GetItemInCon(Guid conGuid);
        IEnumerable<TblMasterActualJobItemsCommodity> GetItemInConL(Guid conGuid);
        IEnumerable<TblMasterActualJobItemsCommodity> GetItemInConR(Guid conGuid);
        void UpdateCommodityWithMasterID(IEnumerable<Guid> commodity, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        void UpdateCommodityWithMasterID_Route(IEnumerable<Guid> commodity, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        void RemoveCommodityWithMasterID(IEnumerable<Guid> commodity, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime, int flagConsolidate = 0);
        void RemoveCommodityWithMasterID_Route(IEnumerable<Guid> commodity, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime);
        IEnumerable<TblMasterActualJobItemsCommodity> FindCommodityByListJob(IEnumerable<Guid> jobGuids);
        List<TblMasterActualJobItemsCommodity> FindCommodityOnHandByJob(IEnumerable<Guid> JobGuids);
        IEnumerable<TblMasterActualJobItemsCommodity> FindItemByPrevault(Guid prevaultGuid);
        IEnumerable<TblMasterActualJobItemsCommodity> FindMasterByPrevault(Guid prevaultGuid);
    }

    public class MasterActualJobItemsCommodityRepository : Repository<OceanDbEntities, TblMasterActualJobItemsCommodity>, IMasterActualJobItemsCommodityRepository
    {
        public MasterActualJobItemsCommodityRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterActualJobItemsCommodity> FindCommodities(IEnumerable<Guid> commodities)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(w => commodities.Contains(w.Guid));
        }

        public IEnumerable<TblMasterActualJobItemsCommodity> FindCommodityByListJob(IEnumerable<Guid> jobGuids)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(x => x.FlagCommodityDiscrepancies == false ||
                                                             (x.FlagCommodityDiscrepancies == true && x.Quantity > 0))
                    .Join(jobGuids,
                    a => a.MasterActualJobHeader_Guid,
                    b => b,
                    (a, b) => a);
        }

        #region For Check Out Department
        public IEnumerable<PrevaultDepartmentBarcodeScanOutResult> Func_CheckOutDepartment_NonBarcode_Get(Guid prevaultGuid, Guid internalDepartmentGuid, bool flagTVD, bool flgWithOutOWD)
        {
            return DbContext.Up_OceanOnlineMVC_PrevaultDepartment_Barcode_ScanOut_Get(prevaultGuid, internalDepartmentGuid, flagTVD, flgWithOutOWD);
        }

        public IEnumerable<TblMasterActualJobItemsCommodity> GetItemInCon(IEnumerable<Guid> conGuid)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(
                    o => conGuid
                        .Any(c => o.MasterConAndDeconsolidateHeaderMasterID_Guid == c || o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == c)
                    );

        }

        public void UpdateNonbarcode(IEnumerable<Guid> nonGuid, Guid internalDeptGuid, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            #region Original
            //string guids = string.Join("','", nonGuid);
            //string a = $"('{guids}')";
            //string sql = $@"UPDATE TblMasterActualJobItemsCommodity
            //                        SET  MasterCustomerLocation_InternalDepartment_Guid = '{internalDeptGuid}',
            //                             MasterCustomerLocation_InternalDepartmentArea_Guid = null,
            //                             MasterCustomerLocation_InternalDepartmentSubArea_Guid = null,
            //                             FlagCommodityDiscrepancies = 0,
            //                             UserModifed = '{userName}',
            //                             DatetimeModified = '{clientDatetime}',
            //                             UniversalDatetimeModified = '{universalDateTime}'
            //                             WHERE Guid in {a}";
            //DbContext.Database.Connection.QueryAsync(sql);
            #endregion

            #region Separate list data
            //int nRow = 200;
            //var subList = nonGuid.Select((c, i) => new { Value = c, Index = i })
            //         .GroupBy(item => item.Index / nRow, item => item.Value);

            //foreach (var item in subList)
            //{
            //    string guids = string.Join("','", item.Select(e => e.ToString()));
            //    string a = $"('{guids}')";
            //    string sql = $@"UPDATE TblMasterActualJobItemsCommodity
            //                        SET  MasterCustomerLocation_InternalDepartment_Guid = '{internalDeptGuid}',
            //                             MasterCustomerLocation_InternalDepartmentArea_Guid = null,
            //                             MasterCustomerLocation_InternalDepartmentSubArea_Guid = null,
            //                             FlagCommodityDiscrepancies = 0,
            //                             UserModifed = '{userName}',
            //                             DatetimeModified = '{clientDatetime}',
            //                             UniversalDatetimeModified = '{universalDateTime}'
            //                             WHERE Guid in {a}";
            //    DbContext.Database.Connection.ExecuteAsync(sql);
            //}
            #endregion

            #region Update List
            var data = nonGuid.Select(o => new { NonbarCodeGuid = o, clientDatetime = clientDatetime, universalDateTime = universalDateTime });

            string sql = $@" UPDATE TblMasterActualJobItemsCommodity
                            SET  MasterCustomerLocation_InternalDepartment_Guid = '{internalDeptGuid}' ,
                                MasterCustomerLocation_InternalDepartmentArea_Guid = null,
                                MasterCustomerLocation_InternalDepartmentSubArea_Guid = null,
                                FlagCommodityDiscrepancies = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = @clientDatetime,
                                UniversalDatetimeModified = @universalDateTime
                            WHERE Guid = @NonbarcodeGuid";

            DbContext.Database.Connection.Execute(sql, data);
            #endregion
        }
        #endregion

        public IEnumerable<TblMasterActualJobItemsCommodity> GetItemInCon(Guid conGuid)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == conGuid
                                                                      || o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == conGuid);
        }

        public IEnumerable<TblMasterActualJobItemsCommodity> GetItemInCon(Guid conGuid, bool isConR)
        {
            if (isConR)
            {
                return DbContext.TblMasterActualJobItemsCommodity.Where(o => o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == conGuid);
            }
            else
            {
                return DbContext.TblMasterActualJobItemsCommodity.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == conGuid);
            }
        }

        public IEnumerable<TblMasterActualJobItemsCommodity> GetItemInConL(Guid conGuid)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == conGuid
                                                                      && o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == null);
        }

        public IEnumerable<TblMasterActualJobItemsCommodity> GetItemInConR(Guid conGuid)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == null
                                                                      && o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == conGuid);
        }

        public void UpdateCommodityWithMasterID(IEnumerable<Guid> commodity, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            var data = commodity.Select(o => new { CommodityGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsCommodity
                            SET Master_ID = '{masterID}',
                                MasterID_DP = '{masterID}',
                                MasterConAndDeconsolidateHeaderMasterID_Guid = '{masterID_Guid}',
                                FlagConsolidate = 1,
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @CommodityGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }

        public void UpdateCommodityWithMasterID_Route(IEnumerable<Guid> commodity, Guid masterID_Guid, string masterID, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            var data = commodity.Select(o => new { CommodityGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsCommodity
                            SET MasterID_Route = '{masterID}',
                                MasterID_Route_DP = '{masterID}',
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = '{masterID_Guid}',
                                FlagConsolidate = 1,
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @CommodityGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }

        public void RemoveCommodityWithMasterID(IEnumerable<Guid> commodity, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime, int flagConsolidate)
        {
            var data = commodity.Select(o => new { CommodityGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsCommodity
                            SET Master_ID = NULL,
                                MasterID_DP = NULL,
                                MasterConAndDeconsolidateHeaderMasterID_Guid = NULL,
                                FlagConsolidate = {flagConsolidate},
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @CommodityGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }

        public void RemoveCommodityWithMasterID_Route(IEnumerable<Guid> commodity, string userName, DateTime clientDatetime, DateTimeOffset universalDateTime)
        {
            var data = commodity.Select(o => new { CommodityGuid = o });
            string sql = $@"UPDATE TblMasterActualJobItemsCommodity
                            SET MasterID_Route = NULL,
                                MasterID_Route_DP = NULL,
                                MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = NULL,
                                FlagConsolidate = 0,
                                FlagRouteBalance = 0,
                                UserModifed = '{userName}',
                                DatetimeModified = '{clientDatetime}',
                                UniversalDatetimeModified = '{universalDateTime}'
                            WHERE Guid = @CommodityGuid";

            DbContext.Database.Connection.Execute(sql, data);
        }

        public List<TblMasterActualJobItemsCommodity> FindCommodityOnHandByJob(IEnumerable<Guid> JobGuids)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(e => e.FlagCommodityDiscrepancies != true ||
                                                                   (e.FlagCommodityDiscrepancies == true && e.Quantity > 0))
                .Join(DbContext.TblMasterActualJobHeader.Where(e => JobGuids.Contains(e.Guid)),
                c => c.MasterActualJobHeader_Guid,
                j => j.Guid,
                (c, j) => new { c, j })
                .Where(o =>
                (o.j.SystemStatusJobID == IntStatusJob.ReturnToPreVault && o.j.FlagNonDelivery == false && o.c.FlagPartial == true) ||
                (o.j.SystemStatusJobID == IntStatusJob.ReturnToPreVault && o.j.FlagNonDelivery == true) ||
                (o.j.SystemStatusJobID != IntStatusJob.ReturnToPreVault))
                .Select(o => o.c).ToList();
        }

        #region Vault Balance
        public IEnumerable<TblMasterActualJobItemsCommodity> FindItemByPrevault(Guid prevaultGuid)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(o => o.MasterCustomerLocation_InternalDepartment_Guid == prevaultGuid
            && (o.Quantity > 0 || o.QuantityActual > 0));
        }

        public IEnumerable<TblMasterActualJobItemsCommodity> FindMasterByPrevault(Guid prevaultGuid)
        {
            return DbContext.TblMasterActualJobItemsCommodity.Where(o => o.MasterCustomerLocation_InternalDepartment_Guid == prevaultGuid
            && (o.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue || o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue));
        }
        public IEnumerable<VaultBalanceNonbarcodeModel> GetNonbarcodeDetailVaultBalance(IEnumerable<TblMasterActualJobItemsCommodity> nonbs)
        {
            var commodity = nonbs.Select(e => e.MasterCommodity_Guid).Distinct()
                            .Join(DbContext.TblMasterCommodity,
                            i => i,
                            c => c.Guid,
                            (i, c) => c);
            var result = nonbs.GroupBy(e => e.MasterCommodity_Guid).Select(o =>
            {
                var comm = commodity.FirstOrDefault(x => x.Guid == o.Key);
                return new VaultBalanceNonbarcodeModel
                {
                    CommodityGuid = o.Key.GetValueOrDefault(),
                    CommodityName = comm.CommodityName,
                    STC = 0,
                    CommodityValue = (decimal)comm.CommodityValue,
                    CommodityAmount = (decimal)comm.CommodityAmount,
                    PreAdviceQty = o.Sum(s => s.Quantity.GetValueOrDefault()),
                    ActualQty = 0,
                    ItemState = EnumState.Unchanged, //Use 0,2
                    FlagTemp = false
                };
            });
            return result;
        }
        #endregion

    }
}
