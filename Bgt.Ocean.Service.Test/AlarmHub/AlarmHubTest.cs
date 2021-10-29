using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Models.Group;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Service.Test.AlarmHub
{
    public class AlarmHubTest
    {
        public class Connect : BaseTest
        {
            private readonly Bgt.Ocean.WebAPI.Hubs.AlarmHub _alarmHub;
            private readonly Mock<IMasterGroupRepository> _mockMasterGroupRepository;
            

            public Connect()
            {
                _mockMasterGroupRepository = Util.CreateMock<IMasterGroupRepository>();

                _alarmHub = new Bgt.Ocean.WebAPI.Hubs.AlarmHub(_mockMasterGroupRepository.Object);
            }

            public static IEnumerable<object[]> PermittedUser()
            {

                var obj0 = Util.CreateDummy<MasterGroupAlarmModel>(0);              

                var obj1 = Util.CreateDummy<MasterGroupAlarmModel>(1)
                    .Select(e =>
                    {
                        e.FlagAllowAcknowledge = true;
                        e.FlagAllowDeactivate = true;
                        e.Guid = Guid.NewGuid();
                        return e;
                    })
                    .ToList();

                var obj2 = Util.CreateDummy<MasterGroupAlarmModel>(2)
                    .Select(e =>
                    {
                        e.FlagAllowAcknowledge = true;
                        e.FlagAllowDeactivate = true;
                        e.Guid = Guid.NewGuid();
                        return e;
                    })
                    .ToList();

                var obj3 = Util.CreateDummy<MasterGroupAlarmModel>(3)
                    .Select(e =>
                    {
                        e.FlagAllowAcknowledge = true;
                        e.FlagAllowDeactivate = true;
                        e.Guid = Guid.NewGuid();
                        return e;
                    })
                    .ToList();

                return new List<object[]>
                {
                    new object[] { obj0, 0 }, // no group 
                    new object[] { obj1, 1 }, // has 1 valid group
                    new object[] { obj2, 2 }, // has 2 valid group
                    new object[] { obj3, 3 }, // 
                };
            }

            [Theory]
            [MemberData(nameof(PermittedUser))]
            public void PermittedUser_ShouldAddGroupCorrectly(IEnumerable<MasterGroupAlarmModel> userPermitted, int timeCall)
            {
                #region Arrange

                var userGuid = Guid.NewGuid();
                var userName = "someone";

                var mockClients = Util.CreateMock<IHubCallerConnectionContext<dynamic>>();
                var mockGroup = Util.CreateMock<IGroupManager>();
                var mockNameValueCollection = Util.CreateMock<INameValueCollection>();
                var mockRequest = Util.CreateMock<IRequest>();                

                _mockMasterGroupRepository
                    .Setup(e => e.GetPermittedAlarmGroupByUser(userGuid))
                    .Returns(userPermitted);                

                mockNameValueCollection
                    .Setup(fn => fn["userGuid"])
                    .Returns(userGuid.ToString());

                mockNameValueCollection
                    .Setup(fn => fn["userName"])
                    .Returns(userName);

                mockRequest
                    .Setup(fn => fn.QueryString)
                    .Returns(mockNameValueCollection.Object);

                mockGroup
                    .Setup(e => e.Add(It.IsAny<string>(), It.IsAny<string>()))
                    .Verifiable();           

                var callerContext = new HubCallerContext(mockRequest.Object, Guid.NewGuid().ToString());

                _alarmHub.Clients = mockClients.Object;
                _alarmHub.Groups = mockGroup.Object;
                _alarmHub.Context = callerContext;

                #endregion

                _alarmHub.OnConnected();

                mockGroup.Verify(e => e.Add(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(timeCall));
                
            }
            
        }

        public class Disconnect : BaseTest
        {
            private readonly Bgt.Ocean.WebAPI.Hubs.AlarmHub _alarmHub;
            private readonly Mock<IMasterGroupRepository> _mockMasterGroupRepository;

            public Disconnect()
            {
                _mockMasterGroupRepository = Util.CreateMock<IMasterGroupRepository>();

                _alarmHub = new Bgt.Ocean.WebAPI.Hubs.AlarmHub(_mockMasterGroupRepository.Object);
            }

            public static IEnumerable<object[]> ConnectedUser()
            {
                var obj1 = Util.CreateDummy<MasterGroupAlarmModel>(1)
                    .Select(e =>
                    {
                        e.FlagAllowAcknowledge = true;
                        e.FlagAllowDeactivate = true;
                        e.Guid = Guid.NewGuid();
                        return e;
                    })
                    .ToList();

                var obj2 = Util.CreateDummy<MasterGroupAlarmModel>(2)
                    .Select(e =>
                    {
                        e.FlagAllowAcknowledge = true;
                        e.FlagAllowDeactivate = true;
                        e.Guid = Guid.NewGuid();
                        return e;
                    })
                    .ToList();

                return new List<object[]>
                {
                    new object[] { obj1, 1 },
                    new object[] { obj2, 2 }
                };
            }

            [Theory]
            [MemberData(nameof(ConnectedUser))]
            public void ConnectedUser_ShouldRemoveFromGroupCorrectly(IEnumerable<MasterGroupAlarmModel> userPermitted, int timeCall)
            {
                #region Arrange

                var userGuid = Guid.NewGuid();
                var userName = "someone";

                var mockClients = Util.CreateMock<IHubCallerConnectionContext<dynamic>>();
                var mockGroup = Util.CreateMock<IGroupManager>();
                var mockNameValueCollection = Util.CreateMock<INameValueCollection>();
                var mockRequest = Util.CreateMock<IRequest>();

                _mockMasterGroupRepository
                    .Setup(e => e.GetPermittedAlarmGroupByUser(userGuid))
                    .Returns(userPermitted);

                mockNameValueCollection
                    .Setup(fn => fn["userGuid"])
                    .Returns(userGuid.ToString());

                mockNameValueCollection
                    .Setup(fn => fn["userName"])
                    .Returns(userName);

                mockRequest
                    .Setup(fn => fn.QueryString)
                    .Returns(mockNameValueCollection.Object);

                mockGroup
                    .Setup(fn => fn.Remove(It.IsAny<string>(), It.IsAny<string>()))
                    .Callback<string, string>((connId, groupName) =>
                    {
                        Assert.Contains(userPermitted, e => e.Guid == groupName.ToGuid());
                    });

                var callerContext = new HubCallerContext(mockRequest.Object, Guid.NewGuid().ToString());

                _alarmHub.Clients = mockClients.Object;
                _alarmHub.Groups = mockGroup.Object;
                _alarmHub.Context = callerContext;

                #endregion

                _alarmHub.OnConnected();

                _alarmHub.OnDisconnected(false);

                mockGroup.Verify(e => e.Remove(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(timeCall));
                
            }
        }
    }
    
}
 