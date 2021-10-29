using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterUserRepository : IRepository<TblMasterUser>
    {
        TblMasterUser FindByEmail(string mail);
        TblMasterUser FindByUserName(string userName);
        IEnumerable<TblSystemLog_SaveLastPWD> FindLastLogin(Guid userGuid, int days);
        string EncryptPassword(string password, Guid saltKeyGuid);
        int Func_ChangePassword_Set(string userName, string oldPassword, string newPassword);

        int Func_ResetPassword_Set(string userName, string newPassword);
        TblSystemDomainAiles FindDomainAilesByName(string ailesName);
        TblSystemDomainDC FindDomainDC(Guid ailesGuid);
        AuthenLoginResult Func_AuthenLogin_Get(string username, string password, int applicationId);
        IEnumerable<UserGroupView> FindGroup(int roleType);
        IEnumerable<UserGroupView> FindGroupByUser(Guid userGuid);
        IEnumerable<Fn_UserDetailCountryAndRole_Result> Func_UserDetailCountryAndRole_Get(Guid? countryGuid = null, int? roleNumber = null, bool? flagCanApprove = null, bool? flagLock = null);
        IEnumerable<TblMasterGroup_MenuCommand> FindMenuCommandByGroups(List<Guid?> groups);
        IEnumerable<MasterGroupView> GetMasterGroupByCountry(Guid countryGuid);
        MasterUserGroupView GetUserGroup(Guid userGuid);
        IEnumerable<UserGroupView> FindUserCanSeeMenu(Guid masterMenuDetailGuid, Guid masterSiteGuid);
        IEnumerable<UserGroupView> FindUserByMenuCommand(Guid masterMenuDetailCommandGuid, Guid masterSiteGuid);
        IEnumerable<UserView> GetUsersInCompany(Guid company_Guid);
        IEnumerable<UserView> GetUsersInCountry(Guid country_Guid, string filterUserName, string filterFullName, string filterEmail, int? pageNumber, int? numberPerPage, string sortBy, string sortWith);

    }

    public class MasterUserRepository : Repository<OceanDbEntities, TblMasterUser>, IMasterUserRepository
    {
        public MasterUserRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblMasterUser FindByEmail(string mail)
        {
            return DbContext.TblMasterUser.FirstOrDefault(e => e.Email == mail);
        }

        public TblMasterUser FindByUserName(string userName)
        {
            return DbContext.TblMasterUser.FirstOrDefault(e => e.UserName.ToLower() == userName.ToLower());
        }

        public IEnumerable<TblSystemLog_SaveLastPWD> FindLastLogin(Guid userGuid, int days)
        {
            return DbContext.TblSystemLog_SaveLastPWD.Where(e => e.MasterUser_Guid == userGuid).OrderByDescending(e => e.DatetimeCreated).Take(days);
        }

        public string EncryptPassword(string password, Guid saltKeyGuid)
        {
            return DbContext.Up_OceanOnlineMVC_SystemSaltEncryption_Get(password, saltKeyGuid).FirstOrDefault();
        }

        public int Func_ChangePassword_Set(string userName, string oldPassword, string newPassword)
        {
            return DbContext.Up_OceanOnlineMVC_ChangePassword_Set(userName, oldPassword, newPassword);
        }

        public TblSystemDomainAiles FindDomainAilesByName(string ailesName)
        {
            return DbContext.TblSystemDomainAiles.FirstOrDefault(e => e.AilesName == ailesName);
        }

        public TblSystemDomainDC FindDomainDC(Guid ailesGuid)
        {
            return DbContext.TblSystemDomainDC.FirstOrDefault(e => e.SystemDomain_Guid == ailesGuid);
        }

        public AuthenLoginResult Func_AuthenLogin_Get(string username, string password, int applicationId)
        {
            return DbContext.Up_OceanOnlineMVC_AuthenLogin_Get(username, password, applicationId).FirstOrDefault();
        }

        public int Func_ResetPassword_Set(string userName, string newPassword)
        {
            return DbContext.Up_OceanOnlineMVC_ResetPassword_Set(userName, newPassword);
        }

        public IEnumerable<UserGroupView> FindGroup(int roleType)
        {
            var findAllGroupInUser = DbContext.TblMasterGroup.Where(e => e.RoleType == roleType && e.FlagDisable == false).ToList();
            var result = findAllGroupInUser.Select(e => new UserGroupView()
            {
                Group = new GroupView(e.Guid)
                {
                    MasterCountry_Guid = e.MasterCountry_Guid,
                    Email = e.Email,
                    Emails = String.IsNullOrEmpty(e.Email) ? new List<string>() : e.Email.Split(',').ToList(),
                    RoleType = e.RoleType.Value
                }
            });

            return result;
        }

        public IEnumerable<UserGroupView> FindGroupByUser(Guid userGuid)
        {
            var userGroup = DbContext.TblMasterUserGroup.Where(e => e.MasterUser_Guid == userGuid && e.FlagDisable == false).ToList();
            List<Guid> groupGuids = userGroup.Select(e => e.MasterGroup_Guid).ToList();

            var findAllGroupInUser = DbContext.TblMasterGroup.Where(e => groupGuids.Contains(e.Guid)).ToList();

            var result = findAllGroupInUser.Select(e => new UserGroupView()
            {
                User = new UserView(userGuid),
                Group = new GroupView(e.Guid)
                {
                    MasterCountry_Guid = e.MasterCountry_Guid,
                    Email = e.Email,
                    Emails = e.Email != null ? e.Email.Split(',').ToList() : null,
                    RoleType = e.RoleType.Value
                }
            });

            return result;
        }

        public IEnumerable<Fn_UserDetailCountryAndRole_Result> Func_UserDetailCountryAndRole_Get(Guid? countryGuid = null, int? roleNumber = null, bool? flagCanApprove = null, bool? flagLock = null)
        {
            return DbContext.Fn_UserDetailCountryAndRole(countryGuid, roleNumber, flagCanApprove, flagLock);
        }

        public IEnumerable<TblMasterGroup_MenuCommand> FindMenuCommandByGroups(List<Guid?> groups)
        {
            return DbContext.TblMasterGroup_MenuCommand.Where(
                e => groups.Contains(e.MasterGroup_Guid)
                && e.TblMasterGroup.FlagDisable == false
                && e.TblMasterMenuDetailCommand.FlagDisable == false);
        }

        public IEnumerable<MasterGroupView> GetMasterGroupByCountry(Guid countryGuid)
        {
            var masterGroupByCountryData = DbContext.TblMasterGroup.Where(u => u.MasterCountry_Guid == countryGuid);
            var masterGroupByCountryList = masterGroupByCountryData.Select(c => new MasterGroupView()
            {
                Guid = c.Guid,
                GroupName = c.GroupName,
                MasterCountry_Guid = c.MasterCountry_Guid,
                RoleType = c.RoleType,
                FlagDisable = c.FlagDisable,
                SystemRoleGroupType_Guid = c.SystemRoleGroupType_Guid
            });

            return masterGroupByCountryList;
        }

        public MasterUserGroupView GetUserGroup(Guid userGuid)
        {
            var userGroupData = DbContext.TblMasterUserGroup.Where(u => u.MasterUser_Guid == userGuid);
            List<MasterGroupView> listGroupData = new List<MasterGroupView>();

            MasterUserGroupView userGroupResult = new MasterUserGroupView()
            {
                MasterUser_Guid = userGuid,
            };

            foreach (var itemUserGroup in userGroupData)
            {
                var masterGroupData = DbContext.TblMasterGroup.Find(itemUserGroup.MasterGroup_Guid);
                MasterGroupView groupData = new MasterGroupView()
                {
                    Guid = masterGroupData.Guid,
                    GroupName = masterGroupData.GroupName,
                    FlagDisable = masterGroupData.FlagDisable,
                    MasterCountry_Guid = masterGroupData.MasterCountry_Guid,
                    RoleType = masterGroupData.RoleType,
                    SystemRoleGroupType_Guid = masterGroupData.SystemRoleGroupType_Guid,
                    MasterSiteGuidList = DbContext.TblMasterGroup_Site.Where(c => c.MasterGroup_Guid == itemUserGroup.MasterGroup_Guid).Select(s => s.MasterSite_Guid)
                };

                listGroupData.Add(groupData);
            }

            userGroupResult.UserGroupList = listGroupData;
            return userGroupResult;
        }

        public IEnumerable<UserGroupView> FindUserCanSeeMenu(Guid masterMenuDetailGuid, Guid masterSiteGuid)
        {
            List<UserGroupView> userGroupViewList = new List<UserGroupView>();
            var groupSiteHandle = DbContext.TblMasterGroup_Site.Where(s => s.MasterSite_Guid == masterSiteGuid)
                                .SelectMany(g => DbContext.TblMasterGroup.Where(x => x.Guid == g.MasterGroup_Guid && !x.FlagDisable));

            var groupAllowToSeeMenu = groupSiteHandle.Distinct().SelectMany(
                                    x => DbContext.TblMasterGroup_Menu
                                    .Where(s => s.MasterMenuDetail_Guid == masterMenuDetailGuid && s.MasterGroup_Guid == x.Guid))
                                    .Select(c => c.MasterGroup_Guid);

            var userInGroupAllowance = groupAllowToSeeMenu.Distinct().SelectMany(groupGuid => DbContext.TblMasterUserGroup.Where(u => u.MasterGroup_Guid == groupGuid));
            foreach (var itemUserGroup in userInGroupAllowance)
            {
                var groupData = DbContext.TblMasterGroup.FirstOrDefault(g => g.Guid == itemUserGroup.MasterGroup_Guid);
                UserGroupView userGroupView = new UserGroupView()
                {
                    User = new UserView()
                    {
                        Guid = itemUserGroup.MasterUser_Guid,
                        UserName = DbContext.TblMasterUser.Find(itemUserGroup.MasterUser_Guid).UserName
                    },
                    Group = new GroupView()
                    {
                        Guid = itemUserGroup.MasterGroup_Guid,
                        GroupName = groupData.GroupName,
                        RoleType = groupData.RoleType.GetValueOrDefault()
                    }
                };
                userGroupViewList.Add(userGroupView);
            }

            return userGroupViewList;
        }

        public IEnumerable<UserGroupView> FindUserByMenuCommand(Guid masterMenuDetailCommandGuid, Guid masterSiteGuid)
        {
            List<UserGroupView> userGroupViewList = new List<UserGroupView>();
            var groupSiteHandle = DbContext.TblMasterGroup_Site.Where(s => s.MasterSite_Guid == masterSiteGuid)
                                .SelectMany(g => DbContext.TblMasterGroup.Where(x => x.Guid == g.MasterGroup_Guid && x.RoleType == 3 && !x.FlagDisable));

            var groupAllowToDoCommand = groupSiteHandle.Distinct().SelectMany(
                                    x => DbContext.TblMasterGroup_MenuCommand
                                    .Where(s => s.MasterMenuDetailCommand_Guid == masterMenuDetailCommandGuid && s.MasterGroup_Guid == x.Guid))
                                    .Select(c => c.MasterGroup_Guid);

            var userInGroupAllowance = groupAllowToDoCommand.Distinct().SelectMany(groupGuid => DbContext.TblMasterUserGroup.Where(u => u.MasterGroup_Guid == groupGuid));
            var groupUserData = userInGroupAllowance.GroupBy(c => c.MasterUser_Guid);

            foreach (var itemUserGroup in groupUserData)
            {
                UserGroupView userGroupView = new UserGroupView()
                {
                    User = new UserView()
                    {
                        Guid = itemUserGroup.Key,
                        UserName = DbContext.TblMasterUser.Find(itemUserGroup.Key).UserName
                    }
                };
                userGroupViewList.Add(userGroupView);
            }

            return userGroupViewList;
        }

        public IEnumerable<UserView> GetUsersInCompany(Guid company_Guid)
        {
            var userData = DbContext.TblMasterUser.Where(u => u.MasterCustomer_Guid == company_Guid && !u.FlagDisable);
            var userDataList = userData.Select(c => new UserView()
            {
                Guid = c.Guid,
                Email = c.Email,
                UserName = c.UserName,
                FlagDisable = c.FlagDisable,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                FullName = c.FirstName + " " + c.LastName,
                UserModify = c.UserModifed,
                DateTimeModify = c.DatetimeModified
            });

            return userDataList;
        }

        public IEnumerable<UserView> GetUsersInCountry(Guid country_Guid, string filterUserName, string filterFullName, string filterEmail, int? pageNumber, int? numberPerPage, string sortBy, string sortWith)
        {
            string orderBy;
            if (!string.IsNullOrWhiteSpace(sortBy))
                orderBy = sortBy;
            else
                orderBy = "UserName";

            var userData = DbContext.TblMasterUser.Where(u => u.MasterCountry_Guid == country_Guid && !u.FlagDisable);
            var userDataList = userData.Select(c => new UserView()
            {
                Guid = c.Guid,
                Email = c.Email,
                UserName = c.UserName,
                FlagDisable = c.FlagDisable,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                FullName = c.FirstName + " " + c.LastName,
                UserModify = c.UserModifed,
                DateTimeModify = c.DatetimeModified,
                TotalRecord = userData.Count()
            })
             .Where(e =>
              (filterUserName == null || filterUserName == string.Empty || e.UserName.Contains(filterUserName))
              &&
              (filterFullName == null || filterFullName == string.Empty || e.FullName.Contains(filterFullName))
              &&
              (filterEmail == null || filterEmail == string.Empty || e.Email.Contains(filterEmail))
            );

            if (sortWith == "asc")
            {
                userDataList = userDataList.OrderBy(o => o.UserName);
            }
            else
            {
                userDataList = userDataList.OrderByDescending(o => orderBy);
            }

            userDataList = userDataList.Skip((pageNumber.Value - 1) * numberPerPage.Value).Take(numberPerPage.Value);

            return userDataList;
        }
    }
}
