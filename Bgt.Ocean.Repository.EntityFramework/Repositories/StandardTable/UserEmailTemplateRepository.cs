using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IUserEmailTemplateRepository : IRepository<TblMasterUserEmailTemplate>
    {

    }

    public class UserEmailTemplateRepository : Repository<OceanDbEntities, TblMasterUserEmailTemplate>, IUserEmailTemplateRepository
    {
        public UserEmailTemplateRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
