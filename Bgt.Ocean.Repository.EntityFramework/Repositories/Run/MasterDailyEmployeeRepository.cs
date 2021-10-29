using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Run
{
    #region Interface
    public interface IMasterDailyEmployeeRepository
    {
        bool CheckCrewOnDispatchRun(Guid employeeGuid, DateTime workDate);
        int GetCrewRole(Guid dailyEmployeeGuid);

        IEnumerable<string> CheckEmployeeAssignInAnotherRun(IEnumerable<Guid> empGuid, Guid dailyRunGuid, DateTime? workDate);
    }
    #endregion

    #region Class
    public class MasterDailyEmployeeRepository : Repository<OceanDbEntities, TblMasterDailyEmployee>, IMasterDailyEmployeeRepository
    {
        #region Constructor
        public MasterDailyEmployeeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        #endregion

        #region Function
        public bool CheckCrewOnDispatchRun(Guid employeeGuid, DateTime workDate)
        {
            return DbContext.TblMasterDailyEmployee.Where(o => o.MasterEmployee_Guid == employeeGuid)
                    .Join(DbContext.TblMasterDailyRunResource.Where(o => o.WorkDate == workDate), employee => employee.MasterDailyRunResource_Guid, run => run.Guid, (employee, run) => run)
                    .Any(o => o.RunResourceDailyStatusID == 2);
        }

        public int GetCrewRole(Guid dailyEmployeeGuid)
        {
            return DbContext.TblMasterDailyEmployee.FirstOrDefault(o => o.Guid == dailyEmployeeGuid).RoleInRunResourceID.GetValueOrDefault();
        }

        public IEnumerable<string> CheckEmployeeAssignInAnotherRun(IEnumerable<Guid> empGuid,Guid dailyRunGuid,DateTime? workDate) {
            return DbContext.TblMasterDailyEmployee.Where(o => empGuid.Contains(o.MasterEmployee_Guid))
                   .Join(DbContext.TblMasterDailyRunResource.Where(o => o.Guid != dailyRunGuid && o.RunResourceDailyStatusID == 2 && o.WorkDate == workDate),
                   employee => employee.MasterDailyRunResource_Guid,
                   run => run.Guid, (employee, run) => employee)
                   .Join(DbContext.TblMasterEmployee.Where(o=> empGuid.Contains(o.Guid)),
                   emp => emp.MasterEmployee_Guid,
                   ee => ee.Guid,
                   (emp, ee) => ee).Select(ee => ee.FirstName).Distinct();
        }

        #endregion
    }
    #endregion
}
