using Bgt.Ocean.Models.MasterRoute;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using Bgt.Ocean.WebAPI.Controllers.Internals.v1;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace Bgt.Ocean.Controller.Test.v1
{
    public class MasterRouteControllerTest
    {
        private readonly v1_MasterRouteController _controller;

        public MasterRouteControllerTest()
        {
            _controller = Util.CreateInstance<v1_MasterRouteController>();
            Util.CreateFakeContext();

            _controller.GetMock<ISystemService>()
                .Setup(fn => fn.GetMessageByMsgId(-17373))
                .Returns(new Service.ModelViews.Systems.SystemMessageView { IsSuccess = false, MsgID = -17373 });
        }

        public static List<object[]> MasterRouteJobData
        {
            get
            {
                return new List<object[]>
                {
                    new object[] { new MasterRouteJobServiceStopLegsView { MasterCustomerLocation_Guid = Guid.NewGuid() }, null }, // P has, D Doesn't
                    new object[] { null, new MasterRouteJobServiceStopLegsView { MasterCustomerLocation_Guid = Guid.NewGuid() } }, // P Doesn't, P has
                    new object[] { new MasterRouteJobServiceStopLegsView(), new MasterRouteJobServiceStopLegsView() }, // P empty, D empty
                    new object[] { null, null }
                };
            }
        }

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void CreateMasterJobTVMultiBranch_ReturnFalseForCryptoLock(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateFalseCrypto(pLeg, dLeg, _controller.CreateMasterJobTVMultiBranch);

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void CreateMasterJobTVMultiBranch_ReturnTrue(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateTrue(pLeg, dLeg, _controller.CreateMasterJobTVMultiBranch, fn => fn.CreateMasterJobTVMultiBranch(It.IsAny<MasterRouteCreateJobRequest>()));

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void CreateMasterJobBCDMultiBranch_ReturnFalseForCryptoLock(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateFalseCrypto(pLeg, dLeg, _controller.CreateMasterJobBCDMultiBranch);

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void CreateMasterJobBCDMultiBranch_ReturnTrue(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateTrue(pLeg, dLeg, _controller.CreateMasterJobBCDMultiBranch, fn => fn.CreateMasterJobBCDMultiBranch(It.IsAny<MasterRouteCreateJobRequest>()));

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void CreateMasterJobPickupMultiBranch_ReturnFalseForCryptoLock(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateFalseCrypto(pLeg, dLeg, _controller.CreateMasterJobPickupMultiBranch);

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void CreateMasterJobPickupMultiBranch_ReturnTrue(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateTrue(pLeg, dLeg, _controller.CreateMasterJobPickupMultiBranch, fn => fn.CreateMasterJobPickupMultiBranch(It.IsAny<MasterRouteCreateJobRequest>()));

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void UpdateMasterJobBCDMultiBranch_ReturnFalseForCryptoLock(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateFalseCrypto(pLeg, dLeg, _controller.UpdateMasterJobBCDMultiBranch);

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void UpdateMasterJobBCDMultiBranch_ReturnTrue(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateTrue(pLeg, dLeg, _controller.UpdateMasterJobBCDMultiBranch, fn => fn.UpdateMasterJobBCDMultiBranch(It.IsAny<MasterRouteCreateJobRequest>()));

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void UpdateMasterJobPickupMultiBranch_ReturnFalseForCryptoLock(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateFalseCrypto(pLeg, dLeg, _controller.UpdateMasterJobPickupMultiBranch);

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void UpdateMasterJobPickupMultiBranch_ReturnTrue(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateTrue(pLeg, dLeg, _controller.UpdateMasterJobPickupMultiBranch, fn => fn.UpdateMasterJobPickupMultiBranch(It.IsAny<MasterRouteCreateJobRequest>()));

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void UpdateMasterJobTVMultiBranch_ReturnFalseForCryptoLock(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateFalseCrypto(pLeg, dLeg, _controller.UpdateMasterJobTVMultiBranch);

        [Theory]
        [MemberData(nameof(MasterRouteJobData))]
        public void UpdateMasterJobTVMultiBranch_ReturnTrue(MasterRouteJobServiceStopLegsView pLeg, MasterRouteJobServiceStopLegsView dLeg)
            => ValidateTrue(pLeg, dLeg, _controller.UpdateMasterJobTVMultiBranch, fn => fn.UpdateMasterJobTVMultiBranch(It.IsAny<MasterRouteCreateJobRequest>()));


        private void ValidateFalseCrypto(
            MasterRouteJobServiceStopLegsView pLeg,
            MasterRouteJobServiceStopLegsView dLeg,
            Func<MasterRouteCreateJobRequest, MasterRouteCreateJobResponse> fnProcess)
        {
            #region Setup

            _controller.GetMock<IMachineService>()
                .Setup(fn => fn.IsMachineHasCryptoLock(It.IsAny<Guid[]>()))
                .Returns(true);

            #endregion

            var result = fnProcess(new MasterRouteCreateJobRequest
            {
                Pickupleg = pLeg,
                Deliveryleg = dLeg
            });

            Assert.False(result.IsSuccess);
            Assert.Equal(-17373, result.MsgID);
        }

        private void ValidateTrue(MasterRouteJobServiceStopLegsView pLeg,
            MasterRouteJobServiceStopLegsView dLeg,
            Func<MasterRouteCreateJobRequest, MasterRouteCreateJobResponse> fnController,
            Expression<Func<IMasterRouteService, MasterRouteCreateJobResponse>> fnService)
        {
            #region Setup

            var response = new MasterRouteCreateJobResponse
            {
                IsSuccess = true,
                MsgID = 200
            };

            _controller.GetMock<IMachineService>()
                .Setup(fn => fn.IsMachineHasCryptoLock(It.IsAny<Guid[]>()))
                .Returns(false);

            _controller.GetMock<IMasterRouteService>()
                .Setup(fnService)
                .Returns(response);

            #endregion

            var result = fnController(new MasterRouteCreateJobRequest
            {
                Pickupleg = pLeg,
                Deliveryleg = dLeg
            });

            Assert.True(result.IsSuccess);
            Assert.NotEqual(-17373, result.MsgID);
            Assert.Equal(response.MsgID, result.MsgID);

            _controller.GetMock<IMasterRouteService>()
                .Verify(fnService, Times.Once());
        }

    }
}
