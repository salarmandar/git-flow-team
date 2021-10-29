using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Email
{
    public interface IMasterEmailTemplateRepository : IRepository<TblMasterEmailTemplate>
    {
        TblMasterEmailTemplate FindByName(string inTemplateName);
    }

    public class MasterEmailTemplateRepository : Repository<OceanDbEntities, TblMasterEmailTemplate>, IMasterEmailTemplateRepository
    {
        public MasterEmailTemplateRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblMasterEmailTemplate FindByName(string inTemplateName)
        {
            return DbContext.TblMasterEmailTemplate.FirstOrDefault(o => !o.FlagDisable && o.Email_Template_Name == inTemplateName);
        }
    }
}
