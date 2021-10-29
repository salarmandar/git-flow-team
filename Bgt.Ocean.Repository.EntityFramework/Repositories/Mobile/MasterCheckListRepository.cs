using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Mobile
{
    public interface IMobileATMCheckListEERepository : IRepository<TblMobileATMCheckListEE>
    {
        IEnumerable<CheckListItem> GetCheckListEE(Guid jobGuid);
    }

    public class MobileATMCheckListEERepository : Repository<OceanDbEntities, TblMobileATMCheckListEE>, IMobileATMCheckListEERepository
    {
        public MobileATMCheckListEERepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
        public IEnumerable<CheckListItem> GetCheckListEE(Guid jobGuid)
        {
            var result = DbContext.TblMobileATMCheckListEE
                        .Where(e => DbContext.TblMasterActualJobServiceStopLegs.Any(o => o.MasterActualJobHeader_Guid == jobGuid && o.Guid == e.MasterActualJobServiceStopLeg_Guid))
                        .OrderBy(c => c.CheckListName)
                        .Select(x => new CheckListItem
                        {
                            Item = x.CheckListName,
                            IsChecked = x.FlagIsChecked
                        });
            return result;
        }
    }
}
