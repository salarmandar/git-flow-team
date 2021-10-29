using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Environment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{

    public interface ISystemEnvironmentMasterCountryTreeViewRepository : IRepository<TblSystemEnvironmentMasterCountryTreeView>
    {
        IEnumerable<TreeNode> GetEnvironmentMasterCountryTreeViewByLanguageGuid(Guid? languageGuid, string appKey);
    }

    public class SystemEnvironmentMasterCountryTreeViewRepository : Repository<OceanDbEntities, TblSystemEnvironmentMasterCountryTreeView>, ISystemEnvironmentMasterCountryTreeViewRepository
    {
        public SystemEnvironmentMasterCountryTreeViewRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory) { }

        public IEnumerable<TreeNode> GetEnvironmentMasterCountryTreeViewByLanguageGuid(Guid? languageGuid, string appKey)
        {

            var result = (from SysEnvCountryTree in DbContext.TblSystemEnvironmentMasterCountryTreeView
                          join SysLang in DbContext.TblSystemDisplayTextControlsLanguage on SysEnvCountryTree.SystemDisplayTextControls_Guid equals SysLang.Guid
                          join SysEnvCountry in DbContext.TblSystemEnvironmentMasterCountry on SysEnvCountryTree.SystemEnvironmentMasterCountry_Guid equals SysEnvCountry.Guid
                          where
                           SysLang.SystemLanguageGuid == languageGuid
                           && !SysEnvCountryTree.FlagDisable
                           && SysEnvCountry.AppKey == appKey
                          select new TreeNode
                          {
                              Guid = SysEnvCountryTree.Guid,
                              Index = SysEnvCountryTree.NodeIndexId,
                              Name = SysLang.DisplayText,
                              Parent = SysEnvCountryTree.NodeParentId,
                              NodeOrderingNo = SysEnvCountryTree.NodeOrderingNo,
                              AppKey = SysEnvCountry.AppKey,
                              Checked = false,
                              expanded = true,
                              enabled = true
                          });

            return result;
        }


    }
}