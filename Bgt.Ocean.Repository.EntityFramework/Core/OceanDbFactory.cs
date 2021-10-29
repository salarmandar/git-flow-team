using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Core
{
    public class OceanDbFactory : DbFactory<OceanDbEntities>
    {
        protected override OceanDbEntities NewDbContext()
        {
            return new OceanDbEntities(FlagUseSTG);
        }

        public static OceanDbEntities GetNewDbContext() => new OceanDbFactory().GetNewDataContext;
    }
}
