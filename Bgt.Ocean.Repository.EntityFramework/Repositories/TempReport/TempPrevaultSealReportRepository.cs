using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Reports.VaultBalance;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.TempReport
{
    public interface ITempPrevaultSealReportRepository : IRepository<TblTempPrevaultSealReport>
    {
        void InsertTempSealConReport(IEnumerable<Guid> sealScanedList, Guid tempMainRptGuid, Guid reportStyleGuid);
        void InsertTempSealReport(IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> sealScanedList, Guid tempMainRptGuid, Guid reportStyleGuid);
    }
    public class TempPrevaultSealReportRepository : Repository<OceanDbEntities, TblTempPrevaultSealReport>, ITempPrevaultSealReportRepository
    {
        public TempPrevaultSealReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        #region For check out department
        public void InsertTempSealConReport(IEnumerable<Guid> sealScanedList, Guid tempMainRptGuid, Guid reportStyleGuid)
        {
            #region Original
            //string sql = "";
            //Parallel.ForEach(sealScanedList, o =>
            //{

            //    sql += $@"insert into TblTempPrevaultSealReport(
            //                            Guid,
            //                            TempMainPrevaultReport_Guid,
            //                            SystemReportStyle_Guid,
            //                            MasterActualJobHeader_Guid ,
            //                            MasterActualJobItemsSeal_Guid ,
            //                            LocationGuid ,
            //                            LocationName ,
            //                            SealNo ,
            //                            ShortageQty ,
            //                            OverageQty ,
            //                            ExpectedQty,
            //                            ActualQty,
            //                            LiabilityGuid ,
            //                            LiabilityValue,
            //                            CurrencyGuid,
            //                            ConsolidateRouteID,
            //                            ConsolidateLocationID)
            //            SELECT NEWID(),'{tempMainRptGuid}','{reportStyleGuid}',s.MasterActualJobHeader_Guid,s.Guid,NULL,
            //                    NULL,s.SealNo,{0},{0},{1},{1},s.MasterActualJobItemsCommodity_Guid,
            //                    TblMasterActualJobItemsLiability.Liability, 
            //                    TblMasterActualJobItemsLiability.MasterCurrency_Guid,
            //                    s.MasterID_Route,s.Master_ID
            //            FROM TblMasterActualJobItemsSeal s
            //  LEFT JOIN TblMasterActualJobItemsLiability ON s.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid 
            //            WHERE ( s.MasterConAndDeconsolidateHeaderMasterID_Guid IS NOT NULL OR s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NOT NULL) AND s.Guid = '{o}'
            //            ";

            //});
            //DbContext.Database.Connection.QueryAsync(sql);
            #endregion

            #region Separate list data
            int nRow = 200;
            var subList = sealScanedList.Select((s, i) => new { Value = s, Index = i })
                     .GroupBy(item => item.Index / nRow, item => item.Value);

            foreach (var item in subList)
            {
                string guids = string.Join("','", item.Select(e => e.ToString()));
                string o = $"('{guids}')";

                string sql = $@"insert into TblTempPrevaultSealReport(
                                        Guid,
                                        TempMainPrevaultReport_Guid,
                                        SystemReportStyle_Guid,
                                        MasterActualJobHeader_Guid ,
                                        MasterActualJobItemsSeal_Guid ,
                                        LocationGuid ,
                                        LocationName ,
                                        SealNo ,
                                        ShortageQty ,
                                        OverageQty ,
                                        ExpectedQty,
                                        ActualQty,
                                        LiabilityGuid ,
                                        LiabilityValue,
                                        CurrencyGuid,
                                        ConsolidateRouteID,
                                        ConsolidateLocationID)
                        SELECT NEWID(),'{tempMainRptGuid}','{reportStyleGuid}',s.MasterActualJobHeader_Guid,s.Guid,NULL,
                                NULL,s.SealNo,{0},{0},{1},{1},s.MasterActualJobItemsCommodity_Guid,
                                TblMasterActualJobItemsLiability.Liability, 
                                TblMasterActualJobItemsLiability.MasterCurrency_Guid,
                                s.MasterID_Route,s.Master_ID
                        FROM TblMasterActualJobItemsSeal s
						        LEFT JOIN TblMasterActualJobItemsLiability ON s.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid 
                        WHERE ( s.MasterConAndDeconsolidateHeaderMasterID_Guid IS NOT NULL OR s.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NOT NULL) AND s.Guid IN {o} 
                        ";
             DbContext.Database.Connection.Execute(sql);
            }
            #endregion

            #region Select Insert
            //var insertTmpSealConReport = from s in sealScanedList
            //                             join seal in DbContext.TblMasterActualJobItemsSeal on s equals seal.Guid
            //                             join li in DbContext.TblMasterActualJobItemsLiability on seal.MasterActualJobItemsCommodity_Guid equals li.Guid into tmpLia
            //                             from lia in tmpLia.DefaultIfEmpty()
            //                             where (seal.MasterConAndDeconsolidateHeaderMasterID_Guid != null || seal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid != null)
            //                             select new TblTempPrevaultSealReport
            //                             {
            //                                 Guid = Guid.NewGuid(),
            //                                 TempMainPrevaultReport_Guid = tempMainRptGuid,
            //                                 SystemReportStyle_Guid = reportStyleGuid,
            //                                 MasterActualJobHeader_Guid = seal.MasterActualJobHeader_Guid,
            //                                 MasterActualJobItemsSeal_Guid = seal.Guid,
            //                                 LocationGuid = null,
            //                                 LocationName = null,
            //                                 SealNo = seal.SealNo,
            //                                 ShortageQty = 0,
            //                                 OverageQty = 0,
            //                                 ExpectedQty = 1,
            //                                 ActualQty = 1,
            //                                 LiabilityGuid = seal.MasterActualJobItemsCommodity_Guid,
            //                                 LiabilityValue = lia.Liability,
            //                                 CurrencyGuid = lia.MasterCurrency_Guid,
            //                                 ConsolidateRouteID = seal.MasterID_Route,
            //                                 ConsolidateLocationID = seal.Master_ID
            //                             };
            //DbContext.TblTempPrevaultSealReport.AddRange(insertTmpSealConReport);
            //DbContext.SaveChanges();
            #endregion
        }
        public void InsertTempSealReport(IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> sealScanedList, Guid tempMainRptGuid, Guid reportStyleGuid)
        {
            #region Original
            //string sql = "";
            //Parallel.ForEach(sealScanedList, o =>
            //{

            //    sql += $@"insert into TblTempPrevaultSealReport(
            //                            Guid,
            //                            TempMainPrevaultReport_Guid,
            //                            SystemReportStyle_Guid,
            //                            MasterActualJobHeader_Guid ,
            //                            MasterActualJobItemsSeal_Guid ,
            //                            LocationGuid ,
            //                            LocationName ,
            //                            SealNo ,
            //                            ShortageQty ,
            //                            OverageQty ,
            //                            ExpectedQty,
            //                            ActualQty,
            //                            LiabilityGuid ,
            //                            LiabilityValue,
            //                            CurrencyGuid,
            //                            ConsolidateRouteID,
            //                            ConsolidateLocationID)
            //            SELECT NEWID(),'{tempMainRptGuid}','{reportStyleGuid}',s.MasterActualJobHeader_Guid,s.Guid,'{o.CustomerLocationGuid}' ,
            //                    '{o.Location}' ,s.SealNo,{0},{0},{1},{1},s.MasterActualJobItemsCommodity_Guid,
            //                    TblMasterActualJobItemsLiability.Liability, 
            //                    TblMasterActualJobItemsLiability.MasterCurrency_Guid,
            //                    s.MasterID_Route,s.Master_ID
            //            FROM TblMasterActualJobItemsSeal s
            //  LEFT JOIN TblMasterActualJobItemsLiability ON s.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid 
            //            WHERE s.Guid = '{o.Guid}'
            //            ";

            //});
            //DbContext.Database.Connection.QueryAsync(sql);
            #endregion

            #region Separate list data
            //int nRow = 200;
            //var subList = sealScanedList.Select((s, i) => new { Value = s, Index = i })
            //         .GroupBy(item => item.Index / nRow, item => item.Value);

            //foreach (var item in subList)
            //{
            //    string guids = string.Join("','", item.Select(e => e.Guid.ToString()));
            //    string o = $"('{guids}')";

            //    string sql = $@"insert into TblTempPrevaultSealReport(
            //                            Guid,
            //                            TempMainPrevaultReport_Guid,
            //                            SystemReportStyle_Guid,
            //                            MasterActualJobHeader_Guid ,
            //                            MasterActualJobItemsSeal_Guid ,
            //                            LocationGuid ,
            //                            LocationName ,
            //                            SealNo ,
            //                            ShortageQty ,
            //                            OverageQty ,
            //                            ExpectedQty,
            //                            ActualQty,
            //                            LiabilityGuid ,
            //                            LiabilityValue,
            //                            CurrencyGuid,
            //                            ConsolidateRouteID,
            //                            ConsolidateLocationID)
            //            SELECT NEWID(),'{tempMainRptGuid}','{reportStyleGuid}',s.MasterActualJobHeader_Guid,s.Guid,'{o.CustomerLocationGuid}' ,
            //                    '{o.Location}',s.SealNo,{0},{0},{1},{1},s.MasterActualJobItemsCommodity_Guid,
            //                    TblMasterActualJobItemsLiability.Liability, 
            //                    TblMasterActualJobItemsLiability.MasterCurrency_Guid,
            //                    s.MasterID_Route,s.Master_ID
            //            FROM TblMasterActualJobItemsSeal s
            //            LEFT JOIN TblMasterActualJobItemsLiability ON s.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid 
            //            WHERE s.Guid IN {o}
            //            ";
            //    DbContext.Database.Connection.Execute(sql);
            //}
            #endregion

            #region Select Insert
            //var insertTmpSealReport = from s in sealScanedList
            //                          join li in DbContext.TblMasterActualJobItemsLiability on s.LiabilityGuid equals li.Guid into tmpLia
            //                                                                                from lia in tmpLia.DefaultIfEmpty()
            //                          select new TblTempPrevaultSealReport
            //                          {
            //                              Guid = Guid.NewGuid(),
            //                              TempMainPrevaultReport_Guid = tempMainRptGuid,
            //                              SystemReportStyle_Guid = reportStyleGuid,
            //                              MasterActualJobHeader_Guid = s.JobGuid,
            //                              MasterActualJobItemsSeal_Guid = s.Guid,
            //                              LocationGuid = s.CustomerLocationGuid,
            //                              LocationName = s.Location,
            //                              SealNo = s.SealNo,
            //                              ShortageQty = 0,
            //                              OverageQty = 0,
            //                              ExpectedQty = 1,
            //                              ActualQty = 1,
            //                              LiabilityGuid = s.LiabilityGuid,
            //                              LiabilityValue = lia.Liability,
            //                              CurrencyGuid = lia.MasterCurrency_Guid,
            //                              ConsolidateRouteID = null,
            //                              ConsolidateLocationID = null
            //                          };
            //DbContext.TblTempPrevaultSealReport.AddRange(insertTmpSealReport);
            //DbContext.SaveChanges();
            #endregion

            #region List Insert

            var data = sealScanedList.Select(o=> new {
                SealGuid = o.Guid,
                LocationGuid = o.CustomerLocationGuid,
                LocationName = o.Location
            });

            var sql1 = $@"INSERT INTO TblTempPrevaultSealReport(
                                        Guid,
                                        TempMainPrevaultReport_Guid,
                                        SystemReportStyle_Guid,
                                        MasterActualJobHeader_Guid ,
                                        MasterActualJobItemsSeal_Guid ,
                                        LocationGuid ,
                                        LocationName ,
                                        SealNo ,
                                        ShortageQty ,
                                        OverageQty ,
                                        ExpectedQty,
                                        ActualQty,
                                        LiabilityGuid ,
                                        LiabilityValue,
                                        CurrencyGuid,
                                        ConsolidateRouteID,
                                        ConsolidateLocationID)
                        SELECT NEWID(), '{tempMainRptGuid}', '{reportStyleGuid}', s.MasterActualJobHeader_Guid, s.Guid,@LocationGuid,
                                @LocationName, s.SealNo,{ 0},{ 0},{ 1},{ 1},s.MasterActualJobItemsCommodity_Guid,
                                TblMasterActualJobItemsLiability.Liability, 
                                TblMasterActualJobItemsLiability.MasterCurrency_Guid,
                                s.MasterID_Route,s.Master_ID
                        FROM TblMasterActualJobItemsSeal s
                        LEFT JOIN TblMasterActualJobItemsLiability ON s.MasterActualJobItemsCommodity_Guid = TblMasterActualJobItemsLiability.Guid
                        WHERE s.Guid = @SealGuid
                        ";

            DbContext.Database.Connection.Execute(sql1,data);
            #endregion
        }
        #endregion
        
    }
}