using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{


    public interface ISystemJobHideScreenRepository : IRepository<TblSystemJobHideScreen>
    {
        IEnumerable<SystemJobHideScreenView> FindAll(Guid? languageGuid);
    }
    public class SystemJobHideScreenRepository : Repository<OceanDbEntities, TblSystemJobHideScreen>, ISystemJobHideScreenRepository
    {
        public SystemJobHideScreenRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<SystemJobHideScreenView> FindAll(Guid? languageGuid)
        {
            return DbContext.TblSystemJobHideScreen.Where(o => !o.FlagDisable)
                                                  .Select(o => new SystemJobHideScreenView
                                                  {
                                                      Guid = o.Guid,
                                                      ScreenName = (DbContext.TblSystemDisplayTextControlsLanguage
                                                      .FirstOrDefault(s => s.Guid == o.SystemDisplayTextControls_Guid && s.SystemLanguageGuid == languageGuid)
                                                      .DisplayText) ?? o.ScreenName
                                                  });
        }
    }
}
