using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterMachineLockTypeRepository : IRepository<SFOTblMasterMachine_LockType>
    {
        IEnumerable<SFOTblMasterMachine_LockType> FileByListId(List<Guid?> machine);
    }

    #endregion

    public class SFOMasterMachineLockTypeRepository : Repository<OceanDbEntities, SFOTblMasterMachine_LockType>, ISFOMasterMachineLockTypeRepository
    {
        public SFOMasterMachineLockTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<SFOTblMasterMachine_LockType> FileByListId(List<Guid?> machine)
        {
            return DbContext.SFOTblMasterMachine_LockType.Where(w => machine.Contains(w.SFOMasterMachine_Guid));
        }
    }
}
