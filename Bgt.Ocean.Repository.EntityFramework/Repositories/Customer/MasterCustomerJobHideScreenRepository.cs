using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Customer
{
    public interface IMasterCustomerJobHideScreenRepository : IRepository<TblMasterCustomer_JobHideScreen>
    {
        IEnumerable<CustomerJobHideView> GetJobHideScreenConfig(Guid? MasterCustomer_Guid, Guid? SystemLineOfBusiness_Guid, Guid? SystemServiceJobType_Guid, Guid? MasterSubServiceType_Guid);
    }

    public class MasterCustomerJobHideScreenRepository : Repository<OceanDbEntities, TblMasterCustomer_JobHideScreen>, IMasterCustomerJobHideScreenRepository
    {
        public MasterCustomerJobHideScreenRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<CustomerJobHideView> GetJobHideScreenConfig(Guid? MasterCustomer_Guid, Guid? SystemLineOfBusiness_Guid, Guid? SystemServiceJobType_Guid, Guid? MasterSubServiceType_Guid)
        {

            return   DbContext.TblMasterCustomer_JobHideScreen.Where(o =>
                     o.MasterCustomer_Guid == MasterCustomer_Guid
                     && o.SystemLineOfBusiness_Guid == SystemLineOfBusiness_Guid
                     && o.SystemServiceJobType_Guid == SystemServiceJobType_Guid
                     && o.MasterSubServiceType_Guid == MasterSubServiceType_Guid)
                     .Select(o => new CustomerJobHideView() { SystemJobHideScreen_Guid = o.SystemJobHideScreen_Guid 
                                                            , SystemJobHideField_Guid = o.SystemJobHideField_Guid });
        }
    }
}
