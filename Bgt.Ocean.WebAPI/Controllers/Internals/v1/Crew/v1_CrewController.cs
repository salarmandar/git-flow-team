using Bgt.Ocean.Service.Implementations.RunControl;
using Bgt.Ocean.WebAPI.Models.CrewManagement;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_CrewController : ApiControllerBase
    {
        #region Objects & Variables
        private readonly ICrewService _crewService;
        #endregion

        #region Constructor
        public v1_CrewController(
            ICrewService crewService)
        {
            _crewService = crewService;
        }
        #endregion

        #region Functions

        #region Get
        public bool CheckIfCrewAvailable(Guid employeeGuid, DateTime workDate)
        {
            return _crewService.CheckIfCrewOnDispatchedRun(employeeGuid, workDate);
        }

       

        public int GetCrewRole(Guid dailyEmployeeGuid)
        {
            return _crewService.GetCrewRold(dailyEmployeeGuid);
        }

        public IEnumerable<string> GetCrewRoleDolphin(Guid countryGuid, Guid siteGuid)
        {
            return _crewService.GetDolphinConfiguration(countryGuid, siteGuid);
        }
        #endregion

        #region POST
        [HttpPost]
        public IEnumerable<string> FindCrewInOtherDispatchRun([FromBody]CrewInRunViewModel model)
        {
            return _crewService.CheckEmployeeAssignInAnotherRun(model.EmployeeGuid, model.DailyRunGuid,model.WorkDate);
        }
        #endregion



        #endregion
    }
}