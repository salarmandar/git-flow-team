using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData
{
    #region Interface

    public interface IMasterReasonTypeRepository : IRepository<TblMasterReasonType>
    {
        IEnumerable<TblMasterReasonType> FindBySiteGuid(Guid siteGuid);
    }

    #endregion
    public class MasterReasonTypeRepository : Repository<OceanDbEntities, TblMasterReasonType>, IMasterReasonTypeRepository
    {
        public MasterReasonTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterReasonType> FindBySiteGuid(Guid siteGuid)
        {
            var customerList = DbContext.TblMasterCustomerLocation.Where(e => e.MasterSite_Guid == siteGuid).Select(o => o.MasterCustomer_Guid);
            var customerGuid = DbContext.TblMasterCustomer.FirstOrDefault(e => customerList.Contains(e.Guid) && e.FlagChkCustomer == false).Guid;
            return DbContext.TblMasterReasonType.Where(e => e.MasterCustomer_Guid == customerGuid && !e.FlagDisable);
        }
    }
}
