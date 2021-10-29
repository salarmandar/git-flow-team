

using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{


    public interface ISystemMaintenanceStatusRepository : IRepository<TblSystemMaintenanceStatus>
    {
        IEnumerable<TblSystemMaintenanceStatus> GetMaintenanceStatus();
    }

    public class SystemMaintenanceStatusRepository : Repository<OceanDbEntities, TblSystemMaintenanceStatus>, ISystemMaintenanceStatusRepository
    {
        public SystemMaintenanceStatusRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblSystemMaintenanceStatus> GetMaintenanceStatus()
        {

            var languageGuid = ApiSession.UserLanguage_Guid;
            var maintenanceStatus = (from s in DbContext.TblSystemMaintenanceStatus
                                     join l in DbContext.TblSystemDisplayTextControlsLanguage.Where(o => o.SystemLanguageGuid == languageGuid) on s.SystemDisplayTextControls_Guid equals l.Guid into DisplayText
                                     from LDisplayText in DisplayText.DefaultIfEmpty()
                                     select new { s, lang = LDisplayText })
                                     .AsEnumerable()
                                     .Select(o =>
                                     {
                                         o.s.MaintenanceStatusName = o.lang.DisplayText == null ? o.s.MaintenanceStatusName : o.lang.DisplayText;
                                         return o.s;
                                     });
            return maintenanceStatus;
        }
    }
}
