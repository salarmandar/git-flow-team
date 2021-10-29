using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemJobHideFieldRepository : IRepository<TblSystemJobHideField>
    {
        IEnumerable<SystemJobHideFieldView> FindByHideScreen(Guid? languageGuid, Guid HideScreenGuid);
        IEnumerable<SystemJobHideFieldView> FindAll(Guid? languageGuid);
    }

    public class SystemJobHideFieldRepository : Repository<OceanDbEntities, TblSystemJobHideField>, ISystemJobHideFieldRepository
    {
      
            public SystemJobHideFieldRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
            {
            }

        public IEnumerable<SystemJobHideFieldView> FindByHideScreen(Guid? languageGuid , Guid HideScreenGuid)
        {
            return DbContext.TblSystemJobHideField.Where(o => !o.FlagDisable && o.SystemJobHideScreen_Guid == HideScreenGuid)
                                                  .Select(o => new SystemJobHideFieldView
                                                  {
                                                      Guid = o.Guid,
                                                      FieldName = (DbContext.TblSystemDisplayTextControlsLanguage
                                                      .FirstOrDefault(s => s.Guid == o.SystemDisplayTextControls_Guid && s.SystemLanguageGuid == languageGuid)
                                                      .DisplayText) ?? o.FieldName
                                                  });
        }


        public IEnumerable<SystemJobHideFieldView> FindAll(Guid? languageGuid)
            {
                return DbContext.TblSystemJobHideField.Where(o => !o.FlagDisable)
                                                      .Select(o => new SystemJobHideFieldView
                                                      {
                                                          Guid = o.Guid,
                                                          FieldName = (DbContext.TblSystemDisplayTextControlsLanguage
                                                          .FirstOrDefault(s => s.Guid == o.SystemDisplayTextControls_Guid && s.SystemLanguageGuid == languageGuid)
                                                          .DisplayText) ?? o.FieldName
                                                      });
            }
        

    }
}
