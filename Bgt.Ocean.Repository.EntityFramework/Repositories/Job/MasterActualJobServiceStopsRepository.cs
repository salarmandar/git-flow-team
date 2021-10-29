using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    #region Interface

    public interface IMasterActualJobServiceStopsRepository : IRepository<TblMasterActualJobServiceStop>
    {
        IEnumerable<TblMasterActualJobServiceStop> FindByJobHeader(Guid jobGuid);
    }

    #endregion

    public class MasterActualJobServiceStopsRepository : Repository<OceanDbEntities, TblMasterActualJobServiceStop>, IMasterActualJobServiceStopsRepository
    {
        public MasterActualJobServiceStopsRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterActualJobServiceStop> FindByJobHeader(Guid jobGuid)
        {
            return DbContext.TblMasterActualJobServiceStop.Where(e => e.MasterActualJobHeader_Guid == jobGuid);
        }
    }
}
