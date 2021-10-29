using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.Group;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.WebAPI.Filters;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace Bgt.Ocean.WebAPI.Hubs
{
    [HubAuthorize]
    public class AlarmHub : Hub
    {
        private readonly IMasterGroupRepository _masterGroupRepository;

        public AlarmHub(                
                IMasterGroupRepository masterGroupRepository
            )
        {
            _masterGroupRepository = masterGroupRepository;
        }

        public void helloWorld()
        {
            Clients.All.helloWorld();
        }

        public override Task OnConnected()
        {
            GroupAction((group, masterGroup) =>
            {
                group.Add(Context.ConnectionId, masterGroup.Guid.ToString());
            });
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            GroupAction((group, masterGroup) =>
            {
                group.Remove(Context.ConnectionId, masterGroup.Guid.ToString());
            });

            return base.OnDisconnected(stopCalled);
        }

        private void GroupAction(Action<IGroupManager, MasterGroupAlarmModel> fnAction)
        {
            var user = new HubUser(Context);
            var userGroup = _masterGroupRepository.GetPermittedAlarmGroupByUser(user.UserGuid);            

            foreach (var item in userGroup)
            {
                fnAction(Groups, item);
            }
        }

        #region Util class

        private class HubUser
        {
            public Guid UserGuid { get; }
            public string UserName { get; }

            public HubUser(Microsoft.AspNet.SignalR.Hubs.HubCallerContext context)
            {
                UserGuid = context.QueryString["userGuid"].ToGuid();
                UserName = context.QueryString["userName"];

            }
        }

        #endregion

    }


}
