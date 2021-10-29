using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations.Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using System;
using System.Dynamic;
using Xunit;

namespace Bgt.Ocean.Service.Test.RouteOptimization
{
    public class RouteOptimizationBroadcastServiceTest : BaseTest
    {
        private readonly IRouteOptimizationBroadcastService _service;
        private readonly Mock<IHubConnectionContext<dynamic>> _mockHubConnection;

        public RouteOptimizationBroadcastServiceTest()
        {
            _mockHubConnection = CreateMock<IHubConnectionContext<dynamic>>();

            _service = new RouteOptimizationBroadcastService(
                    _mockHubConnection.Object
                );
        }

        [Fact]
        public void NotifyOptimizationStatus_ShouldCallWithCorrectGroup()
        {
            var requestGuid = Guid.NewGuid().ToString().ToLower();
            var obj = new { data = 1, value = "sample" };

            #region Arrange

            dynamic proxy = new ExpandoObject();

            proxy.onRouteOptimizationUpdate = new Action<object>(model =>
            {
                Assert.Equal(obj, model);
            });

            _mockHubConnection
                .Setup(fn => fn.Group(requestGuid))
                .Returns((ExpandoObject)proxy)
                .Verifiable();

            #endregion

            _service.NotifyRouteOptimizationStatus(requestGuid.ToGuid(), obj);
            _mockHubConnection.VerifyAll();
        }

        [Fact]
        public void NotifyOptimizationStatus_ShouldNotBeCalled()
        {
            var requestGuid = Guid.NewGuid().ToString().ToLower();
            var anotherRequest = Guid.NewGuid().ToString().ToLower();

            var obj = new { data = 1, value = "sample" };

            #region Arrange

            dynamic proxy1 = new ExpandoObject();
            dynamic proxy2 = new ExpandoObject();

            proxy1.onRouteOptimizationUpdate = new Action<object>(model =>
            {                
            });

            proxy2.onRouteOptimizationUpdate = new Action<object>(model =>
            {
                Assert.Equal(obj, model);
            });

            _mockHubConnection
                .Setup(fn => fn.Group(requestGuid))
                .Returns((ExpandoObject)proxy1)
                .Verifiable();

            _mockHubConnection
                .Setup(fn => fn.Group(anotherRequest))
                .Returns((ExpandoObject)proxy2);

            #endregion

            _service.NotifyRouteOptimizationStatus(anotherRequest.ToGuid(), obj);
            _service.NotifyRouteOptimizationStatus(anotherRequest.ToGuid(), obj);

            _mockHubConnection.Verify(client => client.Group(requestGuid), Times.Never);
            _mockHubConnection.Verify(client => client.Group(anotherRequest), Times.Exactly(2));
        }
    }
}
