using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.TempReport
{
    public interface ITempPrevaultNonBarcodeReportRepository : IRepository<TblTempPrevaultNonBarcodeReport>
    {
        void InsertTempNonReport(IEnumerable<PrevaultDepartmentBarcodeScanOutResult> nonScanedList, Guid tempMainRptGuid, Guid reportStyleGuid, Guid countryGuid,bool FlagGroupNonBarcode);
        void InsertTempNonConReport(IEnumerable<Guid> nonScanedList, Guid tempMainRptGuid, Guid reportStyleGuid, Guid countryGuid,bool FlagGroupNonBarcode);
    }
    public class TempPrevaultNonBarcodeReportRepository : Repository<OceanDbEntities, TblTempPrevaultNonBarcodeReport>, ITempPrevaultNonBarcodeReportRepository
    {
        public TempPrevaultNonBarcodeReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void InsertTempNonConReport(IEnumerable<Guid> nonScanedList, Guid tempMainRptGuid, Guid reportStyleGuid, Guid countryGuid,bool FlagGroupNonBarcode)
        {
            #region original
            //string sql = "";
            //Parallel.ForEach(nonScanedList, o =>
            //{
            //    sql = $@"INSERT INTO TblTempPrevaultNonBarcodeReport(
            //                  Guid,
            //                  TempMainPrevaultReport_Guid,
            //                  SystemReportStyle_Guid,
            //                  MasterActualJobHeader_Guid,
            //                  MasterActualJobItemsCommodity_Guid,
            //                  CommodityName,
            //                  ConsolidateRouteID,
            //                  ConsolidateLocationID,
            //                  ExpectedQty,
            //                  ActualQty,
            //                  ShortageQty,
            //                  OverageQty,
            //                  CommodityValue,
            //                  CommoditySeq,
            //                  LocationGuid,
            //                  LocationName)

            //                  SELECT
            //                  NEWID(),
            //                  '{tempMainRptGuid}',
            //                  '{reportStyleGuid}',
            //                  c.MasterActualJobHeader_Guid,
            //                  c.Guid,
            //                  com.CommodityName, 
            //                  c.MasterID_Route,   
            //                  c.Master_ID,                 
            //                  c.QuantityExpected,
            //                  c.QuantityActual,
            //                  0,
            //                  0,
            //                  (case when com.FlagCommodityGlobal = 1 then ISNULL(c.Quantity, 0) * ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
            //	                else ISNULL(c.Quantity, 0) * ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS CommodityValue,
            //                  com.ColumnInReport,
            //                  NULL,
            //                  NULL   

            //                  FROM TblMasterActualJobItemsCommodity c
            //                  INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
            //                  INNER JOIN TblMasterCommodityGroup CommodityGroup ON CommodityGroup.Guid = com.MasterCommodityGroup_Guid
            //                  LEFT JOIN TblMasterCommodityCountry cc ON com.Guid = cc.MasterCommodity_Guid
            //                  WHERE  ( c.MasterConAndDeconsolidateHeaderMasterID_Guid IS NOT NULL OR c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NOT NULL)
            //                         AND c.Guid = '{o}'
            //                         AND((com.FlagCommodityGlobal = 1) 
            //                         or (com.FlagCommodityGlobal = 0 AND cc.MasterCountry_Guid IN(select MasterCountry_Guid from TblMasterSite where guid = '{siteGuid}')))";

            //    DbContext.Database.Connection.Execute(sql);
            //});


            #endregion

            #region Separate list data
            int nRow = 200;
            var subList = nonScanedList.Select((s, i) => new { Value = s, Index = i })
                     .GroupBy(item => item.Index / nRow, item => item.Value);

            foreach (var item in subList)
            {
                string guids = string.Join("','", item.Select(e => e.ToString()));
                string o = $"('{guids}')";

                string sql = $@"INSERT INTO TblTempPrevaultNonBarcodeReport(
                              Guid,
                              TempMainPrevaultReport_Guid,
                              SystemReportStyle_Guid,
                              MasterActualJobHeader_Guid,
                              MasterActualJobItemsCommodity_Guid,
                              CommodityName,
                              ConsolidateRouteID,
                              ConsolidateLocationID,
                              ExpectedQty,
                              ActualQty,
                              ShortageQty,
                              OverageQty,
                              CommodityValue,
                              CommoditySeq,
                              LocationGuid,
                              LocationName)

                              SELECT
                              NEWID(),
                              '{tempMainRptGuid}',
                              '{reportStyleGuid}',
                              c.MasterActualJobHeader_Guid,
                              c.Guid,
                              com.CommodityName, 
                              c.MasterID_Route,   
                              c.Master_ID,                 
                              c.QuantityExpected,
                              c.Quantity,
                              0,
                              0,
                              (case when com.FlagCommodityGlobal = 1 
                                    then ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
            	              else ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS CommodityValue,
                              ISNULL(com.ColumnInReport,0),
                              NULL,
                              NULL   

                              FROM TblMasterActualJobItemsCommodity c
                              INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
                              LEFT JOIN TblMasterCommodityCountry cc ON com.Guid = cc.MasterCommodity_Guid AND cc.MasterCountry_Guid = '{countryGuid}'
                              WHERE (c.MasterConAndDeconsolidateHeaderMasterID_Guid IS NOT NULL OR c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NOT NULL)
                                     AND c.Guid IN {o}
                              ";
                DbContext.Database.Connection.Execute(sql);
            }
            #endregion

            #region Select Insert
            //Guid coutryGuid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == siteGuid).MasterCountry_Guid;

            //var insertTmpNonConReport = from n in nonScanedList
            //                            join non in DbContext.TblMasterActualJobItemsCommodity on n equals non.Guid
            //                            join comm in DbContext.TblMasterCommodity on non.MasterCommodity_Guid equals comm.Guid
            //                            join cgrp in DbContext.TblMasterCommodityGroup on comm.MasterCommodityGroup_Guid equals cgrp.Guid
            //                            join ccoutry in DbContext.TblMasterCommodityCountry on comm.Guid equals ccoutry.MasterCountry_Guid into tmpComm
            //                            from commgrp in tmpComm.DefaultIfEmpty()
            //                            where (non.MasterConAndDeconsolidateHeaderMasterID_Guid != null || non.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid != null)
            //                            && comm.FlagCommodityGlobal || (!comm.FlagCommodityGlobal && commgrp.MasterCountry_Guid == coutryGuid)
            //                            select new TblTempPrevaultNonBarcodeReport
            //                            {
            //                                Guid = Guid.NewGuid(),
            //                                TempMainPrevaultReport_Guid = tempMainRptGuid,
            //                                SystemReportStyle_Guid = reportStyleGuid,
            //                                MasterActualJobHeader_Guid = non.MasterActualJobHeader_Guid,
            //                                MasterActualJobItemsCommodity_Guid = non.Guid,
            //                                CommodityName = comm.CommodityName,
            //                                ConsolidateRouteID = non.MasterID_Route,
            //                                ConsolidateLocationID = non.Master_ID,
            //                                ExpectedQty = non.QuantityExpected,
            //                                ActualQty = non.QuantityActual,
            //                                ShortageQty = 0,
            //                                OverageQty = 0,
            //                                CommodityValue = comm.FlagCommodityGlobal ? (non.Quantity * comm.CommodityAmount) * comm.CommodityValue : (non.Quantity * commgrp.CommodityAmount) * commgrp.CommodityValue,
            //                                CommoditySeq = Int32.Parse(comm.ColumnInReport),
            //                                LocationGuid = null,
            //                                LocationName = null
            //                            };
            //DbContext.TblTempPrevaultNonBarcodeReport.AddRange(insertTmpNonConReport);
            //DbContext.SaveChanges();
            #endregion

            #region List Insert
            //     Guid countryGuid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == siteGuid).Guid;
            //     var data = nonScanedList.Select(o => new {
            //         NonbarcodeGuid = o,
            //         CountryGuid = countryGuid
            //     });

            //     var sql = $@"INSERT INTO TblTempPrevaultNonBarcodeReport(
            //                                 Guid,
            //                                 TempMainPrevaultReport_Guid,
            //                                 SystemReportStyle_Guid,
            //                                 MasterActualJobHeader_Guid,
            //                                 MasterActualJobItemsCommodity_Guid,
            //                                 CommodityName,
            //                                 ConsolidateRouteID,
            //                                 ConsolidateLocationID,
            //                                 ExpectedQty,
            //                                 ActualQty,
            //                                 ShortageQty,
            //                                 OverageQty,
            //                                 CommodityValue,
            //                                 CommoditySeq,
            //                                 LocationGuid,
            //                                 LocationName)

            //                  SELECT NEWID(),'{tempMainRptGuid}','{reportStyleGuid}',c.MasterActualJobHeader_Guid,c.Guid,
            //                         com.CommodityName,c.MasterID_Route,c.Master_ID,c.QuantityExpected,c.QuantityActual,
            //                         0,0,
            //                         (case when com.FlagCommodityGlobal = 1 then ISNULL(c.Quantity, 0) * ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
            //	 else ISNULL(c.Quantity, 0) * ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS CommodityValue,
            //                         com.ColumnInReport,NULL,NULL 
            //                  FROM TblMasterActualJobItemsCommodity c
            //                  INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
            //                  INNER JOIN TblMasterCommodityGroup CommodityGroup ON CommodityGroup.Guid = com.MasterCommodityGroup_Guid
            //LEFT JOIN TblMasterCommodityCountry cc ON com.Guid = cc.MasterCommodity_Guid
            //                  WHERE  ( c.MasterConAndDeconsolidateHeaderMasterID_Guid IS NOT NULL OR c.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid IS NOT NULL)
            //                         AND c.Guid = @NonbarcodeGuid
            //                         AND((com.FlagCommodityGlobal = 1) 
            //                         or (com.FlagCommodityGlobal = 0 AND cc.MasterCountry_Guid = @CountryGuid)))
            //                 ";

            //     var xx = DbContext.Database.Connection.Execute(sql, data);
            //     Console.WriteLine(xx);
            #endregion
        }

        public void InsertTempNonReport(IEnumerable<PrevaultDepartmentBarcodeScanOutResult> nonScanedList, Guid tempMainRptGuid, Guid reportStyleGuid, Guid countryGuid,bool FlagGroupNonBarcode)
        {
            #region original
            //     string sql = "";
            //     Parallel.ForEach(nonScanedList, o =>
            //     {
            //         sql += $@"INSERT INTO TblTempPrevaultNonBarcodeReport(
            //                  Guid,
            //                  TempMainPrevaultReport_Guid,
            //                  SystemReportStyle_Guid,
            //                  MasterActualJobHeader_Guid,
            //                  MasterActualJobItemsCommodity_Guid,
            //                  CommodityName,
            //                  ConsolidateRouteID,
            //                  ConsolidateLocationID,
            //                  ExpectedQty,
            //                  ActualQty,
            //                  ShortageQty,
            //                  OverageQty,
            //                  CommodityValue,
            //                  CommoditySeq,
            //                  LocationGuid,
            //                  LocationName)

            //                  SELECT
            //                  NEWID(),
            //                  '{tempMainRptGuid}',
            //                  '{reportStyleGuid}',
            //                  c.MasterActualJobHeader_Guid,
            //                  c.Guid,
            //                  com.CommodityName, 
            //                  c.MasterID_Route,   
            //                  c.Master_ID,                 
            //                  c.QuantityExpected,
            //                  c.QuantityActual,
            //                  0,
            //                  0,
            //                  (case when com.FlagCommodityGlobal = 1 then ISNULL(c.Quantity, 0) * ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
            //	 else ISNULL(c.Quantity, 0) * ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS CommodityValue,
            //                  com.ColumnInReport,
            //                  '{o.CustomerLocationGuid}',
            //                  '{o.Location}'  

            //                  FROM TblMasterActualJobItemsCommodity c
            //                  INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
            //                  INNER JOIN TblMasterCommodityGroup CommodityGroup ON CommodityGroup.Guid = com.MasterCommodityGroup_Guid
            //LEFT JOIN TblMasterCommodityCountry cc ON com.Guid = cc.MasterCommodity_Guid
            //                  WHERE  c.Guid = '{o.Guid}'
            //                         AND((com.FlagCommodityGlobal = 1) 
            //                         or (com.FlagCommodityGlobal = 0 AND cc.MasterCountry_Guid IN(select MasterCountry_Guid from TblMasterSite where guid = '{siteGuid}')))";

            //         DbContext.Database.Connection.Execute(sql);
            //     });
            #endregion

            #region Separate list data
            var data = prepareData(nonScanedList,tempMainRptGuid,reportStyleGuid,countryGuid,FlagGroupNonBarcode);

            string sql = $@"INSERT INTO TblTempPrevaultNonBarcodeReport(
                              Guid,
                              TempMainPrevaultReport_Guid,
                              SystemReportStyle_Guid,
                              MasterActualJobHeader_Guid,
                              MasterActualJobItemsCommodity_Guid,
                              CommodityName,
                              ConsolidateRouteID,
                              ConsolidateLocationID,
                              ExpectedQty,
                              ActualQty,
                              ShortageQty,
                              OverageQty,
                              CommodityValue,
                              CommoditySeq,
                              LocationGuid,
                              LocationName)

                              SELECT
                              NEWID(),
                              '{tempMainRptGuid}',
                              '{reportStyleGuid}',
                              c.MasterActualJobHeader_Guid,
                              c.Guid,
                              com.CommodityName, 
                              c.MasterID_Route,   
                              c.Master_ID,                 
                              c.QuantityExpected,
                              c.Quantity,
                              0,
                              0,
                              (case when com.FlagCommodityGlobal = 1 
                                    then ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
            	              else ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS CommodityValue,
                              ISNULL(com.ColumnInReport,0),
                              @CustomerLocationGuid,
                              @Location   

                              FROM TblMasterActualJobItemsCommodity c
                              INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
                              LEFT JOIN TblMasterCommodityCountry cc ON com.Guid = cc.MasterCommodity_Guid AND cc.MasterCountry_Guid = '{countryGuid}'
                              WHERE  c.Guid = @Guid
                              ";
            DbContext.Database.Connection.Execute(sql, data);
            #endregion

            #region Select Insert
            //Guid coutryGuid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == siteGuid).MasterCountry_Guid;

            //var insertTmpNonConReport = from n in nonScanedList
            //                            join comm in DbContext.TblMasterCommodity on n.MasterCommodity_Guid equals comm.Guid
            //                            join cgrp in DbContext.TblMasterCommodityGroup on comm.MasterCommodityGroup_Guid equals cgrp.Guid
            //                            join ccoutry in DbContext.TblMasterCommodityCountry on comm.Guid equals ccoutry.MasterCountry_Guid into tmpComm
            //                            from commgrp in tmpComm.DefaultIfEmpty()
            //                            where comm.FlagCommodityGlobal || (!comm.FlagCommodityGlobal && commgrp.MasterCountry_Guid == coutryGuid)
            //                            select new TblTempPrevaultNonBarcodeReport
            //                            {
            //                                Guid = Guid.NewGuid(),
            //                                TempMainPrevaultReport_Guid = tempMainRptGuid,
            //                                SystemReportStyle_Guid = reportStyleGuid,
            //                                MasterActualJobHeader_Guid = n.JobGuid,
            //                                MasterActualJobItemsCommodity_Guid = n.Guid,
            //                                CommodityName = comm.CommodityName,
            //                                ConsolidateRouteID = null,
            //                                ConsolidateLocationID = null,
            //                                ExpectedQty = n.Quantity,
            //                                ActualQty = n.Quantity,
            //                                ShortageQty = 0,
            //                                OverageQty = 0,
            //                                CommodityValue = comm.FlagCommodityGlobal ? (n.Quantity * comm.CommodityAmount) * comm.CommodityValue : (n.Quantity * commgrp.CommodityAmount) * commgrp.CommodityValue,
            //                                CommoditySeq = Int32.Parse(comm.ColumnInReport),
            //                                LocationGuid = null,
            //                                LocationName = null
            //                            };
            //DbContext.TblTempPrevaultNonBarcodeReport.AddRange(insertTmpNonConReport);
            //DbContext.SaveChanges();
            #endregion

            #region List Insert

            //     Guid countryGuid = DbContext.TblMasterSite.FirstOrDefault(o=>o.Guid == siteGuid).Guid;

            //     var data = nonScanedList.Select(o => new {
            //         NonbarcodeGuid = o.Guid,
            //         LocationGuid = o.CustomerLocationGuid,
            //         LocationName = o.Location,
            //         CountryGuid = countryGuid
            //     });

            //     var sql = $@"SELECT NEWID(),'{tempMainRptGuid}','{reportStyleGuid}',c.MasterActualJobHeader_Guid,c.Guid,
            //                         com.CommodityName,c.MasterID_Route,c.Master_ID,c.QuantityExpected,c.QuantityActual,
            //                         0,0,
            //                         (case when com.FlagCommodityGlobal = 1 then ISNULL(c.Quantity, 0) * ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
            //	 else ISNULL(c.Quantity, 0) * ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS CommodityValue,
            //                         com.ColumnInReport,@LocationGuid,@LocationName 
            //                  FROM TblMasterActualJobItemsCommodity c
            //                  INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
            //                  INNER JOIN TblMasterCommodityGroup CommodityGroup ON CommodityGroup.Guid = com.MasterCommodityGroup_Guid
            //LEFT JOIN TblMasterCommodityCountry cc ON com.Guid = cc.MasterCommodity_Guid
            //                  WHERE  c.Guid = (@NonbarcodeGuid)
            //                         AND((com.FlagCommodityGlobal = 1) 
            //                         or (com.FlagCommodityGlobal = 0 AND cc.MasterCountry_Guid = @CountryGuid))
            //                 ";

            //     //     var sql = $@"INSERT INTO TblTempPrevaultNonBarcodeReport(
            //     //                                 Guid,
            //     //                                 TempMainPrevaultReport_Guid,
            //     //                                 SystemReportStyle_Guid,
            //     //                                 MasterActualJobHeader_Guid,
            //     //                                 MasterActualJobItemsCommodity_Guid,
            //     //                                 CommodityName,
            //     //                                 ConsolidateRouteID,
            //     //                                 ConsolidateLocationID,
            //     //                                 ExpectedQty,
            //     //                                 ActualQty,
            //     //                                 ShortageQty,
            //     //                                 OverageQty,
            //     //                                 CommodityValue,
            //     //                                 CommoditySeq,
            //     //                                 LocationGuid,
            //     //                                 LocationName)

            //     //                  SELECT NEWID(),'{tempMainRptGuid}','{reportStyleGuid}',c.MasterActualJobHeader_Guid,c.Guid,
            //     //                         com.CommodityName,c.MasterID_Route,c.Master_ID,c.QuantityExpected,c.QuantityActual,
            //     //                         0,0,
            //     //                         (case when com.FlagCommodityGlobal = 1 then ISNULL(c.Quantity, 0) * ISNULL(com.CommodityAmount, 0) * ISNULL(com.CommodityValue, 0) 
            //     //	 else ISNULL(c.Quantity, 0) * ISNULL(cc.CommodityAmount, 0) * ISNULL(cc.CommodityValue, 0) end) AS CommodityValue,
            //     //                         com.ColumnInReport,@LocationGuid,@LocationName 
            //     //                  FROM TblMasterActualJobItemsCommodity c
            //     //                  INNER JOIN TblMasterCommodity com ON c.MasterCommodity_Guid = com.Guid
            //     //                  INNER JOIN TblMasterCommodityGroup CommodityGroup ON CommodityGroup.Guid = com.MasterCommodityGroup_Guid
            //     //LEFT JOIN TblMasterCommodityCountry cc ON com.Guid = cc.MasterCommodity_Guid
            //     //                  WHERE  c.Guid = (@NonbarcodeGuid)
            //     //                         AND((com.FlagCommodityGlobal = 1) 
            //     //                         or (com.FlagCommodityGlobal = 0 AND cc.MasterCountry_Guid = @CountryGuid))
            //     //                 ";

            //     var xx = DbContext.Database.Connection.QueryMultiple(sql, data);

            #endregion
        }

        public IEnumerable<PrevaultDepartmentBarcodeScanOutResult> prepareData(IEnumerable<PrevaultDepartmentBarcodeScanOutResult> nonScanedList, Guid tempMainRptGuid, Guid reportStyleGuid, Guid countryGuid, bool FlagGroupNonBarcode)
        {
            if (!FlagGroupNonBarcode)
            {
                var data = nonScanedList.Select(o => new PrevaultDepartmentBarcodeScanOutResult
                {
                     Guid = o.Guid,
                     CustomerLocationGuid = o.CustomerLocationGuid,
                     Location = o.Location
                });
                return data;
            }
            else {
                var grpdata = nonScanedList.SelectMany(o => o.ItemsInGroup).Select(a => new PrevaultDepartmentBarcodeScanOutResult
                {
                    Guid = a.Guid,
                    CustomerLocationGuid = a.CustomerLocationGuid,
                    Location = a.Location
                });
                return grpdata;
            }
        }
    }
}