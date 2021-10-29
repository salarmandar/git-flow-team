using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation
{
    #region Interface
    public interface IMasterCustomerLocation_EmailActionRepository : IRepository<TblMasterCustomerLocation_EmailAction>
    {
        IEnumerable<TblMasterCustomerLocation_EmailAction> GetLocationsEmail(IEnumerable<Guid?> locations);
    }
    #endregion

    public class MasterCustomerLocation_EmailActionRepository : Repository<OceanDbEntities, TblMasterCustomerLocation_EmailAction>, IMasterCustomerLocation_EmailActionRepository
    {
        public MasterCustomerLocation_EmailActionRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterCustomerLocation_EmailAction> GetLocationsEmail(IEnumerable<Guid?> locations)
        {
            return DbContext.TblMasterCustomerLocation_EmailAction
                    .Join(locations, locEmail => locEmail.MasterCustomerLocation_Guid, loc => loc, (locEmail, loc) => locEmail);
        }
    }
}
