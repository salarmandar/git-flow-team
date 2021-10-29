
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Models.RunControl.LiabilityLimitModel;
using Bgt.Ocean.Repository.EntityFramework;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.TruckLiabilityLimit;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.TruckLiabilityLimit;
using DataSoruce.Test;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Service.Test.TruckLiabilityLimit
{
    public class TruckLiabilityLimitServiceTest : BaseTest
    {
        private readonly ITruckLiabilityLimitService _truckLiabilityLimitService;
        private Mock<IDbFactory<OceanDbEntities>> _mockDbFactory;
        private IDbFactory<OceanDbEntities> SetUpMockDbFactory()
        {
            var _db = new OceanDbEntities();
            _mockDbFactory = CreateMock<IDbFactory<OceanDbEntities>>();
            _mockDbFactory
                .Setup(fn => fn.GetCurrentDbContext)
                .Returns(_db);
            _mockDbFactory
                .Setup(fn => fn.GetNewDataContext)
                .Returns(_db);


            _mockDbFactory
           .Setup(fn => fn.GetCurrentDbContext.TblSystemJobAction)
           .Returns(MockSystemData<TblSystemJobAction>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblSystemServiceJobType)
            .Returns(MockSystemData<TblSystemServiceJobType>.Data);
            _mockDbFactory
             .Setup(fn => fn.GetCurrentDbContext.TblSystemEnvironmentMasterCountry)
            .Returns(MockSystemData<TblSystemEnvironmentMasterCountry>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblSystemEnvironmentMasterCountryValue)
            .Returns(MockSystemData<TblSystemEnvironmentMasterCountryValue>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.SFOTblSystemEnvironment_Global)
            .Returns(MockSystemData<SFOTblSystemEnvironment_Global>.Data);

            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterSite)
            .Returns(Mock63341_01<TblMasterSite>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterDailyRunResource)
            .Returns(Mock63341_01<TblMasterDailyRunResource>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobServiceStopLegs)
            .Returns(Mock63341_01<TblMasterActualJobServiceStopLegs>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobHeader)
            .Returns(Mock63341_01<TblMasterActualJobHeader>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobItemsLiability)
            .Returns(Mock63341_01<TblMasterActualJobItemsLiability>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobItemsCommodity)
            .Returns(Mock63341_01<TblMasterActualJobItemsCommodity>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCustomerLocation)
            .Returns(Mock63341_01<TblMasterCustomerLocation>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCustomer)
            .Returns(Mock63341_01<TblMasterCustomer>.Data);
            _mockDbFactory
             .Setup(fn => fn.GetCurrentDbContext.TblMasterRunResource)
             .Returns(Mock63341_01<TblMasterRunResource>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCurrency_ExchangeRate)
            .Returns(Mock63341_01<TblMasterCurrency_ExchangeRate>.Data);
            return _mockDbFactory.Object;
        }

        public TruckLiabilityLimitServiceTest()
        {
            var DbFac = SetUpMockDbFactory();
            var RepoList = new List<object>();
            RepoList.Add(new MasterDailyRunResourceRepository(DbFac));
            RepoList.Add(new MasterCurrencyRepository(DbFac));
            _truckLiabilityLimitService = Util.CreateInstanceWithRepository<TruckLiabilityLimitService>(RepoList);
        }
        private void SetUpData()
        {
            var RunData = Mock63341_01<TblMasterRunResource>.Data.AsEnumerable()
                                                              .Select(o => { o.LiabilityLimit = 1000; o.LiabilityLimitCurrency_Guid = Guid.Parse("4E51A9FA-2B5B-4CE1-A52A-7B412FCBFF2F"); return o; })
                                                              .ReBuildDbSet();


            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblSystemJobAction)
            .Returns(MockSystemData<TblSystemJobAction>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblSystemServiceJobType)
            .Returns(MockSystemData<TblSystemServiceJobType>.Data);
            _mockDbFactory
             .Setup(fn => fn.GetCurrentDbContext.TblSystemEnvironmentMasterCountry)
            .Returns(MockSystemData<TblSystemEnvironmentMasterCountry>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblSystemEnvironmentMasterCountryValue)
            .Returns(MockSystemData<TblSystemEnvironmentMasterCountryValue>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.SFOTblSystemEnvironment_Global)
            .Returns(MockSystemData<SFOTblSystemEnvironment_Global>.Data);

            _mockDbFactory
             .Setup(fn => fn.GetCurrentDbContext.TblMasterUser)
             .Returns(Mock63341_01<TblMasterUser>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterRouteGroup)
            .Returns(Mock63341_01<TblMasterRouteGroup>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterRouteGroup_Detail)
            .Returns(Mock63341_01<TblMasterRouteGroup_Detail>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterSite)
            .Returns(Mock63341_01<TblMasterSite>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterDailyRunResource)
            .Returns(Mock63341_01<TblMasterDailyRunResource>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobServiceStopLegs)
            .Returns(Mock63341_01<TblMasterActualJobServiceStopLegs>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobHeader)
            .Returns(Mock63341_01<TblMasterActualJobHeader>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobItemsLiability)
            .Returns(Mock63341_01<TblMasterActualJobItemsLiability>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobItemsCommodity)
            .Returns(Mock63341_01<TblMasterActualJobItemsCommodity>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCustomerLocation)
            .Returns(Mock63341_01<TblMasterCustomerLocation>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCustomer)
            .Returns(Mock63341_01<TblMasterCustomer>.Data);
            _mockDbFactory
             .Setup(fn => fn.GetCurrentDbContext.TblMasterCurrency)
             .Returns(Mock63341_01<TblMasterCurrency>.Data);
            _mockDbFactory
             .Setup(fn => fn.GetCurrentDbContext.TblMasterRunResource)
             .Returns(RunData);
        }

        public static IEnumerable<object[]> GetConfig()
        {

            return new List<object[]>
            {
                new object[] { true, false,false },
                new object[] { true, true, false },
                new object[] { true, true, true },

                new object[] { false, true, true },
                new object[] { false, false, true },
                new object[] { false, false, false }
            };

        }

        [Theory]
        [MemberData(nameof(GetConfig))]
        public void IsOverLialibityLimitWhenNoExistsJobs(
         bool flagValidateRunLiabilityLimit,
         bool percentageLiabilityLimitAlert,
         bool flagAllowExceedLiabilityLimit
          )
        {
            ApiSession.UserGuid = Guid.Parse("A2F6DE1D-1E08-422A-85BF-1902128377BD");
            var siteGuid = Guid.Parse("742346f7-f2e8-4347-ad07-77a2811ee636");
            var currencyGuid = Guid.Parse("4E51A9FA-2B5B-4CE1-A52A-7B412FCBFF2F");
            var model = new LiabilityLimitNoExistsJobsRequest()
            {
                NoJobsActionModel = new LiabilityLimitNoJobsActionModel
                {
                    SiteGuid = siteGuid,
                    RequestList = new List<LiabilityLimitNoJobsAction>()
                    {
                        new LiabilityLimitNoJobsAction {
                            DailyRunGuid_Target = Guid.Parse("D72EDD85-890B-4F58-AE60-3571C946ADE0"),
                            JobData = new RawJobDataView() {

                                JobItems = new RawItemsView() {

                                    Liabilities = new List<ItemsLibilityView>() {
                                        new ItemsLibilityView
                                        {
                                            JobStatusID = 1,
                                            Liability = 1000,
                                            JobGuid = Guid.NewGuid(),
                                            ItemState = EnumState.Added,
                                            LibilityGuid = Guid.NewGuid(),
                                            DocCurrencyGuid = currencyGuid
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }.NoJobsActionModel;

            SetUpData();

            _truckLiabilityLimitService.GetMock<IMasterCommodityRepository>().Setup(fn => fn.GetAllCommodityBySite(It.IsAny<Guid?>(), It.IsAny<Guid?>(), false)).Returns(Enumerable.Empty<CommodityView>());
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagValidateRunLiabilityLimit)).Returns(flagValidateRunLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.PercentageLiabilityLimitAlert)).Returns(percentageLiabilityLimitAlert);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagAllowExceedLiabilityLimit)).Returns(flagAllowExceedLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(0, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 0,
                MessageTextContent = "Passed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(6110, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 6110,
                MessageTextContent = "Continue Exceed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(-17341, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = -17341,
                MessageTextContent = "Alert Exceed"
            });
            _truckLiabilityLimitService.GetMock<IBaseRequest>().Setup(fn => fn.Data).Returns(new RequestBase()
            {
                UserLanguageGuid = Guid.Empty
            });

            var result = _truckLiabilityLimitService.IsOverLialibityLimitWhenNoExistsJobs(new LiabilityLimitNoExistsJobsRequest() { NoJobsActionModel = model });

            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetConfig))]
        public void IsOverLiabilityLimitWhenExistJobs(
           bool flagValidateRunLiabilityLimit,
           bool percentageLiabilityLimitAlert,
           bool flagAllowExceedLiabilityLimit
            )
        {
            ApiSession.UserGuid = Guid.Parse("A2F6DE1D-1E08-422A-85BF-1902128377BD");
            var siteGuid = Guid.Parse("742346f7-f2e8-4347-ad07-77a2811ee636");
            var model = new LiabilityLimitExistsJobsRequest()
            {
                JobsActionModel = new LiabilityLimitJobsActionModel
                {
                    SiteGuid = siteGuid,
                    RequestList = new List<LiabilityLimitJobsAction>()
                    {
                        new LiabilityLimitJobsAction {
                            DailyRunGuid_Target = Guid.Parse("D72EDD85-890B-4F58-AE60-3571C946ADE0"),
                            JobGuids = new List<RawExistJobView> { new RawExistJobView { JobGuid = Guid.Parse("8A9A217E-C7F9-41E4-B20B-431C730A4E9A"),JobAction ="P" } }
                        }
                    }
                }
            }.JobsActionModel;

            SetUpData();

            _truckLiabilityLimitService.GetMock<IMasterCommodityRepository>().Setup(fn => fn.GetAllCommodityBySite(It.IsAny<Guid?>(), It.IsAny<Guid?>(), false)).Returns(Enumerable.Empty<CommodityView>());
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagValidateRunLiabilityLimit)).Returns(flagValidateRunLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.PercentageLiabilityLimitAlert)).Returns(percentageLiabilityLimitAlert);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagAllowExceedLiabilityLimit)).Returns(flagAllowExceedLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(0, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 0,
                MessageTextContent = "Passed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(6110, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 6110,
                MessageTextContent = "Continue Exceed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(-17341, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = -17341,
                MessageTextContent = "Alert Exceed"
            });
            _truckLiabilityLimitService.GetMock<IBaseRequest>().Setup(fn => fn.Data).Returns(new RequestBase()
            {
                UserLanguageGuid = Guid.Empty
            });

            var result = _truckLiabilityLimitService.IsOverLiabilityLimitWhenExistJobs(new LiabilityLimitExistsJobsRequest() { JobsActionModel = model });

            Assert.NotNull(result);
        }


        [Theory]
        [MemberData(nameof(GetConfig))]
        public void IsOverLiabilityLimitWhenNoExistsItems(
           bool flagValidateRunLiabilityLimit,
           bool percentageLiabilityLimitAlert,
           bool flagAllowExceedLiabilityLimit
            )
        {
            ApiSession.UserGuid = Guid.Parse("A2F6DE1D-1E08-422A-85BF-1902128377BD");
            var siteGuid = Guid.Parse("742346f7-f2e8-4347-ad07-77a2811ee636");
            var currencyGuid = Guid.Parse("4E51A9FA-2B5B-4CE1-A52A-7B412FCBFF2F");
            var jobGuid = Guid.NewGuid();
            var liaGuid = Guid.NewGuid();
            RawItemsView itemList = new RawItemsView();
            itemList.Liabilities = new ItemsLibilityView[] {new ItemsLibilityView {
                                DocCurrencyGuid = currencyGuid,
                                Liability = 10000,
                                LibilityGuid  = liaGuid,
                                JobGuid = jobGuid,
                                ItemState = EnumState.Added
                            }};


            var model = new LiabilityLimitNoExistsItemsRequest()
            {
                ItemsActionModel = new LiabilityLimitItemsActionModel
                {
                    SiteGuid = siteGuid,
                    RequestList = new List<LiabilityLimitItemsAction>()
                    {
                        new LiabilityLimitItemsAction { DailyRunGuid_Target =  Guid.Parse("D72EDD85-890B-4F58-AE60-3571C946ADE0"), JobItems = itemList }
                    }
                }
            }.ItemsActionModel;

            SetUpData();

            _truckLiabilityLimitService.GetMock<IMasterCommodityRepository>().Setup(fn => fn.GetAllCommodityBySite(It.IsAny<Guid?>(), It.IsAny<Guid?>(),false)).Returns(Enumerable.Empty<CommodityView>());
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagValidateRunLiabilityLimit)).Returns(flagValidateRunLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.PercentageLiabilityLimitAlert)).Returns(percentageLiabilityLimitAlert);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagAllowExceedLiabilityLimit)).Returns(flagAllowExceedLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(0, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 0,
                MessageTextContent = "Passed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(6110, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 6110,
                MessageTextContent = "Continue Exceed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(-17341, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = -17341,
                MessageTextContent = "Alert Exceed"
            });
            _truckLiabilityLimitService.GetMock<IBaseRequest>().Setup(fn => fn.Data).Returns(new RequestBase()
            {
                UserLanguageGuid = Guid.Empty
            });

            var result = _truckLiabilityLimitService.IsOverLiabilityLimitWhenNoExistsItems(new LiabilityLimitNoExistsItemsRequest() { ItemsActionModel = model });

            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetConfig))]
        public void GetTruckLiabilityLimitPercentageAlert(
           bool flagValidateRunLiabilityLimit,
           bool percentageLiabilityLimitAlert,
           bool flagAllowExceedLiabilityLimit
            )
        {
            ApiSession.UserGuid = Guid.Parse("A2F6DE1D-1E08-422A-85BF-1902128377BD");
            var siteGuid = Guid.Parse("742346f7-f2e8-4347-ad07-77a2811ee636");
            var model = new LiabilityLimitExistsRunRequest()
            {
                RunsActionModel = new LiabilityLimitRunsActionModel
                {
                    SiteGuid = siteGuid,
                    DailyRunGuids = new Guid?[] { Guid.Parse("D72EDD85-890B-4F58-AE60-3571C946ADE0") }
                }
            }.RunsActionModel;

            SetUpData();

            _truckLiabilityLimitService.GetMock<IMasterCommodityRepository>().Setup(fn => fn.GetAllCommodityBySite(It.IsAny<Guid?>(), It.IsAny<Guid?>(), false)).Returns(Enumerable.Empty<CommodityView>());
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagValidateRunLiabilityLimit)).Returns(flagValidateRunLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.PercentageLiabilityLimitAlert)).Returns(percentageLiabilityLimitAlert);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagAllowExceedLiabilityLimit)).Returns(flagAllowExceedLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(0, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 0,
                MessageTextContent = "Passed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(6110, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 6110,
                MessageTextContent = "Continue Exceed"
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(-17341, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = -17341,
                MessageTextContent = "Alert Exceed"
            });
            _truckLiabilityLimitService.GetMock<IBaseRequest>().Setup(fn => fn.Data).Returns(new RequestBase()
            {
                UserLanguageGuid = Guid.Empty
            });
            var result = _truckLiabilityLimitService.GetTruckLiabilityLimitPercentageAlert(new LiabilityLimitExistsRunRequest() { RunsActionModel = model });

            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(GetConfig))]
        public void GetConvertBankCleanOutTotalLiability(
            bool flagValidateRunLiabilityLimit,
           bool percentageLiabilityLimitAlert,
           bool flagAllowExceedLiabilityLimit
            )
        {
            ApiSession.UserGuid = Guid.Parse("A2F6DE1D-1E08-422A-85BF-1902128377BD");
            var siteGuid = Guid.Parse("742346f7-f2e8-4347-ad07-77a2811ee636");
            var model = new ConvertBankCleanOutTotalLiabilityRequest()
            {
                JobModel = new ConvertBankCleanOutLiabilityModel
                {
                    SiteGuid = siteGuid,
                    RawJobs = new BankCleanOutJobView[] {
                        new BankCleanOutJobView {
                            FlagTemp = false,
                            JobGuid  =  Guid.Parse("69E23E7C-84CE-484C-B3B7-7F3C619A9CF2")
                        }
                    }
                }
            }.JobModel;

            SetUpData();
            _truckLiabilityLimitService.GetMock<IMasterCommodityRepository>().Setup(fn => fn.GetAllCommodityBySite(It.IsAny<Guid?>(), It.IsAny<Guid?>(), false)).Returns(Enumerable.Empty<CommodityView>());
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagValidateRunLiabilityLimit)).Returns(flagValidateRunLiabilityLimit);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.PercentageLiabilityLimitAlert)).Returns(percentageLiabilityLimitAlert);
            _truckLiabilityLimitService.GetMock<ISystemEnvironmentMasterCountryRepository>().Setup(fn => fn.FindAppkeyValueByEnumAppkeyName(model.SiteGuid, EnumAppKey.FlagAllowExceedLiabilityLimit)).Returns(flagAllowExceedLiabilityLimit);
            _truckLiabilityLimitService.GetMock<IBaseRequest>().Setup(fn => fn.Data).Returns(new RequestBase()
            {
                UserLanguageGuid = Guid.Empty
            });
            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(0, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 0,
                MessageTextContent = "Passed"
            });

            _truckLiabilityLimitService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(-1, Guid.Empty)).Returns(new TblSystemMessage
            {
                MsgID = 0,
                MessageTextContent = "Failed"
            });
            var result = _truckLiabilityLimitService.GetConvertBankCleanOutTotalLiability(new ConvertBankCleanOutTotalLiabilityRequest() { JobModel = model });

            Assert.NotNull(result);
        }
    }
}
