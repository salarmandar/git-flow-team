using Bgt.Ocean.WebAPI.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using System;
using Xunit;

namespace Bgt.Ocean.Service.Test.RouteOptimization
{
    public class RouteOptimizationHubTest : BaseTest
    {
        private readonly RouteOptimizationHub _routeOptimizationHub;

        public RouteOptimizationHubTest()
        {
            _routeOptimizationHub = new RouteOptimizationHub();
        }

        [Fact]
        public void RegisterRouteOptimization_ShouldOk()
        {
            var requestGuid = Guid.NewGuid();

            #region Arrange            

            var mockClients = CreateMock<IHubCallerConnectionContext<dynamic>>();
            var mockGroup = CreateMock<IGroupManager>();
            var mockRequest = CreateMock<IRequest>();

            mockGroup
                .Setup(fn => fn.Add(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((connId, groupName) =>
                {
                    Assert.Equal(requestGuid.ToString().ToLower(), groupName);
                });

            var callerContext = new HubCallerContext(mockRequest.Object, Guid.NewGuid().ToString());

            _routeOptimizationHub.Clients = mockClients.Object;
            _routeOptimizationHub.Groups = mockGroup.Object;
            _routeOptimizationHub.Context = callerContext;

            #endregion

            _routeOptimizationHub.RegisterRouteOptimization(requestGuid);

            mockGroup.VerifyAll();
        }

        [Fact]
        public void Disconnect_ShouldRemoveFromGroupCorrectly()
        {
            var requestGuid = Guid.NewGuid().ToString().ToLower();

            #region Arrange

            var mockClients = Util.CreateMock<IHubCallerConnectionContext<dynamic>>();
            var mockGroup = Util.CreateMock<IGroupManager>();
            var mockNameValueCollection = Util.CreateMock<INameValueCollection>();
            var mockRequest = Util.CreateMock<IRequest>();                        

            mockNameValueCollection
                .Setup(fn => fn["userGuid"])
                .Returns(requestGuid);

            mockRequest
                .Setup(fn => fn.QueryString)
                .Returns(mockNameValueCollection.Object);

            mockGroup
                .Setup(fn => fn.Remove(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((connId, groupName) =>
                {
                    Assert.Equal(requestGuid, groupName);
                });

            var callerContext = new HubCallerContext(mockRequest.Object, Guid.NewGuid().ToString());

            _routeOptimizationHub.Clients = mockClients.Object;
            _routeOptimizationHub.Groups = mockGroup.Object;
            _routeOptimizationHub.Context = callerContext;

            #endregion

            _routeOptimizationHub.OnDisconnected(true);

            mockGroup.VerifyAll();
        }

        [Fact]
        public void Disconnect_ShouldHandleNullCorrectly()
        {
            #region Arrange

            var mockClients = Util.CreateMock<IHubCallerConnectionContext<dynamic>>();
            var mockGroup = Util.CreateMock<IGroupManager>();
            var mockNameValueCollection = Util.CreateMock<INameValueCollection>();
            var mockRequest = Util.CreateMock<IRequest>();            

            mockRequest
                .Setup(fn => fn.QueryString)
                .Returns(mockNameValueCollection.Object);

            mockGroup
                .Setup(fn => fn.Remove(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((connId, groupName) =>
                {
                    Assert.Null(groupName);
                });

            var callerContext = new HubCallerContext(mockRequest.Object, Guid.NewGuid().ToString());

            _routeOptimizationHub.Clients = mockClients.Object;
            _routeOptimizationHub.Groups = mockGroup.Object;
            _routeOptimizationHub.Context = callerContext;

            #endregion

            _routeOptimizationHub.OnDisconnected(true);

            mockGroup.VerifyAll();
        }
    }
}
