using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Models.Customer;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Customer;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Xunit;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Service.Test.Util;

namespace Bgt.Ocean.Service.Test.Adhoc
{
    public class AdhocServiceTest
    {
        public class CreateAdhocJob_AddSpot : BaseTest
        {
            private readonly IAdhocService _adhocService;

            public CreateAdhocJob_AddSpot()
            {
                _adhocService = Util.CreateInstance<AdhocService>();
            }

            public static IEnumerable<object[]> AddSpotData()
            {
                Util.CreateFakeContext();
                var ActionPGuid = Guid.NewGuid();
                var ActionDGuid = Guid.NewGuid();
                Guid? dailyRunGuid = Guid.NewGuid();
                var refLocation = Guid.NewGuid();

                ///CASE 1 : re-order from 5 -> 11
                var request_HasNotSame = new CreateJobAdHocRequest
                {
                    flagAddSpot = true,
                    AdhocJobHeaderView = new AdhocJobHeaderRequest { ServiceJobTypeID = 1 },
                    ServiceStopLegPickup = new AdhocLegRequest { jobOrderSpot = 5 },
                    ServiceStopLegDelivery = new AdhocLegRequest { jobOrderSpot = 5 }
                };
                var refStop_HasNotSame = new TblMasterActualJobServiceStopLegs
                {
                    JobOrder = 5,
                    CustomerLocationAction_Guid = ActionPGuid,
                    MasterRunResourceDaily_Guid = dailyRunGuid,
                    MasterCustomerLocation_Guid = refLocation
                };


                var allLegInRun_HasNotSame = new int[] { 1, 2, 3, 5, 6, 7, 8, 9, 10, 11 }.Select((i) =>
                         new TblMasterActualJobServiceStopLegs
                         {
                             JobOrder = i,
                             CustomerLocationAction_Guid = ActionPGuid,
                             MasterRunResourceDaily_Guid = dailyRunGuid,
                             MasterCustomerLocation_Guid = Guid.NewGuid()

                         });


                ///CASE 2 : merge only
                var request_HasSame = new CreateJobAdHocRequest
                {
                    flagAddSpot = true,
                    AdhocJobHeaderView = new AdhocJobHeaderRequest { ServiceJobTypeID = 1 },
                    ServiceStopLegPickup = new AdhocLegRequest { jobOrderSpot = 1 },
                    ServiceStopLegDelivery = new AdhocLegRequest { jobOrderSpot = 1 }
                };
                var refStop_HasSame = new TblMasterActualJobServiceStopLegs
                {
                    JobOrder = 1,
                    CustomerLocationAction_Guid = ActionPGuid,
                    MasterRunResourceDaily_Guid = dailyRunGuid,
                    MasterCustomerLocation_Guid = refLocation
                };

                var allLegInRun_HasSame = new int[] { 1, 1, 1, 1, 2, 3, 3, 4, 5, 6 }.Select((i) =>
                    new TblMasterActualJobServiceStopLegs
                    {
                        JobOrder = i,
                        CustomerLocationAction_Guid = ActionPGuid,
                        MasterRunResourceDaily_Guid = dailyRunGuid,
                        MasterCustomerLocation_Guid = i == 1 ? refLocation : Guid.NewGuid()

                    });
                return new List<object[]>
                {
                    new object[] { dailyRunGuid, ActionPGuid, ActionDGuid, request_HasNotSame, refStop_HasNotSame, allLegInRun_HasNotSame, 7 },
                    new object[] { dailyRunGuid, ActionPGuid, ActionDGuid, request_HasSame, refStop_HasSame, allLegInRun_HasSame, 0}
                };

            }

            [Theory]
            [MemberData(nameof(AddSpotData))]
            public void ValidateCreateAdhocJobAddSpot_ShouldWorkingNormally(Guid? dailyRunGuid, Guid ActionPGuid, Guid ActionDGuid, CreateJobAdHocRequest request, TblMasterActualJobServiceStopLegs refStop, IEnumerable<TblMasterActualJobServiceStopLegs> allLegInRun, int expected)
            {

                _adhocService.GetMock<ISystemJobActionsRepository>().Setup(fn => fn.FindByAbbrevaition(JobActionAbb.StrPickUp)).Returns(new TblSystemJobAction { Guid = ActionPGuid });
                _adhocService.GetMock<ISystemJobActionsRepository>().Setup(fn => fn.FindByAbbrevaition(JobActionAbb.StrDelivery)).Returns(new TblSystemJobAction { Guid = ActionDGuid });
                _adhocService.GetMock<IMasterActualJobServiceStopLegsRepository>().Setup(fn => fn.FindByDailyRun(dailyRunGuid)).Returns(allLegInRun);

                _adhocService.InvokeMethod("CreateAdhocAddSpot", new object[] { request, new List<TblMasterActualJobServiceStopLegs>() { refStop } });

                _adhocService.GetMock<IMasterActualJobServiceStopLegsRepository>().Verify(fn => fn.Modify(It.IsAny<TblMasterActualJobServiceStopLegs>()), Times.Exactly(expected));
            }
        }


        public class CreateAdhocJob_MachineCashService : BaseTest
        {
            private readonly IAdhocService _adhocService;

            public CreateAdhocJob_MachineCashService()
            {
                _adhocService = Util.CreateInstance<AdhocService>();
            }

            #region GET
            [Fact]
            public void DailyRunNotUnderAlarm_ResponseNull()
            {
                Guid[] mockDailyRunRequest = { Guid.NewGuid() };
                IEnumerable<Guid> MockDailyRunUnderAlarm = new List<Guid>();
                _adhocService.GetMock<IAlarmHubService>().Setup(fn => fn.IsHasAlarm(It.IsAny<IEnumerable<Guid>>())).Returns(MockDailyRunUnderAlarm);
                var result = _adhocService.CheckDailyRunResourceUnderAlarm(-817, Guid.Empty, mockDailyRunRequest);
                Assert.Null(result);
            }

            [Fact]
            public void DailyRunUnderAlarm_ResponseNotNull()
            {
                Guid[] mockDailyRunRequest = { Guid.NewGuid() };
                IEnumerable<Guid> MockDailyRunUnderAlarm = mockDailyRunRequest;
                _adhocService.GetMock<IAlarmHubService>().Setup(fn => fn.IsHasAlarm(It.IsAny<IEnumerable<Guid>>())).Returns(MockDailyRunUnderAlarm);
                _adhocService.GetMock<ISystemMessageRepository>().Setup(f => f.FindByMsgId(It.IsAny<int>(), It.IsAny<Guid>())).Returns(new Models.TblSystemMessage
                {
                    MsgID = -817
                });
                var result = _adhocService.CheckDailyRunResourceUnderAlarm(-817, Guid.NewGuid(), mockDailyRunRequest);
                Assert.NotNull(result);
                Assert.Equal(-817, result.MsgID);
                Assert.True(result.IsWarning);
                Assert.False(result.IsSuccess);
            }

            public static IEnumerable<object[]> CITDeliveryStatusData()
            {
                Util.CreateFakeContext();
                return new List<object[]>
                        {

                             new object[] { new CreateJobAdHocRequest { AdhocJobHeaderView = new AdhocJobHeaderRequest { ServiceJobTypeID = 11} } },
                             new object[] { new CreateJobAdHocRequest { AdhocJobHeaderView = new AdhocJobHeaderRequest { ServiceJobTypeID = 12} } },
                             new object[] { new CreateJobAdHocRequest { AdhocJobHeaderView = new AdhocJobHeaderRequest { ServiceJobTypeID = 13} } },
                        };
            }
            [Theory]
            [MemberData(nameof(CITDeliveryStatusData))]
            public void CheckCITDeliveryStatus_Return(CreateJobAdHocRequest request)
            {
                var result = (int?)_adhocService.InvokeMethod("CheckCITDeliveryStatus", request);

                switch (request.AdhocJobHeaderView.ServiceJobTypeID)
                {
                    case IntTypeJob.MCS:
                        Assert.True(result == (int?)CITDeliveryStatus.Inprogress);
                        break;
                    default:
                        Assert.Null(result);
                        break;
                }
            }

            #endregion

            #region SET

            [Fact]
            public void ValidateCreateMCSJobHideScreen_ReturnNull()
            {
                var request = new CreateJobAdHocRequest();
                request.ServiceStopLegPickup = CreateDummy<AdhocLegRequest>();
                var headDetail = request.AdhocJobHeaderView = CreateDummy<AdhocJobHeaderRequest>();
                _adhocService.GetMock<IMasterCustomerJobHideScreenRepository>().Setup(fn => fn.GetJobHideScreenConfig(request.ServiceStopLegPickup.CustomerGuid, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid))
                                                                               .Returns(CreateDummy<IEnumerable<CustomerJobHideView>>());

                var ex = AssertEx.NoExceptionThrown<Exception>(() =>
                {
                    _adhocService.InvokeMethod("CreateJobHideScreenConfiguaration", new object[] { request });
                });
                Assert.Null(ex);
            }
            [Fact]
            public void ValidateCreateMCSJobHideScreen_ReturnException()
            {
                var request = new CreateJobAdHocRequest();
                request.ServiceStopLegPickup = CreateDummy<AdhocLegRequest>();
                var headDetail = request.AdhocJobHeaderView = CreateDummy<AdhocJobHeaderRequest>();
                headDetail.JobGuid = null;
                _adhocService.GetMock<IMasterCustomerJobHideScreenRepository>().Setup(fn => fn.GetJobHideScreenConfig(request.ServiceStopLegPickup.CustomerGuid, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid))
                                                                               .Returns(CreateDummy<IEnumerable<CustomerJobHideView>>());

                var ex = AssertEx.NoExceptionThrown<Exception>(() =>
                {
                    _adhocService.InvokeMethod("CreateJobHideScreenConfiguaration", new object[] { request });
                });
                Assert.Null(ex);
            }
            [Fact]
            public void ValidateCreateATMTransaction_ReturnCassetteASC()
            {
                var FlagOrderMCSAscending = true;
                var cassetteMock = CreateDummy<IEnumerable<CassetteModelView>>();

                var request = new CreateJobAdHocRequest();
                request.ServiceStopLegPickup = CreateDummy<AdhocLegRequest>();
                var headDetail = request.AdhocJobHeaderView = CreateDummy<AdhocJobHeaderRequest>();
                _adhocService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(headDetail.BrinkSiteGuid, EnumAppKey.FlagOrderMCSAscending))
                                                                               .Returns(FlagOrderMCSAscending);

                var cassette = (IEnumerable<CassetteModelView>)_adhocService.InvokeMethod("OrderCassetteByFlagOrderMCSAscending", new object[] { cassetteMock, headDetail.BrinkSiteGuid });
                var minSeq = cassette.Min(o => o.CassetteSequence);
                //FlagOrderMCSAscending = true : min seq should be min deno
                var minDeo = cassette.FirstOrDefault(o => o.CassetteSequence == minSeq).DenominationValue;

                foreach (var item in cassette)
                {
                    if (item.CassetteSequence == minSeq) continue;
                    Assert.True(minSeq < item.CassetteSequence && minDeo < item.DenominationValue);
                }
                //must be list
                Assert.True(cassette.GetType() == typeof(List<CassetteModelView>));
                Assert.False(cassette.GetType() == typeof(IEnumerable<CassetteModelView>));
            }
            [Fact]
            public void ValidateCreateATMTransaction_ReturnCassetteNotChange()
            {
                var FlagOrderMCSAscending = false;
                var cassetteMock = CreateDummy<IEnumerable<CassetteModelView>>();

                var request = new CreateJobAdHocRequest();
                request.ServiceStopLegPickup = CreateDummy<AdhocLegRequest>();
                var headDetail = request.AdhocJobHeaderView = CreateDummy<AdhocJobHeaderRequest>();
                _adhocService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(headDetail.BrinkSiteGuid, EnumAppKey.FlagOrderMCSAscending))
                                                                               .Returns(FlagOrderMCSAscending);

                var cassette = (IEnumerable<CassetteModelView>)_adhocService.InvokeMethod("OrderCassetteByFlagOrderMCSAscending", new object[] { cassetteMock, headDetail.BrinkSiteGuid });
                //FlagOrderMCSAscending = false : not re-order cassette
                //must be list
                Assert.True(cassette.GetType() == typeof(List<CassetteModelView>));
                Assert.False(cassette.GetType() == typeof(IEnumerable<CassetteModelView>));
            }


            #endregion

        }

        public class BulkDeposit : BaseTest
        {
            private readonly IAdhocService _adhocService;
            private readonly string PrePareDataBulkCassett = "PrePareDataBulkCassette";
            private readonly string PrePareDataBulkDenomination = "PrePareDataBulkDenomination";
            public BulkDeposit()
            {
                _adhocService = CreateInstance<AdhocService>();
            }

            public static IEnumerable<object[]> DataRequest()
            {
                CreateFakeContext();
                Guid currencyGuid = new Guid("AEF33F4A-173B-47D4-9924-065DDB8A70F0");
                IEnumerable<DenominationOnMachineCassetteView> cassetteData = new List<DenominationOnMachineCassetteView>() {
                    new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetA",
                    Seq = 6
                },   new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetB",
                    Seq = 5
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetC",
                    Seq = 4
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetD",
                    Seq = 3
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetE",
                    Seq = 2
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetF",
                    Seq = 1
                }};

                MachineCashServiceRequest request = new MachineCashServiceRequest()
                {
                    IsAscSequence = false,
                    Cassette = cassetteData,
                    JobHeaderGuid = Guid.NewGuid(),
                    JobLegGuid = Guid.NewGuid(),
                    UserName = "Unit-Test",
                    UniversalDatetime = DateTimeOffset.Now,


                };
                var data = new List<object[]> {
                    new object []{ request }
                };

                return data;
            }
            #region SetCassette
            [Theory]
            [MemberData(nameof(DataRequest))]
            public void CreateOneCassette_ShouldBeOneCassess(MachineCashServiceRequest request)
            {
                request.Cassette = request.Cassette.Where(w => w.Seq == 1);
                var result = (TblMasterActualJobMCSBulkDepositReport)_adhocService.InvokeMethod(PrePareDataBulkCassett, request);

                Assert.Equal(1, result.TblMasterActualJobMCSBulkDepositReportEntry.Count);
                Assert.All(result.TblMasterActualJobMCSBulkDepositReportEntry, a =>
                {
                    Assert.Contains(request.Cassette, i => i.CasseteName == a.CassetteName);
                    Assert.Contains(request.Cassette, i => i.Seq == a.CassetteSequence);
                    Assert.Equal(result.Guid, a.MasterMCSBulkDepositReport_Guid);
                });
            }

            [Theory]
            [MemberData(nameof(DataRequest))]
            public void CreateTwoCassette_ShouldBeTwoCassess(MachineCashServiceRequest request)
            {
                request.Cassette = request.Cassette.OrderBy(o => o.Seq).Take(2);
                var result = (TblMasterActualJobMCSBulkDepositReport)_adhocService.InvokeMethod(PrePareDataBulkCassett, request);

                Assert.Equal(2, result.TblMasterActualJobMCSBulkDepositReportEntry.Count);
                Assert.All(result.TblMasterActualJobMCSBulkDepositReportEntry, a =>
                {
                    Assert.Contains(request.Cassette, i => i.CasseteName == a.CassetteName);
                    Assert.Contains(request.Cassette, i => i.Seq == a.CassetteSequence);
                    Assert.Equal(result.Guid, a.MasterMCSBulkDepositReport_Guid);
                });
            }

            [Theory]
            [MemberData(nameof(DataRequest))]
            public void CreateSixCassette_ShouldBeSixCassess(MachineCashServiceRequest request)
            {
                request.Cassette = request.Cassette.OrderBy(o => o.Seq).Take(6);
                var result = (TblMasterActualJobMCSBulkDepositReport)_adhocService.InvokeMethod(PrePareDataBulkCassett, request);

                Assert.Equal(6, result.TblMasterActualJobMCSBulkDepositReportEntry.Count);
                Assert.All(result.TblMasterActualJobMCSBulkDepositReportEntry, a =>
                {
                    Assert.Contains(request.Cassette, i => i.CasseteName == a.CassetteName);
                    Assert.Contains(request.Cassette, i => i.Seq == a.CassetteSequence);
                    Assert.Equal(result.Guid, a.MasterMCSBulkDepositReport_Guid);
                });

            }

            [Fact]
            public void IsAddCassetteByConfigTrunOn_OrderByCassetteName()
            {
                Guid currencyGuid = new Guid("AEF33F4A-173B-47D4-9924-065DDB8A70F0");
                IEnumerable<DenominationOnMachineCassetteView> cassetteData = new List<DenominationOnMachineCassetteView>() {
                    new DenominationOnMachineCassetteView {
                        MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetA",
                    Seq = 6
                },   new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetB",
                    Seq = 5
                },
                 new DenominationOnMachineCassetteView {
                     MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetC",
                    Seq = 4
                },
                 new DenominationOnMachineCassetteView {
                     MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetD",
                    Seq = 3
                },
                 new DenominationOnMachineCassetteView {
                     MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetE",
                    Seq = 2
                },
                 new DenominationOnMachineCassetteView {
                     MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetF",
                    Seq = 1
                }};

                MachineCashServiceRequest request = new MachineCashServiceRequest()
                {
                    IsAscSequence = true,
                    Cassette = cassetteData,
                    JobHeaderGuid = Guid.NewGuid(),
                    JobLegGuid = Guid.NewGuid(),
                    UserName = "Unit-Test",
                    UniversalDatetime = DateTimeOffset.Now,
                    CurrencyGuid = currencyGuid


                };

                var result = (TblMasterActualJobMCSBulkDepositReport)_adhocService.InvokeMethod(PrePareDataBulkCassett, request);
                int i = 1;

                //Assert 
                Assert.NotNull(result);
                foreach (var item in request.Cassette.OrderBy(o => o.CasseteName))
                {
                    var re = result.TblMasterActualJobMCSBulkDepositReportEntry.Single(s => s.CassetteName == item.CasseteName).CassetteSequence;
                    Assert.Equal(i, re);
                    i++;
                }

            }

            [Fact]
            public void IsAddCassetteByConfigTrunOff_OrderBySeqCassette()
            {
                Guid currencyGuid = new Guid("AEF33F4A-173B-47D4-9924-065DDB8A70F0");
                IEnumerable<DenominationOnMachineCassetteView> cassetteData = new List<DenominationOnMachineCassetteView>() {
                    new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetA",
                    Seq = 6
                },   new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetB",
                    Seq = 5
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetC",
                    Seq = 4
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetD",
                    Seq = 3
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetE",
                    Seq = 2
                },
                 new DenominationOnMachineCassetteView {
                    MachineCassetteGuid = Guid.NewGuid(),
                    CasseteName = "CassetF",
                    Seq = 1
                }};

                MachineCashServiceRequest request = new MachineCashServiceRequest()
                {
                    IsAscSequence = false,
                    Cassette = cassetteData,
                    JobHeaderGuid = Guid.NewGuid(),
                    JobLegGuid = Guid.NewGuid(),
                    UserName = "Unit-Test",
                    UniversalDatetime = DateTimeOffset.Now,


                };
                var result = (TblMasterActualJobMCSBulkDepositReport)_adhocService.InvokeMethod(PrePareDataBulkCassett, request);

                //Assert 
                Assert.NotNull(result);
                foreach (var item in request.Cassette.OrderBy(o => o.CasseteName))
                {
                    var re = result.TblMasterActualJobMCSBulkDepositReportEntry.Single(s => s.CassetteName == item.CasseteName).CassetteSequence;
                    Assert.Equal(item.Seq, re);

                }
            }
            #endregion
            #region SetDeno
            [Theory]
            [MemberData(nameof(DataRequest))]
            public void CurrencyNotHasDeno_Responsenull(MachineCashServiceRequest request)
            {
                List<DenominationView> mockDenomination = new List<DenominationView>();

                request.Cassette = request.Cassette.Where(w => w.Seq == 1);
                _adhocService.GetMock<IMasterDenominationRepository>()
                   .Setup(fn => fn.GetDenoByCurrency(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(mockDenomination.AsEnumerable());

                var result = (PrepareBulkDenomination)_adhocService.InvokeMethod(PrePareDataBulkDenomination, request);
                Assert.Null(result);
            }
            [Theory]
            [MemberData(nameof(DataRequest))]
            public void CurrencyHasDeno_ResponseNotnull(MachineCashServiceRequest request)
            {
                List<DenominationView> mockDenomination = new List<DenominationView>();
                int v = 10;
                for (int i = 1; i < 5; i++)
                {
                    mockDenomination.Add(new DenominationView
                    {
                        Guid = Guid.NewGuid(),
                        DenominationText = (i * v).ToString(),
                        DenominationValue = i * v,
                        MasterCurrency_Guid = request.Cassette.First().CurrencyGuid,

                    });
                    v = v * 10;
                }

                request.Cassette = request.Cassette.Where(w => w.Seq == 1);
                _adhocService.GetMock<IMasterDenominationRepository>()
                   .Setup(fn => fn.GetDenoByCurrency(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(mockDenomination.AsEnumerable());

                var result = (PrepareBulkDenomination)_adhocService.InvokeMethod(PrePareDataBulkDenomination, request);
                Assert.NotNull(result);

                Assert.Equal(request.Cassette.First().CurrencyGuid, result.Retract.MasterCurrency_Guid);
                Assert.Equal(mockDenomination.Count, result.Retract.TblMasterActualJobMCSBulkRetractEntry.Count);

                Assert.Equal(request.Cassette.First().CurrencyGuid, result.Jammet.MasterCurrency_Guid);
                Assert.Equal(mockDenomination.Count, result.Jammet.TblMasterActualJobMCSBulkJammedEntry.Count);

                Assert.Equal(request.Cassette.First().CurrencyGuid, result.SuspectFake.MasterCurrency_Guid);
                Assert.Equal(mockDenomination.Count, result.SuspectFake.TblMasterActualJobMCSBulkSuspectFakeEntry.Count);
            }
            #endregion


        }
        public class AdhocUtility : BaseTest
        {
            private readonly IAdhocService _adhocService;
            public AdhocUtility()
            {
                _adhocService = CreateInstance<AdhocService>();
            }
            public static List<object[]> DataCustomerLocationGuid()
            {
                List<Guid> data = new List<Guid>() {
                    new Guid("AAA168BF-D95A-4233-A801-A339F2D4A175"),
                    new Guid("2EB24A42-CA08-414B-8617-5A3B1568A418"),
                    new Guid("47AE56F1-EF05-49F7-8A4C-5D08EFC7FE00"),
                    new Guid("91E60D32-33F8-4897-9EEB-8AC051B30C05"),
                    new Guid("AFE5B23E-AF16-4F64-99D7-8406034A5352")
                };
                List<TblMasterCustomerLocation> cusLocData = new List<TblMasterCustomerLocation>(5);
                int i = 1;
                foreach (var f in data)
                {
                    cusLocData.Add(new TblMasterCustomerLocation
                    {
                        Guid = f,
                        Branch = string.Format("CustomerLocation {0}", i)
                    });
                    i++;
                }

                var site = new TblMasterSite
                {
                    Guid = Guid.NewGuid(),
                    SiteCode = "0010"
                };
                var appkey = new TblSystemRunningVaule_Global
                {
                    RunningKey = "JobNo",
                    RunningVaule1 = 1010

                };
                return new List<object[]> {
                    new object[] { data, cusLocData,site, new List<TblSystemRunningVaule_Global>() { appkey }, 0 },
                    new object[] { data, cusLocData, site, new List<TblSystemRunningVaule_Global>() { appkey }, 100 },
                    new object[] { data, cusLocData, site, new List<TblSystemRunningVaule_Global>() { appkey }, 1000 }

                };
            }
            [Theory]
            [MemberData(nameof(DataCustomerLocationGuid))]
            public void CheckMultiLocation_SortByLocationName(List<Guid> location, IEnumerable<TblMasterCustomerLocation> customerLocatationData, TblMasterSite site, IEnumerable<TblSystemRunningVaule_Global> key, int maxStop)
            {
                Assert.NotEmpty(location);
                _adhocService.GetMock<IMasterCustomerLocationRepository>().Setup(fn => fn.FindLocationByListGuid(location)).Returns(customerLocatationData.AsQueryable());
                _adhocService.GetMock<IMasterSiteRepository>().Setup(fn => fn.FindById(site.Guid)).Returns(site);
                _adhocService.GetMock<ISystemRunningValueGlobalRepository>().Setup(fn => fn.FindAll(It.IsAny<Func<TblSystemRunningVaule_Global, bool>>())).Returns(key);
                _adhocService.GetMock<IUnitOfWork<OceanDbEntities>>()
                    .Setup(f => f.BeginTransaction(IsolationLevel.Unspecified))
                    .Returns(
                        new TransactionScope()
                    );
                CreateMultiJobRequest requestInvoke = new CreateMultiJobRequest
                {
                    MasterCustomerLocationGuids = location,
                    BrinksSiteGuid = site.Guid,
                    IsCreateToRun = false,
                    MaxStop = maxStop,
                    UnassignedBy = "Unit-test",
                    UnassignedDate = DateTime.Now
                };
                var result = (IEnumerable<AdhocJob_Info>)_adhocService.InvokeMethod("MultiLocation", requestInvoke);
                int i = maxStop + 1;
                Assert.NotNull(result);
                foreach (var r in customerLocatationData.OrderBy(o => o.BranchName))
                {
                    Assert.Equal(result.Single(w => w.LocationGuid == r.Guid).LocationSeq, i);
                    i++;
                }
            }
        }
    }
}
