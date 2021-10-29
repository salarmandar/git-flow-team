using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region interface
    public interface ISystemTitleNameRepository : IRepository<TblSystemTitleName>
    {
        IEnumerable<SystemTitleNameView> GetTitleNameListByLanguageGuid(Guid languageGuid);
    }
    #endregion
    public class SystemTitleNameRepository : Repository<OceanDbEntities, TblSystemTitleName>, ISystemTitleNameRepository
    {
        public SystemTitleNameRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<SystemTitleNameView> GetTitleNameListByLanguageGuid(Guid languageGuid)
        {
            return DbContext.TblSystemTitleName
                    .Join(DbContext.TblSystemDisplayTextControlsLanguage.Where(x => x.SystemLanguageGuid == languageGuid),
                    t => t.SystemDisplayTextControls_Guid,
                    l => l.Guid,
                    (t, l) => new SystemTitleNameView()
                    {
                        Guid = t.Guid,
                        TitleName = l.DisplayText
                    });
        }
    }
}
