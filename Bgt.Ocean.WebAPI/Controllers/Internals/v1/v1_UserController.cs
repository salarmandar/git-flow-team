using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_UserController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public v1_UserController(
            IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get Master Group
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<MasterGroupView> GetMasterGroupByCountry(Guid countryGuid)
        {
            return _userService.GetMasterGroupByCountry(countryGuid);
        }

        /// <summary>
        /// Get User Group
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public MasterUserGroupView GetUserGroup(Guid userGuid)
        {
            return _userService.GetUserGroup(userGuid);
        }

        /// <summary>
        /// Find User Can See Menu
        /// </summary>
        /// <param name="masterMenuDetailGuid"></param>
        /// <param name="masterSiteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<UserGroupView> FindUserCanSeeMenu(Guid masterMenuDetailGuid, Guid masterSiteGuid)
        {
            return _userService.FindUserCanSeeMenu(masterMenuDetailGuid,masterSiteGuid);
        }

        /// <summary>
        /// Find User by menu command
        /// </summary>
        /// <param name="masterMenuDetailCommandGuid"></param>
        /// <param name="masterSiteGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<UserGroupView> FindUserByMenuCommand(Guid masterMenuDetailCommandGuid, Guid masterSiteGuid)
        {
            return _userService.FindUserByMenuCommand(masterMenuDetailCommandGuid, masterSiteGuid);
        }

        /// <summary>
        /// Get user by company
        /// </summary>
        /// <param name="company_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<UserView> GetUsersInCompany(Guid company_Guid)
        {
            return _userService.GetUsersInCompany(company_Guid);
        }
        /// <summary>
        /// Get user by country , filter , sort server side paging
        /// </summary>
        /// <param name="country_Guid"></param>
        /// <param name="filterUserName"></param>
        /// <param name="filterFullName"></param>
        /// <param name="filterEmail"></param>
        /// <param name="pageNumber"></param>
        /// <param name="numberPerPage"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortWith"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<UserView> GetUsersInCountry(Guid country_Guid, int? pageNumber, int? numberPerPage, string sortBy, string sortWith, string filterUserName = null, string filterFullName = null, string filterEmail = null)
        {
            return _userService.GetUsersInCountry(country_Guid, filterUserName, filterFullName, filterEmail, pageNumber, numberPerPage, sortBy, sortWith);
        }
    }
}
