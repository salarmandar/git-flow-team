using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Service.Messagings.AlarmHub;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Bgt.Ocean.Models;
using AutoFixture;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Models.Group;
using System.Linq.Expressions;
using Bgt.Ocean.Models.RunControl;

namespace Bgt.Ocean.Service.Test.AlarmHub
{

    public class AlarmHubServiceTest
    {
        #region Trigger Alarm

        public class TriggerAlarm : BaseTest
        {
            private readonly Mock<IAlarmHubBroadcastService> _mockAlarmHubBroadcastService;
            private readonly Mock<IMasterDailyRunResourceAlarmRepository> _mockDailyRunAlarmRepo;
            private readonly Mock<IMasterDailyRunResourceHistoryRepository> _mockDailyRunHistoryRepo;
            private readonly Mock<IMasterGroupRepository> _mockMasterGroupRepository;
            private readonly IAlarmHubService _service;

            public TriggerAlarm()
            {
                _mockDailyRunAlarmRepo = CreateMock<IMasterDailyRunResourceAlarmRepository>();
                _mockAlarmHubBroadcastService = CreateMock<IAlarmHubBroadcastService>();
                _mockMasterGroupRepository = CreateMock<IMasterGroupRepository>();
                _mockDailyRunHistoryRepo = CreateMock<IMasterDailyRunResourceHistoryRepository>();

                _service = new AlarmHubService(
                        _mockDailyRunAlarmRepo.Object,
                        _mockUow.Object,
                        _mockLogErrorRepo.Object,
                        _mockAlarmHubBroadcastService.Object,
                        _mockMasterGroupRepository.Object,
                        _mockDailyRunHistoryRepo.Object
                    );
            }

            [Fact]
            public void CorrectData_ShouldWorkingNormally()
            {

                var request = Util.CreateDummy<AlarmHubCreateRequest>();

                _mockDailyRunAlarmRepo
                    .Setup(e => e.Create(It.IsAny<TblMasterDailyRunResource_Alarm>()))
                    .Callback<TblMasterDailyRunResource_Alarm>(entity =>
                    {
                        Assert.Equal(request.DateTimeAlert, entity.DatetimeCreated);
                        Assert.Equal(request.EmployeeName, entity.EmployeeName);
                        Assert.Equal(request.Latitude, entity.Latitude);
                        Assert.Equal(request.Longitude, entity.Longitude);
                        Assert.Equal(request.MasterDailyRunResouceGuid, entity.MasterDailyRunResource_Guid);
                        Assert.Equal(request.MasterSiteGuid, entity.MasterSite_Guid);
                        Assert.Equal(request.Phone, entity.Phone);
                        Assert.Equal(request.RouteName, entity.RouteName);
                        Assert.Equal(request.RunNo, entity.RunNo);
                        Assert.Equal(request.SiteName, entity.SiteName);
                        Assert.Equal(request.UserCreated, entity.UserCreated);
                        Assert.Equal(request.LocationGuid, entity.MasterCustomerLocation_Guid);
                        Assert.Equal(request.EmployeeGuid, entity.MasterEmployee_Guid);

                        Assert.NotNull(entity.UniversalDatetimeCreated);
                        Assert.NotEqual(entity.DatetimeCreated, entity.UniversalDatetimeCreated);

                        Assert.False(entity.FlagAcknowledged);
                        Assert.Null(entity.DatetimeAcknowledged);
                        Assert.Null(entity.DatetimeDeactivated);
                    });


                _service.TriggerAlarm(request);

                _mockDailyRunAlarmRepo.Verify(fn => fn.Create(It.IsAny<TblMasterDailyRunResource_Alarm>()), Times.Once());
                _mockAlarmHubBroadcastService.Verify(fn => fn.BroadcastAlarm(request.MasterDailyRunResouceGuid), Times.Once());
                _mockUow.Verify(fn => fn.Commit(), Times.Once());
            }

        }

        #endregion

        #region Get Alarm List

        public class GetAlarm : BaseTest
        {
            private readonly Mock<IAlarmHubBroadcastService> _mockAlarmHubBroadcastService;
            private readonly Mock<IMasterDailyRunResourceAlarmRepository> _mockDailyRunAlarmRepo;
            private readonly Mock<IMasterGroupRepository> _mockMasterGroupRepository;
            private readonly IAlarmHubService _service;
            private readonly Mock<IMasterDailyRunResourceHistoryRepository> _mockDailyRunHistoryRepo;

            public GetAlarm()
            {
                _mockDailyRunAlarmRepo = CreateMock<IMasterDailyRunResourceAlarmRepository>();
                _mockAlarmHubBroadcastService = CreateMock<IAlarmHubBroadcastService>();
                _mockMasterGroupRepository = CreateMock<IMasterGroupRepository>();
                _mockDailyRunHistoryRepo = CreateMock<IMasterDailyRunResourceHistoryRepository>();

                _service = new AlarmHubService(
                        _mockDailyRunAlarmRepo.Object,
                        _mockUow.Object,
                        _mockLogErrorRepo.Object,
                        _mockAlarmHubBroadcastService.Object,
                        _mockMasterGroupRepository.Object,
                        _mockDailyRunHistoryRepo.Object
                    );
            }

            public static IEnumerable<object[]> SiteWithNoRunOrNoGroup()
            {
                Action<Fixture> defaultSetup = fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                };

                var alarmRunList1 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(1, defaultSetup);

                var groupList1 = Util.CreateDummy<MasterGroupAlarmModel>(1);

                return new List<object[]>
                {
                    new object[] { Enumerable.Empty<TblMasterDailyRunResource_Alarm>(), Enumerable.Empty<MasterGroupAlarmModel>() },
                    new object[] { alarmRunList1, Enumerable.Empty<MasterGroupAlarmModel>() },
                    new object[] { Enumerable.Empty<TblMasterDailyRunResource_Alarm>(), groupList1 }
                };
            }

            [Theory]
            [MemberData(nameof(SiteWithNoRunOrNoGroup))]
            public void SiteWithNoRunOrNoGroup_ShoudEmpty(IEnumerable<TblMasterDailyRunResource_Alarm> alarmRunList, IEnumerable<MasterGroupAlarmModel> groupList)
            {
                var siteGuid = Guid.NewGuid();
                var userGuid = Guid.NewGuid();

                #region Arrange

                foreach (var item in alarmRunList)
                {
                    item.MasterSite_Guid = siteGuid;
                }

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable())
                    .Returns(alarmRunList.AsQueryable());

                _mockMasterGroupRepository
                    .Setup(fn => fn.GetPermittedAlarmGroupByUser(It.IsAny<Guid>()))
                    .Returns(groupList);

                #endregion

                var response = _service.GetCurrentAlarmTriggeredList(userGuid);

                Assert.NotNull(response);
                Assert.Empty(response);
            }

            public static IEnumerable<object[]> CorrectSiteWithRunAlarm()
            {
                Action<Fixture> defaultSetup = fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { BranchName = "GET_ALARM_LIST" });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register<bool>(() => false);
                };

                Func<TblMasterDailyRunResource_Alarm, TblMasterDailyRunResource_Alarm> defaultSetupData = e =>
                {
                    return e;
                };

                var siteGuid1 = Guid.NewGuid();
                var alarmRunList1 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(1, defaultSetup)
                    .Select(defaultSetupData);

                ////////////////////////////////////////////////////////////////////

                var siteGuid2 = Guid.NewGuid();
                var alarmRunList2 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(20, defaultSetup)
                    .Select(defaultSetupData);

                ////////////////////////////////////////////////////////////////////

                var siteGuid3 = Guid.NewGuid();
                var alarmRunList3 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(3, defaultSetup)
                    .Select(defaultSetupData)
                    .Select((e, i) =>
                    {
                        if ((i + 1) % 2 == 0) e.FlagAcknowledged = true;
                        return e;
                    });

                ////////////////////////////////////////////////////////////////////

                var siteGuid4 = Guid.NewGuid();
                var alarmRunList4 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(10, defaultSetup)
                    .Select(defaultSetupData)
                    .Select((e, i) =>
                    {
                        if ((i + 1) % 2 == 0) e.FlagAcknowledged = true;
                        return e;
                    });

                ////////////////////////////////////////////////////////////////////

                var siteGuid5 = Guid.NewGuid();
                var alarmRunList5 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(10, defaultSetup)
                    .Select(defaultSetupData)
                    .Select((e, i) =>
                    {
                        if ((i + 1) % 2 == 0) e.FlagDeactivated = true;
                        return e;
                    });

                ////////////////////////////////////////////////////////////////////

                var siteGuid6 = Guid.NewGuid();
                var alarmRunList6 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(10, defaultSetup)
                    .Select(defaultSetupData)
                    .Select((e, i) =>
                    {
                        if ((i + 1) % 2 == 0)
                        {
                            e.FlagAcknowledged = true;
                            e.FlagDeactivated = true;
                        }
                        return e;
                    });

                return new List<object[]>
                {
                    new object[] { siteGuid1, alarmRunList1, 1 },
                    new object[] { siteGuid2, alarmRunList2, 20 },
                    new object[] { siteGuid3, alarmRunList3, 2 },
                    new object[] { siteGuid4, alarmRunList4, 5 },
                    new object[] { siteGuid5, alarmRunList5, 5 },
                    new object[] { siteGuid6, alarmRunList6, 5 }
                };
            }

            [Theory]
            [MemberData(nameof(CorrectSiteWithRunAlarm))]
            public void CorrectSiteAndOneGroupWithRunAlarm_ShoudHasCorrectData(Guid siteGuid, IEnumerable<TblMasterDailyRunResource_Alarm> alarmRunList, int expectedCount)
            {
                var userGuid = Guid.NewGuid();
                var locationName = "GET_ALARM_LIST";

                #region Arrange

                foreach (var item in alarmRunList)
                {
                    item.MasterSite_Guid = siteGuid;
                }

                var groupList = CreateDummy<MasterGroupAlarmModel>(1)
                    .Select(e =>
                    {
                        e.FlagAllowAcknowledge = true;
                        e.MasterSiteHandleList = Enumerable.Repeat(siteGuid, 1);
                        return e;
                    });

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable())
                    .Returns(alarmRunList.AsQueryable());

                _mockMasterGroupRepository
                    .Setup(fn => fn.GetPermittedAlarmGroupByUser(It.IsAny<Guid>()))
                    .Returns(groupList);

                #endregion

                var response = _service.GetCurrentAlarmTriggeredList(userGuid);

                Assert.Equal(expectedCount, response.Count());
                Assert.All(response, item =>
                {
                    Assert.Equal(siteGuid, item.MasterSite_Guid);
                    Assert.True(item.FlagAllowAcknowledged);
                    Assert.Equal(locationName, item.LocationName);
                });
            }

            public static IEnumerable<object[]> CorrectRunAlarmWithMultipleGroup()
            {
                var groupList1 = Util.CreateDummy<MasterGroupAlarmModel>(2).ToList();
                groupList1[0].FlagAllowAcknowledge = true;
                groupList1[1].FlagAllowAcknowledge = true;

                var groupList2 = Util.CreateDummy<MasterGroupAlarmModel>(2).ToList();
                groupList2[0].FlagAllowAcknowledge = false;
                groupList2[1].FlagAllowAcknowledge = true;

                return new List<object[]>
                {
                    new object[] { groupList1, 10 }, // allow acknowledge all group
                    new object[] { groupList2, 10 }
                };
            }

            [Theory]
            [MemberData(nameof(CorrectRunAlarmWithMultipleGroup))]
            public void CorrectRunAlarmWithMultipleGroup_ShouldHasCorrectData(IEnumerable<MasterGroupAlarmModel> groupList, int expectedCount)
            {
                var userGuid = Guid.NewGuid();
                var siteGuid = Guid.NewGuid();

                var alarmRunList = CreateDummy<TblMasterDailyRunResource_Alarm>(10, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                });

                foreach (var item in alarmRunList)
                {
                    item.MasterSite_Guid = siteGuid;
                }

                foreach (var item in groupList)
                {
                    item.MasterSiteHandleList = Enumerable.Repeat(siteGuid, 1);
                }

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable())
                    .Returns(alarmRunList.AsQueryable());

                _mockMasterGroupRepository
                    .Setup(fn => fn.GetPermittedAlarmGroupByUser(It.IsAny<Guid>()))
                    .Returns(groupList);

                var response = _service.GetCurrentAlarmTriggeredList(userGuid);

                Assert.Equal(expectedCount, response.Count());
                Assert.All(response, item =>
                {
                    Assert.True(item.FlagAllowAcknowledged);
                });
            }

            [Fact]
            public void NotAllowAcknowledgeGroup_ShouldNotAllowAcknowledgeInRun()
            {
                var userGuid = Guid.NewGuid();
                var siteGuid = Guid.NewGuid();

                var groupList = CreateDummy<MasterGroupAlarmModel>(5);

                var alarmRunList = CreateDummy<TblMasterDailyRunResource_Alarm>(10, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                });

                foreach (var item in alarmRunList)
                {
                    item.MasterSite_Guid = siteGuid;
                }

                foreach (var item in groupList)
                {
                    item.MasterSiteHandleList = Enumerable.Repeat(siteGuid, 1);
                    item.FlagAllowAcknowledge = false;
                }

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable())
                    .Returns(alarmRunList.AsQueryable());

                _mockMasterGroupRepository
                    .Setup(fn => fn.GetPermittedAlarmGroupByUser(It.IsAny<Guid>()))
                    .Returns(groupList);

                var response = _service.GetCurrentAlarmTriggeredList(userGuid);

                Assert.All(response, item =>
                {
                    Assert.False(item.FlagAllowAcknowledged);
                });
            }

            public static IEnumerable<object[]> RunMultipleSiteForUser()
            {
                var targetSite1 = Guid.NewGuid();
                var alarmRunList1 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(3, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                })
                .Select(e =>
                {
                    e.MasterSite_Guid = targetSite1;
                    return e;
                });

                var targetSite2 = Guid.NewGuid();
                var alarmRunList2 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(5, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                });

                alarmRunList2.FirstOrDefault().MasterSite_Guid = targetSite2;

                var targetSite3 = Guid.NewGuid();
                var alarmRunList3 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(10, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                });

                return new List<object[]>
                {
                    new object[] { targetSite1, alarmRunList1, 3 },
                    new object[] { targetSite2, alarmRunList2, 1 },
                    new object[] { targetSite3, alarmRunList3, 0 }
                };
            }

            [Theory]
            [MemberData(nameof(RunMultipleSiteForUser))]
            public void RunMultipleSiteForUser_ShouldGetOnlyPermittedSite(Guid siteGuid, IEnumerable<TblMasterDailyRunResource_Alarm> alarmRunList, int expectedCount)
            {
                var userGuid = Guid.NewGuid();

                var groupList = CreateDummy<MasterGroupAlarmModel>(1)
                    .Select(e =>
                    {
                        e.MasterSiteHandleList = Enumerable.Repeat(siteGuid, 1);
                        e.FlagAllowAcknowledge = true;

                        return e;
                    });

                _mockDailyRunAlarmRepo
                   .Setup(fn => fn.FindAllAsQueryable())
                   .Returns(alarmRunList.AsQueryable());

                _mockMasterGroupRepository
                    .Setup(fn => fn.GetPermittedAlarmGroupByUser(It.IsAny<Guid>()))
                    .Returns(groupList);

                var response = _service.GetCurrentAlarmTriggeredList(userGuid);

                Assert.Equal(expectedCount, response.Count());
                Assert.All(response, item =>
                {
                    Assert.Equal(siteGuid, item.MasterSite_Guid);
                    Assert.True(item.FlagAllowAcknowledged);
                });
            }

            public static IEnumerable<object[]> RunMulipleSiteAndUserMultipleSiteHandle()
            {
                Func<TblMasterDailyRunResource_Alarm, Guid, TblMasterDailyRunResource_Alarm> defaultSetSite =
                    (model, siteGuid) =>
                    {
                        model.MasterSite_Guid = siteGuid;
                        return model;
                    };

                #region Arrange 1  (Run has 4 Alarm. User has 1 group which handle 1 site and can acknowledge. Response should has 2 alarm with allow acknowledge)

                var siteGuid1_1 = Guid.NewGuid();
                var siteGuid1_2 = Guid.NewGuid();

                var alarmRunList1 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(4, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                })
                .Select(e => defaultSetSite(e, siteGuid1_1))
                .ToList();

                alarmRunList1[2].MasterSite_Guid = siteGuid1_2;
                alarmRunList1[3].MasterSite_Guid = siteGuid1_2;

                var groupList1 = Util.CreateDummy<MasterGroupAlarmModel>(1)
                    .Select(e =>
                    {
                        e.MasterSiteHandleList = new Guid[] { siteGuid1_1 };
                        e.FlagAllowAcknowledge = true;

                        return e;
                    });

                Action<IEnumerable<AlarmHubTriggeredResponse>> assertion1 = response =>
                {
                    Assert.Equal(2, response.Count());
                    Assert.All(response, data =>
                    {
                        Assert.Equal(siteGuid1_1, data.MasterSite_Guid);
                        Assert.True(data.FlagAllowAcknowledged);
                    });
                };

                #endregion

                /////////////////////////////////////////////////////////////////////////

                #region Arrange 2 (Run has 4 Alarm. User has 1 group which handle 1 site and cannot acknowledge. Response should has 2 alarm with not allow acknowledge)

                var siteGuid2_1 = Guid.NewGuid();
                var siteGuid2_2 = Guid.NewGuid();

                var alarmRunList2 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(4, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                })
                .Select(e => defaultSetSite(e, siteGuid2_1))
                .ToList();

                alarmRunList2[2].MasterSite_Guid = siteGuid2_2;
                alarmRunList2[3].MasterSite_Guid = siteGuid2_2;

                var groupList2 = Util.CreateDummy<MasterGroupAlarmModel>(1)
                    .Select(e =>
                    {
                        e.MasterSiteHandleList = new Guid[] { siteGuid2_1 };
                        e.FlagAllowAcknowledge = false;

                        return e;
                    });

                Action<IEnumerable<AlarmHubTriggeredResponse>> assertion2 = response =>
                {
                    Assert.Equal(2, response.Count());
                    Assert.All(response, data =>
                    {
                        Assert.Equal(siteGuid2_1, data.MasterSite_Guid);
                        Assert.False(data.FlagAllowAcknowledged);
                    });
                };


                #endregion

                /////////////////////////////////////////////////////////////////////////

                #region Arrange 3 (Run has 4 Alarm. User has 1 group which handle 2 site and can acknowledge. Response should has 4 alarm with allow acknowledge)

                var siteGuid3_1 = Guid.NewGuid();
                var siteGuid3_2 = Guid.NewGuid();

                var alarmRunList3 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(4, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                })
                .Select(e => defaultSetSite(e, siteGuid3_1))
                .ToList();

                alarmRunList3[2].MasterSite_Guid = siteGuid3_2;
                alarmRunList3[3].MasterSite_Guid = siteGuid3_2;

                var groupList3 = Util.CreateDummy<MasterGroupAlarmModel>(1)
                    .Select(e =>
                    {
                        e.MasterSiteHandleList = new Guid[] { siteGuid3_1, siteGuid3_2 };
                        e.FlagAllowAcknowledge = true;

                        return e;
                    });

                Action<IEnumerable<AlarmHubTriggeredResponse>> assertion3 = response =>
                {
                    Assert.Equal(4, response.Count());
                    Assert.All(response, data =>
                    {
                        Assert.True(data.MasterSite_Guid == siteGuid3_1 || data.MasterSite_Guid == siteGuid3_2);
                        Assert.True(data.FlagAllowAcknowledged);
                    });
                };


                #endregion

                /////////////////////////////////////////////////////////////////////////

                #region Arrange 4 (Run has 4 Alarm. User has 2 group which handle 1 site per each and can acknowledge. Response should has 4 alarm with allow acknowledge)

                var siteGuid4_1 = Guid.NewGuid();
                var siteGuid4_2 = Guid.NewGuid();

                var alarmRunList4 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(4, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                })
                .Select(e => defaultSetSite(e, siteGuid4_1))
                .ToList();

                alarmRunList4[2].MasterSite_Guid = siteGuid4_2;
                alarmRunList4[3].MasterSite_Guid = siteGuid4_2;

                var groupList4 = new List<MasterGroupAlarmModel>
                {
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid4_1 };
                        data.FlagAllowAcknowledge = true;
                    }),
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid4_2 };
                        data.FlagAllowAcknowledge = true;
                    })
                };

                Action<IEnumerable<AlarmHubTriggeredResponse>> assertion4 = response =>
                {
                    Assert.Equal(4, response.Count());
                    Assert.All(response, data =>
                    {
                        Assert.True(data.MasterSite_Guid == siteGuid4_1 || data.MasterSite_Guid == siteGuid4_2);
                        Assert.True(data.FlagAllowAcknowledged);
                    });
                };


                #endregion

                /////////////////////////////////////////////////////////////////////////

                #region Arrange 5 (Run has 4 Alarm. User has 2 group which handle 1 site per each and cannot acknowledge. Response should has 4 alarm with not allow acknowledge)

                var siteGuid5_1 = Guid.NewGuid();
                var siteGuid5_2 = Guid.NewGuid();

                var alarmRunList5 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(4, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                })
                .Select(e => defaultSetSite(e, siteGuid5_1))
                .ToList();

                alarmRunList5[2].MasterSite_Guid = siteGuid5_2;
                alarmRunList5[3].MasterSite_Guid = siteGuid5_2;

                var groupList5 = new List<MasterGroupAlarmModel>
                {
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid5_1 };
                        data.FlagAllowAcknowledge = false;
                    }),
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid5_2 };
                        data.FlagAllowAcknowledge = false;
                    })
                };

                Action<IEnumerable<AlarmHubTriggeredResponse>> assertion5 = response =>
                {
                    Assert.Equal(4, response.Count());
                    Assert.All(response, data =>
                    {
                        Assert.True(data.MasterSite_Guid == siteGuid5_1 || data.MasterSite_Guid == siteGuid5_2);
                        Assert.False(data.FlagAllowAcknowledged);
                    });
                };

                #endregion

                /////////////////////////////////////////////////////////////////////////
                #region Arrange 6 (Run has 4 Alarm. User has 2 group which handle 1 site per each and can/cannot acknowledge. Response should has 4 alarm with allow/not allow acknowledge)

                var siteGuid6_1 = Guid.NewGuid();
                var siteGuid6_2 = Guid.NewGuid();

                var alarmRunList6 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(4, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                })
                .Select(e => defaultSetSite(e, siteGuid6_1))
                .ToList();

                alarmRunList6[2].MasterSite_Guid = siteGuid6_2;
                alarmRunList6[3].MasterSite_Guid = siteGuid6_2;

                var groupList6 = new List<MasterGroupAlarmModel>
                {
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid6_1 };
                        data.FlagAllowAcknowledge = false;
                    }),
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid6_2 };
                        data.FlagAllowAcknowledge = true;
                    })
                };

                Action<IEnumerable<AlarmHubTriggeredResponse>> assertion6 = response =>
                {
                    Assert.Equal(4, response.Count());
                    Assert.All(response, data =>
                    {
                        Assert.True(data.MasterSite_Guid == siteGuid6_1 || data.MasterSite_Guid == siteGuid6_2);
                    });

                    Assert.Equal(2, response.Count(e => !e.FlagAllowAcknowledged && e.MasterSite_Guid == siteGuid6_1));
                    Assert.Equal(2, response.Count(e => e.FlagAllowAcknowledged && e.MasterSite_Guid == siteGuid6_2));
                };

                #endregion
                /////////////////////////////////////////////////////////////////////////

                #region Arrange 7 (Run has 4 Alarm. User has 2 group which handle 1 site per each and can/cannot acknowledge but not handle site correctly. Response should empty)

                var siteGuid7_1 = Guid.NewGuid();
                var siteGuid7_2 = Guid.NewGuid();

                var alarmRunList7 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(4, fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource { });
                    fixture.Inject(new TblMasterSite { });
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                });

                var groupList7 = new List<MasterGroupAlarmModel>
                {
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid7_1 };
                        data.FlagAllowAcknowledge = false;
                    }),
                    Util.CreateDummyAndModify<MasterGroupAlarmModel>(data =>
                    {
                        data.MasterSiteHandleList = new Guid[] { siteGuid7_2 };
                        data.FlagAllowAcknowledge = true;
                    })
                };

                Action<IEnumerable<AlarmHubTriggeredResponse>> assertion7 = response =>
                {
                    Assert.Empty(response);
                };

                #endregion

                return new List<object[]>
                {
                    new object[] { alarmRunList1, groupList1, assertion1 },
                    new object[] { alarmRunList2, groupList2, assertion2 },
                    new object[] { alarmRunList3, groupList3, assertion3 },
                    new object[] { alarmRunList4, groupList4, assertion4 },
                    new object[] { alarmRunList5, groupList5, assertion5 },
                    new object[] { alarmRunList6, groupList6, assertion6 },
                    new object[] { alarmRunList7, groupList7, assertion7 }
                };
            }

            [Theory]
            [MemberData(nameof(RunMulipleSiteAndUserMultipleSiteHandle))]
            public void RunMulipleSiteAndUserMultipleSiteHandle_ShouldHasCorrectData(IEnumerable<TblMasterDailyRunResource_Alarm> alarmRunList, IEnumerable<MasterGroupAlarmModel> groupList, Action<IEnumerable<AlarmHubTriggeredResponse>> assertion)
            {
                var userGuid = Guid.NewGuid();

                _mockDailyRunAlarmRepo
                   .Setup(fn => fn.FindAllAsQueryable())
                   .Returns(alarmRunList.AsQueryable());

                _mockMasterGroupRepository
                    .Setup(fn => fn.GetPermittedAlarmGroupByUser(It.IsAny<Guid>()))
                    .Returns(groupList);

                var response = _service.GetCurrentAlarmTriggeredList(userGuid);

                Assert.NotNull(response);
                assertion(response);

            }
        }

        #endregion

        #region Check has alarm

        public class CheckHasAlarm : BaseTest
        {
            private readonly Mock<IAlarmHubBroadcastService> _mockAlarmHubBroadcastService;
            private readonly Mock<IMasterDailyRunResourceAlarmRepository> _mockDailyRunAlarmRepo;
            private readonly Mock<IMasterGroupRepository> _mockMasterGroupRepository;
            private readonly IAlarmHubService _service;
            private readonly Mock<IMasterDailyRunResourceHistoryRepository> _mockDailyRunHistoryRepo;

            public CheckHasAlarm()
            {
                _mockDailyRunAlarmRepo = CreateMock<IMasterDailyRunResourceAlarmRepository>();
                _mockAlarmHubBroadcastService = CreateMock<IAlarmHubBroadcastService>();
                _mockMasterGroupRepository = CreateMock<IMasterGroupRepository>();
                _mockDailyRunHistoryRepo = CreateMock<IMasterDailyRunResourceHistoryRepository>();

                _service = new AlarmHubService(
                        _mockDailyRunAlarmRepo.Object,
                        _mockUow.Object,
                        _mockLogErrorRepo.Object,
                        _mockAlarmHubBroadcastService.Object,
                        _mockMasterGroupRepository.Object,
                        _mockDailyRunHistoryRepo.Object
                    );
            }

            public static IEnumerable<object[]> CorrectRun()
            {
                Action<Fixture> defaultSetup = fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource());
                    fixture.Inject(new TblMasterSite());
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                };

                Func<Guid, TblMasterDailyRunResource_Alarm, TblMasterDailyRunResource_Alarm> setGuid = (guid, newItem) =>
                {
                    newItem.MasterDailyRunResource_Guid = guid;
                    return newItem;
                };

                #region Arrange 1

                var dailyRunResourceGuid1 = Guid.NewGuid();

                var alarmList1 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(1, defaultSetup)
                    .Select(e => setGuid(dailyRunResourceGuid1, e));

                #endregion

                #region Arrange 2

                var dailyRunResourceGuid2 = Guid.NewGuid();

                var alarmList2 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(1, defaultSetup)
                    .Select(e => setGuid(dailyRunResourceGuid2, e))
                    .Select(e =>
                    {
                        e.FlagAcknowledged = true;
                        e.FlagDeactivated = false;
                        return e;
                    });

                #endregion

                #region Arrange 3

                var dailyRunResourceGuid3 = Guid.NewGuid();

                var alarmList3 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(1, defaultSetup)
                    .Select(e => setGuid(dailyRunResourceGuid3, e))
                    .Select(e =>
                    {
                        e.FlagAcknowledged = false;
                        e.FlagDeactivated = true;
                        return e;
                    });

                #endregion

                #region Arrange 4 

                var dailyRunResourceGuid4 = Guid.NewGuid();

                var alarmList4 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(2, defaultSetup)
                    .Select(e => setGuid(dailyRunResourceGuid4, e))
                    .Select(e =>
                    {
                        e.FlagAcknowledged = false;
                        e.FlagDeactivated = false;
                        return e;
                    })
                    .ToList();

                alarmList4[0].FlagAcknowledged = true;
                alarmList4[0].FlagDeactivated = true;

                #endregion

                #region Arrange 5 different run guid

                var dailyRunResourceGuid5_1 = Guid.NewGuid();
                var dailyRunResourceGuid5_2 = Guid.NewGuid();

                var alarmList5 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(2, defaultSetup)
                    .Select(e =>
                    {
                        e.FlagAcknowledged = false;
                        e.FlagDeactivated = false;
                        return e;
                    })
                    .ToList();

                alarmList5[0].MasterDailyRunResource_Guid = dailyRunResourceGuid5_1;
                alarmList5[1].MasterDailyRunResource_Guid = dailyRunResourceGuid5_2;

                #endregion

                return new List<object[]>
                {
                    new object[] { Enumerable.Repeat(dailyRunResourceGuid1, 1), alarmList1, 1 },
                    new object[] { Enumerable.Repeat(dailyRunResourceGuid2, 1), alarmList2, 1 },
                    new object[] { Enumerable.Repeat(dailyRunResourceGuid3, 1), alarmList3, 1 },
                    new object[] { Enumerable.Repeat(dailyRunResourceGuid4, 1), alarmList4, 1 },
                    new object[] { new Guid[] { dailyRunResourceGuid5_1, dailyRunResourceGuid5_2 }, alarmList5, 2 }
                };
            }

            [Theory]
            [MemberData(nameof(CorrectRun))]
            public void CorrectRun_ShouldNotEmptyAndCorrectGuid(IEnumerable<Guid> dailyRunResourceGuidList, IEnumerable<TblMasterDailyRunResource_Alarm> alarmList, int expectedCountTrue)
            {
                #region Arrange    

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable(It.IsAny<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>()))
                    .Returns<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>(expr => alarmList.AsQueryable().Where(expr));

                #endregion

                var response = _service.IsHasAlarm(dailyRunResourceGuidList);

                Assert.Equal(expectedCountTrue, response.Count());

            }

            public static IEnumerable<object[]> IncorrectRun()
            {

                Action<Fixture> defaultSetup = fixture =>
                {
                    fixture.Inject(new TblMasterDailyRunResource());
                    fixture.Inject(new TblMasterSite());
                    fixture.Inject(new TblMasterCustomerLocation { });
                    fixture.Inject(new TblMasterEmployee { });

                    fixture.Register(() => false);
                };

                Func<Guid, TblMasterDailyRunResource_Alarm, TblMasterDailyRunResource_Alarm> setGuid = (guid, newItem) =>
                {
                    newItem.MasterDailyRunResource_Guid = guid;
                    return newItem;
                };

                #region Arrange 1 acknowledged and deactivated

                var dailyRunResourceGuid1 = Guid.NewGuid();

                var alarmList1 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(1, defaultSetup)
                    .Select(e => setGuid(dailyRunResourceGuid1, e))
                    .Select(e =>
                    {
                        e.FlagAcknowledged = true;
                        e.FlagDeactivated = true;
                        return e;
                    });

                #endregion

                #region Arrange 2 not acknowledged and deactivated but different guid

                var dailyRunResourceGuid2 = Guid.NewGuid();

                var alarmList2 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(1, defaultSetup)
                    .Select(e => setGuid(Guid.NewGuid(), e))
                    .Select(e =>
                    {
                        e.FlagAcknowledged = false;
                        e.FlagDeactivated = false;
                        return e;
                    });

                #endregion

                return new List<object[]>
                {
                    new object[] {  Enumerable.Repeat(dailyRunResourceGuid1, 1), alarmList1, 0 },
                    new object[] {  Enumerable.Repeat(dailyRunResourceGuid2, 1), alarmList2, 0 }
                };
            }

            [Theory()]
            [MemberData(nameof(IncorrectRun))]
            public void IncorrectRun_ShouldEmpty(IEnumerable<Guid> masterDailyRunGuidList, IEnumerable<TblMasterDailyRunResource_Alarm> alarmList, int expectedCountInvalid)
            {
                //#region Arrange    

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable(It.IsAny<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>()))
                    .Returns<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>(expr => alarmList.AsQueryable().Where(expr));

                //#endregion

                var response = _service.IsHasAlarm(masterDailyRunGuidList);

                Assert.Equal(expectedCountInvalid, response.Count());
            }
        }

        #endregion

        #region AcknowledgeAlarm

        public class AcknowledgeAlarm : BaseTest
        {
            private readonly Mock<IAlarmHubBroadcastService> _mockAlarmHubBroadcastService;
            private readonly Mock<IMasterDailyRunResourceAlarmRepository> _mockDailyRunAlarmRepo;
            private readonly Mock<IMasterDailyRunResourceHistoryRepository> _mockDailyRunHistoryRepo;
            private readonly Mock<IMasterGroupRepository> _mockMasterGroupRepository;
            private readonly IAlarmHubService _service;

            public AcknowledgeAlarm()
            {
                _mockDailyRunAlarmRepo = CreateMock<IMasterDailyRunResourceAlarmRepository>();
                _mockAlarmHubBroadcastService = CreateMock<IAlarmHubBroadcastService>();
                _mockMasterGroupRepository = CreateMock<IMasterGroupRepository>();
                _mockDailyRunHistoryRepo = CreateMock<IMasterDailyRunResourceHistoryRepository>();

                _service = new AlarmHubService(
                        _mockDailyRunAlarmRepo.Object,
                        _mockUow.Object,
                        _mockLogErrorRepo.Object,
                        _mockAlarmHubBroadcastService.Object,
                        _mockMasterGroupRepository.Object,
                        _mockDailyRunHistoryRepo.Object
                    );
            }

            public static IEnumerable<object[]> CorrectDailyRunAlarm()
            {
                var guid1 = new Guid[] { Guid.NewGuid() }.AsEnumerable();
                var guid2 = new Guid[] { Guid.NewGuid(), Guid.NewGuid() }.AsEnumerable();

                var dailyRunAlarmModel1 = new TblMasterDailyRunResource_Alarm() { MasterDailyRunResource_Guid = Guid.NewGuid() };
                var dailyRunAlarmModel2 = new TblMasterDailyRunResource_Alarm() { MasterDailyRunResource_Guid = Guid.NewGuid() };

                yield return new object[] { guid1, dailyRunAlarmModel1 };
                yield return new object[] { guid2, dailyRunAlarmModel2 }; // call from multiple thread must update only once                    
            }

            [Theory]
            [MemberData(nameof(CorrectDailyRunAlarm))]
            public void CorrectDailyRunAlarm_ShouldSuccess(IEnumerable<Guid> dailyRunAlarmGuidList, TblMasterDailyRunResource_Alarm dailyRunAlarmModel)
            {
                var userAcknowledge = CreateDummy<string>();
                var dateTimeAcknowledge = DateTime.Now;

                var resultList = new List<bool>(2);

                foreach (var dailyRunResourceAlarmGuid in dailyRunAlarmGuidList)
                {
                    #region Arrange    

                    _mockDailyRunAlarmRepo
                        .Setup(fn => fn.FindOne(It.IsAny<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>()))
                        .Returns<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>(expr =>
                        {
                            dailyRunAlarmModel.Guid = dailyRunResourceAlarmGuid;

                            var list = new List<TblMasterDailyRunResource_Alarm>()
                            {
                                dailyRunAlarmModel
                            };

                            return list.FirstOrDefault(expr.Compile());
                        });

                    _mockAlarmHubBroadcastService
                        .Setup(fn => fn.BroadcastAlarmAcknowledged(It.IsAny<Guid>()))
                        .Verifiable();

                    _mockDailyRunHistoryRepo
                        .Setup(fn => fn.CreateDailyRunLog(It.IsAny<MasterDailyRunLogView>()))
                        .Callback<MasterDailyRunLogView>(data =>
                        {
                            Assert.Equal(869, data.MsgID);
                        });

                    #endregion

                    var result = _service.AcknowledgeAlarm(dailyRunResourceAlarmGuid, userAcknowledge, dateTimeAcknowledge);

                    resultList.Add(result);

                    Assert.True(dailyRunAlarmModel.FlagAcknowledged);
                    Assert.Equal(userAcknowledge, dailyRunAlarmModel.UserAcknowledged);
                    Assert.Equal(dateTimeAcknowledge, dailyRunAlarmModel.DatetimeAcknowledged);
                    Assert.NotNull(dailyRunAlarmModel.UniversalDatetimeAcknowledged);

                    _mockAlarmHubBroadcastService.Verify(fn => fn.BroadcastAlarmAcknowledged(It.IsAny<Guid>()), Times.Once());
                    _mockDailyRunAlarmRepo.Verify(fn => fn.Modify(dailyRunAlarmModel), Times.Once());

                    _mockUow.Verify(e => e.Commit(), Times.Once());
                }

                Assert.Single(resultList, true);
            }

            public static IEnumerable<object[]> IncorrectDailyRunAlarm
            {
                get
                {
                    Action<Fixture> defaultSetup = fixture =>
                    {
                        fixture.Inject(new TblMasterDailyRunResource { });
                        fixture.Inject(new TblMasterSite { });
                        fixture.Inject(new TblMasterCustomerLocation { });
                        fixture.Inject(new TblMasterEmployee { });

                        fixture.Register(() => false);
                    };

                    #region Arrange 1 : acknowledged alarm

                    var guid1 = Guid.NewGuid();

                    var alarmRunList1 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(2, defaultSetup);
                    alarmRunList1[0].Guid = guid1;
                    alarmRunList1[0].FlagAcknowledged = true;

                    #endregion

                    #region Arrange 2 : incorrect guid but not acknowledged

                    var guid2 = Guid.NewGuid();

                    var alarmRunList2 = Util.CreateDummy<TblMasterDailyRunResource_Alarm>(2, defaultSetup);
                    alarmRunList2[0].FlagAcknowledged = false;

                    #endregion

                    yield return new object[] { guid1, alarmRunList1 };
                    yield return new object[] { guid2, alarmRunList2 };
                }
            }

            [Theory]
            [MemberData(nameof(IncorrectDailyRunAlarm))]            
            public void IncorrectDailyRunAlarm_ShouldFailed(Guid dailyRunAlarmGuid, List<TblMasterDailyRunResource_Alarm> dailyRunAlarmViewModelList)
            {
                var userAcknowledge = CreateDummy<string>();
                var dateTimeAcknowledge = DateTime.Now;

                #region Arrange    

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindOne(It.IsAny<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>()))
                    .Returns<Expression<Func<TblMasterDailyRunResource_Alarm, bool>>>(expr =>
                    {
                        return dailyRunAlarmViewModelList.FirstOrDefault(expr.Compile());
                    });

                _mockAlarmHubBroadcastService
                    .Setup(fn => fn.BroadcastAlarmAcknowledged(It.IsAny<Guid>()))
                    .Verifiable();

                _mockDailyRunHistoryRepo
                    .Setup(fn => fn.CreateDailyRunLog(It.IsAny<MasterDailyRunLogView>()))
                    .Verifiable();

                #endregion

                var result = _service.AcknowledgeAlarm(dailyRunAlarmGuid, userAcknowledge, dateTimeAcknowledge);

                Assert.False(result);
                _mockAlarmHubBroadcastService.Verify(fn => fn.BroadcastAlarmAcknowledged(It.IsAny<Guid>()), Times.Never());
                _mockDailyRunAlarmRepo.Verify(fn => fn.Modify(It.IsAny<TblMasterDailyRunResource_Alarm>()), Times.Never());

                _mockUow.Verify(e => e.Commit(), Times.Never());
            }
        }

        #endregion
    }
}
