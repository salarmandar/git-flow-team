using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterCustomerContractRepository : IRepository<TblMasterCustomerContract>
    {
        IEnumerable<TblLeedToCashContract_PricingRules> FindPricingRuleInProduct(Guid contractGuid, Guid productGuid);
        IEnumerable<CustomerContractView> FindCustomerContract(IEnumerable<Guid> locationPKGuids, IEnumerable<Guid> locationDLGuids, Guid? lobGuid, Guid? servicetypeGuid, Guid? subservicetypeGuid, DateTime? workdatePK, DateTime? workdateDL);
    }

    public class MasterCustomerContractRepository : Repository<OceanDbEntities, TblMasterCustomerContract>, IMasterCustomerContractRepository
    {
        public MasterCustomerContractRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblLeedToCashContract_PricingRules> FindPricingRuleInProduct(Guid contractGuid, Guid productGuid)
        {
            return DbContext.TblLeedToCashContract_PricingRules.Where(e => e.MasterCustomerContract_Guid == contractGuid && e.TblPricingRule.Product_Guid == productGuid);
        }

        public IEnumerable<CustomerContractView> FindCustomerContract(IEnumerable<Guid> locationPKGuids, IEnumerable<Guid> locationDLGuids, Guid? lobGuid, Guid? servicetypeGuid, Guid? subservicetypeGuid, DateTime? workdatePK, DateTime? workdateDL)
        {
            var cont = DbContext.TblMasterCustomerContract_ServiceLocation
                                    .Where(o => locationPKGuids.Any(l => l == o.MasterCustomerLocation_Guid)
                                    //Check date expired PK
                                    && (o.TblMasterCustomerContract.ContractExpiredDate >= workdatePK)
                                    && (!o.TblMasterCustomerContract.FlagDisable)
                                    //check lobs,services,sub services
                                    && (o.SystemSubServiceType_Guid == subservicetypeGuid)
                                    && (o.SystemLineOfBusiness_Guid == lobGuid && o.SystemServiceJobType_Guid == servicetypeGuid)
                                    )
                                    .GroupBy(o => o.MasterCustomerContract_Guid)
                                    //all excatly match in DL location list
                                    .Where(o => locationPKGuids.All(l => o.Any(x => x.MasterCustomerLocation_Guid == l)))
                                    .SelectMany(o => o)
                                    .Union(
                           DbContext.TblMasterCustomerContract_ServiceLocation
                                    .Where(o => locationDLGuids.Any(l => l == o.MasterCustomerLocation_Guid)
                                    //Check date expired DL
                                    && (o.TblMasterCustomerContract.ContractExpiredDate >= workdateDL)
                                    && (!o.TblMasterCustomerContract.FlagDisable)
                                    //check lobs,services,sub services
                                    && (o.SystemSubServiceType_Guid == subservicetypeGuid)
                                    && (o.SystemLineOfBusiness_Guid == lobGuid && o.SystemServiceJobType_Guid == servicetypeGuid)
                                    )
                                    //all excatly match in DL location list
                                    .GroupBy(o => o.MasterCustomerContract_Guid)
                                    .Where(o => locationDLGuids.All(l => o.Any(x => x.MasterCustomerLocation_Guid == l))
                                    ).SelectMany(o => o));

            var result = cont.Select(c => new CustomerContractView
                                    {
                                        ContractGuid = c.TblMasterCustomerContract.Guid,
                                        ContractNo = c.TblMasterCustomerContract.ContractNo,
                                        strExpiredDate = c.TblMasterCustomerContract.ContractExpiredDate.ToString()
                                    }).Distinct();

            return result;
        }
    }
}
