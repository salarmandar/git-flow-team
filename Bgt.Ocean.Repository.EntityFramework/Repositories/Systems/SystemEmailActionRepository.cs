using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemEmailActionRepository : IRepository<TblSystemEmailAction>
    {
        List<CustomerEmailActionModel> GetEmailAction(Guid languageGuid);
    }

    public class SystemEmailActionRepository : Repository<OceanDbEntities, TblSystemEmailAction>, ISystemEmailActionRepository
    {
        public SystemEmailActionRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }


        public List<CustomerEmailActionModel> GetEmailAction(Guid languageGuid)
        {
            return DbContext.TblSystemEmailAction.Where(o => !o.FlagDisable)
                    .Join(DbContext.TblSystemDisplayTextControlsLanguage.Where(o => o.SystemLanguageGuid == languageGuid), action => action.SystemDisplayTextControls_Guid, language => language.Guid, (action, language)
                      => new CustomerEmailActionModel
                      {
                          Guid = action.Guid,
                          ActionName = language.DisplayText,
                          FlagDisable = action.FlagDisable,
                          SystemDisplayTextControls_Guid = action.SystemDisplayTextControls_Guid,
                          ActionID = (EnumEmailAction)action.ActionID
                      }).AsEnumerable().OrderBy(o => o.ActionName).ToList();
        }
    }
}
