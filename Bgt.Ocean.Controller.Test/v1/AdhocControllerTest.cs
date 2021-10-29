using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.WebAPI.Controllers.Internals.v1;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Bgt.Ocean.Controller.Test.v1
{
    public class AdhocControllerTest
    {
        private readonly v1_AdhocController _controller;
        private readonly Mock<IMachineService> _mockMachineService;
        private readonly Mock<IAdhocService> _mockAdhocService;

        public AdhocControllerTest()
        {
            _mockMachineService = new Mock<IMachineService>();

            _mockAdhocService = new Mock<IAdhocService>();
            _mockAdhocService
                .Setup(fn => fn.CreateResponseFromID(It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns((int msgId, Guid langaugeGuid) =>
                {
                    return new CreateJobAdHocResponse(new TblSystemMessage
                    {
                        Guid = Guid.NewGuid(),
                        MsgID = msgId,
                        SystemLanguage_Guid = langaugeGuid
                    });
                });

            _controller = new v1_AdhocController(
                    _mockAdhocService.Object,
                    new Mock<IOTCRunControlService>().Object,
                    _mockMachineService.Object
                );
            Util.CreateFakeContext();
        }


        #region [OO#TFS#70970] Crypto Lock

        public static IEnumerable<object[]> LegGuidForCryptoLockData
        {
            get
            {
                return new List<object[]>
                {
                    new object[] { new Guid?[] { Guid.NewGuid() }, new Guid?[] { } }, // P has Guid, D doesn't
                    new object[] { new Guid?[] { }, new Guid?[] { Guid.NewGuid() } }, // P doesn't , P has Guid
                    new object[] { new Guid?[] { }, new Guid?[] { } } // none
                };
            }
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobPickUp_SuccessFalseForCryptoLock(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoFalse(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobPickUp);
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobDelivery_SuccessFalseForCryptoLock(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoFalse(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobDelivery);
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobTransfer_SuccessFalseForCryptoLock(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoFalse(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobTransfer);
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobTransferVault_SuccessFalseForCryptoLock(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoFalse(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobTransferVault);
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobPickUp_SuccessTrue(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoTrue(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobPickUp, fn => fn.CreateJobPickUp(It.IsAny<CreateJobAdHocRequest>()));
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobDelivery_SuccessTrue(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoTrue(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobDelivery, fn => fn.CreateJobDelivery(It.IsAny<CreateJobAdHocRequest>()));
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobTransfer_SuccessTrue(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoTrue(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobTransfer, fn => fn.CreateJobTransfer(It.IsAny<CreateJobAdHocRequest>()));
        }

        [Theory]
        [MemberData(nameof(LegGuidForCryptoLockData))]
        public void CreateJobTransferVault_SuccessTrue(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList)
        {
            ValidateCryptoTrue(pickupLegGuidList, deliveryLegGuidList, _controller.CreateJobTransferVault, fn => fn.CreateJobTransferVault(It.IsAny<CreateJobAdHocRequest>()));
        }

        private void ValidateCryptoFalse(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList, Func<CreateJobAdHocRequest, CreateJobAdHocResponse> fnCreateJob)
        {
            #region Setup

            _mockMachineService
                .Setup(fn => fn.IsMachineHasCryptoLock(It.IsAny<Guid[]>()))
                .Returns(true);

            #endregion

            var result = fnCreateJob(new CreateJobAdHocRequest
            {
                ServiceStopLegPickup = new AdhocLegRequest
                {
                    arr_LocationGuid = pickupLegGuidList.ToList()
                },
                ServiceStopLegDelivery = new AdhocLegRequest
                {
                    arr_LocationGuid = deliveryLegGuidList.ToList()
                }
            });

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(-17373, result.MsgID);
        }

        private void ValidateCryptoTrue(Guid?[] pickupLegGuidList, Guid?[] deliveryLegGuidList, 
            Func<CreateJobAdHocRequest, CreateJobAdHocResponse> fnCreateJob,
            Expression<Func<IAdhocService, CreateJobAdHocResponse>> fnVerify
           )
        {
            #region Setup

            var response = new CreateJobAdHocResponse(new TblSystemMessage { MsgID = 1 });
            response.IsSuccess = true;

            _mockMachineService
                .Setup(fn => fn.IsMachineHasCryptoLock(It.IsAny<Guid[]>()))
                .Returns(false);

            _mockAdhocService
                .Setup(fn => fn.CreateJobPickUp(It.IsAny<CreateJobAdHocRequest>()))
                .Returns(response);

            _mockAdhocService
                .Setup(fn => fn.CreateJobDelivery(It.IsAny<CreateJobAdHocRequest>()))
                .Returns(response);

            _mockAdhocService
                .Setup(fn => fn.CreateJobTransfer(It.IsAny<CreateJobAdHocRequest>()))
                .Returns(response);

            _mockAdhocService
                .Setup(fn => fn.CreateJobTransferVault(It.IsAny<CreateJobAdHocRequest>()))
                .Returns(response);

            #endregion

            var result = fnCreateJob(new CreateJobAdHocRequest
            {
                ServiceStopLegPickup = new AdhocLegRequest
                {
                    arr_LocationGuid = pickupLegGuidList.ToList()
                },
                ServiceStopLegDelivery = new AdhocLegRequest
                {
                    arr_LocationGuid = deliveryLegGuidList.ToList()
                }
            });

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotEqual(-17373, result.MsgID);
            Assert.Equal(1, result.MsgID);

            _mockAdhocService
                .Verify(fnVerify, Times.Once());
        }

        #endregion


        #region Alarm
        
        [Fact]
        public void CreateJobPickup_HasAlarmOnDailyRun()
        {
            ValidateHasAlarm(_controller.CreateJobPickUp);
        }

        [Fact]
        public void CreateJobDelivery_HasAlarmOnDailyRun()
        {
            ValidateHasAlarm(_controller.CreateJobDelivery);
        }

        [Fact]
        public void CreateJobTransfer_HasAlarmOnDailyRun()
        {
            ValidateHasAlarm(_controller.CreateJobTransfer);
        }

        [Fact]
        public void CreateJobTransferVault_HasAlarmOnDailyRun()
        {
            ValidateHasAlarm(_controller.CreateJobTransferVault);
        }

        private void ValidateHasAlarm(Func<CreateJobAdHocRequest, CreateJobAdHocResponse> fnCreateJob)
        {
            #region Setup

            var response = new CreateJobAdHocResponse(new TblSystemMessage { MsgID = -700 });
            response.IsSuccess = false;

            _mockAdhocService
                .Setup(fn => fn.CheckDailyRunResourceUnderAlarm(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Guid[]>()))
                .Returns(response);

            #endregion

            var result = fnCreateJob(new CreateJobAdHocRequest
            {
                ServiceStopLegPickup = new AdhocLegRequest(),
                ServiceStopLegDelivery = new AdhocLegRequest()
            });

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(response.MsgID, result.MsgID);

            _mockMachineService
                .Verify(fn => fn.IsMachineHasCryptoLock(It.IsAny<Guid[]>()), Times.Never());
        }

        #endregion

    }
}
