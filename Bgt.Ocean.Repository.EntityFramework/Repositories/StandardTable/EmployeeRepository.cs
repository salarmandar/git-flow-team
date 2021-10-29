using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IEmployeeRepository : IRepository<TblMasterEmployee>
    {
        IEnumerable<TblMasterEmployee> GetEmployeeBySite(Guid siteGuid);
    }

    public class EmployeeRepository : Repository<OceanDbEntities, TblMasterEmployee>, IEmployeeRepository
    {
        public EmployeeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public IEnumerable<TblMasterEmployee> GetEmployeeBySite(Guid siteGuid)
        {
            return DbContext.TblMasterEmployee.Where(e => e.MasterSite_Guid == siteGuid && !(bool)e.FlagDisable);
        }
    }
}
