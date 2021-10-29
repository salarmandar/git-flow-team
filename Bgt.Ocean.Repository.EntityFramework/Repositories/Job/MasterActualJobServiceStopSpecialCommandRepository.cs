using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    #region Interface

    public interface IMasterActualJobServiceStopSpecialCommandRepository : IRepository<TblMasterActualJobServiceStopSpecialCommand>
    {
        IEnumerable<TblMasterActualJobServiceStopSpecialCommand> FindByServiceStop(Guid serviceStopGuidPK, Guid serviceStopGuidDEL);
    }

    #endregion

    public class MasterActualJobServiceStopSpecialCommandRepository : Repository<OceanDbEntities, TblMasterActualJobServiceStopSpecialCommand>, IMasterActualJobServiceStopSpecialCommandRepository
    {
        public MasterActualJobServiceStopSpecialCommandRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterActualJobServiceStopSpecialCommand> FindByServiceStop(Guid serviceStopGuidPK, Guid serviceStopGuidDEL)
        {
            return DbContext.TblMasterActualJobServiceStopSpecialCommand.Where(e => e.MasterActualJobServiceStop_Guid == serviceStopGuidPK || e.MasterActualJobServiceStop_Guid == serviceStopGuidDEL);

        }
    }
}
