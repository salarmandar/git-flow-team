using Bgt.Ocean.Infrastructure.CompareHelper;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Models.RunControl.LiabilityLimitModel
{

    public static class JobDetailViewExtension
    {
        public static IEnumerable<Guid?> GetDailyRunTargetGuids(this IEnumerable<LiabilityLimitNoJobsAction> model)
        {

            return model.Where(o => o.DailyRunGuid_Target != null).Select(o => o.DailyRunGuid_Target).Distinct();
        }

        public static IEnumerable<Guid?> GetDailyRunTargetGuids(this IEnumerable<LiabilityLimitJobsAction> model)
        {
            return model.Where(o => o.DailyRunGuid_Target != null).Select(o => o.DailyRunGuid_Target).Distinct();
        }
        public static IEnumerable<Guid> GetMergeJobGuids(this IEnumerable<LiabilityLimitJobsAction> model, IEnumerable<Guid?> jobGuids)
        {
            var guids = model.SelectMany(o => o.JobGuids.Select(j => j.JobGuid));
            return (jobGuids.Union(guids)).Where(o => o != null).Select(o => (Guid)o);
        }

        public static IEnumerable<LiabilityLimitJobsAction> ConvertToJobsActionModel(this IEnumerable<LiabilityLimitNoJobsAction> items)
        {
            return items.Select(o => new LiabilityLimitJobsAction
            {
                DailyRunGuid_Target = o.DailyRunGuid_Target,
                JobGuids = o.JobData.JobItems.GetJobGuids().Select(j => new RawExistJobView { JobGuid = j })
            });
        }
        public static IEnumerable<LiabilityLimitJobsAction> ConvertToJobsActionModel(this IEnumerable<LiabilityLimitItemsAction> items)
        {
            return items.Select(o => new LiabilityLimitJobsAction
            {
                DailyRunGuid_Target = o.DailyRunGuid_Target,
                JobGuids = o.JobItems.GetJobGuids().Select(j => new RawExistJobView { JobGuid = j })
            });
        }
        public static IEnumerable<LiabilityLimitJobsAction> ConvertToJobsActionModel(this LiabilityLimitRunsActionModel model)
        {
            return model.DailyRunGuids.Select(o => new LiabilityLimitJobsAction { DailyRunGuid_Target = o });
        }
        public static IEnumerable<JobDetailResult> GetOnlyExceedJobs(this IEnumerable<JobDetailResult> jobs)
        {
            return jobs.Where(o => o.FlagAllowSelect);
        }
        public static TruckLibilityLimitResult ProcessBulkExceedJobs(this IEnumerable<JobDetailResult> allJobs)
        {
            TruckLibilityLimitResult result = new TruckLibilityLimitResult();

            //total STC per run
            var jobsInRun = allJobs.Where(o => o.FlagJobInRun)
                                 .GroupBy(o => new { o.DailyRunGuid, o.Target_CurrencyGuid, o.Target_RunLibilityLimit })
                                 .Select(g =>
                                 {
                                     return new
                                     {
                                         Target_DailyRunGuid = g.Key.DailyRunGuid,
                                         Target_RunLibilityLimit = g.Key.Target_RunLibilityLimit,
                                         Target_CurrencyGuid = g.Key.Target_CurrencyGuid,
                                         Target_DailyRunSTC = g.Sum(o => o.TotalJobSTC)
                                     };
                                 }).ToList();

            //stamp exceed job
            var jobsOutRun = allJobs.Where(o => o.FlagJobOutRun);
            var jobsInput = jobsOutRun.GroupBy(o => new { o.Target_DailyRunGuid, o.Target_CurrencyGuid })
                                  .SelectMany(g =>
                                  {

                                      var target_run = jobsInRun.FirstOrDefault(r => r.Target_DailyRunGuid == g.Key.Target_DailyRunGuid);
                                      var job_target_run = g.FirstOrDefault();
                                      var target_DailyRunSTC = (target_run?.Target_DailyRunSTC) ?? 0;
                                      var target_RunLibilityLimit = ((target_run?.Target_RunLibilityLimit) ?? job_target_run?.Target_RunLibilityLimit) ?? 0;
                                      var job_STC = g.Sum(j => j.TotalJobSTC);
                                      var bulk_STC = job_STC + target_DailyRunSTC;
                                      var bulk_OverLimit = target_RunLibilityLimit != 0 && bulk_STC > target_RunLibilityLimit;
                                      if (bulk_OverLimit && g.Key.Target_DailyRunGuid.HasValue && job_STC > 0)
                                      {
                                          var jobs = g.Select(j =>
                                          {
                                              if (!j.FlagDisplayChkBox)
                                                  j.FlagDisplayChkBox = ((j.DailyRunGuid == null) || (j.DailyRunGuid != null && j.DailyRunGuid == j.Target_DailyRunGuid));

                                              //allow select when cannot covert exchange
                                              j.FlagAllowSelect = (j.TotalJobSTC > 0 && j.FlagDisplayChkBox) || j.FlagNotConvertExchange;
                                              //not set selected when cannot covert exchange
                                              j.FlagSelected = j.TotalJobSTC == 0 && !j.FlagNotConvertExchange;
                                              j.FlagDisplayJob = true;
                                              j.Target_DailyRunCurrentSTC = target_run?.Target_DailyRunSTC;
                                              j.Target_RunLibilityLimit = target_RunLibilityLimit;

                                              return j;
                                          });
                                          return jobs;
                                      }
                                      else
                                      {
                                          var jobs = g.Select(j =>
                                          {
                                              j.FlagDisplayJob = false;
                                              j.Target_DailyRunCurrentSTC = target_run?.Target_DailyRunSTC;
                                              j.Target_RunLibilityLimit = target_RunLibilityLimit;
                                              return j;
                                          });
                                          return jobs;
                                      }

                                  }).OrderBy(o => o.FlagDisplayChkBox && !o.FlagSelected ? 0 : o.FlagDisplayChkBox && o.FlagSelected ? 2 : 1)
                                    .ThenBy(o => o.Location)
                                    .ThenBy(o => o.TotalJobSTC)
                                    .ThenBy(o => o.JobNo).ToList();


            result.SiteDetailList = jobsInput.Where(o => o.FlagDisplayJob)
                                             .GroupBy(o => new { o.Target_DailyRunSiteGuid, o.Target_DailyRunSiteName })
                                             .Select(o => new SiteDetailResult
                                             {
                                                 Target_DailyRunSiteGuid = o.Key.Target_DailyRunSiteGuid,
                                                 Target_DailyRunSiteName = o.Key.Target_DailyRunSiteName
                                             }).OrderBy(o => o.Target_DailyRunSiteName);

            result.RunDetailList = jobsInput.Where(o => o.FlagDisplayJob)
                                            .GroupBy(o => new
                                            {
                                                o.Target_DailyRunGuid,
                                                o.Target_DailyRunSiteGuid,
                                                o.Target_WorkDate,
                                                o.Target_RunNo,
                                                o.Target_DailyRunCurrentSTC,
                                                o.Target_CurrencyGuid,
                                                o.Target_CurrencyAbb,
                                                o.Target_RunLibilityLimit,
                                                o.Target_RouteDetail
                                            })
                                            .Select(o => new RunDetailResult
                                            {
                                                Target_DailyRunGuid = o.Key.Target_DailyRunGuid,
                                                Target_DailyRunSiteGuid = o.Key.Target_DailyRunSiteGuid,
                                                Target_RunNo = o.Key.Target_RunNo,
                                                Target_WorkDate = o.Key.Target_WorkDate,
                                                Target_DailyRunCurrentSTC = o.Key.Target_DailyRunCurrentSTC,
                                                Target_CurrencyAbb = o.Key.Target_CurrencyAbb,
                                                Target_RunLibilityLimit = o.Key.Target_RunLibilityLimit,
                                                Target_RouteDetail = o.Key.Target_RouteDetail
                                            }).OrderBy(o => o.Target_RouteDetail).ThenBy(o => o.Target_RunNo);

            result.JobDetailList = jobsInput;

            return result;
        }

        public static IEnumerable<JobDetailResult> ProcessExceedJobs(this IEnumerable<JobDetailResult> allJobs)
        {

            //total STC per run
            var jobsInRun = allJobs.Where(o => o.FlagJobInRun)
                                  .GroupBy(o => new { o.DailyRunGuid, o.Target_CurrencyGuid, o.Target_RunLibilityLimit })
                                  .Select(g =>
                                  {
                                      return new
                                      {
                                          Target_DailyRunGuid = g.Key.DailyRunGuid,
                                          Target_RunLibilityLimit = g.Key.Target_RunLibilityLimit,
                                          Target_CurrencyGuid = g.Key.Target_CurrencyGuid,
                                          Target_DailyRunSTC = g.Sum(o => o.TotalJobSTC)
                                      };
                                  }).ToList();

            //stamp exceed job
            var jobsOutRun = allJobs.Where(o => o.FlagJobOutRun);
            var jobsInput = jobsOutRun.Where(o => o.TotalJobSTC != 0)
                                  .GroupBy(o => new { o.Target_DailyRunGuid, o.Target_CurrencyGuid })
                                  .SelectMany(g =>
                                  {
                                      var jobs = Enumerable.Empty<JobDetailResult>();
                                      var target_run = jobsInRun.FirstOrDefault(r => r.Target_DailyRunGuid == g.Key.Target_DailyRunGuid);
                                      var job_target_run = g.FirstOrDefault();
                                      var target_DailyRunSTC = target_run?.Target_DailyRunSTC ?? 0;
                                      var target_RunLibilityLimit = (target_run?.Target_RunLibilityLimit ?? job_target_run?.Target_RunLibilityLimit) ?? 0;
                                      var bulk_STC = g.Sum(j => j.TotalJobSTC) + target_DailyRunSTC;
                                      var bulk_OverLimit = target_RunLibilityLimit != 0 && bulk_STC > target_RunLibilityLimit;
                                      if (bulk_OverLimit)
                                      {
                                          var totalRunSTC = target_DailyRunSTC;
                                          jobs = g.Select(j =>
                                          {
                                              totalRunSTC += j.TotalJobSTC;
                                              j.FlagAllowSelect = totalRunSTC > target_RunLibilityLimit;
                                              j.Target_RunLibilityLimit = target_RunLibilityLimit;
                                              if (j.FlagAllowSelect)
                                              {
                                                  totalRunSTC -= j.TotalJobSTC;
                                              }
                                              return j;
                                          });
                                      }
                                      return jobs;
                                  }).ToList();

            return jobsInput.OrderBy(o => o.Location)
                            .ThenBy(o => o.TotalJobSTC)
                            .ThenBy(o => o.FlagNotConvertExchange ? 1 : 0)
                            .ThenBy(o => o.FlagDisplayChkBox ? 0 : 1)
                            .ThenBy(o => o.JobNo);
        }

        public static IEnumerable<RawJobDataView> MergeJobsToExistsJobs(this IEnumerable<RawJobDataView> inputJobs, IEnumerable<RawJobDataView> baseJobs)
        {
            var inputJobsMapped = inputJobs.Select(o =>
            {
                //override base job
                var existsJob = baseJobs.FirstOrDefault(j => o.JobItems.GetJobGuids().Any(i => i == j.JobGuid));
                existsJob = existsJob ?? o;
                var overideItems = existsJob.Clone();
                overideItems.DailyRunGuid = null;
                overideItems.FlagJobInRun = false;
                overideItems.FlagJobOutRun = true;
                overideItems.JobItems = o.JobItems;
                return overideItems;
            }).ToList();

            baseJobs = baseJobs.Union(inputJobsMapped);
            return baseJobs;
        }

        public static IEnumerable<RawJobDataView> MergeItemsToExistsJobs(this IEnumerable<LiabilityLimitItemsAction> inputJobs, IEnumerable<RawJobDataView> baseJobs)
        {
            var allInputJobGuids = inputJobs.SelectMany(o => o.JobItems.GetJobGuids()).Distinct();
            var inputJobsMapped = allInputJobGuids.Select(o =>
             {
                 var inputLia = inputJobs.SelectMany(i => i.JobItems.Liabilities.Where(l => l.JobGuid == o));
                 var inputComm = inputJobs.SelectMany(i => i.JobItems.Commodities.Where(l => l.JobGuid == o));
                 //override base job
                 var existsJob = baseJobs.FirstOrDefault(j => o == j.JobGuid);
                 existsJob = existsJob ?? new RawJobDataView();
                 var overideItems = existsJob.Clone();
                 overideItems.DailyRunGuid = null;
                 overideItems.FlagJobInRun = false;
                 overideItems.FlagJobOutRun = true;
                 overideItems.JobItems = new RawItemsView { Liabilities = inputLia, Commodities = inputComm };
                 return overideItems;
             }).ToList();

            var finalBaseJobs = baseJobs.Select(o =>
            {
                var updateJobs = inputJobsMapped.FirstOrDefault(j => j.JobGuid == o.JobGuid);
                o.JobItems = updateJobs == null ? o.JobItems : o.JobItems.ExcludeUpdateBaseJobItems(updateJobs.JobItems);
                return o;
            });

            baseJobs = finalBaseJobs.Union(inputJobsMapped);
            return baseJobs;
        }

        private static RawItemsView ExcludeUpdateBaseJobItems(this RawItemsView baseItems, RawItemsView inputItems)
        {
            var updateLia = inputItems.Liabilities.Where(o => o.ItemState == EnumState.Modified || o.ItemState == EnumState.Deleted).ToList();
            var updateComm = inputItems.Commodities.Where(o => o.ItemState == EnumState.Modified || o.ItemState == EnumState.Deleted).ToList();

            var baseUpdateLia = baseItems.Liabilities.Where(o => !updateLia.Any(l => l.LibilityGuid == o.LibilityGuid)).ToList();
            var baseUpdateComm = baseItems.Commodities.Where(o => !updateComm.Any(l => l.ActualCommodityGuid == o.ActualCommodityGuid)).ToList();
            var addLia = inputItems.Liabilities.Where(o => o.ItemState != EnumState.Deleted).ToList();
            var addComm = inputItems.Commodities.Where(o => o.ItemState != EnumState.Deleted).ToList();

            baseItems.Liabilities = baseUpdateLia;
            baseItems.Commodities = baseUpdateComm;
            inputItems.Liabilities = addLia;
            inputItems.Commodities = addComm;
            return baseItems;
        }

        public static IEnumerable<Guid?> GetJobGuids(this RawItemsView items)
        {
            var jobInLia = items.Liabilities.Where(o => o.JobGuid != null).Select(o => o.JobGuid);
            var jobInComm = items.Commodities.Where(o => o.JobGuid != null).Select(o => o.JobGuid);
            return jobInLia.Union(jobInComm);
        }


        public static JobDetailResult CalculateJobSTC(this RawJobDataView o, IEnumerable<TblMasterCurrency_ExchangeRate> currentExchange, IEnumerable<CommodityView> template, bool flagCovertExchangeRate = true)
        {
            //Calcucate STC
            var _totalLia = TotalLiabilities_STC(o, currentExchange, flagCovertExchangeRate);
            var _totalComm = TotalCommodities_STC(o, template);
            o.TotalLiabilities_STC = _totalLia.Sum(l => l.ItemSTC);
            o.TotalCommodities_STC = _totalComm.Sum(c => c.ItemSTC);
            o.TotalJobSTC = o.TotalLiabilities_STC + o.TotalCommodities_STC;
            o.TotalJobSTC = Math.Round(o.TotalJobSTC, 2, MidpointRounding.AwayFromZero);
            o.SummaryLiability = _totalLia;
            return o.Clone();
        }

        private static double parseExRate(Guid? sourceCurrencyGuid, Guid? tarGetCurrencyGuid, decimal? exRateValue, bool flagCovertExchangeRate)
        {
            double exRate = 0;

            //no exchange rate
            if (exRateValue == null)
                exRate = 0;

            //diff currency
            if (sourceCurrencyGuid != tarGetCurrencyGuid && exRateValue != null)
                exRate = Convert.ToDouble(exRateValue);

            //same currcency or not convert currency
            if (sourceCurrencyGuid == tarGetCurrencyGuid || !flagCovertExchangeRate)
                exRate = 1;


            return exRate;
        }
        private static IEnumerable<ItemSummaryView> TotalLiabilities_STC(RawJobDataView job, IEnumerable<TblMasterCurrency_ExchangeRate> exchangeRate, bool flagCovertExchangeRate)
        {
            // check all doc currency in exchange table
            var FlagCanCovertWholeDoc = job.JobItems.Liabilities.Where(l => l.DocCurrencyGuid.HasValue
                                         && job.Target_CurrencyGuid.HasValue
                                         && l.DocCurrencyGuid != Guid.Empty
                                         && job.Target_CurrencyGuid != Guid.Empty
                                         && l.DocCurrencyGuid != job.Target_CurrencyGuid).All(l =>
                                         exchangeRate.Any(e => job.Target_CurrencyGuid == e.MasterCurrencySource_Guid
                                         && l.DocCurrencyGuid == e.MasterCurrencyTarget_Guid));

            job.FlagNotConvertExchange = !FlagCanCovertWholeDoc;

            var summaryLiabilities = job.JobItems.Liabilities.GroupBy(o => new
            {
                o.JobGuid,
                o.Liability,
                job.Target_DailyRunGuid,
                DocCurrencyGuid = o.DocCurrencyGuid,
                job.Target_CurrencyGuid
            }).Select(g =>
            {

                var exRateValue = exchangeRate.FirstOrDefault(e => e.MasterCurrencySource_Guid == g.Key.Target_CurrencyGuid && e.MasterCurrencyTarget_Guid == g.Key.DocCurrencyGuid)?.ExchangeRate;
                double exRate = parseExRate(g.Key.DocCurrencyGuid, g.Key.Target_CurrencyGuid, exRateValue, flagCovertExchangeRate);
                return new ItemSummaryView
                {
                    ItemSTC = g.Sum(j => j.Liability * exRate),
                    MasterActualJobHeader_Guid = g.Key.JobGuid,
                    RunCurrencyGuid = g.Key.Target_CurrencyGuid,
                    DailyRunGuid = g.Key.Target_DailyRunGuid,
                    IsConvert = exRate != 0,
                    SourceCurrencyGuid = g.Key.DocCurrencyGuid
                };
            });

            return summaryLiabilities;
        }
        private static IEnumerable<ItemSummaryView> TotalCommodities_STC(RawJobDataView job, IEnumerable<CommodityView> template)
        {


            var summaryCommodities = (from comm in job.JobItems.Commodities
                                      join t in template on comm.CommodityGuid equals t.MasterCommodity_Guid
                                      select new { comm, temp = t })
              .AsEnumerable()
              .Select(c =>
              {
                  bool IsAllShortage = (c.comm.Quantity) == 0 && (c.comm.FlagCommodityDiscrepancies);
                  bool IsSomeShortage = (c.comm.Quantity) > 0 && (c.comm.FlagCommodityDiscrepancies);
                  c.temp.MasterActualJobHeader_Guid = c.comm.JobGuid;
                  c.temp.MasterActualJobItemsCommodity_Guid = c.comm.CommodityGuid;
                  c.temp.Quantity = c.comm.Quantity;
                  c.temp.calQTY = c.comm.Quantity;
                  c.temp.calSTC = c.temp.calQTY * c.temp.CommodityAmount * c.temp.CommodityValue;
                  c.temp.QuantityExpected = c.comm.QuantityExpected;
                  if (IsAllShortage) c.temp.QuantityExpected = 0;
                  if (IsSomeShortage) c.temp.QuantityExpected = c.comm.QuantityActual;
                  return new { c.temp, c.comm };
              }).GroupBy(o => new
              {
                  job.Target_DailyRunGuid,
                  o.comm.JobGuid,
                  Target_CurrencyGuid = job.Target_CurrencyGuid
              }).Select(o =>
              {
                  return new ItemSummaryView
                  {
                      ItemSTC = o.Sum(c => c.temp.calSTC),
                      MasterActualJobHeader_Guid = o.Key.JobGuid,
                      RunCurrencyGuid = o.Key.Target_CurrencyGuid,
                      DailyRunGuid = o.Key.Target_DailyRunGuid
                  };
              });

            return summaryCommodities;
        }

        public static bool ExcludeJobs(this RawJobDataView job)
        {
            Func<JobDetailResult, bool> AllowJob = (o) =>
            {
                var allowStatus = AllowStatusID.Any(s => s == o.JobStatusID);

                switch (o.JobTypeID)
                {
                    case IntTypeJob.BCD:
                    case IntTypeJob.BCD_MultiBr:
                    case IntTypeJob.D:
                        if (!allowStatus)
                            allowStatus = IncludeDStatus.Any(s => s == o.JobStatusID);
                        break;
                    case IntTypeJob.T:
                        if (o.JobStatusID == IntStatusJob.Open)
                            allowStatus = !o.FlagDestination;
                        else
                            allowStatus = o.FlagDestination;
                        break;
                    case IntTypeJob.TV_MultiBr:
                    case IntTypeJob.TV:
                        break;
                    default:
                        break;
                }

                return allowStatus;
            };
            return AllowJob(job);
        }

        private static int[] IncludeDStatus { get; set; } = new int[] { IntStatusJob.Open, IntStatusJob.InPreVault };
        private static int[] AllowStatusID { get; set; } = new int[]
            {
            IntStatusJob.OnTruck
            , IntStatusJob.OnTheWayPickUp
            , IntStatusJob.OnTruckPickUp
            , IntStatusJob.OnTruckDelivery
            , IntStatusJob.OnTheWayDelivery
            , IntStatusJob.OnTheWay

            , IntStatusJob.MissingStop
            , IntStatusJob.UnableToService
            , IntStatusJob.Unrealized
            , IntStatusJob.VisitWithStamp
            , IntStatusJob.VisitWithOutStamp

            , IntStatusJob.NoArrived
            , IntStatusJob.ReturnToPreVault
            , IntStatusJob.PartialDelivery
            , IntStatusJob.IntransitInterBranch
            , IntStatusJob.NonDelivery
            , IntStatusJob.CancelAndReturnToPreVault
            , IntStatusJob.WaitingforApproveCancel
            , IntStatusJob.WaitingforApproveScheduleTime
            , IntStatusJob.WaitingforDeconsolidate
            , IntStatusJob.WaitingforDolphinReceive
            , IntStatusJob.WaitingPickUp
            , IntStatusJob.PartialDelivery

            , IntStatusJob.DeliverToPreVault
            , IntStatusJob.Process
            , IntStatusJob.ReadyToPreVault
            , IntStatusJob.PickedUp
            , IntStatusJob.InPreVaultPickUp
            , IntStatusJob.InPreVaultDelivery
            , IntStatusJob.Open
                //, IntStatusJob.PickedUp //Except TVP
                //, IntStatusJob.InPreVault //D

            };
    }
}
