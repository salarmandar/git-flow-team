using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemDisplayTextControlsLanguageRepository : IRepository<TblSystemDisplayTextControlsLanguage>
    {
        TblSystemDisplayTextControlsLanguage FindDisplayControlLanguage(Guid displayControlId, Guid languageGuid);
        IEnumerable<TblSystemDisplayTextControlsLanguage> FindDisplayControlLanguageList(IEnumerable<Guid> displayControlGuids, Guid languageGuid);
    }

    public class SystemDisplayTextControlsLanguageRepository : Repository<OceanDbEntities, TblSystemDisplayTextControlsLanguage>, ISystemDisplayTextControlsLanguageRepository
    {
        public SystemDisplayTextControlsLanguageRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemDisplayTextControlsLanguage FindDisplayControlLanguage(Guid displayControlId, Guid languageGuid)
        {
            var enGuid = Guid.Parse("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0");
            var controlDisplayTexts = DbContext.TblSystemDisplayTextControlsLanguage.Where(e => e.Guid == displayControlId);
            var result = controlDisplayTexts.FirstOrDefault(e => e.SystemLanguageGuid == languageGuid);
            return result == null ? controlDisplayTexts.FirstOrDefault(e => e.SystemLanguageGuid == enGuid) : result;
        }

        public IEnumerable<TblSystemDisplayTextControlsLanguage> FindDisplayControlLanguageList(IEnumerable<Guid> displayControlGuids, Guid languageGuid)
        {
            var result = DbContext.TblSystemDisplayTextControlsLanguage.Where(e => e.SystemLanguageGuid == languageGuid)
                                      .Join(displayControlGuids,
                                      l => l.Guid,
                                      d => d,
                                      (l, d) => l).AsEnumerable();

            return result;
        }
    }
}
