

using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Denomination;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.Denomination;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.Denomination;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Service.Test.Denimination
{
    public class DenominationServiceTest : BaseTest
    {
        private readonly IDenominationService _denominationService;
        private readonly IMasterActualJobItemsLiabilityDenominationRepository _masterActualJobItemsLiabilityDenominationRepository;

        public DenominationServiceTest()
        {
            _denominationService = Util.CreateInstance<DenominationService>();
        }
        /// <summary>
        /// => TFS#71743:Thailand Customers have specific requirements about denomination for each cash preparation order.
        /// </summary>
        [Fact]
        public void GetMasterDenominationTest()
        {
            var request = new GetDenominationRequest();
            request.CurrencyGuid = Guid.NewGuid();

            _denominationService.GetMock<IBaseRequest>().Setup(fn => fn.Data)
                              .Returns(new RequestBase()
                              {
                                  UserName = "unittest",
                                  UserLanguageGuid = Guid.Empty
                              });

            var denoGuids = new Guid[]
                      {
                        EnumHelper.GetDescription(DenoUnit.Coin).ToGuid(),
                        EnumHelper.GetDescription(DenoUnit.BankNote).ToGuid()
                      };
            // MockData list MasterDenominationView
            var denoList = Enumerable.Range(0, 5).Select(o => new DenominationDetailView { Qty = o });
            _denominationService.GetMock<IMasterDenominationRepository>()
                                .Setup(fn => fn.GetDenominationByDenoUnit(request.CurrencyGuid, Guid.Empty, denoGuids))
                                .Returns(denoList);

            var result = _denominationService.GetMasterDenomination(request);

            Assert.NotNull(result);
            Assert.True(denoList.Count() == result.DenominationList.Count());
            Assert.True(result != null);
        }

        [Fact]
        public void GetLiabilityDenominationByLiabilityGuid()
        {
            var request = new GetDenominationRequest();
            request.CurrencyGuid = Guid.NewGuid();

            _denominationService.GetMock<IBaseRequest>().Setup(fn => fn.Data)
                              .Returns(new RequestBase()
                              {
                                  UserName = "unittest",
                                  UserLanguageGuid = Guid.Empty
                              });

            var denoGuids = new Guid[]
                   {
                        EnumHelper.GetDescription(DenoUnit.Coin).ToGuid(),
                        EnumHelper.GetDescription(DenoUnit.BankNote).ToGuid()
                   };
            // MockData list MasterDenominationView
            var denoList = Enumerable.Range(0, 5).Select(o => new DenominationDetailView { Qty = o });
            _denominationService.GetMock<IMasterActualJobItemsLiabilityDenominationRepository>()
                                .Setup(fn => fn.GetDenominationByDenoUnit(request.CurrencyGuid, It.IsAny<Guid>(), denoGuids))
                                .Returns(denoList);
            var result = _denominationService.GetLiabilityDenominationByLiabilityGuid(request);

            Assert.NotNull(result);
        }

        public static IEnumerable<object[]> DataRequest()
        {
            Util.CreateFakeContext();
            IEnumerable<DenominationDetailView> denoDetailList = Enumerable.Range(0, 3).Select((o, i) => new DenominationDetailView
            {
                DenoGuid = Guid.Empty,
                DenoName = "DenoName " + i,
                DenoUnitGuid = Guid.Empty,
                DenoValue = i,
                ItemState = i == 1 ? EnumState.Added : i == 2 ? EnumState.Modified : EnumState.Deleted,
                LiabilityDenoGuid = i == 1 ? Guid.Empty : new Guid("CF8574D1-792C-49EC-A06B-00F8B4C39210"), // liabilityGuid --> TblMasterActualJobItemsLiability 
                Qty = i,
                Type = "",
                Value = i,
            });
            IEnumerable<DenominationHeaderView> denominationData = Enumerable.Range(0, 1).Select((o, i) => new DenominationHeaderView
            {
                DenominationList = denoDetailList,
                CurrencyGuid = Guid.Empty,
                LiabilityGuid = Guid.Empty,
            });
            IEnumerable<DenominationHeaderView> denominationDataUpdate = Enumerable.Range(0, 1).Select((o, i) => new DenominationHeaderView
            {
                DenominationList = denoDetailList,
                CurrencyGuid = Guid.Empty,
                LiabilityGuid = new Guid("CF8574D1-792C-49EC-A06B-00F8B4C39210"),
            });
            IEnumerable<DenominationHeaderView> denominationDataDelete = Enumerable.Range(0, 1).Select((o, i) => new DenominationHeaderView
            {
                DenominationList = denoDetailList,
                CurrencyGuid = Guid.Empty,
                LiabilityGuid = new Guid("CF8574D1-792C-49EC-A06B-00F8B4C39210"),
                FlagDeletePrevDeno = true
            });

            SetDenominationAsyncRequest InsertData = new SetDenominationAsyncRequest()
            {
                DenominationHeaderList = denominationData,
                LanguageGuid = Guid.NewGuid(),
                UserName = "Unit Test UserName1",
                UniversalDatetime = DateTimeOffset.Now,
            };
            SetDenominationAsyncRequest UpdateData = new SetDenominationAsyncRequest()
            {
                DenominationHeaderList = denominationDataUpdate,
                LanguageGuid = Guid.NewGuid(),
                UserName = "Unit Test UserName2",
                UniversalDatetime = DateTimeOffset.Now,
            };
            SetDenominationAsyncRequest DeleteData = new SetDenominationAsyncRequest()
            {
                DenominationHeaderList = denominationDataDelete,
                LanguageGuid = Guid.NewGuid(),
                UserName = "Unit Test UserName2",
                UniversalDatetime = DateTimeOffset.Now,
            };
            SetDenominationAsyncRequest ExeptedData = new SetDenominationAsyncRequest()
            {
                DenominationHeaderList = denominationData,
                LanguageGuid = Guid.NewGuid(),
                UserName = "Unit Test UserName2",
                UniversalDatetime = DateTimeOffset.Now,
            };

            var liabilityDeno = new TblMasterActualJobItemsLiability { MasterCurrency_Guid = Guid.NewGuid() };

            var data = new List<object[]> {
                new object[] { InsertData, liabilityDeno, Times.Once(), Times.Never(), Times.Never(), 0, true },
                new object[] { UpdateData, liabilityDeno, Times.Never(), Times.Once(), Times.Never(), 0, true },
                new object[] { DeleteData, liabilityDeno, Times.Never(), Times.Never(), Times.Once(), 0, true },
                //new object[] { ExeptedData, liabilityDeno, Times.Never(), Times.Never(), Times.Once(), 0, false },

            };

            return data;
        }
        /// <summary>
        /// => TFS#71743:Thailand Customers have specific requirements about denomination for each cash preparation order.
        /// </summary>
        [Theory]
        [MemberData(nameof(DataRequest))]
        public void InsertOrUpdateDenominationTest(SetDenominationAsyncRequest request, TblMasterActualJobItemsLiability liabilityDeno, Times createTime, Times updateTime, Times deleteTime, int msgID, bool? exepted)
        {
            var times = Times.Once();
            // message 
            _denominationService.GetMock<ISystemMessageRepository>().Setup(fn => fn.FindByMsgId(It.IsAny<int>(), It.IsAny<Guid>())).Returns(new TblSystemMessage { MsgID = 0, MessageTextContent = "Passed" });
            // update
            if (updateTime == times) _denominationService.GetMock<IMasterActualJobItemsLiabilityDenominationRepository>().Setup(fn => fn.FindById(It.IsAny<Guid>())).Returns(new TblMasterActualJobItemsLiability_Denomination
            {
                Qty = 1,
                DenominationValue = It.IsAny<double>(),
                Value = It.IsAny<double>(),
            });

            var result = _denominationService.InsertOrUpdateDenominationAsync(request);
            TblMasterActualJobItemsLiability liabilityGuid = new TblMasterActualJobItemsLiability() { Guid = new Guid("CF8574D1-792C-49EC-A06B-00F8B4C39210") };
            TblMasterActualJobItemsLiability_Denomination liabilityDenoGuid = new TblMasterActualJobItemsLiability_Denomination() { Guid = new Guid("CF8574D1-792C-49EC-A06B-00F8B4C39210") };

            // create
            _denominationService.GetMock<IMasterActualJobItemsLiabilityRepository>().Setup(fn => fn.FindById(It.IsAny<Guid>())).Returns(liabilityGuid);
            _denominationService.GetMock<IMasterActualJobItemsLiabilityDenominationRepository>().Verify(fn => fn.CreateRange(It.IsAny<IEnumerable<TblMasterActualJobItemsLiability_Denomination>>()), createTime);
           
            // update
            _denominationService.GetMock<IMasterActualJobItemsLiabilityRepository>().Setup(fn => fn.FindById(It.IsAny<Guid>())).Returns(liabilityGuid);
            _denominationService.GetMock<IMasterActualJobItemsLiabilityDenominationRepository>().Setup(fn => fn.FindById(It.IsAny<Guid>())).Returns(liabilityDenoGuid);
            _denominationService.GetMock<IMasterActualJobItemsLiabilityDenominationRepository>().Verify(fn => fn.Modify(It.IsAny<TblMasterActualJobItemsLiability_Denomination>()), updateTime);
            
            // delete
            _denominationService.GetMock<IMasterActualJobItemsLiabilityRepository>().Setup(fn => fn.FindById(It.IsAny<Guid>())).Returns(liabilityDeno);
            var liabilityDenoList = Enumerable.Range(0, 5).Select(o => new TblMasterActualJobItemsLiability_Denomination { Qty = 1, Value = 2 });
            _denominationService.GetMock<IMasterActualJobItemsLiabilityDenominationRepository>().Setup(fn => fn.FindByLiabilityGuid(It.IsAny<Guid>())).Returns(liabilityDenoList);
            _denominationService.GetMock<IMasterActualJobItemsLiabilityDenominationRepository>().Verify(fn => fn.RemoveRange(It.IsAny<IEnumerable<TblMasterActualJobItemsLiability_Denomination>>()), deleteTime);

            Assert.NotNull(result);
        }

    }
}
