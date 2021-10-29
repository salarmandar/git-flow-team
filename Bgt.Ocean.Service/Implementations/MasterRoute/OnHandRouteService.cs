using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using Bgt.Ocean.Service.Messagings.TruckLiabilityLimit;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Service.Implementations.MasterRoute
{
    public interface IOnHandRouteService
    {
        OnHandRouteResponse GetDetailJob(OnHandMasterRouteRequest request);
        OnHandRouteSummaryResponse GetOnHandRouteSummary(OnHandMasterRouteRequest request);
        JobWithSTCResponse GetSTCOnHandSummaryTips(JobWithSTCRequest request);
    }
    public class OnHandRouteService : IOnHandRouteService
    {
        private readonly ISystemService _systemService;
        private readonly IMasterRouteRepository _masterRouteRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterActualJobItemsSealRepository _masterActualJobItemsSealRepository;
        private readonly IMasterActualJobItemsCommodityRepository _masterActualJobItemsCommodityRepository;
        private readonly IMasterCurrencyRepository _masterCurrencyRepository;
        private readonly IMasterUserRepository _masterUserRepository;
        public OnHandRouteService(
            ISystemService systemService,
            IMasterRouteRepository masterRouteRepository,
            ISystemMessageRepository systemMessageRepository,
            IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
            IMasterActualJobItemsSealRepository masterActualJobItemsSealRepository,
            IMasterActualJobItemsCommodityRepository masterActualJobItemsCommodityRepository,
            IMasterCurrencyRepository masterCurrencyRepository,
            IMasterUserRepository masterUserRepository
            )
        {
            _systemService = systemService;
            _masterRouteRepository = masterRouteRepository;
            _systemMessageRepository = systemMessageRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterActualJobItemsSealRepository = masterActualJobItemsSealRepository;
            _masterActualJobItemsCommodityRepository = masterActualJobItemsCommodityRepository;
            _masterCurrencyRepository = masterCurrencyRepository;
            _masterUserRepository = masterUserRepository;
        }

        private OnHandRouteResponse GetDetailJobByDailyRun(Guid siteGuid, List<Guid> dailyRunGuids)
        {
            OnHandRouteResponse response = new OnHandRouteResponse();
            Guid languageGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault();
            try
            {
                if (dailyRunGuids != null && dailyRunGuids.Any())
                {
                    #region Get job detail
                    var jobDetail = _masterRouteRepository.GetJobDetailOnRun(dailyRunGuids, languageGuid).ConvertToOnHandMasterRouteResponse().ToList();
                    jobDetail = SeparateSectionJobs(jobDetail);
                    var jobGuidList = jobDetail.Select(x => x.JobGuid).Distinct().ToList();
                    #endregion

                    #region Get STC
                    var jobWithSTC = GetJobWithSTCList(dailyRunGuids, jobDetail, siteGuid);
                    #endregion

                    #region Count item in job
                    //SEAL
                    var sealInJob = _masterActualJobItemsSealRepository.FindSealOnHandByJob(jobGuidList);
                    var sealNotInCon = sealInJob.Where(x => !x.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue && !x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue);
                    var sealInCon = sealInJob.Where(x => !sealNotInCon.Select(o => o.Guid).Contains(x.Guid));
                    var sealInConRoute = sealInCon.Where(x => x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue);
                    var sealInConLoc = sealInCon.Where(x => !x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue && x.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue);
                    var masterSInJob = sealInCon.Select(s => new MasterIDGroup
                    {
                        JobGuid = s.MasterActualJobHeader_Guid.GetValueOrDefault(),
                        MasterLocID = s.Master_ID,
                        MasterRouteID = s.MasterID_Route
                    });

                    //NON-BARCODE
                    var nonInJob = _masterActualJobItemsCommodityRepository.FindCommodityOnHandByJob(jobGuidList);
                    var nonNotInCon = nonInJob.Where(x => !x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue && !x.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue);
                    var nonInCon = nonInJob.Where(x => !nonNotInCon.Select(o => o.Guid).Contains(x.Guid));
                    var nonInConRoute = nonInCon.Where(x => x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue);
                    var nonInConLoc = nonInCon.Where(x => !x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid.HasValue && x.MasterConAndDeconsolidateHeaderMasterID_Guid.HasValue);
                    var masterNInJob = nonInCon.Select(s => new MasterIDGroup
                    {
                        JobGuid = s.MasterActualJobHeader_Guid.GetValueOrDefault(),
                        MasterLocID = s.Master_ID,
                        MasterRouteID = s.MasterID_Route
                    });
                    #endregion

                    var masterInJob = masterSInJob.Union(masterNInJob)
                                      .GroupBy(g => g.JobGuid).Select(o => new MasterIDGroup
                                      {
                                          JobGuid = o.Key,
                                          MasterLocID = JoinStringMaster(o.Select(e => e.MasterLocID)),
                                          MasterRouteID = JoinStringMaster(o.Select(e => e.MasterRouteID))
                                      }).ToList();

                    List<int> jobTypeTV = new List<int> { IntTypeJob.TV, IntTypeJob.TV_MultiBr };
                    #region Job on run
                    var result = jobDetail.Select(o =>
                    {
                        var countSeal = sealNotInCon.Count(x => x.MasterActualJobHeader_Guid == o.JobGuid);
                        var sealR = sealInConRoute.Where(x => x.MasterActualJobHeader_Guid == o.JobGuid).Select(x => x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid).Distinct();
                        var sealL = sealInConLoc.Where(x => x.MasterActualJobHeader_Guid == o.JobGuid).Select(x => x.MasterConAndDeconsolidateHeaderMasterID_Guid).Distinct();
                        var countSealRoute = sealR.Count();
                        var countSealLocaton = sealL.Count();

                        var countNon = nonNotInCon.Where(x => x.MasterActualJobHeader_Guid == o.JobGuid).Sum(c => c.Quantity.GetValueOrDefault());
                        var nonR = nonInConRoute.Where(x => x.MasterActualJobHeader_Guid == o.JobGuid).Select(x => x.MasterConAndDeconsolidateHeaderMasterIDRoute_Guid).Distinct();
                        var nonL = nonInConLoc.Where(x => x.MasterActualJobHeader_Guid == o.JobGuid).Select(x => x.MasterConAndDeconsolidateHeaderMasterID_Guid).Distinct();
                        var countNonRoute = nonR.Count();
                        var countNonLoc = nonL.Count();

                        var isNotDisplayTV = !(jobTypeTV.Any(t => t == o.JobTypeID) &&
                                           (o.JobAction == "D" && o.JobStatusID == IntStatusJob.PickedUp) || (o.JobAction == "P" && StatusForTV_D().Any(d => d == o.JobStatusID)));

                        var isNotOnHand = !JobStatusNotOnHand().Any(a => a == o.JobStatusID);
                        var mInJob = masterInJob.FirstOrDefault(x => x.JobGuid == o.JobGuid);

                        var jobSTC = jobWithSTC.FirstOrDefault(x => x.JobGuid == o.JobGuid);

                        if (jobSTC != null)
                        {
                            o.STC = jobSTC.STC;

                            o.FlagSecLiability = jobSTC.CurrencyList.Any() && isNotOnHand && isNotDisplayTV;
                            o.LiabilityOnHandList = jobSTC.CurrencyList;

                            o.FlagSecNonbarcode = jobSTC.CommodityList.Any() && isNotOnHand && isNotDisplayTV;
                            o.CommodityOnHandList = jobSTC.CommodityList;

                            o.Commodity = jobSTC.strCommodity;
                            o.FlagExConvertToRun = jobSTC.FlagExConvertToRun;
                        }

                        o.Seal_Qty = countSeal + countSealRoute + countSealLocaton;
                        o.Non_Qty = countNon + countNonRoute + countNonLoc;

                        //Include seal has liability.
                        o.FlagSecItem = o.Seal_Qty > 0 && !JobStatusNotOnHand().Any(a => a == o.JobStatusID)
                                        && isNotOnHand && isNotDisplayTV;

                        o.MachineID = o.MachineID ?? string.Empty;
                        o.MasterLocationID = mInJob?.MasterLocID;
                        o.MasterRouteID = mInJob?.MasterRouteID;

                        return o;
                    }).ToList();

                    response.OnHandJobOnRun = result;
                    response.CurrencyNotConvert = jobWithSTC.SelectMany(o => o.CurrencyNotConvert.Distinct()).ToList();
                    #endregion

                    #region Summary job on run

                    response.GrpJobOnRunList = result
                                               .GroupBy(g => g.GroupJobTypeID)
                                               .Select(o => new GroupOnHandQtyOnRunView
                                               {
                                                   JobTypeName = o.First().GroupJobTypeName,
                                                   JobTypeIDGrp = o.Key,
                                                   Qty = o.Select(j => j.JobGuid).Distinct().Count()
                                               }).OrderBy(b => b.JobTypeName).ToList();

                    response.GrpJobServiceDone = result.Where(e => e.FlagSecServiceDone)
                                                .GroupBy(g => g.GroupJobTypeID)
                                                .Select(o => new GroupOnHandQtyOnRunView
                                                {
                                                    JobTypeName = o.First().GroupJobTypeName,
                                                    JobTypeIDGrp = o.Key,
                                                    Qty = o.Select(j => j.JobGuid).Distinct().Count()
                                                }).OrderBy(r => r.JobTypeName).ToList();



                    int sumItemSeal = result.Where(x => x.FlagSecItem)
                                      .GroupBy(g => new { g.JobGuid, g.Seal_Qty }).Sum(e => e.Key.Seal_Qty);
                    if (sumItemSeal > 0)
                    {
                        response.GrpItemSeal = new List<GroupOnHandItem> { new GroupOnHandItem { Seal = "Seal", Qty = sumItemSeal } };
                    }

                    response.GrpItemCommodity = result.Where(x => x.FlagSecNonbarcode)
                                                .Select(e => new { e.CommodityOnHandList, e.JobGuid }).Distinct()
                                                .SelectMany(x => x.CommodityOnHandList)
                                                .GroupBy(g => g.CommodityName)
                                                .Select(o => new GroupOnHandCommodity
                                                {
                                                    CommodityName = o.Key,
                                                    Qty = o.Sum(s => s.CommodityQty),
                                                    GoldenRuleNo = o.First().GoldenRuleNo
                                                })
                                                .OrderBy(r => r.GoldenRuleNo != 0 ? 0 : 1).ThenBy(x => x.GoldenRuleNo).ToList();

                    var jobUnable = result.Where(e => e.FlagSecUnableToService);
                    if (jobUnable.Any())
                    {
                        var grpItemUnable = jobUnable
                                        .GroupBy(g => new { g.GroupJobTypeID, g.JobStatusID })
                                                .Select(o => new OnHandUnableItem
                                                {
                                                    JobTypeIDGrp = o.Key.GroupJobTypeID,
                                                    JobTypeID = o.First().JobTypeID,
                                                    JobStatus = o.First().JobStatus,
                                                    Qty = o.Select(e => e.JobGuid).Distinct().Count()
                                                }).ToList();

                        response.GrpJobUnable = jobUnable
                                                    .GroupBy(g => g.GroupJobTypeID)
                                                    .Select(o => new GroupOnHandJobUnable
                                                    {
                                                        JobTypeName = o.First().GroupJobTypeName,
                                                        JobTypeIDGrp = o.Key,
                                                        SumQty = o.Count(),
                                                        OnHandUnableItem = grpItemUnable.Where(x => x.JobTypeIDGrp == o.Key).ToList()
                                                    }).ToList();
                    }

                    response.GrpItemLiability = result.Where(x => x.FlagSecLiability)
                                                .Select(e => new { e.LiabilityOnHandList, e.JobGuid }).Distinct()
                                                .SelectMany(t => t.LiabilityOnHandList)
                                                .GroupBy(g => g.CurrencyName)
                                                .Select(o => new GroupOnHandLiablity
                                                {
                                                    CurrencyAbbr = o.Key,
                                                    LiabilityValue = o.Sum(s => s.LiabilityValue)
                                                }).ToList();
                    #endregion
                }
                response.MessageResponse = _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView(true);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                response.MessageResponse = _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
            }
            return response;
        }
        private List<OnHandJobOnRunView> SeparateSectionJobs(List<OnHandJobOnRunView> jobs)
        {
            List<int> UnableStatus = new List<int> {
                IntStatusJob.ReturnToPreVault,
                IntStatusJob.UnableToService,
                IntStatusJob.Unrealized,
                IntStatusJob.VisitWithStamp,
                IntStatusJob.VisitWithOutStamp,
                IntStatusJob.NoArrived
            };

            List<int> All_Done = new List<int> {
                IntStatusJob.Delivered,
                IntStatusJob.Closed,
                IntStatusJob.Department
            };

            var result = jobs.Select(o =>
            {
                if (UnableStatus.Any(x => x == o.JobStatusID))
                {
                    o.FlagSecUnableToService = true;
                }

                if (All_Done.Any(x => x == o.JobStatusID))
                {
                    o.FlagSecServiceDone = true;
                }
                else
                {
                    List<int> P_Done = new List<int> {
                        IntStatusJob.PickedUp,

                        IntStatusJob.InPreVault,
                        IntStatusJob.ReturnToPreVault,
                        IntStatusJob.PartialDelivery,

                        IntStatusJob.InPreVaultPickUp,
                        IntStatusJob.InPreVaultDelivery,

                        IntStatusJob.IntransitInterBranch
                    };

                    List<int> T_Done = new List<int>{
                        IntStatusJob.InPreVault,
                        IntStatusJob.ReturnToPreVault,
                        IntStatusJob.PartialDelivery,

                        IntStatusJob.InPreVaultPickUp,
                        IntStatusJob.InPreVaultDelivery
                    };

                    List<int> TVP_Done = new List<int>{
                        IntStatusJob.PickedUp,

                        IntStatusJob.InPreVault,
                        IntStatusJob.ReturnToPreVault,
                        IntStatusJob.PartialDelivery,

                        IntStatusJob.InPreVaultPickUp,
                        IntStatusJob.InPreVaultDelivery,

                        IntStatusJob.OnTruckDelivery,
                        IntStatusJob.OnTheWayDelivery,
                        IntStatusJob.IntransitInterBranch
                    };

                    List<int> TVD_Done = new List<int>{
                        IntStatusJob.PickedUp,

                        IntStatusJob.InPreVault,
                        IntStatusJob.PartialDelivery,

                        IntStatusJob.InPreVaultPickUp,
                        IntStatusJob.InPreVaultDelivery
                    };

                    switch (o.JobTypeID)
                    {
                        case IntTypeJob.P:
                        case IntTypeJob.FLM:
                        case IntTypeJob.FSLM:
                        case IntTypeJob.BCP:
                        case IntTypeJob.P_MultiBr:
                            o.FlagSecServiceDone = P_Done.Any(x => x == o.JobStatusID);
                            break;
                        case IntTypeJob.T:
                            o.FlagSecServiceDone = T_Done.Any(x => x == o.JobStatusID);
                            break;
                        case IntTypeJob.TV:
                        case IntTypeJob.TV_MultiBr:
                            o.FlagSecServiceDone = o.JobAction == "P" ? TVP_Done.Any(x => x == o.JobStatusID) : TVD_Done.Any(x => x == o.JobStatusID);
                            break;
                    }
                }
                return o;
            }).ToList();

            return result;
        }
        private IEnumerable<RunControlRunResourceDailyBySiteAndDateGetResult> GetDailyRunBySiteAndDate(Guid siteGuid, DateTime workDate, Guid? userGuid)
        {
            return _masterRouteRepository.GetDailyRunBySiteAndWorkDate(siteGuid, workDate.Date, userGuid);
        }
        public OnHandRouteResponse GetDetailJob(OnHandMasterRouteRequest request)
        {
            var resp = new OnHandRouteResponse();
            if (request.DailyRunGuid != null && request.DailyRunGuid.HasValue)
            {
                resp = GetDetailJobByDailyRun(request.SiteGuid, new List<Guid> { request.DailyRunGuid.Value });
            }
            else
            {
                var dailyRunGuid = GetDailyRunBySiteAndDate(request.SiteGuid, request.WorkDate, ApiSession.UserGuid)
                                   .FirstOrDefault()?.Guid;
                if (dailyRunGuid != null)
                {
                    resp = GetDetailJobByDailyRun(request.SiteGuid, new List<Guid> { (Guid)dailyRunGuid });
                }
                else
                {
                    resp.MessageResponse = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.GetValueOrDefault()).ConvertToMessageView(true);
                }
            }
            return resp;
        }
        public OnHandRouteSummaryResponse GetOnHandRouteSummary(OnHandMasterRouteRequest request)
        {
            OnHandRouteSummaryResponse resp = new OnHandRouteSummaryResponse();
            Guid languageGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault();

            try
            {
                var dailyRunList = GetDailyRunBySiteAndDate(request.SiteGuid, request.WorkDate, ApiSession.UserGuid).ToList();
                var dailyRunGuidList = dailyRunList.Select(x => x.Guid.GetValueOrDefault()).ToList();
                var jobsDetail = GetDetailJobByDailyRun(request.SiteGuid, dailyRunGuidList).OnHandJobOnRun;
                int countJob = jobsDetail.Select(d => d.JobGuid).Distinct().Count();
                int countJobDone = jobsDetail.Where(x => x.FlagSecServiceDone).Select(j => j.JobGuid).Distinct().Count();
                int countJobUnable = jobsDetail.Where(x => x.FlagSecUnableToService).Select(j => j.JobGuid).Distinct().Count();
                int sumItemSeal = jobsDetail.Where(x => x.FlagSecItem).GroupBy(g => new { g.JobGuid, g.Seal_Qty }).Sum(x => x.Key.Seal_Qty);

                var liaDetail = jobsDetail.Where(x => x.FlagSecLiability)
                                .Select(d => new { d.LiabilityOnHandList, d.JobGuid }).Distinct()
                                .SelectMany(x => x.LiabilityOnHandList);
                var grpLiability = liaDetail
                                   .GroupBy(g => g.CurrencyName)
                                   .Select(d => new CurrencyValueView
                                   {
                                       CurrencyName = d.Key,
                                       LiabilityValue = d.Sum(s => s.LiabilityValue),
                                       UserLiabilityValue = d.Sum(s => s.UserLiabilityValue),
                                       FlagExConvert = d.First().FlagExConvert
                                   }).ToList();

                var sumLiability = grpLiability.Sum(s => s.LiabilityValue);
                var sumLiabilityUser = grpLiability.Sum(s => s.UserLiabilityValue);
                bool isMixCurrency = grpLiability.Select(x => x.CurrencyName).Distinct().Count() > 1;
                int sumNonbarcode = jobsDetail.Where(x => x.FlagSecNonbarcode).Select(d => new { d.CommodityOnHandList, d.JobGuid }).Distinct()
                                    .SelectMany(x => x.CommodityOnHandList).Sum(s => s.CommodityQty);
                resp = new OnHandRouteSummaryResponse()
                {
                    SumJobInRun = countJob,
                    SumJobServiceDone = countJobDone,
                    SumJobUnableToService = countJobUnable,
                    SumLiabilityOnHand = sumLiability,
                    SumLiabilityOnHandUser = sumLiabilityUser,
                    SumLiabilityValue = grpLiability,
                    FlagMixCurrency = isMixCurrency,
                    SumItemOnHand = sumItemSeal,
                    SumNonBarcodeOnHand = sumNonbarcode,
                    DailyRunDetailList = dailyRunList
                };

                resp.MessageResponse = _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView(true);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                resp.MessageResponse = _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
            }
            return resp;
        }
        private List<int> JobStatusNotOnHand()
        {
            return new List<int> {
                        IntStatusJob.InPreVault,
                        IntStatusJob.Delivered,
                        IntStatusJob.Closed,
                        IntStatusJob.NonDelivery,
                        IntStatusJob.CancelledJob,
                        IntStatusJob.PartialDelivery,
                        IntStatusJob.InPreVaultPickUp,
                        IntStatusJob.InPreVaultDelivery,

                        IntStatusJob.IntransitInterBranch,
                        IntStatusJob.Department, // In department
                        IntStatusJob.PartialInDepartment,
                        IntStatusJob.InDepartmentandWaitingforDeconsolidate,
                        IntStatusJob.WaitingforDeconsolidate
                   };
        }
        private List<int> StatusForTV_D()
        {
            return new List<int> { IntStatusJob.ReturnToPreVault, IntStatusJob.OnTruckDelivery, IntStatusJob.OnTheWayDelivery };
        }
        private List<JobWithStcView> GetJobWithSTCList(List<Guid> dailyRunGuids, List<OnHandJobOnRunView> jobList, Guid siteGuid)
        {
            var result = new List<JobWithStcView>();
            var currentExchange = _masterCurrencyRepository.GetCurrencyExchangeList(siteGuid).ToList();

            foreach (var runGuid in dailyRunGuids)
            {
                var jobListOnRun = jobList.Where(e => e.DailyRunGuid == runGuid).Select(o => o.JobGuid);
                result.AddRange(_masterActualJobHeaderRepository.GetSTCOnHandByJobList(jobListOnRun, siteGuid, runGuid, currentExchange, ApiSession.UserGuid.GetValueOrDefault(), true, true));
            }

            return result;
        }
        private string JoinStringMaster(IEnumerable<string> masterIDList)
        {
            return string.Join(",", masterIDList.Where(o => !string.IsNullOrEmpty(o)).Distinct().OrderBy(e => e));
        }


        //For summary tips (Run control)
        public JobWithSTCResponse GetSTCOnHandSummaryTips(JobWithSTCRequest request)
        {
            JobWithSTCResponse response = new JobWithSTCResponse();
            var currentExchange = _masterCurrencyRepository.GetCurrencyExchangeList(request.SiteGuid).ToList();

            response.JobWithSTCOnHand = _masterActualJobHeaderRepository.GetSTCOnHandByJobList(request.JobList, request.SiteGuid, request.DailyRunGuid, currentExchange, request.UserGuid, request.FlagCalExchageRate);
            return response;
        }
    }
}
