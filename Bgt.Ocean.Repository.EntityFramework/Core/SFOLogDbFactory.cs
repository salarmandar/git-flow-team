using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Core
{
    public class SFOLogDbFactory : DbFactory<SFOLogDbEntities>
    {
        protected override SFOLogDbEntities NewDbContext()
        {
            return new SFOLogDbEntities(FlagUseSTG);
        }
    }
}
