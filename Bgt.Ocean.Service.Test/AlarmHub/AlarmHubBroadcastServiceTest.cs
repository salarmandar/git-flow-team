using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Service.Implementations.Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Models;
using System.Dynamic;
using Bgt.Ocean.Service.Messagings.AlarmHub;
using Bgt.Ocean.Models.Group;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Service.Test.AlarmHub
{
    public class AlarmHubBroadcastServiceTest
    {
        public class BroadcastAlarm : BaseTest
        {
            private readonly IAlarmHubBroadcastService _alarmHubBroadcastService;
            private readonly Mock<IMasterDailyRunResourceAlarmRepository> _mockDailyRunAlarmRepo;
            private readonly Mock<IHubConnectionContext<dynamic>> _mockHubConnection;
            private readonly Mock<IMasterGroupRepository> _mockMasterGroupRepo;

            public BroadcastAlarm()
            {
                _mockHubConnection = CreateMock<IHubConnectionContext<dynamic>>();
                _mockMasterGroupRepo = CreateMock<IMasterGroupRepository>();
                _mockDailyRunAlarmRepo = CreateMock<IMasterDailyRunResourceAlarmRepository>();

                _alarmHubBroadcastService = new AlarmHubBroadcastService(
                        _mockHubConnection.Object,
                        _mockMasterGroupRepo.Object,
                        _mockDailyRunAlarmRepo.Object,
                        _mockLogErrorRepo.Object
                    );
            }


            [Theory]
            [InlineData(null, null)]
            [InlineData("AE96E217-03CB-4449-BE4F-BE01F6A04FA7", "ALARM_Location")]
            public void ValidLocation_ShouldWorkingNormally(string locationGuidStr, string locationName)
            {
                dynamic proxy = new ExpandoObject();

                #region Arrange

                var group = Util.CreateDummy<MasterGroupAlarmModel>(1);
                group.ForEach(e => e.FlagAllowAcknowledge = true);

                proxy.onAlarmTriggered = new Action<AlarmHubTriggeredResponse>(alertInfo =>
                {
                    Assert.Equal(locationName, alertInfo.LocationName);
                });

                _mockMasterGroupRepo
                    .Setup(fn => fn.GetPermittedAlarmGroupBySite(It.IsAny<Guid>()))
                    .Returns(Util.CreateDummy<MasterGroupAlarmModel>(1));

                _mockHubConnection
                    .Setup(fn => fn.Group(It.IsAny<string>()))
                    .Returns((ExpandoObject)proxy)
                    .Verifiable();

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.GetNewDailyRunAlarmByRunGuid(It.IsAny<Guid>()))
                    .Returns(new TblMasterDailyRunResource_Alarm
                    {
                        MasterCustomerLocation_Guid = locationGuidStr.ToGuid(),
                        MasterSite_Guid = Guid.NewGuid(),
                        TblMasterCustomerLocation = new TblMasterCustomerLocation
                        {
                            BranchName = locationName
                        }
                    });


                #endregion

                _alarmHubBroadcastService.BroadcastAlarm(Guid.NewGuid());

                _mockHubConnection.VerifyAll();
            }

            public static IEnumerable<object[]> ValidRun()
            {
                var validRunModel1 = new ValidRunModel(Guid.NewGuid(), Guid.NewGuid());
                var masterDailyRunAlarm1 = new List<TblMasterDailyRunResource_Alarm>
                {
                    new TblMasterDailyRunResource_Alarm
                    {
                        MasterDailyRunResource_Guid = validRunModel1.DailyRunGuid,
                        MasterSite_Guid = validRunModel1.SiteGuid,
                        MasterCustomerLocation_Guid = Guid.NewGuid()
                    }
                };
                var masterGroup1 = Util.CreateDummy<MasterGroupAlarmModel>(1);

                //////////////////////////////////////////////////////////////////////////////////////////////

                var validRunModel2 = new ValidRunModel(Guid.NewGuid(), Guid.NewGuid());
                var masterDailyRunAlarm2 = new List<TblMasterDailyRunResource_Alarm>
                {
                    new TblMasterDailyRunResource_Alarm {  MasterDailyRunResource_Guid = validRunModel2.DailyRunGuid, MasterSite_Guid = validRunModel2.SiteGuid },
                    new TblMasterDailyRunResource_Alarm {  MasterDailyRunResource_Guid = Guid.NewGuid(), MasterSite_Guid = Guid.NewGuid(), FlagAcknowledged = true }
                };
                var masterGroup2 = Util.CreateDummy<MasterGroupAlarmModel>(1);

                //////////////////////////////////////////////////////////////////////////////////////////////

                var validRunModel3 = new ValidRunModel(Guid.NewGuid(), Guid.NewGuid());
                var masterDailyRunAlarm3 = new List<TblMasterDailyRunResource_Alarm>
                {
                    new TblMasterDailyRunResource_Alarm {  MasterDailyRunResource_Guid = Guid.NewGuid(), MasterSite_Guid = Guid.NewGuid(), FlagAcknowledged = true },
                    new TblMasterDailyRunResource_Alarm {  MasterDailyRunResource_Guid = Guid.NewGuid(), MasterSite_Guid = Guid.NewGuid(), FlagAcknowledged = true, FlagDeactivated = true },
                    new TblMasterDailyRunResource_Alarm {  MasterDailyRunResource_Guid = validRunModel3.DailyRunGuid, MasterSite_Guid = validRunModel3.SiteGuid },
                };
                var masterGroup3 = Util.CreateDummy<MasterGroupAlarmModel>(1);

                //////////////////////////////////////////////////////////////////////////////////////////////

                var validRunModel4 = new ValidRunModel(Guid.NewGuid(), Guid.NewGuid());
                var masterDailyRunAlarm4 = new List<TblMasterDailyRunResource_Alarm>
                {
                    new TblMasterDailyRunResource_Alarm {  MasterDailyRunResource_Guid = validRunModel4.DailyRunGuid, MasterSite_Guid = validRunModel4.SiteGuid }
                };
                var masterGroup4 = Util.CreateDummy<MasterGroupAlarmModel>(3);

                return new List<object[]>
                    {
                        new object[] { validRunModel1, masterDailyRunAlarm1, masterGroup1, 1 }, // 1 run, 1 valid run, 1 group = expect triggered 1 time
                        new object[] { validRunModel2, masterDailyRunAlarm2, masterGroup2, 1 }, // 2 run, 1 valid run, 1 invalid run, 1 group = expect triggered 1 time
                        new object[] { validRunModel3, masterDailyRunAlarm3, masterGroup3, 1 }, // 3 run, 1 valid run, 2 invalid run, 1 group = expect triggered 1 time
                        new object[] { validRunModel4, masterDailyRunAlarm4, masterGroup4, 3 }, // 1 run, 1 valid run, 3 group = expect triggered 3 times
                    };
            }

            [Theory]
            [MemberData(nameof(ValidRun))]
            public void ValidRun_ShouldWorkWithValidGroup(ValidRunModel validInput, List<TblMasterDailyRunResource_Alarm> dailyRunAlarmList, IEnumerable<MasterGroupAlarmModel> masterGrouplist, int expectedCount)
            {
                dynamic proxy = new ExpandoObject();
                int triggeredCount = 0;

                #region Arrange

                proxy.onAlarmTriggered = new Action<AlarmHubTriggeredResponse>(alertInfo =>
                {
                    var input = validInput;

                    Assert.NotNull(input);
                    Assert.Equal(input.DailyRunGuid, alertInfo.MasterDailyRunResource_Guid);
                    Assert.Equal(input.SiteGuid, alertInfo.MasterSite_Guid);

                    triggeredCount += 1;
                });

                _mockMasterGroupRepo
                    .Setup(fn => fn.GetPermittedAlarmGroupBySite(It.IsAny<Guid>()))
                    .Returns(masterGrouplist);

                _mockHubConnection
                    .Setup(fn => fn.Group(It.IsAny<string>()))
                    .Returns((ExpandoObject)proxy);

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.GetNewDailyRunAlarmByRunGuid(It.IsAny<Guid>()))
                    .Returns<Guid>(guid =>
                    {
                        return dailyRunAlarmList.FirstOrDefault(e => e.MasterDailyRunResource_Guid == guid && !e.FlagAcknowledged && !e.FlagDeactivated);
                    });

                #endregion

                _alarmHubBroadcastService.BroadcastAlarm(validInput.DailyRunGuid);

                Assert.Equal(expectedCount, triggeredCount);
            }

            public static IEnumerable<object[]> InvalidRun()
            {
                var masterDailyRunAlarm1 = new List<TblMasterDailyRunResource_Alarm>
                {
                    new TblMasterDailyRunResource_Alarm {  FlagAcknowledged = true, FlagDeactivated = true }
                };
                var masterGroup1 = new List<MasterGroupAlarmModel>(1);

                //////////////////////////////////////////////////////////////////////////////////////////////

                var masterDailyRunAlarm2 = new List<TblMasterDailyRunResource_Alarm>
                {
                    new TblMasterDailyRunResource_Alarm {  FlagAcknowledged = true, FlagDeactivated = false }
                };
                var masterGroup2 = new List<MasterGroupAlarmModel>(1);

                //////////////////////////////////////////////////////////////////////////////////////////////

                var masterDailyRunAlarm3 = new List<TblMasterDailyRunResource_Alarm>
                {
                    new TblMasterDailyRunResource_Alarm {  FlagAcknowledged = false, FlagDeactivated = false }
                };
                var masterGroup3 = new List<MasterGroupAlarmModel>(0);

                return new List<object[]>
                {
                    new object[] { masterDailyRunAlarm1, masterGroup1 }, // has acknowledged and deactivated run, has 1 group
                    new object[] { masterDailyRunAlarm2, masterGroup2 }, // has acknowledged but not deactivate run, has 1 group
                    new object[] { masterDailyRunAlarm3, masterGroup3 }  // has run, but no group
                };
            }

            [Theory]
            [MemberData(nameof(InvalidRun))]
            public void InvalidRunOrGroup_ShouldNotTriggeredAlarm(List<TblMasterDailyRunResource_Alarm> dailyRunAlarmList, IEnumerable<MasterGroupAlarmModel> masterGrouplist)
            {
                int triggeredCount = 0;
                dynamic proxy = new ExpandoObject();

                #region Arrange


                proxy.onAlarmTriggered = new Action<AlarmHubTriggeredResponse>(alertInfo =>
                {
                    triggeredCount += 1;
                });

                _mockMasterGroupRepo
                    .Setup(fn => fn.GetPermittedAlarmGroupBySite(It.IsAny<Guid>()))
                    .Returns(masterGrouplist);

                _mockHubConnection
                    .Setup(fn => fn.Group(It.IsAny<string>()))
                    .Returns((ExpandoObject)proxy);

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable())
                    .Returns(dailyRunAlarmList.AsQueryable());

                #endregion

                Assert.Equal(0, triggeredCount);

            }

            public static IEnumerable<object[]> ValidGroup()
            {
                var masterGroup1 = Util.CreateDummy<MasterGroupAlarmModel>(1).Select(e =>
                {
                    e.FlagAllowAcknowledge = true;
                    e.FlagAllowDeactivate = true;
                    return e;
                });

                var masterGroup2 = Util.CreateDummy<MasterGroupAlarmModel>(1).Select(e =>
                {
                    e.FlagAllowAcknowledge = true;
                    e.FlagAllowDeactivate = false;
                    return e;
                });

                var masterGroup3 = Util.CreateDummy<MasterGroupAlarmModel>(2).Select(e =>
                {
                    e.FlagAllowAcknowledge = true;
                    e.FlagAllowDeactivate = false;
                    return e;
                });

                var masterGroup4 = Util.CreateDummy<MasterGroupAlarmModel>(2).ToList();
                masterGroup4[0].FlagAllowAcknowledge = false;
                masterGroup4[1].FlagAllowAcknowledge = true;

                return new List<object[]>
                {
                    new object[] { masterGroup1, 1 }, // allow acknowledge and allow deactivate group
                    new object[] { masterGroup2, 1 }, // allow acknowledge only
                    new object[] { masterGroup3, 2 }, // multiple group which allow acknowledge only
                    new object[] { masterGroup4, 1 }  // multiple group which allow acknowledge in some group
                };
            }

            [Theory]
            [MemberData(nameof(ValidGroup))]
            public void ValidRunAndValidGroup_ShouldSetAllowAcknowledge(IEnumerable<MasterGroupAlarmModel> masterGrouplist, int expectedCountTrue)
            {
                var dailyRunGuid = Guid.NewGuid();
                var siteGuid = Guid.NewGuid();

                int countTrue = 0;
                dynamic proxy = new ExpandoObject();

                proxy.onAlarmTriggered = new Action<AlarmHubTriggeredResponse>(alertInfo =>
                {
                    countTrue += alertInfo.FlagAllowAcknowledged ? 1 : 0;
                });

                _mockMasterGroupRepo
                    .Setup(fn => fn.GetPermittedAlarmGroupBySite(It.IsAny<Guid>()))
                    .Returns(masterGrouplist);

                _mockHubConnection
                    .Setup(fn => fn.Group(It.IsAny<string>()))
                    .Returns((ExpandoObject)proxy);

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.GetNewDailyRunAlarmByRunGuid(dailyRunGuid))
                    .Returns(new TblMasterDailyRunResource_Alarm { MasterDailyRunResource_Guid = dailyRunGuid, MasterSite_Guid = siteGuid });

                _alarmHubBroadcastService.BroadcastAlarm(dailyRunGuid);

                Assert.Equal(expectedCountTrue, countTrue);
            }

            public static IEnumerable<object[]> InvalidGroup()
            {
                var masterGroup1 = Util.CreateDummy<MasterGroupAlarmModel>(1).Select(e =>
                {
                    e.FlagAllowAcknowledge = false;
                    e.FlagAllowDeactivate = false;
                    return e;
                });

                var masterGroup2 = Util.CreateDummy<MasterGroupAlarmModel>(1).Select(e =>
                {
                    e.FlagAllowAcknowledge = false;
                    e.FlagAllowDeactivate = true;
                    return e;
                });

                var masterGroup3 = Util.CreateDummy<MasterGroupAlarmModel>(2).Select(e =>
                {
                    e.FlagAllowAcknowledge = false;
                    e.FlagAllowDeactivate = true;
                    return e;
                });

                return new List<object[]>
                {
                    new object[] { masterGroup1 }, // not allow both acknowledge and deactivate
                    new object[] { masterGroup2 }, // not allow acknowledge but allow deactivate
                    new object[] { masterGroup3 }  // not allow acknowledge in multiple group
                };
            }

            [Theory]
            [MemberData(nameof(InvalidGroup))]
            public void ValidRunAndInvalidGroup_ShouldNotAllowAcknowledge(IEnumerable<MasterGroupAlarmModel> masterGrouplist)
            {
                var dailyRunGuid = Guid.NewGuid();
                var siteGuid = Guid.NewGuid();
                dynamic proxy = new ExpandoObject();

                proxy.onAlarmTriggered = new Action<AlarmHubTriggeredResponse>(alertInfo =>
                {
                    Assert.False(alertInfo.FlagAllowAcknowledged);
                });

                _mockMasterGroupRepo
                    .Setup(fn => fn.GetPermittedAlarmGroupBySite(It.IsAny<Guid>()))
                    .Returns(masterGrouplist);

                _mockHubConnection
                    .Setup(fn => fn.Group(It.IsAny<string>()))
                    .Returns((ExpandoObject)proxy);

                _mockDailyRunAlarmRepo
                    .Setup(fn => fn.FindAllAsQueryable())
                    .Returns(new List<TblMasterDailyRunResource_Alarm>
                    {
                       new TblMasterDailyRunResource_Alarm { MasterDailyRunResource_Guid = dailyRunGuid, MasterSite_Guid = siteGuid }
                    }.AsQueryable());

                _alarmHubBroadcastService.BroadcastAlarm(dailyRunGuid);
            }

            public class ValidRunModel
            {
                public Guid SiteGuid { get; set; }
                public Guid DailyRunGuid { get; set; }

                public ValidRunModel(Guid siteGuid, Guid dailyRunGuid)
                {
                    SiteGuid = siteGuid;
                    DailyRunGuid = dailyRunGuid;
                }
            }
        }
    }
}
