using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using Dapper;
using System.Data.Entity;
using System.Collections.Concurrent;
using Bgt.Ocean.Models.RunControl.LiabilityLimitModel;
using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models.PreVault;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    #region Interface

    public interface IMasterActualJobHeaderRepository : IRepository<TblMasterActualJobHeader>
    {
        #region Select
        IEnumerable<TblMasterActualJobHeader> FindByListJob(IEnumerable<Guid?> listJob);

        IEnumerable<TblMasterActualJobHeader> FindByListJob(IEnumerable<Guid> listJobGuid);

        IEnumerable<TblMasterActualJobHeader> FindByLegGuidList(IEnumerable<Guid> legGuidList);

        IEnumerable<JobWithLegsModel> FindByDailyRun(Guid dailyRunGuid);

        int GetNumberJobsPerRun(Guid rundailyGuid, DateTime workDate);

        IEnumerable<LiabilityView> GetLiability(Guid siteGuid, Guid jobGuid, IEnumerable<CommodityView> MCTemplate);

        TabDetailView GetCashAddJobDetail(Guid jobGuid);

        SFOTblMasterMachine GetMachineTransferSafeModel(Guid? machineGuid);

        IEnumerable<CapturedCard> GetCaptureCardByJobGuid(Guid jobGuid);

        IEnumerable<MasterIDCollection> GetMasterIDCollection(Guid jobGuid, SealTypeID SealTypeID, IEnumerable<CommodityView> MCTemplate);

        IEnumerable<CitSealView> GetBankBrachSeal(Guid JobGuid, SealTypeID SealTypeID);

        int FindSubServiceTypeIDByJobGuid(Guid jobGuid);
        #endregion

        #region Update
        int UpdateJobOrderByRun(Guid runGuid, bool? appValue1, Guid? routeGuid, DateTime? workDate, Guid? siteGuid, Guid? languageGuid);

        void UpdateJobStatus(IEnumerable<Guid> jobGuid, int statusID, string userName, DateTime clientDateTime, DateTimeOffset universalDateTime);

        int CloseJobsAndRun(Guid dailyRunGuid, Guid userGuid, DateTime clientDate);

        void TruckToTruckTransfer(IEnumerable<TblMasterActualJobHeader> jobList, List<Guid> legGuidList, Guid oldDailyRunGuid, Guid newDailyRunGuid, string userCreated, DateTime datetimeTransfer, string receiverSignature, Guid sender_Guid, string receiverName, DateTime datetimeCreated, DateTimeOffset universalDatetimeCreated);
        #endregion                                                                                                                    

        #region Delete
        void RemoveSFOJob(Guid jobGuid);
        #endregion

        void Func_UpdateJobOrderInRunResource(Guid? runDailyGuid, bool? flagReorder, Guid? routeGuid, DateTime? workDate, Guid? siteGuid, Guid? languageGuid, string strUserClient, DateTime? strDateTimeClient);

        IEnumerable<RunControlJobAndJobLegInRunResult> Func_GetJobAndJobLegInRun(Guid? runGuid);

        void Func_CheckAndUpdateJobOrder(Guid runGuid);

        IEnumerable<RunControlJobByRunDisplayJobResult> Func_JobByRunJob_Get(DateTime? clientDatetime, Guid? userGuid, int? daySequence, Guid? languageGuid, Guid? runResourceGuid, Guid? siteGuid, bool flagShowAll, int? appID);
        IEnumerable<HidePanel> GetJobScreenMapping(Guid jobGuid);

        IEnumerable<CitDeliveryView> GetCitDeliveryView(Guid jobGuid);

        IEnumerable<JobWithStcView> GetSTCOnHandByJobList(IEnumerable<Guid> jobList, Guid siteGuid, Guid? dailyRunGuid,
            List<TblMasterCurrency_ExchangeRate> currencyExchange, Guid userGuid,
            bool calExchangeRate = false, bool isOnHandSummary = false);

        IEnumerable<RawJobDataView> GetJobBCODetail(IEnumerable<BankCleanOutJobView> listData);

        IEnumerable<VaultBalanceJobDetailModel> GetJobDetailVaultBalance(IEnumerable<Guid> jobGuidList);
    }

    #endregion

    public class MasterActualJobHeaderRepository : Repository<OceanDbEntities, TblMasterActualJobHeader>, IMasterActualJobHeaderRepository
    {
        public MasterActualJobHeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void RemoveSFOJob(Guid jobGuid)
        {
            try
            {
                var jobHeader = FindById(jobGuid);

                if (jobHeader == null) return;

                #region Get Data

                // Stop legs
                var stopLegs = DbContext.TblMasterActualJobServiceStopLegs
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // Service Stop
                var stop = DbContext.TblMasterActualJobServiceStop
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // History Actual Job
                var history = DbContext.TblMasterHistory_ActualJob
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // remove otc
                var otc = DbContext.TblMasterActualJobHeader_OTC
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // remove cap
                var cap = DbContext.TblMasterActualJobHeader_Capability
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // remove job sum actual count
                var jobSumActualCount = DbContext.TblMasterActualJobSumActualCount
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // remove job actual count
                var jobActualCount = jobSumActualCount.SelectMany(e => e.TblMasterActualJobActualCount).ToList();

                // remove job sum cash add
                var jobSumCashAdd = DbContext.TblMasterActualJobSumCashAdd
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // remove job cash add
                var jobCashAdd = jobSumCashAdd.SelectMany(e => e.TblMasterActualJobCashAdd).ToList();

                // remove job sum cash return
                var jobSumCashReturn = DbContext.TblMasterActualJobSumCashReturn
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // remove job cash return
                var jobCashReturn = jobSumCashReturn.SelectMany(e => e.TblMasterActualJobCashReturn).ToList();

                // remove job sum machine report
                var jobSumMachineReport = DbContext.TblMasterActualJobSumMachineReport
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                // remove machine report
                var jobMachineReport = jobSumMachineReport.SelectMany(e => e.TblMasterActualJobMachineReport).ToList();

                // remove screen hide
                var screen = DbContext.TblMasterActualJobHideScreenMapping
                    .Where(e => e.MasterActualJobHeader_Guid == jobGuid).ToList();

                #endregion

                #region Remove Data

                // Remove Stop Legs
                DbContext.TblMasterActualJobServiceStopLegs
                    .RemoveRange(stopLegs);

                // Remove Stop
                DbContext.TblMasterActualJobServiceStop
                    .RemoveRange(stop);

                // Remove History
                DbContext.TblMasterHistory_ActualJob
                    .RemoveRange(history);

                // Remove OTC
                DbContext.TblMasterActualJobHeader_OTC
                    .RemoveRange(otc);

                // Remove Cap
                DbContext.TblMasterActualJobHeader_Capability
                    .RemoveRange(cap);

                // Remove Job Sum Actual Count
                DbContext.TblMasterActualJobSumActualCount
                    .RemoveRange(jobSumActualCount);

                // Remove Job Actual Count
                DbContext.TblMasterActualJobActualCount
                    .RemoveRange(jobActualCount);

                // Remove Job Sum Cash Add
                DbContext.TblMasterActualJobSumCashAdd
                    .RemoveRange(jobSumCashAdd);

                // Remove Job Actual Cash Add
                DbContext.TblMasterActualJobCashAdd
                    .RemoveRange(jobCashAdd);

                // Remove Job Sum Cash Return
                DbContext.TblMasterActualJobSumCashReturn
                    .RemoveRange(jobSumCashReturn);

                // Remove Job Cash Return
                DbContext.TblMasterActualJobCashReturn
                    .RemoveRange(jobCashReturn);

                // Remove Job Sum Machine Report
                DbContext.TblMasterActualJobSumMachineReport
                    .RemoveRange(jobSumMachineReport);

                // Remove Job Machine Report
                DbContext.TblMasterActualJobMachineReport
                    .RemoveRange(jobMachineReport);

                // Remove Job Hide Screen
                DbContext.TblMasterActualJobHideScreenMapping
                    .RemoveRange(screen);

                Remove(jobHeader);

                #endregion

            }
            catch
            {
                // no need to handle
            }

        }
        public IEnumerable<TblMasterActualJobHeader> FindByListJob(IEnumerable<Guid?> listJob)
        {
            var jobType = DbContext.TblSystemServiceJobType.AsEnumerable();
            return DbContext.TblMasterActualJobHeader.Where(e => listJob.Contains(e.Guid)).AsEnumerable()
                    .Select(j =>
                    {
                        j.JobTypeID = jobType.FirstOrDefault(t => t.Guid == j.SystemServiceJobType_Guid)?.ServiceJobTypeID ?? 999;
                        return j;
                    });
        }

        public void Func_UpdateJobOrderInRunResource(Guid? runDailyGuid, bool? flagReorder, Guid? routeGuid, DateTime? workDate, Guid? siteGuid, Guid? languageGuid, string strUserClient, DateTime? strDateTimeClient)
        {
            DbContext.Up_OceanOnlineMVC_RunControl_JobOrderByScheduleTimeInRun_Set(runDailyGuid, flagReorder, routeGuid, workDate, siteGuid, languageGuid, strUserClient, strDateTimeClient);
        }
        public IEnumerable<RunControlJobAndJobLegInRunResult> Func_GetJobAndJobLegInRun(Guid? runGuid)
        {
            return DbContext.Up_OceanOnlineMVC_RunControl_JobAndJobLegInRun_Get(runGuid);
        }

        public int GetNumberJobsPerRun(Guid rundailyGuid, DateTime workDate)
        {
            ObjectResult<int?> result = DbContext.Up_OceanOnlineMVC_NumberJobsPerRun_Get(rundailyGuid, workDate);
            int? tmpValue = result != null ? result.FirstOrDefault() : -1;
            int numberJobs = tmpValue != null ? tmpValue.Value : -1;

            return numberJobs;
        }

        public IEnumerable<CitDeliveryView> GetCitDeliveryView(Guid jobGuid)
        {
            var prepareCitDelivery = DbContext.TblMasterActualJobMCSCITDelivery.Where(o => o.MasterActualJobHeader_Guid == jobGuid).AsEnumerable();
            var CitDelivery = prepareCitDelivery.Select(e => new CitDeliveryView
            {
                Seal = e.SealNo,
                Commodity = e.TblSystemMCSCITDeliveryCommodityType?.CommodityTypeName,
                CustomerOrderNumber = e.CustomerOrderNumber,
                Liability = e.STCValue,
                Currency = DbContext.TblMasterCurrency.FirstOrDefault(o => o.Guid == e.MasterCurrency_Guid)?.MasterCurrencyAbbreviation,
                DeliveryDatetime = e.DeliveryDatetime,
                Status = EnumHelper.GetDescription((CITDeliveryStatus)e.CITDeliveryStatus),
                ScanStatus = e.TblSystemMCSCITDeliveryScanStatus.ScanStatusName,
                CancelReason = e.MasterReasonType_ReasonTypename,
                Comment = e.CommentIfNotReceived
            });
            return CitDelivery;
        }

        public IEnumerable<LiabilityView> GetLiability(Guid siteGuid, Guid jobGuid, IEnumerable<CommodityView> MCTemplate)
        {
            #region GET LIABILITY
            //using AsEnumerable for check null(?) on select statement
            var prepareLiability = DbContext.TblMasterActualJobItemsLiability.Where(o => o.MasterActualJobHeader_Guid == jobGuid).AsEnumerable();
            var liabilities = prepareLiability.Select(l => new LiabilityView
            {
                DocumentRef = l.DocumentRef,
                CurrencyAbbr = DbContext.TblMasterCurrency.FirstOrDefault(o => o.Guid == l.MasterCurrency_Guid)?.MasterCurrencyAbbreviation,
                CommodityName = MCTemplate.FirstOrDefault(c => c.MasterCommodity_Guid == l.MasterCommodity_Guid)?.CommodityName,
                Liability = Convert.ToDouble(l.Liability),
                SealList = GetLiabilitySeal(l.Guid, SealTypeID.Other)
            });

            #endregion

            return liabilities.Where(o => o.SealList.Any(s => s.SealTypeID == (int)SealTypeID.Other));
        }

        private IEnumerable<CitSealView> GetLiabilitySeal(Guid Liability_Guid, SealTypeID SealTypeID)
        {

            return (from seal in DbContext.TblMasterActualJobItemsSeal.Where(s => s.MasterActualJobItemsCommodity_Guid == Liability_Guid)
                    join type in DbContext.TblSystemSealType on seal.SystemSealType_Guid equals type.Guid
                    where seal.MasterActualJobItemsCommodity_Guid == Liability_Guid && type.SealTypeID == (int)SealTypeID
                    select new { seal, type })
                    //using AsEnumerable for check null(?) on select statement
                    .AsEnumerable().Select(s => new CitSealView
                    {
                        SealNo = s.seal.SealNo,
                        MasterID = DbContext.TblMasterConAndDeconsolidate_Header.FirstOrDefault(o => o.Guid == s.seal.MasterConAndDeconsolidateHeaderMasterID_Guid)?.MasterID,
                        MasterID_Route = DbContext.TblMasterConAndDeconsolidate_Header.FirstOrDefault(o => o.Guid == s.seal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid)?.MasterID,
                        SealTypeID = s.type.SealTypeID
                    });
        }

        public IEnumerable<CitSealView> GetBankBrachSeal(Guid JobGuid, SealTypeID SealTypeID)
        {

            return (from seal in DbContext.TblMasterActualJobItemsSeal
                    join type in DbContext.TblSystemSealType on seal.SystemSealType_Guid equals type.Guid
                    where seal.MasterActualJobHeader_Guid == JobGuid && type.SealTypeID == (int)SealTypeID
                    select new { seal, type })
                    //using AsEnumerable for check null(?) on select statement
                    .AsEnumerable().Select(s => new CitSealView
                    {
                        SealNo = s.seal.SealNo,
                        MasterID = DbContext.TblMasterConAndDeconsolidate_Header.FirstOrDefault(o => o.Guid == s.seal.MasterConAndDeconsolidateHeaderMasterID_Guid)?.MasterID,
                        MasterID_Route = DbContext.TblMasterConAndDeconsolidate_Header.FirstOrDefault(o => o.Guid == s.seal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid)?.MasterID,
                        SealTypeID = s.type.SealTypeID
                    });
        }
        public IEnumerable<MasterIDCollection> GetMasterIDCollection(Guid jobGuid, SealTypeID SealTypeID, IEnumerable<CommodityView> MCTemplate)
        {

            List<MasterIDCollection> result = new List<MasterIDCollection>();

            //using AsEnumerable for check null(?) on select statement
            //Get all seal
            var allSeals = (from seal in DbContext.TblMasterActualJobItemsSeal
                            join type in DbContext.TblSystemSealType on seal.SystemSealType_Guid equals type.Guid
                            join liability in DbContext.TblMasterActualJobItemsLiability on seal.MasterActualJobItemsCommodity_Guid equals liability.Guid
                            where type.SealTypeID == (int)SealTypeID && seal.MasterActualJobHeader_Guid == jobGuid
                            select new { seal, liability }).AsEnumerable()
                                 .Select(l => new SmallBagAndBulkCashSealView
                                 {
                                     CurrencyAbbr = DbContext.TblMasterCurrency.FirstOrDefault(o => o.Guid == l.liability.MasterCurrency_Guid)?.MasterCurrencyAbbreviation,
                                     CommodityName = MCTemplate.FirstOrDefault(c => c.MasterCommodity_Guid == l.liability.MasterCommodity_Guid)?.CommodityName,
                                     Liability = Convert.ToDouble(l.liability.Liability),
                                     SealNo = l.seal.SealNo,
                                     MasterConAndDeconsolidateHeaderMasterID_Guid = l.seal.MasterConAndDeconsolidateHeaderMasterID_Guid,
                                     MasterConAndDeconsolidateHeaderMasterIDRoute_Guid = l.seal.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid,
                                     Liability_Guid = l.liability.Guid
                                 });

            //prepare
            var flagHasDeconSeal = allSeals.Any(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == null && o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == null);
            var masterID = allSeals.Select(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid);
            var masterIDRoute = allSeals.Select(o => o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid);
            var masterIDs = masterID.Union(masterIDRoute);


            //seal with out master id
            if (flagHasDeconSeal)
            {
                result.Add(new MasterIDCollection
                {
                    MasterID = "-",
                    MasterID_Route = "-",
                    CurrencyAbbr = "-",
                    Liability = 0,
                    SealList = allSeals.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == null && o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == null).OrderBy(o => o.Liability_Guid).ThenBy(o => o.SealNo).ToList()
                });
            }

            //seal has master id
            result.AddRange((from conh in DbContext.TblMasterConAndDeconsolidate_Header
                             join masterid in masterIDs on conh.Guid equals masterid
                             select conh)
                          .AsEnumerable().Select(m => new MasterIDCollection
                          {
                              MasterID = masterID.Any(o => o == m.Guid) ? m.MasterID : "-",
                              MasterID_Route = masterIDRoute.Any(o => o == m.Guid) ? m.MasterID : "-",
                              CurrencyAbbr = DbContext.TblMasterCurrency.FirstOrDefault(o => o.Guid == m.MasterCurrency_Guid)?.MasterCurrencyAbbreviation,
                              Liability = Convert.ToDouble(m.Liability),
                              SealList = allSeals.Where(o => o.MasterConAndDeconsolidateHeaderMasterID_Guid == m.Guid || o.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid == m.Guid).OrderBy(o => o.Liability_Guid).ThenBy(o => o.SealNo).ToList(),
                          }).OrderBy(o => o.MasterID_Route).ThenBy(o => o.MasterID));

            /* Keep only Liability on First Seal - Have MasterID */
            foreach (var master in result.Where(o => o.MasterID != "-" || o.MasterID_Route != "-"))
            {
                var liabilities = master.SealList.GroupBy(o => o.Liability_Guid).Select(o => o.Key).Distinct();
                foreach (var lia in liabilities)
                {
                    var seals = master.SealList.Where(o => o.Liability_Guid == lia).OrderBy(o => o.SealNo).Where((data, index) => index > 0).ToList();
                    seals.ForEach(o => o.Liability = 0);
                }
            }

            /* Keep only Liability on First Seal - No MasterID */
            foreach (var nomaster in result.Where(o => o.MasterID == "-" && o.MasterID_Route == "-"))
            {
                var liabilities = nomaster.SealList.GroupBy(o => o.Liability_Guid).Select(o => o.Key).Distinct();
                foreach (var lia in liabilities)
                {
                    var seals = nomaster.SealList.Where(o => o.Liability_Guid == lia).OrderBy(o => o.SealNo).Where((data, index) => index > 0).ToList();
                    seals.ForEach(o => o.Liability = 0);
                }
            }

            return result;
        }
        public SFOTblMasterMachine GetMachineTransferSafeModel(Guid? machineGuid)
        {
            var tranferSafeID = (from ass in DbContext.SFOTblMasterMachine_AssociatedMachine.Where(o => o.SFOMasterMachine_Guid == machineGuid)
                                 join mac in DbContext.SFOTblMasterMachine on ass.SFOMasterAssociatedMachine_Guid equals mac.Guid
                                 join type in DbContext.TblSystemCustomerLocationType on mac.SystemMachineType_Guid equals type.Guid
                                 where type.CustomerLocationTypeID == CustomerLocationTypeName.TransferSafe
                                 select mac).FirstOrDefault();
            return tranferSafeID;
        }

        public IEnumerable<CapturedCard> GetCaptureCardByJobGuid(Guid jobGuid)
        {

            return DbContext.TblMasterCapturedCard.Where(o => o.MasterActualJobHeader_Guid == jobGuid)
                                                        .Select(o => new CapturedCard
                                                        {
                                                            BankName = o.BankName,
                                                            CardNo = o.CardNo,
                                                            HolderName = o.HolderName
                                                        });
        }

        public int FindSubServiceTypeIDByJobGuid(Guid jobGuid)
        {
            return (from H in DbContext.TblMasterActualJobHeader
                    join SUB in DbContext.TblMasterSubServiceType on H.MasterSubServiceType_Guid equals SUB.Guid
                    where H.Guid == jobGuid
                    select SUB.SubServiceTypeID).FirstOrDefault() ?? 0;
        }
        public TabDetailView GetCashAddJobDetail(Guid jobGuid)
        {

            var result = from H in DbContext.TblMasterActualJobHeader
                         join LOB in DbContext.TblSystemLineOfBusiness on H.SystemLineOfBusiness_Guid equals LOB.Guid
                         join TYPE in DbContext.TblSystemServiceJobType on H.SystemServiceJobType_Guid equals TYPE.Guid
                         join LEG in DbContext.TblMasterActualJobServiceStopLegs on H.Guid equals LEG.MasterActualJobHeader_Guid
                         join LOC in DbContext.TblMasterCustomerLocation on LEG.MasterCustomerLocation_Guid equals LOC.Guid
                         join CUS in DbContext.TblMasterCustomer on LOC.MasterCustomer_Guid equals CUS.Guid
                         //Left join
                         join PLA in DbContext.TblMasterPlace on LOC.MasterPlace_Guid equals PLA.Guid into empPlace
                         from placeEmp in empPlace.DefaultIfEmpty()

                             //Left join
                         join Model in DbContext.SFOTblMasterMachineModelType on LOC.SFOTblMasterMachine.SFOMasterMachineModelType_Guid equals Model.Guid into empModel
                         from ModelEmp in empModel.DefaultIfEmpty()

                             //Left join
                         join CONT in DbContext.TblMasterCustomerContract on H.MasterCustomerContract_Guid equals CONT.Guid into empCONT
                         from CONTEmp in empCONT.DefaultIfEmpty()

                         where H.Guid == jobGuid && LEG.SequenceStop == 1 && TYPE.ServiceJobTypeID == IntTypeJob.MCS
                         select new TabDetailView
                         {
                             JobID = H.JobNo,
                             LOB = LOB.LOBFullName,
                             ServiceStopTransectionDate = LEG.ServiceStopTransectionDate,
                             ServiceTypeName = TYPE.ServiceJobTypeName,
                             WindowsTimeServiceTimeStart = LEG.WindowsTimeServiceTimeStart,
                             CustomerName = CUS.CustomerFullName,
                             LocationName = LOC.BranchName,
                             PlaceName = placeEmp.BuildingName ?? "-",
                             ProblemCode = "-",
                             MachineModel = ModelEmp.MachineModelTypeName ?? "-",
                             LockType1 = placeEmp.FlagLocken ? "Locken" : "No Lock",
                             Machine_Guid = LOC.SFOTblMasterMachine.Guid,
                             ATMID = LOC.SFOTblMasterMachine.MachineID ?? "-",
                             ContractNo = CONTEmp.ContractNo ?? "-"

                         };

            return result.FirstOrDefault();
        }

        public IEnumerable<HidePanel> GetJobScreenMapping(Guid jobGuid)
        {
            List<HidePanel> returnList = new List<HidePanel>();
            var jobScreenlist = DbContext.TblMasterActualJobHideScreenMapping
                  .Where(o => o.MasterActualJobHeader_Guid == jobGuid).AsEnumerable()
                  .Select(o => new { o.SystemJobHideScreen_Guid, o.SystemJobHideField_Guid });
            foreach (var jobScreen in jobScreenlist.GroupBy(e => e.SystemJobHideScreen_Guid))
            {
                HidePanel HidePanel = new HidePanel();
                HidePanel.JobScreen = jobScreen.Key.ConvertToEnumJobScreen<JobScreen>();
                HidePanel.JobField = jobScreen.Select(o => o.SystemJobHideField_Guid.GetValueOrDefault().ConvertToEnumJobScreen<JobField>());
                returnList.Add(HidePanel);
            }

            return returnList;
        }

        public void Func_CheckAndUpdateJobOrder(Guid runGuid)
        {
            DbContext.Database.Connection
                .Execute("Up_OceanOnlineMVC_RunControl_CheckJobOrder_Set", new
                {
                    @StrDailyRunResourceGuid = runGuid
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public int UpdateJobOrderByRun(Guid runGuid, bool? appValue1, Guid? routeGuid, DateTime? workDate, Guid? siteGuid, Guid? languageGuid)
        {
            return DbContext.Database.Connection
                .Execute("Up_OceanOnlineMVC_RunControl_JobOrderByScheduleTimeInRun_Set", new
                {
                    @StrDailyRunResourceGuid = runGuid,
                    @FlagAllowAutoReOrderJobIndexWhenAssignToRun = appValue1,
                    @MasterRouteGuid = routeGuid,
                    @WorkDate = workDate,
                    @SiteGuid = siteGuid,
                    @LanguageGuid = languageGuid
                }, commandType: System.Data.CommandType.StoredProcedure);
        }
        public IEnumerable<RunControlJobByRunDisplayJobResult> Func_JobByRunJob_Get(DateTime? clientDatetime, Guid? userGuid, int? daySequence, Guid? languageGuid, Guid? runResourceGuid, Guid? siteGuid, bool flagShowAll, int? appID)
        {
            return DbContext.Up_OceanOnlineMVC_RunControl_JobByRun_Display_Job_Get(daySequence, runResourceGuid, siteGuid, languageGuid, flagShowAll, appID, clientDatetime, userGuid);
        }

        #region For check out to department
        public void UpdateJobStatus(IEnumerable<Guid> jobGuid, int statusID, string userName, DateTime clientDateTime, DateTimeOffset universalDateTime)
        {
            var data = jobGuid.Select(o => new { jobGuid = o, clientDateTime = clientDateTime, universalDateTime = universalDateTime });
            string sql = $@"SET NOCOUNT ON
                            UPDATE TblMasterActualJobHeader
                                        SET  OnwardDestinationType = 1,
                                             SystemStatusJobID = IIF(TblMasterActualJobHeader.SystemStatusJobID = 35, 36, {statusID}),
                                             UserModifed = '{userName}',
                                             DatetimeModified = @clientDateTime,
                                             UniversalDatetimeModified = @universalDateTime
                                         WHERE Guid = @jobGuid";
            DbContext.Database.Connection.Execute(sql, data);
        }
        #endregion

        public IEnumerable<TblMasterActualJobHeader> FindByListJob(IEnumerable<Guid> listJobGuid)
        {
            var jobType = DbContext.TblSystemServiceJobType.AsEnumerable();
            return DbContext.TblMasterActualJobHeader.Where(e => listJobGuid.Contains(e.Guid)).AsEnumerable()
                    .Select(j =>
                    {
                        j.JobTypeID = jobType.FirstOrDefault(t => t.Guid == j.SystemServiceJobType_Guid)?.ServiceJobTypeID ?? 999;
                        return j;
                    });
        }

        public IEnumerable<JobWithLegsModel> FindByDailyRun(Guid dailyRunGuid)
        {
            return DbContext.TblMasterActualJobServiceStopLegs.Where(o => o.MasterRunResourceDaily_Guid == dailyRunGuid)
                    .Join(DbContext.TblMasterActualJobHeader, leg => leg.MasterActualJobHeader_Guid, job => job.Guid, (leg, job) => new { Job = job, Leg = leg })
                    .GroupBy(o => o.Job, o => o.Leg, (key, x) => new JobWithLegsModel { JobHeader = key, JobLegs = x.ToList() });
        }

        public int CloseJobsAndRun(Guid dailyRunGuid, Guid userGuid, DateTime clientDate)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                using (DbContextTransaction trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var username = context.TblMasterUser.FirstOrDefault(o => o.Guid == userGuid).UserName + " - CloseRunManual";
                        var jobStatus = context.TblSystemJobStatus.ToList();
                        var run = context.TblMasterDailyRunResource.FirstOrDefault(o => o.Guid == dailyRunGuid);
                        if (run == null)
                        {
                            return -184;
                        }

                        var checkDate = run.WorkDate.Value;
                        if (run.RunResourceDailyStatusID >= 2)
                        {
                            checkDate = run.RunResourceDispatchDateTime.HasValue ? run.RunResourceDispatchDateTime.Value : run.WorkDate.Value;
                        }
                        if (checkDate > clientDate.AddHours(-24))
                        {
                            return -759;
                        }

                        var reason = context.TblMasterReasonUnableToService.FirstOrDefault(o => o.MasterCustomer_Guid == context.TblMasterSite.FirstOrDefault(x => x.Guid == run.MasterSite_Guid).MasterCustomer_Guid)?.Guid;
                        DateTime stampTime = Convert.ToDateTime("1900-01-01" + run.RunResourceDispatchDateTime.GetValueOrDefault().ToString(" HH:mm:ss"));

                        var jobs = context.TblMasterActualJobServiceStopLegs.Where(o => o.MasterRunResourceDaily_Guid == dailyRunGuid)
                                    .Join(context.TblMasterActualJobHeader, leg => leg.MasterActualJobHeader_Guid, job => job.Guid, (leg, job) => new { Job = job, Leg = leg })
                                    .GroupBy(o => o.Job, o => o.Leg, (key, x) => new JobWithLegsModel { JobHeader = key, JobLegs = x.ToList() }).ToList();

                        jobs.Where(o => o.JobHeader.SystemStatusJobID == 4 || o.JobHeader.SystemStatusJobID == 5)
                            .Select(o =>
                            {
                                o.Seals = context.TblMasterActualJobItemsSeal.Where(x => x.MasterActualJobHeader_Guid.Value == o.JobHeader.Guid).ToList();
                                o.Commodities = context.TblMasterActualJobItemsCommodity.Where(x => x.MasterActualJobHeader_Guid.Value == o.JobHeader.Guid).ToList();
                                return o;
                            }).ToList();

                        var prevaultTypeGuid = Guid.Parse("68E97615-0B28-4F8C-A8A6-BC0FB6F29635");
                        var allSite = new List<Guid>();
                        if (jobs.Any(o => o.JobHeader.SystemServiceJobType_Guid.ToString().ToUpper() == "7E6B12FE-965D-4043-83BC-8A3906F561EA" && o.JobHeader.SystemStatusJobID == 4))
                        {
                            allSite = jobs.Where(o => o.JobHeader.SystemServiceJobType_Guid.ToString().ToUpper() == "7E6B12FE-965D-4043-83BC-8A3906F561EA" && o.JobHeader.SystemStatusJobID == 4)
                                        .Select(o => o.JobLegs.Where(x => x.MasterSite_Guid.HasValue).OrderByDescending(x => x.SequenceStop).FirstOrDefault())
                                        .Select(o => o.MasterSite_Guid.Value).Distinct().ToList();
                        }

                        var allVaults = allSite.Any() ? allSite.Join(context.TblMasterCustomerLocation, site => site, loc => loc.MasterSite_Guid, (site, loc) => loc)
                                        .Join(context.TblMasterCustomerLocation_InternalDepartment.Where(x => x.FlagDisable == false && x.InternalDepartmentType == prevaultTypeGuid), brinks => brinks.Guid, vault => vault.MasterCustomerLocation_Guid, (brinks, vault)
                                            => new { SiteGuid = brinks.MasterSite_Guid, VaultGuid = vault.Guid, VaultCode = vault.InternalDepartmentReferenceCode })
                                        .GroupBy(o => o.SiteGuid).Select(o => o.OrderBy(x => x.VaultCode)).FirstOrDefault() : null;

                        var allJobs = new ConcurrentBag<JobWithLegsModel>(jobs);
                        var historyJobs = new ConcurrentBag<TblMasterHistory_ActualJob>();

                        #region Update Job Status
                        Parallel.ForEach(allJobs, o =>
                        {
                            o.JobHeader.SystemStatusJobIDPrevious = o.JobHeader.SystemStatusJobID;
                            switch (o.JobHeader.SystemServiceJobType_Guid.ToString().ToUpper())
                            {
                                #region Pick Up
                                case "37E2F276-C461-4AB8-A22F-62D3673CD16A":
                                    if (o.JobHeader.FlagJobInterBranch)
                                    {
                                        switch (o.JobHeader.SystemStatusJobID)
                                        {
                                            case 2:
                                                o.JobHeader.SystemStatusJobID = 14;
                                                o.JobHeader.FlagCancelAll = true;
                                                o.FlagUpdate = true;
                                                break;
                                            case 3:
                                                o.JobHeader.SystemStatusJobID = 20;
                                                o.FlagUpdate = true;
                                                break;
                                            case 4:
                                            case 7:
                                            case 29:
                                            case 35:
                                                o.JobHeader.SystemStatusJobID = 11;
                                                o.FlagUpdate = true;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (o.JobHeader.SystemStatusJobID)
                                        {
                                            case 2:
                                                o.JobHeader.SystemStatusJobID = 14;
                                                o.JobHeader.FlagCancelAll = true;
                                                o.FlagUpdate = true;
                                                break;
                                            case 3:
                                                o.JobHeader.SystemStatusJobID = 20;
                                                o.FlagUpdate = true;
                                                break;
                                            case 4:
                                            case 7:
                                            case 35:
                                                o.JobHeader.SystemStatusJobID = 11;
                                                o.FlagUpdate = true;
                                                break;
                                        }
                                    }
                                    break;
                                #endregion

                                #region Delivery
                                case "979CC475-FA28-4AB4-B039-2EE780BEE762":
                                    switch (o.JobHeader.SystemStatusJobID)
                                    {
                                        case 2:
                                        case 3:
                                        case 6:
                                        case 35:
                                            o.JobHeader.SystemStatusJobID = 8;
                                            o.FlagUpdate = true;
                                            break;
                                        case 5:
                                            if (o.Seals.Any() || o.Commodities.Any())
                                            {
                                                o.JobHeader.SystemStatusJobID = 8;
                                            }
                                            else
                                            {
                                                o.JobHeader.SystemStatusJobID = 14;
                                                o.JobHeader.FlagCancelAll = true;
                                            }
                                            o.FlagUpdate = true;
                                            break;
                                        case 7:
                                            o.JobHeader.SystemStatusJobID = !o.JobHeader.FlagNonDelivery.Value ? 8 : 16;
                                            o.FlagUpdate = true;
                                            break;
                                        case 12:
                                            o.JobHeader.SystemStatusJobID = 20;
                                            o.FlagUpdate = true;
                                            break;
                                    }
                                    break;
                                #endregion

                                #region Transfer Vault
                                case "7E6B12FE-965D-4043-83BC-8A3906F561EA":
                                    if (o.JobHeader.FlagJobInterBranch)
                                    {
                                        switch (o.JobHeader.SystemStatusJobID)
                                        {
                                            case 2:
                                                o.JobHeader.SystemStatusJobID = 14;
                                                o.JobHeader.FlagCancelAll = true;
                                                o.FlagUpdate = true;
                                                break;
                                            case 4:
                                                o.JobHeader.SystemStatusJobID = o.JobLegs.FirstOrDefault().SequenceStop <= 2 ? 29 : 8;
                                                o.FlagUpdate = true;
                                                break;
                                            case 12:
                                            case 24:
                                            case 26:
                                                o.JobHeader.SystemStatusJobID = 20;
                                                o.FlagUpdate = true;
                                                break;
                                            case 7:
                                            case 25:
                                            case 27:
                                            case 28:
                                            case 29:
                                            case 35:
                                                if (o.JobLegs.FirstOrDefault().SequenceStop > 2)
                                                {
                                                    o.JobHeader.SystemStatusJobID = 8;
                                                    o.FlagUpdate = true;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (o.JobHeader.SystemStatusJobID)
                                        {
                                            case 2:
                                                o.JobHeader.SystemStatusJobID = 14;
                                                o.JobHeader.FlagCancelAll = true;
                                                o.FlagUpdate = true;
                                                break;
                                            case 4:
                                                o.JobHeader.SystemStatusJobID = o.JobLegs.FirstOrDefault().SequenceStop <= 2 ? 29 : 8;
                                                o.FlagUpdate = true;
                                                break;
                                            case 12:
                                            case 24:
                                            case 26:
                                                o.JobHeader.SystemStatusJobID = 20;
                                                o.FlagUpdate = true;
                                                break;
                                            case 25:
                                            case 27:
                                            case 28:
                                            case 29:
                                            case 35:
                                                if (o.JobLegs.FirstOrDefault().SequenceStop > 2)
                                                {
                                                    o.JobHeader.SystemStatusJobID = 8;
                                                    o.FlagUpdate = true;
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                #endregion

                                #region Transfer
                                case "34C0E3E0-6505-4D18-B327-5BD12B44ED07":
                                    switch (o.JobHeader.SystemStatusJobID)
                                    {
                                        case 2:
                                            o.JobHeader.SystemStatusJobID = 14;
                                            o.JobHeader.FlagCancelAll = true;
                                            o.FlagUpdate = true;
                                            break;
                                        case 3:
                                        case 12:
                                            o.JobHeader.SystemStatusJobID = 20;
                                            o.FlagUpdate = true;
                                            break;
                                        case 4:
                                        case 7:
                                        case 35:
                                            o.JobHeader.SystemStatusJobID = 8;
                                            o.FlagUpdate = true;
                                            break;
                                    }
                                    break;
                                #endregion

                                #region Bank Clean-out Pick Up
                                case "4AB7DF1B-AA14-4E9F-BDDF-2C1858910E05":
                                    switch (o.JobHeader.SystemStatusJobID)
                                    {
                                        case 2:
                                        case 3:
                                        case 4:
                                            o.JobHeader.SystemStatusJobID = 7;
                                            o.FlagUpdate = true;
                                            break;
                                    }
                                    break;
                                #endregion

                                #region Bank Clean-Out Delivery
                                case "F654A676-9679-4982-A62F-2E3D9DAC4EA4":
                                    if (o.JobHeader.FlagJobInterBranch)
                                    {
                                        switch (o.JobHeader.SystemStatusJobID)
                                        {
                                            case 2:
                                            case 3:
                                            case 17:
                                            case 35:
                                                o.JobHeader.SystemStatusJobID = 8;
                                                o.FlagUpdate = true;
                                                break;
                                            case 7:
                                                o.JobHeader.SystemStatusJobID = !o.JobHeader.FlagNonDelivery.Value ? 8 : 16;
                                                o.FlagUpdate = true;
                                                break;
                                            case 12:
                                                o.JobHeader.SystemStatusJobID = 20;
                                                o.FlagUpdate = true;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (o.JobHeader.SystemStatusJobID)
                                        {
                                            case 2:
                                            case 3:
                                            case 17:
                                            case 35:
                                                o.JobHeader.SystemStatusJobID = 8;
                                                o.FlagUpdate = true;
                                                break;
                                            case 7:
                                                o.JobHeader.SystemStatusJobID = !o.JobHeader.FlagNonDelivery.Value ? 8 : 16;
                                                o.FlagUpdate = true;
                                                break;
                                            case 12:
                                                o.JobHeader.SystemStatusJobID = 20;
                                                o.FlagUpdate = true;
                                                break;
                                        }
                                    }
                                    break;
                                #endregion

                                #region MCS
                                case "960F9752-66B9-49F2-BC19-F717FF203593":
                                    switch (o.JobHeader.SystemStatusJobID)
                                    {
                                        case 2:
                                            o.JobHeader.SystemStatusJobID = 14;
                                            o.JobHeader.FlagCancelAll = true;
                                            o.FlagUpdate = true;
                                            break;
                                        case 3:
                                            o.JobHeader.SystemStatusJobID = 20;
                                            o.FlagUpdate = true;
                                            break;
                                        case 111:
                                            o.JobHeader.SystemStatusJobID = 9;
                                            o.FlagUpdate = true;
                                            break;
                                    }
                                    break;
                                #endregion

                                #region Transfer Vault (Multi-Branch)
                                case "09E6E0A4-CA5B-426A-A29B-6A6EADCFE755":
                                    switch (o.JobHeader.SystemStatusJobID)
                                    {
                                        case 4:
                                            o.JobHeader.SystemStatusJobID = o.JobLegs.FirstOrDefault().SequenceStop <= 2 ? 28 : 9;
                                            o.FlagUpdate = true;
                                            break;
                                        case 24:
                                        case 26:
                                            o.JobHeader.SystemStatusJobID = 20;
                                            o.FlagUpdate = true;
                                            break;
                                        case 8:
                                        case 25:
                                        case 27:
                                        case 28:
                                        case 29:
                                        case 35:
                                            if (o.JobLegs.FirstOrDefault().SequenceStop > 2)
                                            {
                                                o.JobHeader.SystemStatusJobID = 9;
                                                o.FlagUpdate = true;
                                            }
                                            break;
                                    }
                                    break;
                                #endregion

                                #region Bank Clean-Out Delivery (Multi-Branch)
                                case "C8D40330-4F1A-4AF2-BE99-7967D0F04A73":
                                    switch (o.JobHeader.SystemStatusJobID)
                                    {
                                        case 2:
                                        case 3:
                                        case 7:
                                        case 8:
                                        case 35:
                                            o.JobHeader.SystemStatusJobID = 9;
                                            o.FlagUpdate = true;
                                            break;
                                    }
                                    break;
                                #endregion

                                #region Pick Up (Multi-Branch)
                                case "8AE01862-EA34-406F-8B80-48AB74C0544C":
                                    switch (o.JobHeader.SystemStatusJobID)
                                    {
                                        case 2:
                                            o.JobHeader.SystemStatusJobID = 14;
                                            o.JobHeader.FlagCancelAll = true;
                                            o.FlagUpdate = true;
                                            break;
                                        case 3:
                                            o.JobHeader.SystemStatusJobID = 20;
                                            o.FlagUpdate = true;
                                            break;
                                        case 4:
                                        case 7:
                                        case 35:
                                            o.JobHeader.SystemStatusJobID = 11;
                                            o.FlagUpdate = true;
                                            break;
                                    }
                                    break;
                                    #endregion
                            }

                            #region Update Header and Legs
                            if (o.FlagUpdate)
                            {
                                if (o.JobHeader.SystemStatusJobID == 20)
                                {
                                    o.JobHeader.ReasonUnableToService_Guid = reason;
                                    o.JobHeader.ReasonUnableToServiceDateTime = run.RunResourceDispatchDateTime.HasValue ? run.RunResourceDispatchDateTime : clientDate;

                                    var leg = o.JobLegs.OrderBy(x => x.SequenceStop).FirstOrDefault();
                                    if (!leg.ArrivalTime.HasValue)
                                    {
                                        leg.InputedDate = run.RunResourceDispatchDateTime;
                                        leg.ArrivalTime = stampTime;
                                        leg.ActualTime = stampTime;
                                        leg.DepartTime = stampTime;
                                    }
                                }
                                else if (o.JobHeader.SystemStatusJobID != 14)
                                {
                                    o.JobHeader.FlagNonDelivery = false;
                                    o.JobHeader.ReasonUnableToService_Guid = null;
                                    o.JobHeader.ReasonUnableToServiceDateTime = null;

                                    var leg = o.JobLegs.OrderByDescending(x => x.SequenceStop).FirstOrDefault();
                                    if (!leg.ArrivalTime.HasValue)
                                    {
                                        leg.InputedDate = run.RunResourceDispatchDateTime;
                                        leg.ArrivalTime = stampTime;
                                        leg.ActualTime = stampTime;
                                        leg.DepartTime = stampTime;
                                    }
                                }

                                o.JobHeader.UserModifed = username;
                                o.JobHeader.DatetimeModified = clientDate;
                                o.JobHeader.UniversalDatetimeModified = DateTime.UtcNow;

                                #region Handle Internal Department for P-Jobs
                                if (o.JobHeader.OnwardDestination_Guid.HasValue)
                                {
                                    if (o.Seals != null && o.Seals.Any())
                                    {
                                        foreach (var seal in o.Seals)
                                        {
                                            seal.MasterCustomerLocation_InternalDepartment_Guid = o.JobHeader.OnwardDestination_Guid;
                                            seal.MasterCustomerLocation_InternalDepartmentArea_Guid = null;
                                            seal.MasterCustomerLocation_InternalDepartmentSubArea_Guid = null;
                                            seal.UserModifed = username;
                                            seal.DatetimeModified = clientDate;
                                            seal.UniversalDatetimeModified = DateTime.UtcNow;
                                        }
                                    }

                                    if (o.Commodities != null && o.Commodities.Any())
                                    {
                                        foreach (var commodity in o.Commodities)
                                        {
                                            commodity.MasterCustomerLocation_InternalDepartment_Guid = o.JobHeader.OnwardDestination_Guid;
                                            commodity.MasterCustomerLocation_InternalDepartmentArea_Guid = null;
                                            commodity.MasterCustomerLocation_InternalDepartmentSubArea_Guid = null;
                                            commodity.UserModifed = username;
                                            commodity.DatetimeModified = clientDate;
                                            commodity.UniversalDatetimeModified = DateTime.UtcNow;
                                        }
                                    }
                                }
                                #endregion

                                #region Handle Pre-Vault for TV
                                if (o.JobHeader.SystemServiceJobType_Guid.ToString().ToUpper() == "7E6B12FE-965D-4043-83BC-8A3906F561EA" && o.JobHeader.SystemStatusJobID == 29)
                                {
                                    o.JobHeader.FlagChkOutInterBranchComplete = o.JobHeader.FlagJobInterBranch;

                                    var prevault = allVaults.FirstOrDefault(x => x.SiteGuid == o.JobLegs.OrderByDescending(y => y.SequenceStop).First().MasterSite_Guid);

                                    if (o.Seals != null && o.Seals.Any() && prevault != null)
                                    {
                                        foreach (var seal in o.Seals)
                                        {
                                            seal.MasterCustomerLocation_InternalDepartment_Guid = prevault.VaultGuid;
                                            seal.MasterCustomerLocation_InternalDepartmentArea_Guid = null;
                                            seal.MasterCustomerLocation_InternalDepartmentSubArea_Guid = null;
                                            seal.UserModifed = username;
                                            seal.DatetimeModified = clientDate;
                                            seal.UniversalDatetimeModified = DateTime.UtcNow;
                                        }
                                    }

                                    if (o.Commodities != null && o.Commodities.Any() && prevault != null)
                                    {
                                        foreach (var commodity in o.Commodities)
                                        {
                                            commodity.MasterCustomerLocation_InternalDepartment_Guid = prevault.VaultGuid;
                                            commodity.MasterCustomerLocation_InternalDepartmentArea_Guid = null;
                                            commodity.MasterCustomerLocation_InternalDepartmentSubArea_Guid = null;
                                            commodity.UserModifed = username;
                                            commodity.DatetimeModified = clientDate;
                                            commodity.UniversalDatetimeModified = DateTime.UtcNow;
                                        }
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            #region Add Job History (Temp)
                            if (o.JobHeader.SystemStatusJobIDPrevious != o.JobHeader.SystemStatusJobID)
                            {
                                historyJobs.Add(new TblMasterHistory_ActualJob
                                {
                                    Guid = Guid.NewGuid(),
                                    MasterActualJobHeader_Guid = o.JobHeader.Guid,
                                    MsgID = 800,
                                    MsgParameter = jobStatus.FirstOrDefault(x => x.StatusJobID == o.JobHeader.SystemStatusJobIDPrevious).StatusJobName + ","
                                                    + jobStatus.FirstOrDefault(x => x.StatusJobID == o.JobHeader.SystemStatusJobID).StatusJobName
                                                    + ",Manual Close Run",
                                    UserCreated = username,
                                    DatetimeCreated = clientDate,
                                    UniversalDatetimeCreated = DateTime.UtcNow
                                });
                            }
                            #endregion
                        });
                        #endregion

                        #region Create Job History
                        context.TblMasterHistory_ActualJob.AddRange(historyJobs);
                        #endregion

                        #region Close Run
                        run.RunResourceDailyStatusID = 3;
                        run.OdoMeterStop = run.OdoMeterStart;
                        run.RunResourceDispatchDateTime = run.RunResourceDispatchDateTime.HasValue ? run.RunResourceDispatchDateTime : clientDate;
                        run.RunResourceCloseDateTime = run.RunResourceDispatchDateTime.HasValue ? run.RunResourceDispatchDateTime : clientDate;
                        run.FlagGenOtcCodeAlready = false;
                        run.UserModifed = username;
                        run.DatetimeModified = clientDate;
                        run.UniversalDatetimeModified = DateTime.UtcNow;
                        #endregion

                        context.SaveChanges();
                        trans.Commit();
                        return 0;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

        }

        #region Truck to truck transfer
        public IEnumerable<TblMasterActualJobHeader> FindByLegGuidList(IEnumerable<Guid> legGuidList)
        {
            var legs = DbContext.TblMasterActualJobServiceStopLegs
                       .Join(legGuidList,
                       sl => sl.Guid,
                       l => l,
                       (sl, l) => sl);
            var jobs = legs.Select(x => x.MasterActualJobHeader_Guid);
            var jobList = FindByListJob(jobs);
            return jobList;
        }

        public void TruckToTruckTransfer(IEnumerable<TblMasterActualJobHeader> jobList, List<Guid> legGuidList, Guid oldDailyRunGuid, Guid newDailyRunGuid, string userCreated, DateTime datetimeTransfer, string receiverSignature, Guid sender_Guid, string receiverName, DateTime datetimeCreated, DateTimeOffset universalDatetimeCreated)
        {
            using (var db = new OceanDbEntities())
            {

                #region Get legs data
                var allRun = (from routeGroupDetail in db.TblMasterRouteGroup_Detail
                              join routeGroup in db.TblMasterRouteGroup on routeGroupDetail.MasterRouteGroup_Guid equals routeGroup.Guid
                              join dailyRun in db.TblMasterDailyRunResource on routeGroupDetail.Guid equals dailyRun.MasterRouteGroup_Detail_Guid
                              join masterRun in db.TblMasterRunResource on dailyRun.MasterRunResource_Guid equals masterRun.Guid
                              where dailyRun.Guid == oldDailyRunGuid || dailyRun.Guid == newDailyRunGuid
                              select new
                              {
                                  RouteGroup = routeGroup.MasterRouteGroupName,
                                  RouteGroupDetail_Guid = routeGroupDetail.Guid,
                                  RouteGroupDetail = routeGroupDetail.MasterRouteGroupDetailName,
                                  RunGuid = dailyRun.Guid,
                                  RunNo = masterRun.VehicleNumber
                              }).GroupBy(e => new { e.RunGuid, e.RouteGroup, e.RouteGroupDetail, e.RunNo, e.RouteGroupDetail_Guid });

                var oldRun = allRun.Where(e => e.Key.RunGuid == oldDailyRunGuid).Select(
                    o => new
                    {
                        RouteGroup = o.Key.RouteGroup,
                        RouteGroupDetail = o.Key.RouteGroupDetail,
                        RunGuid = o.Key.RunGuid,
                        RunNo = o.Key.RunNo
                    }).FirstOrDefault();

                var newRun = allRun.Where(e => e.Key.RunGuid == newDailyRunGuid).Select(
                    o => new
                    {
                        RouteGroup = o.Key.RouteGroup,
                        RouteGroupDetail = o.Key.RouteGroupDetail,
                        RouteGroupDetail_Guid = o.Key.RouteGroupDetail_Guid,
                        RunGuid = o.Key.RunGuid,
                        RunNo = o.Key.RunNo
                    }).FirstOrDefault();

                var JobGuidlist = jobList.Select(e => e.Guid);
                var alllegs = (from leg in db.TblMasterActualJobServiceStopLegs
                               join job in db.TblMasterActualJobHeader.Where(e => JobGuidlist.Any(x => x == e.Guid)) on leg.MasterActualJobHeader_Guid equals job.Guid
                               join jobType in db.TblSystemServiceJobType on job.SystemServiceJobType_Guid equals jobType.Guid
                               select new { job, leg, jobType });

                var allowedServiceType = new int?[] { IntTypeJob.P, IntTypeJob.D, IntTypeJob.T, IntTypeJob.BCP };

                var legPickup = alllegs.Where(e => allowedServiceType.Contains(e.jobType.ServiceJobTypeID) ||
                                                                     (e.jobType.ServiceJobTypeID == IntTypeJob.TV && (e.leg.SequenceStop == 1 || e.leg.SequenceStop == 2)) ||
                                                                     (e.jobType.ServiceJobTypeID == IntTypeJob.BCD && !e.job.FlagJobInterBranch)).Select(x => x.leg);

                var legDelivery = alllegs.Where(e => (e.jobType.ServiceJobTypeID == IntTypeJob.TV && (e.leg.SequenceStop == 3 || e.leg.SequenceStop == 4)) ||
                                              (e.jobType.ServiceJobTypeID == IntTypeJob.BCD && e.job.FlagJobInterBranch)).Select(x => x.leg);

                var includeP = legPickup.GroupBy(o => o.MasterActualJobHeader_Guid).Where(o => o.Any(l => legGuidList.Contains(l.Guid))).Select(o => o.Key);
                var includeD = legDelivery.GroupBy(o => o.MasterActualJobHeader_Guid).Where(o => o.Any(l => legGuidList.Contains(l.Guid))).Select(o => o.Key);

                #endregion

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Update ServiceStopLeg

                        //For leg 1,2
                        if (legPickup.Any())
                        {
                            foreach (var leg in legPickup.Where(o => includeP.Any(j => j == o.MasterActualJobHeader_Guid)))
                            {
                                leg.MasterRouteGroupDetail_Guid = newRun.RouteGroupDetail_Guid;
                                leg.MasterRunResourceDaily_Guid = newDailyRunGuid;
                            }
                        }
                        //For leg 3,4
                        if (legDelivery.Any())
                        {
                            foreach (var leg in legDelivery.Where(o => includeD.Any(j => j == o.MasterActualJobHeader_Guid)))
                            {
                                leg.MasterRouteGroupDetail_Guid = newRun.RouteGroupDetail_Guid;
                                leg.MasterRunResourceDaily_Guid = newDailyRunGuid;
                            }
                        }

                        #endregion

                        #region Update Status Job
                        var historyJobs = new ConcurrentBag<TblMasterHistory_ActualJob>();
                        var tblJobHeader = db.TblMasterActualJobHeader.Where(e => JobGuidlist.Any(x => x == e.Guid));
                        var toOntruckDelivery = new int[] { JobStatusHelper.OnTruckDelivery, JobStatusHelper.InPreVaultPickUp, JobStatusHelper.InPreVaultDelivery };
                        var toOntruckPickup = new int[] { JobStatusHelper.OnTruckPickUp };

                        foreach (var job in tblJobHeader)
                        {
                            var statusJob = JobStatusHelper.OnTruck;
                            if (toOntruckDelivery.Any(x => x == job.SystemStatusJobID))
                            {
                                statusJob = JobStatusHelper.OnTruckDelivery;
                            }

                            if (toOntruckPickup.Any(x => x == job.SystemStatusJobID))
                            {
                                statusJob = JobStatusHelper.OnTruckPickUp;
                            }

                            job.SystemStatusJobID = statusJob;
                            job.FlagJobChage = true;

                            #region Insert log job history
                            historyJobs.Add(new TblMasterHistory_ActualJob
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = job.Guid,
                                MsgID = 996,
                                MsgParameter = new string[] { job.JobNo, oldRun.RouteGroup, oldRun.RouteGroupDetail, oldRun.RunNo }.ToJSONString(),
                                UserCreated = userCreated,
                                DatetimeCreated = datetimeCreated,
                                UniversalDatetimeCreated = universalDatetimeCreated
                            });
                        }
                        db.TblMasterHistory_ActualJob.AddRange(historyJobs);
                        #endregion
                        #endregion

                        #region Update Job Order

                        #endregion

                        #region Update Run New
                        var tblDailyRun = db.TblMasterDailyRunResource.FirstOrDefault(e => e.Guid == newDailyRunGuid);
                        if (!tblDailyRun.FlagBreakMasterRoute && !tblDailyRun.FirstMasterRoute_Guid.IsNullOrEmpty())
                        {
                            tblDailyRun.FlagBreakMasterRoute = true;
                        }
                        #endregion

                        #region Update Run Old
                        var tblDailyRunOld = db.TblMasterDailyRunResource.FirstOrDefault(e => e.Guid == oldDailyRunGuid);
                        if (!tblDailyRunOld.FlagBreakMasterRoute && !tblDailyRunOld.FirstMasterRoute_Guid.IsNullOrEmpty())
                        {
                            tblDailyRunOld.FlagBreakMasterRoute = true;
                        }
                        #endregion

                        #region Insert log run history

                        var historyRuns = new ConcurrentBag<TblMasterHistory_DailyRunResource>();

                        #region Insert log for old run
                        historyRuns.Add(new TblMasterHistory_DailyRunResource
                        {
                            Guid = Guid.NewGuid(),
                            MasterDailyRunResource_Guid = tblDailyRun.Guid,
                            MsgID = 994,
                            MsgParameter = new string[] { oldRun.RouteGroup, oldRun.RouteGroupDetail, oldRun.RunNo }.ToJSONString(),
                            UserCreated = userCreated,
                            DatetimeCreated = datetimeCreated,
                            UniversalDatetimeCreated = universalDatetimeCreated
                        });

                        #endregion

                        #region Insert log for new run
                        historyRuns.Add(new TblMasterHistory_DailyRunResource
                        {
                            Guid = Guid.NewGuid(),
                            MasterDailyRunResource_Guid = tblDailyRun.Guid,
                            MsgID = 995,
                            MsgParameter = new string[] { newRun.RouteGroup, newRun.RouteGroupDetail, newRun.RunNo }.ToJSONString(),
                            UserCreated = userCreated,
                            DatetimeCreated = datetimeCreated,
                            UniversalDatetimeCreated = universalDatetimeCreated
                        });
                        #endregion

                        db.TblMasterHistory_DailyRunResource.AddRange(historyRuns);

                        #endregion

                        #region Insert log DolphinAssignToAnotherRun
                        var historyDolphin = new ConcurrentBag<TblMasterHistory_DolphinAssignToAnotherRun>();

                        foreach (var itemJobGuidlist in JobGuidlist)
                        {
                            historyDolphin.Add(new TblMasterHistory_DolphinAssignToAnotherRun()
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = itemJobGuidlist,
                                MasterRunResourceDaily_Guid = oldRun.RunGuid,
                                FlagDolphinRemove = false
                            });
                        }

                        db.TblMasterHistory_DolphinAssignToAnotherRun.AddRange(historyDolphin);
                        #endregion

                        #region Insert signature history
                        if (!receiverSignature.IsEmpty())
                        {
                            byte[] bytes = System.Convert.FromBase64String(receiverSignature);
                            var historySignature = new TblMasterHistory_DailyRunResource_SignatureTruckToTruckTransfer()
                            {
                                Guid = Guid.NewGuid(),
                                OldDailyRun_Guid = oldDailyRunGuid,
                                NewDailyRun_Guid = newDailyRunGuid,
                                Sender_Guid = sender_Guid,
                                ReceiverName = receiverName,
                                ReceiverSignature = bytes,
                                DatetimeTransfer = datetimeTransfer,
                                UserCreated = userCreated,
                                DatetimeCreated = datetimeCreated,
                                UniversalDatetimeCreated = universalDatetimeCreated
                            };
                            db.TblMasterHistory_DailyRunResource_SignatureTruckToTruckTransfer.Add(historySignature);
                        }
                        #endregion

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception)
                    {

                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        private IEnumerable<ItemsLibilityView> Summary_Liabilities(IEnumerable<Guid?> actualJobGuids, IEnumerable<Guid?> tempJobGuids)
        {
            var actualLia = DbContext.TblMasterActualJobItemsLiability.Where(o => actualJobGuids.Any(a => a == o.MasterActualJobHeader_Guid)).ToList();
            var tempLia = DbContext.TblBankCleanOutJobDelivery_Temp.Where(o => tempJobGuids.Any(a => a == o.Guid)).ToList();
            var temSeal = DbContext.TblBankCleanOutJobDelivery_SealTemp.Where(o => tempJobGuids.Any(a => a == o.BankCleanOutJobDelivery_Temp_Guid)).ToList();

            var Raw_actualLia = actualLia.Select(o =>
           {
               return new ItemsLibilityView
               {
                   CommodityGuid = o.MasterCommodity_Guid,
                   DocCurrencyGuid = o.MasterCurrency_Guid,
                   JobGuid = o.MasterActualJobHeader_Guid,
                   Liability = o.Liability ?? 0,
                   LibilityGuid = null,
                   DailyRunGuid = null
               };
           });
            var Raw_tempLia = tempLia.Select(o =>
            {
                var currencyGuid = temSeal.FirstOrDefault(s => s.BankCleanOutJobDelivery_Temp_Guid == o.Guid)?.MasterCurrency_Guid;
                return new ItemsLibilityView
                {
                    CommodityGuid = o.MasterCommodity_Guid,
                    DocCurrencyGuid = currencyGuid,
                    JobGuid = o.Guid,
                    Liability = o.STC ?? 0,
                    LibilityGuid = null,
                    DailyRunGuid = null
                };
            });


            return Raw_tempLia.Union(Raw_actualLia);
        }

        private IEnumerable<ItemsCommodityView> Summary_Commodies(IEnumerable<Guid?> actualJobGuids, IEnumerable<Guid?> tempJobGuids)
        {
            //Init Data
            var commGrp = DbContext.TblMasterCommodityGroup.Where(x => x.CommodityGroupName == "CX").FirstOrDefault().Guid;
            var allCommodities = DbContext.TblMasterCommodity.Where(x => x.CommodityCode == "CX" && x.MasterCommodityGroup_Guid == commGrp && !x.FlagDisable).ToList();
            var actualNon = DbContext.TblMasterActualJobItemsCommodity.Where(o => actualJobGuids.Any(a => a == o.MasterActualJobHeader_Guid)).ToList();
            var tempNon = DbContext.TblBankCleanOutJobDelivery_NonbarCodeTemp.Where(o => tempJobGuids.Any(a => a == o.BankCleanOutJobDelivery_Temp_Guid)).ToList();

            Func<int?, string, Guid?, TblMasterActualJobItemsCommodity> getComm = (nonValue, nonNAME, jobGuid) =>
            {
                return new TblMasterActualJobItemsCommodity
                {
                    Guid = Guid.NewGuid(),
                    Quantity = nonValue,
                    QuantityActual = nonValue,
                    QuantityExpected = nonValue,
                    MasterActualJobHeader_Guid = jobGuid,
                    MasterCommodity_Guid = allCommodities.FirstOrDefault(o => o.CommodityName.EndsWith(nonNAME))?.Guid
                };
            };
            var entity_Comms = tempNon.SelectMany(c =>
            {
                var comm = new List<TblMasterActualJobItemsCommodity>();
                if (c.PN != 0)
                    comm.Add(getComm(c.PN, nameof(c.PN), c.BankCleanOutJobDelivery_Temp_Guid));
                if (c.NK != 0)
                    comm.Add(getComm(c.NK, nameof(c.NK), c.BankCleanOutJobDelivery_Temp_Guid));
                if (c.DM != 0)
                    comm.Add(getComm(c.DM, nameof(c.DM), c.BankCleanOutJobDelivery_Temp_Guid));
                if (c.QT != 0)
                    comm.Add(getComm(c.QT, nameof(c.QT), c.BankCleanOutJobDelivery_Temp_Guid));
                if (c.TN != 0)
                    comm.Add(getComm(c.TN, nameof(c.TN), c.BankCleanOutJobDelivery_Temp_Guid));
                if (c.LN != 0)
                    comm.Add(getComm(c.LN, nameof(c.LN), c.BankCleanOutJobDelivery_Temp_Guid));
                return comm;
            }).ToList();

            var Raw_actualNon = actualNon.Select(o => new ItemsCommodityView
            {
                ActualCommodityGuid = o.Guid,
                CommodityGuid = o.MasterCommodity_Guid,
                FlagCommodityDiscrepancies = o.FlagCommodityDiscrepancies ?? false,
                Quantity = o.Quantity ?? 0,
                QuantityActual = o.QuantityActual ?? 0,
                QuantityExpected = o.QuantityExpected ?? 0,
                JobGuid = o.MasterActualJobHeader_Guid
            });
            var Raw_tempNon = entity_Comms.Select(o => new ItemsCommodityView
            {
                ActualCommodityGuid = o.Guid,
                CommodityGuid = o.MasterCommodity_Guid,
                FlagCommodityDiscrepancies = o.FlagCommodityDiscrepancies ?? false,
                Quantity = o.Quantity ?? 0,
                QuantityActual = o.QuantityActual ?? 0,
                QuantityExpected = o.QuantityExpected ?? 0,
                JobGuid = o.MasterActualJobHeader_Guid
            });

            return Raw_tempNon.Union(Raw_actualNon);
        }
        public IEnumerable<RawJobDataView> GetJobBCODetail(IEnumerable<BankCleanOutJobView> listData)
        {

            var userGuid = ApiSession.UserGuid;
            var userCurrencyGuid = DbContext.TblMasterUser.FirstOrDefault(o => o.Guid == userGuid)?.MasterCurrency_Default_Guid;
            var currencyAbb = DbContext.TblMasterCurrency.FirstOrDefault(o => o.Guid == userCurrencyGuid)?.MasterCurrencyAbbreviation;

            var actualJobGuids = listData.Where(o => !o.FlagTemp).Select(o => o.JobGuid);
            var tempJobGuids = listData.Where(o => o.FlagTemp).Select(o => o.JobGuid);

            var allJobGuid = actualJobGuids.Union(tempJobGuids);
            var raw_lia = Summary_Liabilities(actualJobGuids, tempJobGuids);
            var raw_comm = Summary_Commodies(actualJobGuids, tempJobGuids);

            var rawJobs = allJobGuid.Select(o =>
            {
                return new RawJobDataView
                {
                    Target_CurrencyGuid = userCurrencyGuid,
                    Target_CurrencyAbb = currencyAbb,
                    JobItems = new RawItemsView
                    {
                        Liabilities = raw_lia.Where(l => l.JobGuid == o),
                        Commodities = raw_comm.Where(c => c.JobGuid == o)
                    }
                };
            });

            return rawJobs;
        }

        #region Get STC by daily run
        public IEnumerable<JobWithStcView> GetSTCOnHandByJobList(IEnumerable<Guid> jobList, Guid siteGuid, Guid? dailyRunGuid,
            List<TblMasterCurrency_ExchangeRate> currencyExchange, Guid userGuid,
            bool calExchangeRate = false, bool isOnHandSummary = false)
        {
            var countryGuid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == siteGuid)?.MasterCountry_Guid;
            var flagValidateRunLiabilityLimit = DbContext.Up_OceanOnlineMVC_CountryOption_Get(EnumAppKey.FlagValidateRunLiabilityLimit, siteGuid, countryGuid)
                                   .FirstOrDefault().AppValue1.ToLower() == "true";

            if (!flagValidateRunLiabilityLimit)
            {
                calExchangeRate = flagValidateRunLiabilityLimit;
            }
            var masterRunGuid = DbContext.TblMasterDailyRunResource.FirstOrDefault(o => o.Guid == dailyRunGuid).MasterRunResource_Guid;
            var currencyRunGuid = DbContext.TblMasterRunResource.FirstOrDefault(o => o.Guid == masterRunGuid)?.LiabilityLimitCurrency_Guid;
            var currencyUserGuid = DbContext.TblMasterUser.FirstOrDefault(e => e.Guid == userGuid).MasterCurrency_Default_Guid.GetValueOrDefault();

            var defaultCurrency = currencyRunGuid ?? currencyUserGuid;

            var runCurrencyExchage = currencyExchange.Where(e => e.MasterCurrencySource_Guid == currencyRunGuid);
            var Template = DbContext.GetAllCommodityBySite(siteGuid, countryGuid).ToList();
            var onHandItem = GetItemOnHand(jobList, dailyRunGuid);

            string strComm = string.Empty;
            int qtyComm = 0;
            List<CurrencyValueView> currencyValue = new List<CurrencyValueView>();
            List<CommodityValueView> commValue = new List<CommodityValueView>();

            var currencyGuid = onHandItem.Liabilities.Select(e => e.DocCurrencyGuid);
            var masterCurrency = DbContext.TblMasterCurrency.Where(e => currencyGuid.Contains(e.Guid)).ToList();
            var exchangeUserList = DbContext.TblMasterCurrency_ExchangeRate.Where(e => e.MasterCurrencySource_Guid == currencyUserGuid).ToList();
            var result = jobList.Select(o =>
            {
                var commodity = onHandItem.Commodities.Where(c => c.JobGuid == o);
                var liability = onHandItem.Liabilities.Where(l => l.JobGuid == o);

                if (isOnHandSummary)
                {
                    commValue = Template
                                    .Join(commodity, t => t.MasterCommodity_Guid, c => c.CommodityGuid, (t, c) =>
                                      new CommodityValueView { CommodityName = t.CommodityName, CommodityQty = c.Quantity, GoldenRuleNo = t.ColumnInReport.ToInt() })
                                    .OrderBy(r => r.GoldenRuleNo != 0 ? 0 : 1).ThenBy(x => x.GoldenRuleNo).ToList();
                    var comLiaValue = Template
                                    .Join(liability, t => t.MasterCommodity_Guid, c => c.CommodityGuid, (t, c) => t.CommodityName).ToList();
                    strComm = string.Join(",", commValue.Select(comm => comm.CommodityName).Union(comLiaValue));
                    qtyComm = commodity.Sum(q => q.Quantity);

                    currencyValue = liability.Join(masterCurrency,
                                      l => l.DocCurrencyGuid,
                                      c => c.Guid,
                                      (l, c) =>
                                      {
                                          bool isNotConvert = !calExchangeRate || (currencyUserGuid == c.Guid);
                                          var rate = isNotConvert ? 1 : (exchangeUserList.FirstOrDefault(e => e.MasterCurrencyTarget_Guid == c.Guid)?.ExchangeRate ?? 0);
                                          var exchangeVal = l.Liability * Convert.ToDouble(rate);
                                          return new CurrencyValueView
                                          {
                                              CurrencyName = c.MasterCurrencyAbbreviation,
                                              LiabilityValue = l.Liability,
                                              UserLiabilityValue = exchangeVal,
                                              FlagExConvert = rate != 0
                                          };
                                      }).ToList();
                }

                var stcDetail = (new RawJobDataView()
                {
                    Target_CurrencyGuid = defaultCurrency,
                    JobItems = new RawItemsView()
                    {
                        Commodities = commodity,
                        Liabilities = liability
                    }
                }).CalculateJobSTC(runCurrencyExchage, Template, calExchangeRate);
                var summaryExchangeLia = stcDetail.SummaryLiability.ToList();
                var currencyGuid_NotExchange = summaryExchangeLia.Where(e => !e.IsConvert).Select(e => e.SourceCurrencyGuid);
                List<string> currencyNotConvert = masterCurrency.Where(e => currencyGuid_NotExchange.Contains(e.Guid)).Select(e => e.MasterCurrencyAbbreviation).ToList();
                return new JobWithStcView()
                {
                    JobGuid = o,
                    STC = stcDetail.TotalJobSTC,
                    strCommodity = strComm,
                    qtyCommodity = qtyComm,
                    CommodityList = commValue,
                    CurrencyList = currencyValue,
                    FlagExConvertToRun = !summaryExchangeLia.Any(e => !e.IsConvert),
                    CurrencyNotConvert = currencyNotConvert
                };
            }).ToList();

            return result;
        }

        private RawItemsView GetItemOnHand(IEnumerable<Guid> jobList, Guid? dailyRunGuid = null)
        {
            List<ItemsLibilityView> itemLiability = GetItemLiability(jobList, dailyRunGuid);
            List<ItemsCommodityView> itemCommodity = GetItemCommodity(jobList, dailyRunGuid);

            var jobReturnPartial = SeparateJobReturnPrevault(jobList).JobPartialList;

            var itemLiaPartial = itemLiability.Where(x => jobReturnPartial.Contains(x.JobGuid.GetValueOrDefault())
                                 && x.FlagPartial).ToList();
            var itemCommPartial = itemCommodity.Where(x => jobReturnPartial.Contains(x.JobGuid.GetValueOrDefault())
                                 && x.FlagPartial).ToList();

            var itemLiaWithOutPartial = itemLiability.Where(x => !jobReturnPartial.Contains(x.JobGuid.GetValueOrDefault())).ToList();
            var itemCommWithOutPartial = itemCommodity.Where(x => !jobReturnPartial.Contains(x.JobGuid.GetValueOrDefault())).ToList();

            return new RawItemsView()
            {
                Liabilities = itemLiaPartial.Union(itemLiaWithOutPartial),
                Commodities = itemCommPartial.Union(itemCommWithOutPartial)
            };
        }
        private List<ItemsCommodityView> GetItemCommodity(IEnumerable<Guid> jobList, Guid? dailyRunGuid = null)
        {
            return DbContext.TblMasterActualJobItemsCommodity
                                            .Join(jobList,
                                            c => c.MasterActualJobHeader_Guid,
                                            j => j,
                                            (c, j) => new ItemsCommodityView
                                            {
                                                DailyRunGuid = dailyRunGuid,
                                                ActualCommodityGuid = c.Guid,
                                                CommodityGuid = c.MasterCommodity_Guid,
                                                FlagCommodityDiscrepancies = c.FlagCommodityDiscrepancies ?? false,
                                                Quantity = c.Quantity ?? 0,
                                                QuantityActual = c.QuantityActual ?? 0,
                                                QuantityExpected = c.QuantityExpected ?? 0,
                                                JobGuid = c.MasterActualJobHeader_Guid,
                                                FlagPartial = c.FlagPartial ?? false
                                            }).ToList();
        }
        private List<ItemsLibilityView> GetItemLiability(IEnumerable<Guid> jobList, Guid? dailyRunGuid = null)
        {
            return DbContext.TblMasterActualJobItemsLiability
                                            .Join(jobList,
                                            l => l.MasterActualJobHeader_Guid,
                                            j => j,
                                            (l, j) => new ItemsLibilityView
                                            {
                                                DailyRunGuid = dailyRunGuid,
                                                DocCurrencyGuid = l.MasterCurrency_Guid,
                                                Liability = l.Liability ?? 0,
                                                LibilityGuid = l.Guid,
                                                JobGuid = l.MasterActualJobHeader_Guid,
                                                FlagPartial = l.FlagPartial ?? false,
                                                CommodityGuid = l.MasterCommodity_Guid
                                            }).ToList();
        }
        public JobListReturnPreVaultView SeparateJobReturnPrevault(IEnumerable<Guid> jobGuidList)
        {
            JobListReturnPreVaultView result = new JobListReturnPreVaultView();
            var jobReturnList = FindByListJob(jobGuidList).Where(e => e.SystemStatusJobID == IntStatusJob.ReturnToPreVault);
            result.JobNonDeliveryList = jobReturnList.Where(o => o.FlagNonDelivery.GetValueOrDefault()).Select(x => x.Guid).ToList();
            result.JobPartialList = jobReturnList.Where(o => !o.FlagNonDelivery.GetValueOrDefault()).Select(x => x.Guid).ToList();
            return result;
        }

        #endregion

        public IEnumerable<VaultBalanceJobDetailModel> GetJobDetailVaultBalance(IEnumerable<Guid> jobGuidList)
        {
            List<int> jobsStatus = new List<int> { IntStatusJob.InPreVault};
            List<int> jobsStatusTV = new List<int> { IntStatusJob.InPreVaultPickUp, IntStatusJob.InPreVaultDelivery, IntStatusJob.PartialInDepartment, IntStatusJob.WaitingforDeconsolidate };
            jobsStatus.AddRange(jobsStatusTV);

            List<int> jobsTypeAll = new List<int>();
            List<int> jobsTypeP_D = new List<int> { IntTypeJob.P, IntTypeJob.D, IntTypeJob.AC, IntTypeJob.AE, IntTypeJob.BCP, IntTypeJob.BCD, IntTypeJob.P_MultiBr, IntTypeJob.BCD_MultiBr };
            jobsTypeAll.AddRange(jobsTypeP_D);

            List<int> jobsTypeTV = new List<int> { IntTypeJob.TV, IntTypeJob.TV_MultiBr };
            jobsTypeAll.AddRange(jobsTypeTV);

            var jobType = DbContext.TblSystemServiceJobType.Where(o => o.ServiceJobTypeID != null && jobsTypeAll.Contains((int)o.ServiceJobTypeID)).ToList();
            var jobsTypeP_D_Guid = jobType.Where(o => o.ServiceJobTypeID != null && jobsTypeP_D.Contains((int)o.ServiceJobTypeID)).Select(o => o.Guid).ToList();
            var jobsTypeTV_Guid = jobType.Where(o => o.ServiceJobTypeID != null && jobsTypeTV.Contains((int)o.ServiceJobTypeID)).Select(o => o.Guid).ToList();

            List<TblMasterActualJobHeader> jobsAll = new List<TblMasterActualJobHeader>();
            var tblJobs = DbContext.TblMasterActualJobHeader.Where(e => e.FlagCancelAll != true && e.FlagJobDiscrepancies != true && (jobGuidList.Contains(e.Guid))).ToList();

            var jobsP_D = tblJobs.Where(e => (e.SystemServiceJobType_Guid != null && jobsTypeP_D_Guid.Contains((Guid)e.SystemServiceJobType_Guid))
                                       && (e.SystemStatusJobID != null && jobsStatus.Contains((int)e.SystemStatusJobID))).ToList();
            jobsAll.AddRange(jobsP_D);
            var jobSTV =  tblJobs.Where(e => (e.SystemServiceJobType_Guid != null && jobsTypeTV_Guid.Contains((Guid)e.SystemServiceJobType_Guid))
                                       && (e.SystemStatusJobID != null && jobsStatusTV.Contains((int)e.SystemStatusJobID))).ToList();
            jobsAll.AddRange(jobSTV);

            var legInJob = DbContext.TblMasterActualJobServiceStopLegs.Where(o => o.MasterActualJobHeader_Guid != null && jobGuidList.Contains((Guid)o.MasterActualJobHeader_Guid));
            var legsAll = legInJob
                          .Join(DbContext.TblMasterCustomerLocation,
                          leg => leg.MasterCustomerLocation_Guid,
                          cl => cl.Guid,
                          (leg, cl) => new { leg, cuslocGuid = cl.MasterCustomer_Guid })
                          .Join(DbContext.TblMasterCustomer.Where(c => c.FlagChkCustomer == true),
                          lcl => lcl.cuslocGuid,
                          cus => cus.Guid,
                          (lcl, cus) => lcl.leg);

            var jobs = jobsAll.Join(legsAll,
                j => j.Guid,
                l => l.MasterActualJobHeader_Guid,
                (j, l) => j);

            var legsP = legInJob.Where(o => o.MasterCustomerLocation_Guid != null && o.SequenceStop == 1).ToList();
            var legsD = legInJob.Where(o => o.MasterCustomerLocation_Guid != null && o.FlagDestination).ToList();

            var cusLocGuids = legsP.Select(o => o.MasterCustomerLocation_Guid.GetValueOrDefault())
                              .Union(legsD.Select(o => o.MasterCustomerLocation_Guid.GetValueOrDefault()));
            var location = DbContext.TblMasterCustomerLocation.Where(o => cusLocGuids.Contains(o.Guid)).ToList();
            var cusGuids = location.Select(o => o.MasterCustomer_Guid);
            var customer = DbContext.TblMasterCustomer.Where(o => cusGuids.Contains(o.Guid)).ToList();

            var result = jobs.Select(o =>
            {
                var jobT = jobType.FirstOrDefault(e => e.Guid == o.SystemServiceJobType_Guid);
                var legP = legsP.FirstOrDefault(e => e.MasterActualJobHeader_Guid == o.Guid);
                var locP = location.FirstOrDefault(e => e.Guid == legP.MasterCustomerLocation_Guid);
                var cusP = customer.FirstOrDefault(e => e.Guid == locP.MasterCustomer_Guid)?.CustomerFullName ?? string.Empty;
                var legD = legsD.FirstOrDefault(e => e.MasterActualJobHeader_Guid == o.Guid);
                var locD = location.FirstOrDefault(e => e.Guid == legD.MasterCustomerLocation_Guid);
                var cusD = customer.FirstOrDefault(e => e.Guid == locD.MasterCustomer_Guid)?.CustomerFullName ?? string.Empty;
                return new VaultBalanceJobDetailModel
                {
                    JobGuid = o.Guid,
                    JobNo = o.JobNo,
                    PickUpLocation = !string.IsNullOrEmpty(cusP) ? cusP + " - " + locP.BranchName : string.Empty,
                    DeliveryLocation = !string.IsNullOrEmpty(cusD) ? cusD + " - " + locD.BranchName : string.Empty,
                    ServiceType = jobT.ServiceJobTypeNameAbb,
                    WorkDate = o.TransectionDate.GetValueOrDefault(),
                    CustomerPickupGuid = locP.MasterCustomer_Guid,
                    LocationPickupGuid = locP.Guid,
                    CustomerDeliveryGuid = locD.MasterCustomer_Guid,
                    LocationDeliveryGuid = locD.Guid
                };
            }).ToList();

            return result;
        }
    }
}
