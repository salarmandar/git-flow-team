using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Group;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData
{
    #region Interface

    public interface IMasterGroupRepository : IRepository<TblMasterGroup>
    {
        /// <summary>
        /// Group of Either Allow Acknowledge or Deactivate alarm
        /// </summary>
        /// <param name="masterSiteGuid"></param>
        /// <returns></returns>
        IEnumerable<MasterGroupAlarmModel> GetPermittedAlarmGroupBySite(Guid masterSiteGuid);
        /// <summary>
        /// Group of Either Allow Acknowledge or Deactivate alarm and user must in that group 
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        IEnumerable<MasterGroupAlarmModel> GetPermittedAlarmGroupByUser(Guid userGuid);
    }

    #endregion

    public class MasterGroupRepository : Repository<OceanDbEntities, TblMasterGroup>, IMasterGroupRepository
    {
        public MasterGroupRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<MasterGroupAlarmModel> GetPermittedAlarmGroupBySite(Guid masterSiteGuid)
        {
            var group = (from g in GetQueryPermittedGroup()
                        join gs in DbContext.TblMasterGroup_Site on g.Guid equals gs.MasterGroup_Guid
                        where gs.MasterSite_Guid == masterSiteGuid
                        group gs by new { g.Guid, g.FlagAllowAcknowledgeAlarm, g.FlagAllowDeactivateAlarm } into groupResult
                        select new MasterGroupAlarmModel
                        {
                            Guid = groupResult.Key.Guid,
                            FlagAllowAcknowledge = groupResult.Key.FlagAllowAcknowledgeAlarm,
                            FlagAllowDeactivate = groupResult.Key.FlagAllowDeactivateAlarm,
                            MasterSiteHandleList = groupResult.Select(data => data.MasterSite_Guid)
                        })
                        .AsEnumerable();

            return group;
        }
        
        public IEnumerable<MasterGroupAlarmModel> GetPermittedAlarmGroupByUser(Guid userGuid)
        {
            var group = (from ug in DbContext.TblMasterUserGroup.Where(e => e.MasterUser_Guid == userGuid)
                         join g in GetQueryPermittedGroup() on ug.MasterGroup_Guid equals g.Guid
                         join gs in DbContext.TblMasterGroup_Site on g.Guid equals gs.MasterGroup_Guid
                         group gs by new { ug.MasterGroup_Guid, ug.MasterUser_Guid, g.FlagAllowAcknowledgeAlarm, g.FlagAllowDeactivateAlarm } into groupResult
                         select new MasterGroupAlarmModel
                         {
                             Guid = groupResult.Key.MasterGroup_Guid,
                             FlagAllowAcknowledge = groupResult.Key.FlagAllowAcknowledgeAlarm,
                             FlagAllowDeactivate = groupResult.Key.FlagAllowDeactivateAlarm,
                             MasterSiteHandleList = groupResult.Select(data => data.MasterSite_Guid)
                         })
                         .AsEnumerable();

            return group;
        }

        private IQueryable<TblMasterGroup> GetQueryPermittedGroup()
            => DbContext.TblMasterGroup.Where(e => !e.FlagDisable && (e.FlagAllowAcknowledgeAlarm || e.FlagAllowDeactivateAlarm));
    }
}
