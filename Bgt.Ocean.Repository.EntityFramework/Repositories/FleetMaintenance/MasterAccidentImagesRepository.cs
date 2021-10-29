using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterAccidentImagesRepository : IRepository<TblMasterRunResource_Accident_Images>
    {
    }
    public class MasterAccidentImagesRepository : Repository<OceanDbEntities, TblMasterRunResource_Accident_Images>, IMasterAccidentImagesRepository
    {
        public MasterAccidentImagesRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
