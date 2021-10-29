using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Service.ModelViews.SiteNetWork;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SiteNetwork;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{
    public interface ISiteNetworkAuditoLogService
    {
        void NewSiteNetwork(SiteNetworkViewRequest SiteNetwork);
        void ModifiedSiteNetwork(SiteNetworkViewRequest NewSiteNetwork, SiteNetworkViewResponse OldSiteNetwork);
        void DisableEnableSiteNetwork(SiteNetworkViewRequest SiteNetwork);
        IEnumerable<SiteNetworkAuditLogView> GetLogListInfoBySiteNetwork(Guid SiteNetworkGuid);
    }
    public class SiteNetworkAuditoLogService : ISiteNetworkAuditoLogService
    {
        private readonly IMasterSiteNetworkAuditLogRepository _siteNetworkAuditLogRepository;
        public SiteNetworkAuditoLogService(IMasterSiteNetworkAuditLogRepository siteNetworkAuditLogRepository) {
            _siteNetworkAuditLogRepository = siteNetworkAuditLogRepository;
        }

        public void NewSiteNetwork(SiteNetworkViewRequest SiteNetwork)
        {
            CreateSiteNetworkLogConfiguration(1, string.Format("Site Network {0} has been created.",SiteNetwork.SiteName),
                SiteNetwork.SiteGuid, SiteNetwork.UserData.UserName);
            foreach(BrinksSiteBySiteNetwork brinkSite in SiteNetwork.BrinksSitelist)
            {
                CreateSiteNetworkLogConfiguration(1, string.Format("Site Network {0} has been created. Brink's Site {1} was added.", SiteNetwork.SiteName, brinkSite.SiteCodeName),
                    SiteNetwork.SiteGuid, SiteNetwork.UserData.UserName);
            }
        }

        public void ModifiedSiteNetwork(SiteNetworkViewRequest NewSiteNetwork, SiteNetworkViewResponse OldSiteNetwork)
        {
            if(NewSiteNetwork.SiteName != OldSiteNetwork.SiteName)
            {
                CreateSiteNetworkLogConfiguration(2, string.Format("Site Network {0} has been changed Site Network Name from {1} to {0}.", NewSiteNetwork.SiteName, OldSiteNetwork.SiteName),
                    NewSiteNetwork.SiteGuid, NewSiteNetwork.UserData.UserName);
            }
            foreach (BrinksSiteBySiteNetwork brinkSite in OldSiteNetwork.BrinksSitelist)
            {
                var item = NewSiteNetwork.BrinksSitelist.FirstOrDefault(x => x.SiteGuid == brinkSite.SiteGuid && x.Guid != null && x.Guid != Guid.Empty);
                if (item == null)
                {
                    CreateSiteNetworkLogConfiguration(4, string.Format("Site Network {0} has been changed. Brink's Site {1} was deleted.", NewSiteNetwork.SiteName, brinkSite.SiteName),
                        NewSiteNetwork.SiteGuid, NewSiteNetwork.UserData.UserName);
                }
            }
            NewSiteNetwork.BrinksSitelist.Where(x => x.Guid == null || x.Guid == Guid.Empty).ToList().ForEach(x => {
                CreateSiteNetworkLogConfiguration(3, string.Format("Site Network {0} has been changed. Brink's Site {1} was added.", NewSiteNetwork.SiteName, x.SiteCodeName),
                    NewSiteNetwork.SiteGuid, NewSiteNetwork.UserData.UserName);
            });
        }

        public void DisableEnableSiteNetwork(SiteNetworkViewRequest SiteNetwork)
        {
            CreateSiteNetworkLogConfiguration((SiteNetwork.FlagDisable?5:6), string.Format("Site Network {0} has been {1}.", SiteNetwork.SiteName, (SiteNetwork.FlagDisable?"Disabled":"Enabled")),
                SiteNetwork.SiteGuid, SiteNetwork.UserData.UserName);
        }

        private void CreateSiteNetworkLogConfiguration(int msgId, string msgParameter, Guid siteNetworkGuid, string user)
        {
            TblMasterSiteNetworkAuditLog newLog = new TblMasterSiteNetworkAuditLog()
            {
                Guid = Guid.NewGuid(),
                MasterSiteNetwork_Guid = siteNetworkGuid,
                //1 = Add value in new Site Network
                //2 = Edit value in edit Site Network
                //3 = Add Brink's Site in edit Site Network
                //4 = Delete Brink's Site in edit Site Network
                //5 = Disable Site Network
                //6 = Enable Site Network
                MsgID = msgId, 
                MsgParameter = msgParameter,
                DatetimeCreated = DateTime.Now,
                UniversalDatetimeCreated = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                UserCreated = user
            };
            _siteNetworkAuditLogRepository.Create(newLog);
        }

        public IEnumerable<SiteNetworkAuditLogView> GetLogListInfoBySiteNetwork(Guid SiteNetworkGuid)
        {
             return _siteNetworkAuditLogRepository.FindAll(x => x.MasterSiteNetwork_Guid == SiteNetworkGuid).OrderByDescending(x => x.DatetimeCreated).ConvertToSiteNetworkAuditLogViewList().ToList();
        }
    }
}
