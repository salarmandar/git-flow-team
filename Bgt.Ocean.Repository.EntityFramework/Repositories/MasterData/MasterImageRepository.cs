using System;
using System.Linq;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface IMasterImageRepository : IRepository<TblMasterImage> {
        TblMasterImage FindByTableID(Guid table_guid);
    }
    public class MasterImageRepository : Repository<OceanDbEntities, TblMasterImage>, IMasterImageRepository
    {
        public MasterImageRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblMasterImage FindByTableID(Guid table_guid)
        {
            return DbContext.TblMasterImage.FirstOrDefault(x => x.Table_Guid == table_guid);
        }
    }
}
