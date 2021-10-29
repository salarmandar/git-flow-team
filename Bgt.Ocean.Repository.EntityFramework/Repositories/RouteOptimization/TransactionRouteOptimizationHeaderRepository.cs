using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.RouteOptimization;
using System.Collections.Generic;
using Bgt.Ocean.Infrastructure.Util;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using Bgt.Ocean.Models.RunControl.LiabilityLimitModel;
using Bgt.Ocean.Infrastructure.Storages;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ITransactionRouteOptimizationHeaderRepository : IRepository<TblTransactionRouteOptimizationHeader>
    {
        IEnumerable<DailyRouteView> GetDailyRunByOptimizationGuid(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit);

        IEnumerable<JobUnassignedView> GetUnassignByOptimizationGuid(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit);
        IEnumerable<RequestManagementView> GetOptimizeRequestMangement(RequestManagementRequest request, Guid? langquageGuid);
        MasterRouteRequestDetailResponse GetOptimizeRequestMangementRmDetail(Guid requestGuid, Guid? langquageGuid, string formatDate);
        DailyRouteRequestDetailResponse GetOptimizeRequestMangementRDDetail(Guid requestGuid, Guid? langquageGuid, string formatDate);
        IEnumerable<MasterRouteOptimizationViewModel> GetRouteOptimizeByRequest(Guid requestGuid, int maxrow, Guid? langquageGuid);
        RoadnetFileViewModel GetOptimizeRequestGenerateFile();
        RouteOptimizeSearchResultModel GetRouteGroupHasOptimize(RouteOptimizeSearchModel request);
    }
    public class TransactionRouteOptimizationHeaderRepository : Repository<OceanDbEntities, TblTransactionRouteOptimizationHeader>, ITransactionRouteOptimizationHeaderRepository
    {
        public TransactionRouteOptimizationHeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        public IEnumerable<DailyRouteView> GetDailyRunByOptimizationGuid(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit)
        {
            var countryGuid = DbContext.TblMasterSite.FirstOrDefault(e => e.Guid == request.SiteGuid).MasterCountry_Guid;
            string currencyOfUser = "";
            var currentExchange = GetCurrencyExchangeList(countryGuid);
            var Template = DbContext.GetAllCommodityBySite(request.SiteGuid, countryGuid).ToList();
            var result = (from Header in DbContext.TblTransactionRouteOptimizationHeader.Where(e => e.Guid == request.RequestGuid)
                          join Detail in DbContext.TblTransactionRouteOptimizationHeader_Detail.Where(e => e.MasterDailyRunResource_Guid != null) on Header.Guid equals Detail.TransactionRouteOptimizationHeader_Guid
                          join Item in DbContext.TblTransactionRouteOptimizationHeader_Detail_Item on Detail.Guid equals Item.TransactionRouteOptimizationHeader_Detail_Guid
                          into Itemleft
                          from Item in Itemleft.DefaultIfEmpty()

                          join DailyRun in DbContext.TblMasterDailyRunResource on Detail.MasterDailyRunResource_Guid equals DailyRun.Guid
                          join RouteGroupDetail in DbContext.TblMasterRouteGroup_Detail on DailyRun.MasterRouteGroup_Detail_Guid equals RouteGroupDetail.Guid
                          join RouteGroup in DbContext.TblMasterRouteGroup on RouteGroupDetail.MasterRouteGroup_Guid equals RouteGroup.Guid
                          join MasterRun in DbContext.TblMasterRunResource on DailyRun.MasterRunResource_Guid equals MasterRun.Guid
                          join ServiceStopLeg in DbContext.TblMasterActualJobServiceStopLegs.Select(e => new { e.Guid, e.MasterActualJobHeader_Guid, e.MasterRunResourceDaily_Guid, e.MasterCustomerLocation_Guid, e.WindowsTimeServiceTimeStart }).Distinct() on Item.MasterActualJobServiceStopLegs_Guid equals ServiceStopLeg.Guid
                          into ServiceStopLegleft
                          from ServiceStopLeg in ServiceStopLegleft.DefaultIfEmpty()
                          join JobHeader in DbContext.TblMasterActualJobHeader on ServiceStopLeg.MasterActualJobHeader_Guid equals JobHeader.Guid
                          into JobHeaderleft
                          from JobHeader in JobHeaderleft.DefaultIfEmpty()
                          join Currency in DbContext.TblMasterCurrency on MasterRun.LiabilityLimitCurrency_Guid equals Currency.Guid
                          into Currencyleft
                          from Currency in Currencyleft.DefaultIfEmpty()
                          join RouteOp in DbContext.TblSystemRouteOptimizationStatus on DailyRun.SystemRouteOptimizationStatus_Guid equals RouteOp.Guid
                          into RouteOpleft
                          from RouteOp in RouteOpleft.DefaultIfEmpty(DbContext.TblSystemRouteOptimizationStatus.Where(e => e.RouteOptimizationStatusID == 0).FirstOrDefault())
                          join RouteOpLang in DbContext.TblSystemDisplayTextControlsLanguage.Where(e => e.SystemLanguageGuid == languageGuid) on RouteOp.SystemDisplayTextControlsLanguage_Guid equals RouteOpLang.Guid
                          into RouteOpLangleft
                          from RouteOpLang in RouteOpLangleft.DefaultIfEmpty()
                          join CusLo in DbContext.TblMasterCustomerLocation on ServiceStopLeg.MasterCustomerLocation_Guid equals CusLo.Guid
                          into CusLoleft
                          from CusLo in CusLoleft.DefaultIfEmpty()
                          join Cus in DbContext.TblMasterCustomer on CusLo.MasterCustomer_Guid equals Cus.Guid
                          into Cusleft
                          from Cus in Cusleft.DefaultIfEmpty()
                          select new
                          {
                              DailyRun.Guid,
                              DailyRun.MasterRunResourceShift,
                              DailyRun.RunResourceDailyStatusID,
                              DailyRun.FlagRouteBalanceDone,
                              DailyRun.StartTime,
                              RouteGroupDetailGuid = RouteGroupDetail.Guid,
                              RouteGroupDetail.MasterRouteGroupDetailName,
                              RouteGroupGuid = RouteGroup.Guid,
                              RouteGroup.MasterRouteGroupName,
                              MasterRun.VehicleNumber,
                              ServiceStopLeg,
                              Currency.MasterCurrencyAbbreviation,
                              RouteOp.RouteOptimizationStatusID,
                              RouteOptimizationStatusName = RouteOpLang.DisplayText,
                              Cus.FlagChkCustomer,
                              jobHeaderGuid = JobHeader.Guid,
                              CurrencyOnrun = MasterRun.LiabilityLimitCurrency_Guid
                          }).GroupBy(e => new { e.Guid, e.MasterRunResourceShift, e.RunResourceDailyStatusID, e.FlagRouteBalanceDone, e.StartTime, e.RouteGroupDetailGuid, e.MasterRouteGroupDetailName, e.RouteGroupGuid, e.MasterRouteGroupName, e.VehicleNumber, e.jobHeaderGuid, e.CurrencyOnrun }).AsEnumerable().Select(e => new DailyRouteView()
                          {
                              Guid = e.Key.Guid,
                              RouteGroupGuid = e.Key.RouteGroupGuid,
                              MasterRouteGroupName = e.Key.MasterRouteGroupName,
                              RouteGroupDetailGuid = e.Key.RouteGroupDetailGuid,
                              MasterRouteGroupDetailName = e.Key.MasterRouteGroupDetailName,
                              RunNo = (e.Key.MasterRunResourceShift > 1) ? e.Key.VehicleNumber + " shift " + e.Key.MasterRunResourceShift : e.Key.VehicleNumber,
                              RunStatusId = e.Key.RunResourceDailyStatusID.Value,
                              Balanced = e.Key.FlagRouteBalanceDone ? "Yes" : "No",
                              StartTime = e.Key.StartTime,
                              Currency = (e.Select(f => f.MasterCurrencyAbbreviation).Distinct().Count() > 1) ? "*" : e.FirstOrDefault().MasterCurrencyAbbreviation,
                              OptimizationStatus = e.FirstOrDefault().RouteOptimizationStatusName,
                              OptimizationStatusId = e.FirstOrDefault().RouteOptimizationStatusID,
                              Jobs = (e.FirstOrDefault().ServiceStopLeg != null) ? e.Select(f => f.ServiceStopLeg?.MasterActualJobHeader_Guid).Distinct().Count() : 0,
                              Locations = (e.FirstOrDefault().ServiceStopLeg != null) ? e.Where(f => f.ServiceStopLeg.MasterCustomerLocation_Guid.HasValue && f.FlagChkCustomer.GetValueOrDefault()).Select(g => g.ServiceStopLeg?.MasterCustomerLocation_Guid).Distinct().Count() : 0,
                              Stops = (e.FirstOrDefault().ServiceStopLeg != null) ? e.Where(f => f.ServiceStopLeg.MasterCustomerLocation_Guid.HasValue && f.FlagChkCustomer.GetValueOrDefault()).Select(g => g.ServiceStopLeg?.MasterCustomerLocation_Guid.ToString() + "_" + g.ServiceStopLeg?.WindowsTimeServiceTimeStart.ToString()).Distinct().Count() : 0,
                              JobHeaderGuid = e.Key.jobHeaderGuid,
                              CurrencyOnrunGuid = e.Key.CurrencyOnrun
                          });

            var jobGuidList = result.Select(e => e.JobHeaderGuid).ToList();
            var ItemLiability = DbContext.TblMasterActualJobItemsLiability.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                                .Select(o => new ItemsLibilityView
                                {
                                    DocCurrencyGuid = o.MasterCurrency_Guid,
                                    Liability = o.Liability ?? 0,
                                    LibilityGuid = o.Guid,
                                    JobGuid = o.MasterActualJobHeader_Guid
                                }).ToList();

            // check only one currency in job.    
            List<CurrencyOfDailyRunResourceView> currencyOnRun = null;
            if (!validateRunLiabilityLimit)
            {
                var userCurrency = DbContext.TblMasterUser.FirstOrDefault(o => o.Guid == ApiSession.UserGuid);
                currencyOfUser = DbContext.TblMasterCurrency.FirstOrDefault(f => f.Guid == userCurrency.MasterCurrency_Default_Guid).MasterCurrencyAbbreviation;

                var checkOneCurrency = ItemLiability.Where(w => w.DocCurrencyGuid.HasValue)
                    .GroupBy(g => new { g.JobGuid, g.DocCurrencyGuid }).Select(s => new
                    {
                        JobGuid = s.Key.JobGuid.Value,
                        DocCurrencyGuid = s.Key.DocCurrencyGuid.Value
                    })?.ToList();
                var currencyFromLiability = checkOneCurrency
                        .Join(DbContext.TblMasterCurrency
                        , cc => cc.DocCurrencyGuid
                        , c => c.Guid,
                        (cc, c) =>
                        new
                        {
                            cc.JobGuid,
                            c.MasterCurrencyAbbreviation,
                            c.Guid
                        })?.ToList();

                //Group by currency by run
                var dr = result.Select(
                    s => new
                    {
                        s.Guid,
                        s.JobHeaderGuid
                    });
                currencyOnRun = (from r in dr
                                 join c in currencyFromLiability on r.JobHeaderGuid equals c.JobGuid
                                 group new { r, c } by new { r.Guid, c.MasterCurrencyAbbreviation } into grc
                                 where grc.Any()
                                 select new CurrencyOfDailyRunResourceView
                                 {
                                     DailyRunGuid = grc.Key.Guid,
                                     CurrencyAbb = grc.Key.MasterCurrencyAbbreviation
                                 }
                  )?.ToList();
            }

            var Commudity = DbContext.TblMasterActualJobItemsCommodity.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                            .Select(o => new ItemsCommodityView
                            {
                                ActualCommodityGuid = o.Guid,
                                CommodityGuid = o.MasterCommodity_Guid,
                                FlagCommodityDiscrepancies = o.FlagCommodityDiscrepancies ?? false,
                                Quantity = o.Quantity ?? 0,
                                QuantityActual = o.QuantityActual ?? 0,
                                QuantityExpected = o.QuantityExpected ?? 0,
                                JobGuid = o.MasterActualJobHeader_Guid
                            }).ToList();

            return result.Select(e =>
            {
                e.STC = (decimal)(new RawJobDataView()
                {
                    Target_CurrencyGuid = e.CurrencyOnrunGuid,
                    JobItems = new RawItemsView()
                    {
                        Commodities = Commudity.Where(o => o.JobGuid == e.JobHeaderGuid),
                        Liabilities = ItemLiability.Where(o => o.JobGuid == e.JobHeaderGuid)
                    }

                }).CalculateJobSTC(currentExchange, Template, validateRunLiabilityLimit).TotalJobSTC;
                if (!validateRunLiabilityLimit)
                {
                    var currency = currencyOnRun.Count(c => c.DailyRunGuid == e.Guid);
                    if (currency == 1)
                    {
                        e.Currency = currencyOnRun.FirstOrDefault(f => f.DailyRunGuid == e.Guid).CurrencyAbb;
                    }
                    else if (currency > 1)
                    {
                        e.Currency = "*";
                    }
                    else
                    {
                        e.Currency = currencyOfUser;
                    }

                }
                return e;
            }).GroupBy(e => e.Guid)
                        .Select(e => new DailyRouteView()
                        {
                            Guid = e.Key,
                            RouteGroupGuid = e.FirstOrDefault().RouteGroupGuid,
                            MasterRouteGroupName = e.FirstOrDefault().MasterRouteGroupName,
                            RouteGroupDetailGuid = e.FirstOrDefault().RouteGroupDetailGuid,
                            MasterRouteGroupDetailName = e.FirstOrDefault().MasterRouteGroupDetailName,
                            RunNo = e.FirstOrDefault().RunNo,
                            RunStatusId = e.FirstOrDefault().RunStatusId,
                            RunStatus = GetRunStatus(e.FirstOrDefault().RunStatusId),
                            Balanced = e.FirstOrDefault().Balanced,
                            StartTime = e.FirstOrDefault().StartTime,
                            STC = e.Sum(f => f.STC),
                            Currency = e.FirstOrDefault().Currency,
                            OptimizationStatus = e.FirstOrDefault().OptimizationStatus,
                            OptimizationStatusId = e.FirstOrDefault().OptimizationStatusId,
                            Jobs = e.Sum(f => f.Jobs),
                            Locations = e.Sum(f => f.Locations),
                            Stops = e.Sum(f => f.Stops),

                            StrStops = e.Sum(f => f.Stops).ToString(),
                            StrJobs = e.Sum(f => f.Jobs).ToString(),
                            StrLocations = e.Sum(f => f.Locations).ToString(),
                            StrStartTime = e.FirstOrDefault().StartTime.Value.ToString("HH:mm"),
                            StrSTCDisplay = e.Sum(f => f.STC) + " " + e.FirstOrDefault().Currency
                        }).OrderBy(e => e.RouteGroupGuid).ThenBy(e => e.RouteGroupDetailGuid).ThenBy(e => e.RunNo);

        }
        private string GetRunStatus(int runId)
        {
            string str = "";
            switch (runId)
            {
                case EnumRun.StatusDailyRun.ReadyRun:
                    str = "Ready";
                    break;
                case EnumRun.StatusDailyRun.ClosedRun:
                    str = "Closed";
                    break;
                case EnumRun.StatusDailyRun.CrewBreak:

                    str = "CrewBreak";
                    break;
                case EnumRun.StatusDailyRun.DispatchRun:
                    str = "Dispatch";
                    break;
                default:
                    str = "Error";
                    break;
            }
            return str;
        }

        private IEnumerable<TblMasterCurrency_ExchangeRate> GetCurrencyExchangeList(Guid contryGuid)
        {
            var global_exchange = DbContext.TblMasterCurrency_ExchangeRate.Where(o => !o.FlagDisable && o.MasterCountry_Guid == null).ToList();
            var local_exchange = DbContext.TblMasterCurrency_ExchangeRate.Where(o => !o.FlagDisable && o.MasterCountry_Guid == contryGuid).ToList();


            var include_global = global_exchange.Where(o => !local_exchange.Any(t => t.MasterCurrencySource_Guid == o.MasterCurrencySource_Guid && t.MasterCurrencyTarget_Guid == o.MasterCurrencyTarget_Guid))
                                                .Select(o => { o.MasterCountry_Guid = contryGuid; return o; });

            var merge_exchange = local_exchange.Union(include_global);

            return merge_exchange;
        }

        public IEnumerable<JobUnassignedView> GetUnassignByOptimizationGuid(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit)
        {
            var countryGuid = DbContext.TblMasterSite.Where(e => e.Guid == request.SiteGuid).Select(e => e.MasterCountry_Guid).FirstOrDefault();
            var currentExchange = GetCurrencyExchangeList(countryGuid);
            var Template = DbContext.GetAllCommodityBySite(request.SiteGuid, countryGuid).ToList();
            var defaultCurrency = DbContext.TblMasterUser.FirstOrDefault(o => o.Guid == ApiSession.UserGuid);
            var defaultCurrencyApp = DbContext.TblMasterCurrency.FirstOrDefault(f => f.Guid == defaultCurrency.MasterCurrency_Default_Guid).MasterCurrencyAbbreviation;

            var result = (from Header in DbContext.TblTransactionRouteOptimizationHeader.Where(e => e.Guid == request.RequestGuid)
                          join Detail in DbContext.TblTransactionRouteOptimizationHeader_Detail.Where(e => e.MasterDailyRunResource_Guid == null) on Header.Guid equals Detail.TransactionRouteOptimizationHeader_Guid
                          join Item in DbContext.TblTransactionRouteOptimizationHeader_Detail_Item on Detail.Guid equals Item.TransactionRouteOptimizationHeader_Detail_Guid

                          join Legs in DbContext.TblMasterActualJobServiceStopLegs on Item.MasterActualJobServiceStopLegs_Guid equals Legs.Guid
                          join jobHeader in DbContext.TblMasterActualJobHeader on Legs.MasterActualJobHeader_Guid equals jobHeader.Guid
                          join CusLo in DbContext.TblMasterCustomerLocation on Legs.MasterCustomerLocation_Guid equals CusLo.Guid
                          join Cus in DbContext.TblMasterCustomer on CusLo.MasterCustomer_Guid equals Cus.Guid
                          join jobAction in DbContext.TblSystemJobAction on Legs.CustomerLocationAction_Guid equals jobAction.Guid
                          join JobType in DbContext.TblSystemServiceJobType on jobHeader.SystemServiceJobType_Guid equals JobType.Guid
                          join LOB in DbContext.TblSystemLineOfBusiness on jobHeader.SystemLineOfBusiness_Guid equals LOB.Guid
                          join RouteOp in DbContext.TblSystemRouteOptimizationStatus on Legs.SystemRouteOptimizationStatus_Guid equals RouteOp.Guid
                          into RouteOpleft
                          from RouteOp in RouteOpleft.DefaultIfEmpty(DbContext.TblSystemRouteOptimizationStatus.Where(e => e.RouteOptimizationStatusID == 0).FirstOrDefault())
                          join RouteOpLang in DbContext.TblSystemDisplayTextControlsLanguage.Where(e => e.SystemLanguageGuid == languageGuid) on RouteOp.SystemDisplayTextControlsLanguage_Guid equals RouteOpLang.Guid
                          into RouteOpLangleft
                          from RouteOpLang in RouteOpLangleft.DefaultIfEmpty()
                          where Cus.FlagChkCustomer.Value
                          select new
                          {
                              JobLegGuid = Legs.Guid,
                              JobNo = jobHeader.JobNo,
                              Action = jobAction.ActionNameAbbrevaition,
                              ServiceJobTypeAbbr = JobType.ServiceJobTypeNameAbb,
                              LOBFullName = LOB.LOBFullName,
                              CusLo.BranchName,
                              Cus.CustomerFullName,
                              MasterCurrencyAbbreviation = defaultCurrencyApp,
                              RouteOp.RouteOptimizationStatusID,
                              RouteOptimizationStatusName = RouteOpLang.DisplayText,
                              JobHeaderGuid = (Guid?)jobHeader.Guid
                          }).AsEnumerable().GroupBy(e => new { e.JobLegGuid, e.JobNo, e.Action, e.ServiceJobTypeAbbr, e.LOBFullName, e.BranchName, e.CustomerFullName, e.RouteOptimizationStatusID, e.RouteOptimizationStatusName, e.JobHeaderGuid });


            var jobGuidList = result.Select(e => e.Key.JobHeaderGuid).ToList();
            var ItemLiability = DbContext.TblMasterActualJobItemsLiability.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                                .Select(o => new ItemsLibilityView
                                {
                                    DocCurrencyGuid = o.MasterCurrency_Guid,
                                    Liability = o.Liability ?? 0,
                                    LibilityGuid = o.Guid,
                                    JobGuid = o.MasterActualJobHeader_Guid
                                }).ToList();
            // check only one currency in job.
            var checkOneCurrency = ItemLiability.Where(w => w.DocCurrencyGuid.HasValue)
                .GroupBy(g => new { g.JobGuid, g.DocCurrencyGuid }).Select(s => new
                {
                    JobGuid = s.Key.JobGuid.Value,
                    DocCurrencyGuid = s.Key.DocCurrencyGuid.Value
                })?.ToList();
            var currencyFromLiability = checkOneCurrency
                    .Join(DbContext.TblMasterCurrency
                    , cc => cc.DocCurrencyGuid
                    , c => c.Guid,
                    (cc, c) =>
                                        new
                                        {
                                            cc.JobGuid,
                                            c.MasterCurrencyAbbreviation,
                                            c.Guid
                                        })?.ToList();

            var Commudity = DbContext.TblMasterActualJobItemsCommodity.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                            .Select(o => new ItemsCommodityView
                            {
                                ActualCommodityGuid = o.Guid,
                                CommodityGuid = o.MasterCommodity_Guid,
                                FlagCommodityDiscrepancies = o.FlagCommodityDiscrepancies ?? false,
                                Quantity = o.Quantity ?? 0,
                                QuantityActual = o.QuantityActual ?? 0,
                                QuantityExpected = o.QuantityExpected ?? 0,
                                JobGuid = o.MasterActualJobHeader_Guid
                            }).ToList();

            return result.Select(e =>
            {
                string currency = "";
                if (validateRunLiabilityLimit)
                {
                    currency = defaultCurrencyApp;
                }
                else
                {
                    var count = checkOneCurrency.Count(c => c.JobGuid == e.Key.JobHeaderGuid);
                    if (count == 1)
                    {
                        currency = currencyFromLiability.FirstOrDefault(f => f.JobGuid == e.Key.JobHeaderGuid).MasterCurrencyAbbreviation;
                    }
                    else if (count > 1)
                    {
                        currency = "*";
                    }
                    else
                    {
                        currency = defaultCurrencyApp;
                    }
                }
                return new JobUnassignedView()
                {
                    JobGuid = e.Key.JobLegGuid,
                    Action = e.Key.Action,
                    CustomerLocationDisplay = e.Key.CustomerFullName + " - " + e.Key.BranchName,
                    JobNo = e.Key.JobNo,
                    LOBFullName = e.Key.LOBFullName,
                    OptimizationStatus = e.Key.RouteOptimizationStatusName,
                    OptimizationStatusId = e.Key.RouteOptimizationStatusID,
                    ServiceJobTypeAbbr = e.Key.ServiceJobTypeAbbr,
                    Currency = currency,
                    STC = (decimal)(new RawJobDataView()
                    {
                        Target_CurrencyGuid = defaultCurrency.MasterCurrency_Default_Guid,
                        JobItems = new RawItemsView()
                        {
                            Commodities = Commudity.Where(o => o.JobGuid == e.Key.JobHeaderGuid),
                            Liabilities = ItemLiability.Where(o => o.JobGuid == e.Key.JobHeaderGuid)
                        }
                    }).CalculateJobSTC(currentExchange, Template, validateRunLiabilityLimit).TotalJobSTC


                };
            });
        }

        public IEnumerable<MasterRouteOptimizationViewModel> GetRouteOptimizeByRequest(Guid requestGuid, int maxrow, Guid? langquageGuid)
        {
            List<MasterRouteOptimizationViewModel> result = new List<MasterRouteOptimizationViewModel>();
            var tranHeader = DbContext.TblTransactionRouteOptimizationHeader.Find(requestGuid);
            var displayStatus = GetSystemRouteOptimizeDisplayByGuid(tranHeader.SystemRouteOptimizationStatus_Guid, GetDefultLangquage(langquageGuid));
            foreach (var d in tranHeader.TblTransactionRouteOptimizationHeader_Detail)
            {
                int stopCount = d.TblTransactionRouteOptimizationHeader_Detail_Item.GroupBy(g => g.SequenceBefore).Count();
                int totalLocation = d.TblTransactionRouteOptimizationHeader_Detail_Item.GroupBy(g => g.MasterCustomerLocation_Guid).Count();
                int totalJob = d.TblTransactionRouteOptimizationHeader_Detail_Item.GroupBy(g => g.TblMasterRouteJobServiceStopLegs.MasterRouteJobHeader_Guid).Count();
                var item = new MasterRouteOptimizationViewModel
                {
                    MasterRouteGroupDetailGuid = (Guid)d.MasterRouteGroupDetail_Guid,
                    RouteGroupDetail = d.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName,
                    RouteGroup = DbContext.TblMasterRouteGroup.FirstOrDefault(f => f.Guid == d.TblMasterRouteGroup_Detail.MasterRouteGroup_Guid).MasterRouteGroupName,
                    Stops = stopCount,
                    StopsStr = stopCount.ToString(),
                    Locations = totalLocation,
                    LocationsStr = totalLocation.ToString(),
                    Jobs = totalJob,
                    JobsStr = totalJob.ToString(),
                    OptimizationStatus = displayStatus,
                    OptimizationStatusID = tranHeader.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID
                };
                result.Add(item);
            }
            return result.OrderBy(o => o.RouteGroup).ThenBy(t => t.RouteGroupDetail);
        }

        public IEnumerable<RequestManagementView> GetOptimizeRequestMangement(RequestManagementRequest request, Guid? langquageGuid)
        {
            langquageGuid = GetDefultLangquage(langquageGuid);
            bool selectedAllSite = request.StatusGuid.IsNullOrEmpty();
            bool selectedAllRouteType = request.RouteTypeGuid.IsNullOrEmpty();
            var statusDisplayText = GetSystemRouteOptimizeDisplay((Guid)langquageGuid);
            var routeTypeDisplayText = GetSystemRouteOptimizeRouteTypeDisplay((Guid)langquageGuid);
            var requestTypeDisplayText = GetSystemRouteOptimizeRequestTypeDisplay((Guid)langquageGuid);
            var data = DbContext.TblTransactionRouteOptimizationHeader
                        .Where(w => w.MasterSite_Guid == request.SiteGuid
                            && (w.SystemRouteOptimizationStatus_Guid == request.StatusGuid || selectedAllSite)
                            && (w.SystemRouteOptimizationRouteType_Guid == request.RouteTypeGuid || selectedAllRouteType))
                        .Take(request.MaxRow)
                        .AsEnumerable()
                        .Select(s =>
                            {
                                string statusDisplay = statusDisplayText.FirstOrDefault(f => f.Key == s.SystemRouteOptimizationStatus_Guid).Value;
                                string routeTypeDisplay = routeTypeDisplayText.FirstOrDefault(f => f.Key == s.SystemRouteOptimizationRouteType_Guid).Value;
                                string requestTypeDisplay = $@"{s.TblSystemRouteOptimizationRequestType.RouteOptimizationRequestTypeCode}-{ requestTypeDisplayText.FirstOrDefault(f => f.Key == s.SystemRouteOptimizationRequestType_Guid).Value}";
                                return new RequestManagementView
                                {
                                    Guid = s.Guid,
                                    RequestID = s.RequestID,
                                    Status = statusDisplay,
                                    StatusID = s.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID,
                                    RouteType = routeTypeDisplay,
                                    RouteTypeCode = s.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode,
                                    RequestType = requestTypeDisplay,
                                    RequestUser = s.UserCreated,
                                    RequestDateTime = s.DatetimeCreated
                                };
                            });
            return data;
        }
        /// <summary>
        /// get detail master route
        /// </summary>
        /// <param name="request"></param>
        /// <param name="langquageGuid"></param>
        /// <returns></returns>
        public MasterRouteRequestDetailResponse GetOptimizeRequestMangementRmDetail(Guid requestGuid, Guid? langquageGuid, string formatDate)
        {
            MasterRouteRequestDetailResponse reusult = null;
            langquageGuid = GetDefultLangquage(langquageGuid);
            var data = DbContext.TblTransactionRouteOptimizationHeader.FirstOrDefault(w => w.Guid == requestGuid);
            if (data != null)
            {
                var statusDisplay = GetSystemRouteOptimizeDisplayByGuid(data.SystemRouteOptimizationStatus_Guid, (Guid)langquageGuid);
                var requestTypeDisplay = GetSystemRouteOptimizeRequestTypeDisplayByGuid(data.SystemRouteOptimizationRequestType_Guid, (Guid)langquageGuid);

                reusult = new MasterRouteRequestDetailResponse
                {
                    Guid = data.Guid,
                    SiteName = string.Format("{0} - {1}", data.TblMasterSite.SiteCode, data.TblMasterSite.SiteName),
                    RequestType = requestTypeDisplay,
                    RequestID = data.RequestID,
                    StatusID = data.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID,
                    Status = statusDisplay,
                    RequestUser = data.RequestUser,
                    RequestDatetime = (DateTime)data.RequestDatetime,
                    RequestDatetimeStr = data.RequestDatetime.ChangeFromDateToString(formatDate),
                    DayOfWeek = DbContext.TblSystemDayOfWeek.FirstOrDefault(f => f.Guid == data.TblMasterRoute.MasterDayOfweek_Guid).MasterDayOfWeek_Name,
                    FlagHoliday = (bool)data.TblMasterRoute.FlagHoliday,
                    Week = data.TblMasterRoute.TblSystemMaterRouteTypeOfWeek.WeekTypeName,
                    MasterRouteName = data.TblMasterRoute.MasterRouteName,
                    CancelUser = data.RequestUser,
                    CancelDatetime = data.RequestDatetime,
                    CancelDatetimeStr = data.RequestDatetime.ChangeFromDateToString(formatDate),
                    CompleteDatetime = data.CompleteDatetime,
                    CompleteDatetimeStr = data.CompleteDatetime.ChangeFromDateToString(formatDate),




                };
            }

            return reusult;
        }

        public DailyRouteRequestDetailResponse GetOptimizeRequestMangementRDDetail(Guid requestGuid, Guid? langquageGuid, string formatDate)
        {
            DailyRouteRequestDetailResponse reusult = null;
            langquageGuid = GetDefultLangquage(langquageGuid);
            var data = DbContext.TblTransactionRouteOptimizationHeader.FirstOrDefault(w => w.Guid == requestGuid);
            if (data != null)
            {
                var statusDisplay = GetSystemRouteOptimizeDisplayByGuid(data.SystemRouteOptimizationStatus_Guid, (Guid)langquageGuid);
                var requestTypeDisplay = GetSystemRouteOptimizeRequestTypeDisplayByGuid(data.SystemRouteOptimizationRequestType_Guid, (Guid)langquageGuid);
                var RunGuid = data.TblTransactionRouteOptimizationHeader_Detail.FirstOrDefault(f => f.MasterDailyRunResource_Guid.HasValue).MasterDailyRunResource_Guid;
                var workDate = DbContext.TblMasterDailyRunResource.Where(e => e.Guid == RunGuid).Select(e => e.WorkDate);

                reusult = new DailyRouteRequestDetailResponse
                {
                    WorkDate = workDate.FirstOrDefault().Value,
                    Guid = data.Guid,
                    SiteName = string.Format("{0} - {1}", data.TblMasterSite.SiteCode, data.TblMasterSite.SiteName),
                    RequestTypeGuid = data.SystemRouteOptimizationRequestType_Guid,
                    RequestType = requestTypeDisplay,
                    RequestID = data.RequestID,
                    StatusID = data.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID,
                    Status = statusDisplay,
                    RequestUser = data.RequestUser,
                    RequestDatetime = (DateTime)data.RequestDatetime,
                    RequestDatetimeStr = data.RequestDatetime.ChangeFromDateToString(formatDate),
                    CancelledUser = data.RequestUser,
                    CancelUser = data.RequestUser,
                    CancelledDatetime = data.RequestDatetime,
                    CancelDatetime = data.RequestDatetime,
                    CancelDatetimeStr = data.RequestDatetime.ChangeFromDateToString(formatDate),
                    CompletedDatetime = data.CompleteDatetime,
                    CompleteDatetime = data.CompleteDatetime,
                    CompleteDatetimeStr = data.CompleteDatetime.ChangeFromDateToString(formatDate),
                };
            }

            return reusult;
        }

        #region GetData For Generate File
        public RoadnetFileViewModel GetOptimizeRequestGenerateFile()
        {
            RoadnetFileViewModel result = null;
            RoadNetExchangeTemplateView templateCurrencyExchange = new RoadNetExchangeTemplateView();
            string keyName = EnumHelper.GetDescription(RoadNetKeyFileName.OptAdvReq);
            var dayOfweek = DbContext.TblSystemDayOfWeek.Where(w => !w.FlagDisable).Select(s => s).ToList();
            var allData = DbContext.TblTransactionRouteOptimizationHeader_Detail_Item
                        .Where(w => w.TblTransactionRouteOptimizationHeader_Detail
                                        .TblTransactionRouteOptimizationHeader
                                        .TblTransactionRouteOptimizationHeader_Queue.Any(a => !a.FlagComplete
                                        && a.FileFormat == keyName)
                                        )
                        .OrderBy(o => o.DatetimeCreated);
            if (allData.Any())
            {
                result = new RoadnetFileViewModel();
                var date = DateTime.Now;
                var strFirstDow = DateTimeHelper.FirstDayOfWeek(date).ToString("yyyy-MM-dd");
                var strLastDow = DateTimeHelper.LastDayOfWeek(date).ToString("yyyy-MM-dd");
                var item = allData.ToList();
                var listStateAndCity = GetStateAndCityByCustomerLocationList(item.Select(e => e.MasterCustomerLocation_Guid.Value).ToList());

                List<ActualJobDetailViewModel> jobHeader = null;
                var actualJob = item.Where(w => w.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader
                                                    .TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode == OptimizationRouteTypeID.RD)
                                .Select(s => new CommodityRequestModel
                                {
                                    JobHeaderGuid = (Guid)s.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid,
                                }).Distinct();
                if (actualJob.Any())
                {
                    var actualJobHeadDisticnt = actualJob.GroupBy(g => g.JobHeaderGuid).Select(s => s.Key).ToList();
                    jobHeader = DbContext.TblMasterActualJobHeader.Where(w => actualJobHeadDisticnt.Any(a => a == w.Guid))
                                .Select(s => new ActualJobDetailViewModel
                                {
                                    JobHeaderGuid = s.Guid,
                                    JobNo = s.JobNo
                                }).ToList();
                }

                result.RouteOptimizeRequestIdAndCountry = item.GroupBy(g => new { g.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.Guid, g.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.RequestID, g.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.TblMasterSite.MasterCountry_Guid, SiteGuid = g.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.TblMasterSite.Guid })
                                                           .Select(s => new RouteOptimizeRequestIdAndCountry
                                                           {
                                                               RequestGuid = s.Key.Guid,
                                                               RequestId = s.Key.RequestID,
                                                               CountryGuid = s.Key.MasterCountry_Guid,
                                                               SiteGuid = s.Key.SiteGuid
                                                           });


                //Get Data for STC
                #region Pre - STC             
                List<CurrencyOfDailyRunView> currencyOnRun = new List<CurrencyOfDailyRunView>();
                List<ItemsLibilityView> itemLiability = new List<ItemsLibilityView>();
                List<RoadNetCurrencyExchangeConfig> configTruckLimit = new List<RoadNetCurrencyExchangeConfig>();
                List<ItemsCommodityView> Commudity = new List<ItemsCommodityView>();
                List<RoadNetCurrencyOnRunresourceView> masterRunResource = new List<RoadNetCurrencyOnRunresourceView>();
                List<DefaultUserCurrencyView> usersCurrency = new List<DefaultUserCurrencyView>();
                List<RoadNetCurrencyJobView> currencyFromLiability = new List<RoadNetCurrencyJobView>();
                var requestTypeRd = item.Where(w => w.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader
                                                    .TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode == OptimizationRouteTypeID.RD)?.ToList();
                if (requestTypeRd.Any())
                {

                    var countryAndSite = requestTypeRd
                           .GroupBy(g => new
                           {
                               g.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.TblMasterSite.MasterCountry_Guid,
                               g.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.MasterSite_Guid
                           })
                           .Select(s => new RoadNetCountryAndSiteRequest
                           {
                               CountryGuid = s.Key.MasterCountry_Guid,
                               SiteGuid = s.Key.MasterSite_Guid
                           })?.ToList();
                    templateCurrencyExchange = GetTemplateCurrencyExchange(countryAndSite);

                    var jobGuidList = actualJob.GroupBy(g => g.JobHeaderGuid).Select(s => s.Key).ToList();
                    itemLiability = DbContext.TblMasterActualJobItemsLiability.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid.Value))
                                       .Select(o => new ItemsLibilityView
                                       {
                                           DocCurrencyGuid = o.MasterCurrency_Guid,
                                           Liability = o.Liability ?? 0,
                                           LibilityGuid = o.Guid,
                                           JobGuid = o.MasterActualJobHeader_Guid
                                       })?.ToList();

                    var jobUnassign = requestTypeRd.Where(w => w.TblTransactionRouteOptimizationHeader_Detail.MasterDailyRunResource_Guid.IsNullOrEmpty());
                    if (jobUnassign.Any())
                    {
                        var user = jobUnassign.GroupBy(g => g.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.RequestUser).Select(s => s.Key);
                        usersCurrency = DbContext.TblMasterUser.Where(w => !w.FlagDisable && user.Contains(w.UserName))
                                        .Join(DbContext.TblMasterCurrency.Where(w => !w.FlagDisable)
                                        , u => u.MasterCurrency_Default_Guid.Value
                                        , c => c.Guid
                                        , (u, c) => new DefaultUserCurrencyView
                                        {
                                            UserName = u.UserName,
                                            CurrencyAbb = c.MasterCurrencyAbbreviation,
                                            CurrencyGuid = c.Guid
                                        })?.ToList();
                    }

                    masterRunResource = (from rd in requestTypeRd.Where(w => !w.TblTransactionRouteOptimizationHeader_Detail.MasterDailyRunResource_Guid.IsNullOrEmpty())
                                       .Select(s => new
                                       {
                                           s.TblTransactionRouteOptimizationHeader_Detail.TblMasterDailyRunResource
                                       })
                                         join r in DbContext.TblMasterRunResource.Where(w => w.FlagDisable != true)
                                            on rd.TblMasterDailyRunResource.MasterRunResource_Guid equals r.Guid
                                         select new RoadNetCurrencyOnRunresourceView
                                         {
                                             DailyRunGuid = rd.TblMasterDailyRunResource.Guid,
                                             CurrencyAbb = r.TblMasterCurrency.MasterCurrencyAbbreviation,
                                             CurrencyGuid = r.TblMasterCurrency.Guid
                                         })?.ToList();
                    var countryList = countryAndSite.GroupBy(g => g.CountryGuid)
                        .Select(s => s.Key);
                    configTruckLimit = FindFlagValidateRunLiabilityLimit(countryList, EnumAppKey.FlagValidateRunLiabilityLimit);
                    foreach (var i in configTruckLimit)
                    {
                        if (!i.FlagValidateRunLiabilityLimit)
                        {
                            var tranItem = requestTypeRd.Where(w => //!w.TblTransactionRouteOptimizationHeader_Detail.MasterRouteGroupDetail_Guid.IsNullOrEmpty() && 
                            w.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader.TblMasterSite.MasterCountry_Guid == i.CountryGuid)?.ToList();
                            var jobInCountry = tranItem
                                           .Join(itemLiability
                                           , j => j.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid
                                           , l => l.JobGuid
                                           , (j, l) => new
                                           {
                                               Liability = l
                                           })?.ToList();
                            var checkOneCurrency = jobInCountry.Where(w => w.Liability.DocCurrencyGuid.HasValue)
                                .GroupBy(g => new { g.Liability.JobGuid, g.Liability.DocCurrencyGuid }).Select(s => new
                                {
                                    JobGuid = s.Key.JobGuid.Value,
                                    DocCurrencyGuid = s.Key.DocCurrencyGuid.Value
                                })?.ToList();
                            currencyFromLiability = checkOneCurrency
                                   .Join(DbContext.TblMasterCurrency
                                   , cc => cc.DocCurrencyGuid
                                   , c => c.Guid,
                                   (cc, c) =>
                                   new RoadNetCurrencyJobView
                                   {
                                       JobGuid = cc.JobGuid,
                                       CurrencyAbb = c.MasterCurrencyAbbreviation,
                                       CurrencyGuid = c.Guid
                                   })?.ToList();

                            ////Group by currency by run
                            var jobOnRun = tranItem.Where(w => !w.TblTransactionRouteOptimizationHeader_Detail.MasterDailyRunResource_Guid.IsNullOrEmpty());
                            if (jobOnRun.Any())
                            {
                                var dr = jobOnRun
                                    .GroupBy(g => new
                                    {
                                        JobHeaderGuid = g.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid,
                                        DailyGuid = g.TblTransactionRouteOptimizationHeader_Detail.MasterDailyRunResource_Guid
                                    })
                                    .Select(s => new
                                    {
                                        JobHeaderGuid = s.Key.JobHeaderGuid,
                                        DailyGuid = s.Key.DailyGuid
                                    })?.ToList();

                                var dataOnRun = (from r in dr
                                                 join c in currencyFromLiability on r.JobHeaderGuid equals c.JobGuid
                                                 group new { r, c } by new { r.DailyGuid, c.CurrencyAbb, c.CurrencyGuid } into grc
                                                 where grc.Any()
                                                 select new CurrencyOfDailyRunView
                                                 {
                                                     DailyRunGuid = grc.Key.DailyGuid.Value,
                                                     CurrencyAbb = grc.Key.CurrencyAbb,
                                                     ValidateRunLiabilityLimit = i.FlagValidateRunLiabilityLimit,
                                                     CurrencyGuid = grc.Key.CurrencyGuid
                                                 }
                                    );
                                currencyOnRun.AddRange(dataOnRun);
                            }
                        }
                    }
                    Commudity = DbContext.TblMasterActualJobItemsCommodity.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid.Value))
                                   .Select(o => new ItemsCommodityView
                                   {
                                       ActualCommodityGuid = o.Guid,
                                       CommodityGuid = o.MasterCommodity_Guid,
                                       FlagCommodityDiscrepancies = o.FlagCommodityDiscrepancies ?? false,
                                       Quantity = o.Quantity ?? 0,
                                       QuantityActual = o.QuantityActual ?? 0,
                                       QuantityExpected = o.QuantityExpected ?? 0,
                                       JobGuid = o.MasterActualJobHeader_Guid
                                   })?.ToList();
                }
                #endregion
                var masterRouteGrupGuid = item.Where(w => w.TblTransactionRouteOptimizationHeader_Detail.MasterRouteGroupDetail_Guid.HasValue)
                    .Select(s => s.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroup_Guid).Distinct();
                var listMasterRouteGroup = DbContext.TblMasterRouteGroup.Where(w => masterRouteGrupGuid.Contains(w.Guid))?.ToList();
                var locHasPlace = item.Where(w => w.TblMasterCustomerLocation.MasterPlace_Guid.HasValue).Select(s => s.TblMasterCustomerLocation.MasterPlace_Guid);
                var placedata = DbContext.TblMasterPlace.Where(w => locHasPlace.Contains(w.Guid) && w.FlagDisable != true);
                result.FileData = item.OrderBy(o => o.RequestItemID).Select(s =>
                      {
                          string masterRouteGroup = s.TblTransactionRouteOptimizationHeader_Detail.MasterRouteGroupDetail_Guid.IsNullOrEmpty() ? "" : listMasterRouteGroup.FirstOrDefault(f => f.Guid == s.TblTransactionRouteOptimizationHeader_Detail.TblMasterRouteGroup_Detail.MasterRouteGroup_Guid)?.MasterRouteGroupName;
                          var header = s.TblTransactionRouteOptimizationHeader_Detail.TblTransactionRouteOptimizationHeader;
                          RoadnetFileDataModel innerModel = new RoadnetFileDataModel(); ;
                          if (header.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode == OptimizationRouteTypeID.RM)
                          {
                              string dayOfweekName = dayOfweek.FirstOrDefault(f => f.Guid == header.TblMasterRoute.MasterDayOfweek_Guid).MasterDayOfWeek_Name;
                              innerModel = SetRoadnetTypeRm(new RoadNetBuildFileRequest
                              {
                                  DayOfweek = dayOfweekName,
                                  StrFirstDow = strFirstDow,
                                  StrLastDow = strLastDow,
                                  MasterRouteGroupName = masterRouteGroup,
                                  ListStateAndCity = listStateAndCity,
                                  OptimizeDetailItem = s
                              });
                          }
                          else if (header.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode == OptimizationRouteTypeID.RD)
                          {
                              var countryGuid = header.TblMasterSite.MasterCountry_Guid;
                              RoadNetCurrencyExchangeRateModel exchangeData = null;
                              var tranDate = (DateTime)s.TblMasterActualJobServiceStopLegs.ServiceStopTransectionDate;
                              var dayOfweekName = dayOfweek.FirstOrDefault(f => (int)f.MasterDayOfWeek_Sequence == ((int)tranDate.DayOfWeek + 1)).MasterDayOfWeek_Name;
                              var jobHeaderGuid = s.TblMasterActualJobServiceStopLegs.MasterActualJobHeader_Guid;
                              var tlm = configTruckLimit.FirstOrDefault(w => w.CountryGuid == header.TblMasterSite.MasterCountry_Guid).FlagValidateRunLiabilityLimit;

                              var dailyRunGuid = s.TblTransactionRouteOptimizationHeader_Detail.MasterDailyRunResource_Guid;

                              // job on daily run

                              if (!dailyRunGuid.IsNullOrEmpty())
                              {

                                  var runCurrency = masterRunResource.FirstOrDefault(f => f.DailyRunGuid == dailyRunGuid);
                                  exchangeData = PrepareDataRoadnetCurrencyExchange(new RoadNetCurrencyExchangeRateRequestModel
                                  {
                                      FlagValidateTruckLimit = tlm,
                                      LiabilityCurrency = currencyOnRun.Where(f => f.DailyRunGuid == dailyRunGuid)
                                                          .Select(
                                                              sl => new RoadNetCurrencyView
                                                              {
                                                                  CurrencyAbb = sl.CurrencyAbb,
                                                                  CurrencyGuid = sl.CurrencyGuid
                                                              }
                                                          ),
                                      DefaultCurrencyGuid = runCurrency.CurrencyGuid,
                                      DefaultCurrencyAbb = runCurrency.CurrencyAbb,
                                  });
                              }
                              else
                              {
                                  var userCurrency = usersCurrency.FirstOrDefault(f => f.UserName == header.RequestUser);
                                  var jobcurrency = currencyFromLiability.Where(w => w.JobGuid == jobHeaderGuid).Select(sl =>
                                       new RoadNetCurrencyView
                                       {
                                           CurrencyAbb = sl.CurrencyAbb,
                                           CurrencyGuid = sl.CurrencyGuid
                                       }).ToList();
                                  var prepareReq = new RoadNetCurrencyExchangeRateRequestModel
                                  {
                                      FlagValidateTruckLimit = tlm,
                                      LiabilityCurrency = jobcurrency,
                                      DefaultCurrencyGuid = userCurrency.CurrencyGuid,
                                      DefaultCurrencyAbb = userCurrency.CurrencyAbb,
                                  };
                                  exchangeData = PrepareDataRoadnetCurrencyExchange(prepareReq);
                                  // unassign
                              }
                              var currentExchange = templateCurrencyExchange.CurrentExchange.Where(w => w.MasterCountry_Guid == countryGuid);
                              var Template = templateCurrencyExchange.Template.Where(w => w.CountryGuid == countryGuid);
                              var itemLib = itemLiability.Where(o => o.JobGuid == jobHeaderGuid);
                              innerModel = SetRoadnetTypeRd(new RoadnetDailyRouteRequestModel
                              {
                                  RouteOptimizeItem = s,
                                  StrDayOfweek = dayOfweekName,
                                  ActualJobHeader = jobHeader.FirstOrDefault(f => f.JobHeaderGuid == jobHeaderGuid),
                                  CurrencyAbb = exchangeData.CurrencyAbb,
                                  Stc = (decimal)(new RawJobDataView()
                                  {
                                      Target_CurrencyGuid = exchangeData.CurrencyGuid,
                                      JobItems = new RawItemsView()
                                      {
                                          Commodities = Commudity.Where(o => o.JobGuid == jobHeaderGuid),
                                          Liabilities = itemLib
                                      }
                                  }).CalculateJobSTC(currentExchange, Template, exchangeData.FlagValidateTrukLimit).TotalJobSTC,
                                  StrFirstDow = strFirstDow,
                                  StrLastDow = strLastDow,
                                  MasterRouteGroupName = masterRouteGroup
                              }, listStateAndCity);
                          }
                          if (s.TblMasterCustomerLocation.MasterPlace_Guid.HasValue)
                          {
                              innerModel.Place_Truck_Stop = placedata.FirstOrDefault(f => f.Guid == s.TblMasterCustomerLocation.MasterPlace_Guid)?.BuildingName ?? "";
                          }
                          return innerModel;
                      }).OrderBy(o => o.MasterRouteGroupDetailName).ThenBy(t => t.Location_Sequence_Original).ToList();

            }

            return result;
        }

        private RoadNetExchangeTemplateView GetTemplateCurrencyExchange(IEnumerable<RoadNetCountryAndSiteRequest> req)
        {
            RoadNetExchangeTemplateView response = new RoadNetExchangeTemplateView();
            foreach (var r in req)
            {
                var currentExchange = GetCurrencyExchangeList(r.CountryGuid)?.ToList();
                response.CurrentExchange.AddRange(currentExchange);
                var Template = DbContext.GetRoadNetAllCommodityBySite(r.SiteGuid, r.CountryGuid)?.ToList();
                response.Template.AddRange(Template);
            }
            return response;
        }
        private RoadNetCurrencyExchangeRateModel PrepareDataRoadnetCurrencyExchange(RoadNetCurrencyExchangeRateRequestModel req)
        {
            var countDataOnRun = req.LiabilityCurrency.Count();
            RoadNetCurrencyExchangeRateModel response = new RoadNetCurrencyExchangeRateModel()
            {
                FlagValidateTrukLimit = req.FlagValidateTruckLimit,
                CurrencyAbb = req.DefaultCurrencyAbb,
                CurrencyGuid = req.DefaultCurrencyGuid
            };
            // Turn off exchange rate.
            if (!req.FlagValidateTruckLimit)
            {
                switch (countDataOnRun)
                {
                    case 1:
                        response.CurrencyAbb = req.LiabilityCurrency.FirstOrDefault().CurrencyAbb;
                        response.CurrencyGuid = req.LiabilityCurrency.FirstOrDefault().CurrencyGuid;
                        break;
                    case 0:
                        // nothing change                   
                        break;
                    default:
                        response.CurrencyAbb = "*";
                        break;
                }
            }
            return response;
        }
        private List<RoadNetCurrencyExchangeConfig> FindFlagValidateRunLiabilityLimit(IEnumerable<Guid> countryGuid, string appkeyName)
        {
            var appKeyGuid = DbContext.TblSystemEnvironmentMasterCountry.FirstOrDefault(w => w.AppKey == appkeyName).Guid;
            var data = countryGuid
                        .Join(DbContext.TblSystemEnvironmentMasterCountryValue.Where(w => w.SystemEnvironmentMasterCountry_Guid == appKeyGuid)
                        , c => c
                        , s => s.MasterCountry_Guid
                        , (c, s) => new RoadNetCurrencyExchangeConfig
                        {
                            CountryGuid = c,
                            FlagValidateRunLiabilityLimit = bool.Parse(s.AppValue1)
                        }
                        )?.ToList();
            return data;
        }
        private readonly Func<string, string> DefualtEmpty = (s) =>
        {
            return s ?? string.Empty;
        };
        /// <summary>
        /// Master route
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dayOfweek"></param>
        /// <returns></returns>
        private RoadnetFileDataModel SetRoadnetTypeRm(RoadNetBuildFileRequest req)
        {

            var detail = req.OptimizeDetailItem.TblTransactionRouteOptimizationHeader_Detail;
            var header = detail.TblTransactionRouteOptimizationHeader;
            var location = req.OptimizeDetailItem.TblMasterCustomerLocation;
            string masterRouteGrupName = !string.IsNullOrEmpty(req.MasterRouteGroupName) ? $@"**{req.MasterRouteGroupName}" : "";
            RoadnetFileDataModel innerModel = new RoadnetFileDataModel
            {
                Request_ID = header.RequestID,
                Request_Item_ID = req.OptimizeDetailItem?.RequestItemID,
                Request_Type = header.TblSystemRouteOptimizationRequestType.RouteOptimizationRequestTypeCode,
                Request_Scope = header.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeName,
                Location_Sequence_Original = DefualtEmpty((req.OptimizeDetailItem.SequenceBefore ?? 0).ToString()),
                Location_Sequence_RoadNet = "",
                Week_Type = header.TblMasterRoute.TblSystemMaterRouteTypeOfWeek.WeekTypeName,
                Site_Code = $@"{header.TblMasterSite.SiteCode}{masterRouteGrupName}",
                Site_Name = header.TblMasterSite.SiteName,
                MasterRouteGroupDetailName = detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName,
                WeekDay = req.DayOfweek,
                Route_Day = $"{detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName}-{req.DayOfweek}",
                Ready_Status = "",
                Balanced_Status = "",
                Route_Start_Time = "",
                StopLocationCode = DefualtEmpty(location.BranchCodeReference),
                StopLocationName = DefualtEmpty(location.BranchName),
                TblMasterCustomerLocation_Guid = location.Guid.ToString(),
                Job_ID = "",
                Job_Type = req.OptimizeDetailItem.TblSystemServiceJobType.ServiceJobTypeNameAbb,
                Stop_Premise_Time = DefualtEmpty(location.PremiseTime.ToString()),
                Job_Premise_Time = "",
                STC = "0.00",
                Place_Truck_Stop = DefualtEmpty(location?.TblMasterPlace?.BuildingName),
                Address = DefualtEmpty(location.Address),
                city = DefualtEmpty(req.ListStateAndCity.FirstOrDefault(e => e.CustomerLocationGuid == location.Guid).City),
                StateName = DefualtEmpty(req.ListStateAndCity.FirstOrDefault(e => e.CustomerLocationGuid == location.Guid).State),
                postal = DefualtEmpty(location.Postcode),
                Latitude = DefualtEmpty(location.Latitude),
                Longitude = DefualtEmpty(location.Longitude),
                WorkDate = "",
                Week_Start_Sunday = req.StrFirstDow,
                Week_End_Saturday = req.StrLastDow,
            };
            return innerModel;
        }

        /// <summary>
        /// DailyRun
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dayOfweek"></param>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        private RoadnetFileDataModel SetRoadnetTypeRd(RoadnetDailyRouteRequestModel req, List<CustomerLocationCityStateModel> ListStateAndCity)
        {
            var dateTimeFormat = "yyyy-MM-dd HH:mm";
            var detail = req.RouteOptimizeItem.TblTransactionRouteOptimizationHeader_Detail;
            var header = detail.TblTransactionRouteOptimizationHeader;
            var location = req.RouteOptimizeItem.TblMasterCustomerLocation;
            string masterRouteGrupName = !string.IsNullOrEmpty(req.MasterRouteGroupName) ? $@"**{req.MasterRouteGroupName}" : "";
            RoadnetFileDataModel innerModel = new RoadnetFileDataModel
            {
                Request_ID = header.RequestID,
                Request_Item_ID = req.RouteOptimizeItem.RequestItemID,
                Request_Type = header.TblSystemRouteOptimizationRequestType.RouteOptimizationRequestTypeCode,
                Request_Scope = header.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeName,
                Location_Sequence_Original = req.RouteOptimizeItem.SequenceBefore.ToString(),
                Location_Sequence_RoadNet = "",
                Week_Type = "",
                Site_Code = $@"{header.TblMasterSite.SiteCode}{masterRouteGrupName}",
                Site_Name = header.TblMasterSite.SiteName,
                WeekDay = req.StrDayOfweek,
                TblMasterCustomerLocation_Guid = location.Guid.ToString(),
                StopLocationCode = DefualtEmpty(location.BranchCodeReference),
                StopLocationName = DefualtEmpty(location.BranchName),
                Job_ID = req.ActualJobHeader.JobNo,
                Job_Type = req.RouteOptimizeItem.TblSystemServiceJobType.ServiceJobTypeNameAbb,
                Stop_Premise_Time = DefualtEmpty(location?.PremiseTime.ToString()),
                Job_Premise_Time = "",
                STC = $@"{req.Stc.ToString("0.00")} {req.CurrencyAbb}",
                Place_Truck_Stop = DefualtEmpty(location.TblMasterPlace?.BuildingName),
                Address = DefualtEmpty(location.Address),
                city = DefualtEmpty(ListStateAndCity.FirstOrDefault(e => e.CustomerLocationGuid == location.Guid).City),
                StateName = DefualtEmpty(ListStateAndCity.FirstOrDefault(e => e.CustomerLocationGuid == location.Guid).State),
                postal = DefualtEmpty(location.Postcode),
                Latitude = DefualtEmpty(location.Latitude),
                Longitude = DefualtEmpty(location.Longitude),
                WorkDate = req.RouteOptimizeItem.TblMasterActualJobServiceStopLegs.ServiceStopTransectionDate.ChangeFromDateToString("yyyy-MM-dd"),
                Week_Start_Sunday = req.StrFirstDow,
                Week_End_Saturday = req.StrLastDow,
            };
            // unassign
            if (detail.MasterDailyRunResource_Guid.IsNullOrEmpty())
            {
                innerModel.MasterRouteGroupDetailName = $"{header.RequestID}-Unassigned";
                innerModel.Route_Day = "";
                innerModel.Ready_Status = "";
                innerModel.Route_Start_Time = "";
                innerModel.Balanced_Status = "";

            }
            else // has run resource.
            {
                innerModel.MasterRouteGroupDetailName = detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName;
                innerModel.Route_Day = $"{detail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName}-{req.StrDayOfweek}";
                innerModel.Ready_Status = "Ready";
                innerModel.Route_Start_Time = detail.TblMasterDailyRunResource.WorkDate.GetValueOrDefault().ToString(dateTimeFormat);
                innerModel.Balanced_Status = detail.TblMasterDailyRunResource.FlagRouteBalanceDone ? "Yes" : "No";

            }
            return innerModel;
        }
        #endregion GetData For Generate File       
        private Dictionary<Guid, string> GetSystemRouteOptimizeRouteTypeDisplay(Guid langquageGuid)
        {
            var data = DbContext.TblSystemRouteOptimizationRouteType
                         .Join(
                             DbContext.TblSystemDisplayTextControlsLanguage.Where(w => w.SystemLanguageGuid == langquageGuid)
                             , op => op.SystemDisplayTextControlsLanguage_Guid
                             , dis => dis.Guid
                             , (op, dis) => new { op, dis })
                             .ToDictionary(
                                k => k.op.Guid,
                                v => v.dis.DisplayText
                            );
            return data;
        }
        private Dictionary<Guid, string> GetSystemRouteOptimizeRequestTypeDisplay(Guid langquageGuid)
        {
            var data = DbContext.TblSystemRouteOptimizationRequestType
                         .Join(
                             DbContext.TblSystemDisplayTextControlsLanguage.Where(w => w.SystemLanguageGuid == langquageGuid)
                             , op => op.SystemDisplayTextControlsLanguage_Guid
                             , dis => dis.Guid
                             , (op, dis) => new { op, dis })
                             .ToDictionary(
                                k => k.op.Guid,
                                v => v.dis.DisplayText
                            );
            return data;
        }
        private Dictionary<Guid, string> GetSystemRouteOptimizeDisplay(Guid langquageGuid)
        {
            var data = DbContext.TblSystemRouteOptimizationStatus
                         .Join(
                             DbContext.TblSystemDisplayTextControlsLanguage.Where(w => w.SystemLanguageGuid == langquageGuid)
                             , op => op.SystemDisplayTextControlsLanguage_Guid
                             , dis => dis.Guid
                             , (op, dis) => new { op, dis })
                             .ToDictionary(
                                k => k.op.Guid,
                                v => v.dis.DisplayText
                            );
            return data;
        }
        private string GetSystemRouteOptimizeDisplayByGuid(Guid statusGuid, Guid langquageGuid)
        {
            var displayGuid = DbContext.TblSystemRouteOptimizationStatus.Find(statusGuid).SystemDisplayTextControlsLanguage_Guid;
            return DbContext.TblSystemDisplayTextControlsLanguage.FirstOrDefault(f => f.Guid == displayGuid && f.SystemLanguageGuid == langquageGuid).DisplayText;
        }
        private string GetSystemRouteOptimizeRouteTypeDisplayByGuid(Guid requestTypeGuid, Guid langquageGuid)
        {
            var displayGuid = DbContext.TblSystemRouteOptimizationRouteType.Find(requestTypeGuid).SystemDisplayTextControlsLanguage_Guid;
            return DbContext.TblSystemDisplayTextControlsLanguage.FirstOrDefault(f => f.Guid == displayGuid && f.SystemLanguageGuid == langquageGuid).DisplayText;
        }

        private string GetSystemRouteOptimizeRequestTypeDisplayByGuid(Guid requestTypeGuid, Guid langquageGuid)
        {
            var displayGuid = DbContext.TblSystemRouteOptimizationRequestType.Find(requestTypeGuid).SystemDisplayTextControlsLanguage_Guid;
            return DbContext.TblSystemDisplayTextControlsLanguage.FirstOrDefault(f => f.Guid == displayGuid && f.SystemLanguageGuid == langquageGuid).DisplayText;
        }
        private Guid GetDefultLangquage(Guid? langquageGuid)
        {
            if (langquageGuid.IsNullOrEmpty())
            {
                langquageGuid = DbContext.TblSystemLanguage.First(f => f.Abbreviation == "EN-US" && f.FlagDisable == false).Guid;
            }
            return (Guid)langquageGuid;
        }

        private List<CustomerLocationCityStateModel> GetStateAndCityByCustomerLocationList(List<Guid> CustomerLocationGuid)
        {
            var result = (from Location in DbContext.TblMasterCustomerLocation
                          join Customer in DbContext.TblMasterCustomer on Location.MasterCustomer_Guid equals Customer.Guid
                          join Country in DbContext.TblMasterCountry on Customer.MasterCountry_Guid equals Country.Guid
                          join State in DbContext.TblMasterCountry_State on Location.MasterCountry_State_Guid equals State.Guid
                          into LeftState
                          from State in LeftState.DefaultIfEmpty()
                          join City in DbContext.TblMasterCity on Location.MasterCity_Guid equals City.Guid
                          into LeftCity
                          from City in LeftCity.DefaultIfEmpty()
                          join District in DbContext.TblMasterDistrict on Location.MasterDistrict_Guid equals District.Guid
                          into LeftDistrict
                          from District in LeftDistrict.DefaultIfEmpty()
                          select new
                          {
                              Location.Guid,
                              Country.FlagHaveState,
                              Country.FlagInputCityManual,
                              State.MasterStateName,
                              Location.StateName,
                              Location.CitryName,
                              District.MasterDistrictName,
                              City.MasterCityName,
                          }).Select(e => new CustomerLocationCityStateModel()
                          {
                              CustomerLocationGuid = e.Guid,
                              State = e.FlagHaveState.Value ? e.MasterStateName : e.FlagInputCityManual.Value ? e.StateName : e.MasterCityName,
                              City = e.FlagInputCityManual.Value ? e.CitryName : e.MasterDistrictName
                          }).ToList();

            return result;
        }



        /// <summary>
        /// function นี้ทำมาเพื่อเช็คว่า ข้อมูลที่ส่งมา
        /// ActualJobHeaderGuid จะเช็คว่าตัวงานนั้นติด lock roadnet หรือไม่ (join jobheader and jobleg แล้วดู optimizeStatus)
        /// JobLegGuid เหมือน actualjobhead แค่รับ jobleg แทน
        /// RunResourceGuid เช็ค Daily Run Resource เช็ค optimizestatus โดยตรง
        /// MasterRouteModel เป็น model เก็บค่า master route guid กับ Route group detail guid ไว้เช็ค master route ว่า lock roadnet ไหม 
        /// CustomerGuid เช็คทั้ง Daily run และ master Route ส่วนใหญ่น่าจะเป็นของ standard table
        /// CustomerLocationGuid เช็คทั้ง Daily run และ master Route ส่วนใหญ่น่าจะเป็นของ standard table
        /// RouteGroupGuid เช็คทั้ง Daily run และ master Route ส่วนใหญ่น่าจะเป็นของ standard table
        /// RouteGroupDetailGuid เช็คทั้ง Daily run และ master Route ส่วนใหญ่น่าจะเป็นของ standard table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RouteOptimizeSearchResultModel GetRouteGroupHasOptimize(RouteOptimizeSearchModel request)
        {
            RouteOptimizeSearchResultModel result = new RouteOptimizeSearchResultModel();
            int[] validateStatus = new int[] {
               (int) OptimizationStatusID.REQUESTING,
               (int) OptimizationStatusID.INPROGRESS,
               (int) OptimizationStatusID.CANCELING
            };

            var getStatusGuid = DbContext.TblSystemRouteOptimizationStatus.Where(e => validateStatus.Contains(e.RouteOptimizationStatusID)).Select(e => e.Guid).ToList();

            #region RunResouce
            if (request.RunResourceGuid.Any())
            {
                var DayRoute = (from DayRun in DbContext.TblMasterDailyRunResource.Where(e => getStatusGuid.Contains(e.SystemRouteOptimizationStatus_Guid.Value))
                                join MasRun in DbContext.TblMasterRunResource on DayRun.MasterRunResource_Guid equals MasRun.Guid
                                where request.RunResourceGuid.Contains(DayRun.Guid)
                                select new { MasRun.VehicleNumber, DayRun.Guid });

                var union = string.Join(",", DayRoute.Select(e => e.VehicleNumber));
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "RunResource";
                    result.returnMessage = union;
                    result.LockedGuid = DayRoute.Select(e => e.Guid).ToList();
                    result.FlaglockProcess = true;
                    return result;
                }
            }

            if (request.ActualJobHeaderGuid.Any())
            {
                var DayRoute = (from JobLeg in DbContext.TblMasterActualJobServiceStopLegs.Where(e => getStatusGuid.Contains(e.SystemRouteOptimizationStatus_Guid.Value))
                                join JobHead in DbContext.TblMasterActualJobHeader on JobLeg.MasterActualJobHeader_Guid equals JobHead.Guid
                                where request.ActualJobHeaderGuid.Contains(JobHead.Guid)
                                select JobHead.JobNo);

                var union = string.Join(",", DayRoute.Select(e => e));
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "ActualJobHeader";
                    result.returnMessage = union;
                    result.FlaglockProcess = true;
                    return result;
                }
            }

            if (request.JobLegGuid.Any())
            {
                var DayRoute = (from JobLeg in DbContext.TblMasterActualJobServiceStopLegs.Where(e => getStatusGuid.Contains(e.SystemRouteOptimizationStatus_Guid.Value))
                                join JobHead in DbContext.TblMasterActualJobHeader on JobLeg.MasterActualJobHeader_Guid equals JobHead.Guid
                                where request.JobLegGuid.Contains(JobLeg.Guid)
                                select JobHead.JobNo).Distinct();

                var union = string.Join(",", DayRoute.Select(e => e));
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "ActualJobHeader";
                    result.returnMessage = union;
                    result.FlaglockProcess = true;
                    return result;
                }
            }
            #endregion

            #region Standard Table
            if (request.CustomerLocationGuid.Any())
            {
                var DayRoute = (from JobLeg in DbContext.TblMasterActualJobServiceStopLegs.Where(e => getStatusGuid.Contains(e.SystemRouteOptimizationStatus_Guid.Value))
                                join Location in DbContext.TblMasterCustomerLocation on JobLeg.MasterCustomerLocation_Guid equals Location.Guid
                                where request.CustomerLocationGuid.Contains(Location.Guid)
                                select Location.BranchName);

                var MasRoute = (from Map in DbContext.TblMasterRoute_RouteGroupDetailMonitoring_Mapping
                                join Status in DbContext.TblMasterRoute_OptimizationStatus on new { A = Map.MasterRoute_Guid, B = Map.MasterRouteGroup_Detail_Guid.Value } equals new { A = Status.MasterRoute_Guid, B = Status.MasterRouteGroupDetail_Guid }
                                join MasLeg in DbContext.TblMasterRouteJobServiceStopLegs on Map.MasterRoute_Guid equals MasLeg.MasterRouteJobHeader_Guid
                                join Location in DbContext.TblMasterCustomerLocation on MasLeg.MasterCustomerLocation_Guid equals Location.Guid
                                where getStatusGuid.Contains(Status.SystemRouteOptimizationStatus_Guid) && request.CustomerLocationGuid.Contains(Location.Guid)
                                select Location.BranchName);

                var union = string.Join(",", DayRoute.Union(MasRoute).Distinct().Select(e => e));
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "CustomerLocation";
                    result.returnMessage = union;
                    result.FlaglockProcess = true;
                    return result;
                }
            }

            if (request.CustomerGuid.Any())
            {
                var DayRoute = (from JobLeg in DbContext.TblMasterActualJobServiceStopLegs.Where(e => getStatusGuid.Contains(e.SystemRouteOptimizationStatus_Guid.Value))
                                join Location in DbContext.TblMasterCustomerLocation on JobLeg.MasterCustomerLocation_Guid equals Location.Guid
                                join Customer in DbContext.TblMasterCustomer on Location.MasterCustomer_Guid equals Customer.Guid
                                where request.CustomerGuid.Contains(Location.MasterCustomer_Guid)
                                select Customer.CustomerFullName);

                var MasRoute = (from Map in DbContext.TblMasterRoute_RouteGroupDetailMonitoring_Mapping
                                join Status in DbContext.TblMasterRoute_OptimizationStatus on new { A = Map.MasterRoute_Guid, B = Map.MasterRouteGroup_Detail_Guid.Value } equals new { A = Status.MasterRoute_Guid, B = Status.MasterRouteGroupDetail_Guid }
                                join MasLeg in DbContext.TblMasterRouteJobServiceStopLegs on Map.MasterRoute_Guid equals MasLeg.MasterRouteJobHeader_Guid
                                join Location in DbContext.TblMasterCustomerLocation on MasLeg.MasterCustomerLocation_Guid equals Location.Guid
                                join Customer in DbContext.TblMasterCustomer on Location.MasterCustomer_Guid equals Customer.Guid
                                where getStatusGuid.Contains(Status.SystemRouteOptimizationStatus_Guid) && request.CustomerGuid.Contains(Location.MasterCustomer_Guid)
                                select Customer.CustomerFullName);

                var union = string.Join(",", DayRoute.Union(MasRoute).Distinct().Select(e => e));
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "Customer";
                    result.returnMessage = union;
                    result.FlaglockProcess = true;
                    return result;
                }
            }

            if (request.RouteGroupGuid.Any())
            {
                var DayRoute = (from DayRun in DbContext.TblMasterDailyRunResource.Where(e => getStatusGuid.Contains(e.SystemRouteOptimizationStatus_Guid.Value))
                                join RouteGroupD in DbContext.TblMasterRouteGroup_Detail on DayRun.MasterRouteGroup_Detail_Guid equals RouteGroupD.Guid
                                join Group in DbContext.TblMasterRouteGroup on RouteGroupD.MasterRouteGroup_Guid equals Group.Guid
                                where request.RouteGroupGuid.Contains(RouteGroupD.MasterRouteGroup_Guid)
                                select Group.MasterRouteGroupName);

                var MasRoute = (from Map in DbContext.TblMasterRoute_RouteGroupDetailMonitoring_Mapping
                                join Route in DbContext.TblMasterRoute on Map.MasterRoute_Guid equals Route.Guid
                                join GroupD in DbContext.TblMasterRouteGroup_Detail on Map.MasterRouteGroup_Detail_Guid equals GroupD.Guid
                                join Group in DbContext.TblMasterRouteGroup on GroupD.MasterRouteGroup_Guid equals Group.Guid
                                join Status in DbContext.TblMasterRoute_OptimizationStatus on new { A = Route.Guid, B = GroupD.Guid } equals new { A = Status.MasterRoute_Guid, B = Status.MasterRouteGroupDetail_Guid }
                                where getStatusGuid.Contains(Status.SystemRouteOptimizationStatus_Guid) && request.RouteGroupGuid.Contains(GroupD.MasterRouteGroup_Guid)
                                select Group.MasterRouteGroupName);

                var union = string.Join(",", DayRoute.Union(MasRoute).Distinct().Select(e => e));
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "RouteGroup";
                    result.returnMessage = union;
                    result.FlaglockProcess = true;
                    return result;
                }
            }

            if (request.RouteGroupDetailGuid.Any())
            {
                var DayRoute = (from DayRun in DbContext.TblMasterDailyRunResource.Where(e => request.RouteGroupDetailGuid.Contains(e.MasterRouteGroup_Detail_Guid.Value))
                                join RouteD in DbContext.TblMasterRouteGroup_Detail on DayRun.MasterRouteGroup_Detail_Guid equals RouteD.Guid
                                where getStatusGuid.Contains(DayRun.SystemRouteOptimizationStatus_Guid.Value)
                                select RouteD.MasterRouteGroupDetailName);

                var MasRoute = (from Map in DbContext.TblMasterRoute_RouteGroupDetailMonitoring_Mapping
                                join Status in DbContext.TblMasterRoute_OptimizationStatus on new { A = Map.Guid, B = Map.Guid } equals new { A = Status.MasterRoute_Guid, B = Status.MasterRouteGroupDetail_Guid }
                                join RouteD in DbContext.TblMasterRouteGroup_Detail on Map.MasterRouteGroup_Detail_Guid equals RouteD.Guid
                                where getStatusGuid.Contains(Status.SystemRouteOptimizationStatus_Guid) && request.RouteGroupDetailGuid.Contains(Map.MasterRouteGroup_Detail_Guid.Value)
                                select RouteD.MasterRouteGroupDetailName);

                var union = string.Join(",", DayRoute.Union(MasRoute).Distinct().Select(e => e));
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "RouteGroupDetail";
                    result.returnMessage = union;
                    result.FlaglockProcess = true;
                    return result;
                }
            }
            #endregion

            #region MasterRoute           

            if (request.MasterRouteModel.Any())
            {
                string union = "";
                List<Guid> LockedGuid = new List<Guid>();
                foreach (var item in request.MasterRouteModel.Where(e => e.RouteGroupDetailGuid.Any()))
                {
                    var MasRoute = (from Map in DbContext.TblMasterRoute_RouteGroupDetailMonitoring_Mapping
                                    join Status in DbContext.TblMasterRoute_OptimizationStatus on new { A = Map.MasterRoute_Guid, B = Map.MasterRouteGroup_Detail_Guid.Value } equals new { A = Status.MasterRoute_Guid, B = Status.MasterRouteGroupDetail_Guid }
                                    join RouteD in DbContext.TblMasterRouteGroup_Detail on Map.MasterRouteGroup_Detail_Guid equals RouteD.Guid
                                    where getStatusGuid.Contains(Status.SystemRouteOptimizationStatus_Guid) && Map.MasterRoute_Guid == item.MasterRouteGuid && item.RouteGroupDetailGuid.Contains(Map.MasterRouteGroup_Detail_Guid.Value)
                                    select new { RouteD.MasterRouteGroupDetailName, Map.MasterRouteGroup_Detail_Guid }).ToList();

                    union = union + string.Join(",", MasRoute.Select(e => e.MasterRouteGroupDetailName).Distinct());
                    LockedGuid.AddRange(MasRoute.Select(e => e.MasterRouteGroup_Detail_Guid.Value).Distinct().ToList());
                }
                if (!string.IsNullOrEmpty(union))
                {
                    result.ProcessStep = "MasterRouteAndGroupDetail";
                    result.returnMessage = union;
                    result.LockedGuid = LockedGuid;
                    result.FlaglockProcess = true;
                    return result;
                }
            }
            #endregion

            return result;

        }




    }

}
