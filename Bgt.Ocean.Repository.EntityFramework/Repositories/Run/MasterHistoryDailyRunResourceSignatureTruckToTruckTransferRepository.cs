using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Run
{
    public interface IMasterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository : IRepository<TblMasterHistory_DailyRunResource_SignatureTruckToTruckTransfer>
    {
    }

    public class MasterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository : Repository<OceanDbEntities, TblMasterHistory_DailyRunResource_SignatureTruckToTruckTransfer>, IMasterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository
    {
        public MasterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
