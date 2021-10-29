using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.PricingRules
{
    public interface ISaleService
    {
        IEnumerable<CustomerGeneralResult> GetCustomersInTeamUser(Guid user_Guid, Guid country_Guid);
    }

    public class SaleService : ISaleService
    {
        private readonly IMasterUserRepository _masterUserRepository;
        private readonly IMasterCountryRepository _masterCountryRepository;
        public SaleService(
            IMasterUserRepository masterUserRepository,
            IMasterCountryRepository masterCountryRepository)
        {
            _masterUserRepository = masterUserRepository;
            _masterCountryRepository = masterCountryRepository;
        }

        public IEnumerable<CustomerGeneralResult> GetCustomersInTeamUser(Guid user_Guid, Guid country_Guid)
        {
            var country = _masterCountryRepository.FindById(country_Guid);
            var userDetail = _masterUserRepository.FindById(user_Guid);
            var customers = userDetail.TblMasterSalesTeamUserMapping.Where(e => !e.TblMasterSalesTeam.FlagDisable)
                .SelectMany(e => e.TblMasterSalesTeam.TblMasterSalesTeamCustomerMapping);
            var customerInSale = customers
                .Select(e => e.TblMasterCustomer);
            var customerInCountry = customerInSale.Where(e => e.MasterCountry_Guid == country_Guid).Distinct();
            var result = customerInCountry.Select(e => new CustomerGeneralResult()
            {
                CustomerCode = e.CustomerCodeExternalReference,
                CustomerFullName = e.CustomerFullName,
                Guid = e.Guid,
                MasterCountryName = country.MasterCountryName,
                MasterCountry_Guid = country_Guid
            });
            return result;
        }
    }
}
