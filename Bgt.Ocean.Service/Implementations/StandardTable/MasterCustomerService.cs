using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.CustomerService;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Customer;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Models.Customer;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{
    public interface IMasterCustomerService
    {
        IEnumerable<AdhocCustomerView> GetAdhocCustomer(RequestAdhocCustomer request);
        IEnumerable<AccountView> GetAccountsInCustomer(Guid masterCustomer_Guid);
        IEnumerable<LocationView> GetLocationsInCustomer(Guid masterCustomer_Guid);

        Task<List<CustomerView>> GetCustomerBySiteGuid(Guid siteGuid);
    }

    public class MasterCustomerService : IMasterCustomerService
    {
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterCustomerLocationBrinksSiteRepository _masterCustomerLocationBrinksSiteRepository;
        private readonly IMasterCustomer_AccountRepository _masterCustomer_AccountRepository;
        public MasterCustomerService(
            IMasterCustomerRepository masterCustomerRepository,
            IMasterCustomerLocationRepository masterCustomerLocationRepository,
            IMasterCustomerLocationBrinksSiteRepository masterCustomerLocationBrinksSiteRepository,
            IMasterCustomer_AccountRepository masterCustomer_AccountRepository
        )
        {
            _masterCustomerRepository = masterCustomerRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _masterCustomerLocationBrinksSiteRepository = masterCustomerLocationBrinksSiteRepository;
            _masterCustomer_AccountRepository = masterCustomer_AccountRepository;
        }

        public IEnumerable<AdhocCustomerView> GetAdhocCustomer(RequestAdhocCustomer request)
        {
            return _masterCustomerRepository.Func_AdhocCustomer_Get(request.ConvertToAdhocCustomerRequest()).ConvertToAdhocCustomerView();
        }
 

        public IEnumerable<CustomerAdhocView> CustomerForAdhoc(Guid brinkSiteGuid, bool flagDestLocation)
        {
            Guid cusTypeGuid = _masterCustomerRepository.GetCustomerOfTypeByID(1).Guid;
            return _masterCustomerRepository.FindAll(o => o.FlagDisable != true && o.FlagChkCustomer == true && o.SystemCustomerOfType_Guid == cusTypeGuid)
                               .Join(_masterCustomerLocationRepository.FindAll(o => !o.FlagDisable)
                               , cus => cus.Guid
                               , cusLoc => cusLoc.MasterCustomer_Guid
                               , (cus, cusLoc) => new { cus, cusLoc })
                               .Join(_masterCustomerLocationBrinksSiteRepository.FindAll(o => o.FlagDefaultBrinksSite && o.MasterSite_Guid == brinkSiteGuid)
                               , c1 => c1.cusLoc.Guid
                               , cusBrinks => cusBrinks.MasterCustomerLocation_Guid
                               , (c1, cusBrinks) => new { c1, cusBrinks })
                               .Select(o => new CustomerAdhocView
                               {
                                   Guid = o.c1.cus.Guid,
                                   CustomerFullName = o.c1.cus.CustomerFullName,
                                   FlagLocationDestination = flagDestLocation
                               }).Distinct().OrderBy(x => x.CustomerFullName);
        }

        public IEnumerable<AccountView> GetAccountsInCustomer(Guid masterCustomer_Guid)
        {
            var accounts = _masterCustomer_AccountRepository.FindAll(e => e.MasterCustomer_Guid == masterCustomer_Guid && !e.FlagDisable).Select(e => new AccountView()
            {
                Guid = e.Guid,
                Account_Guid = e.MasterAccount_Guid.GetValueOrDefault(),
                MasterCustomer_Guid = e.MasterCustomer_Guid,
                MasterCustomer_Code = e.TblMasterAccount.CustomerCodeExternalReference,
                AccountName = e.AccountName,
                AccountCustomerName = e.TblMasterAccount.CustomerFullName,
                AccountNo = e.AccountNumber,
                MasterCountry_Guid = e.TblMasterAccount.MasterCountry_Guid.GetValueOrDefault(),
            });
            return accounts.OrderBy(e => e.AccountNo);
        }

        public IEnumerable<LocationView> GetLocationsInCustomer(Guid masterCustomer_Guid)
        {
            var allLocationInCustomer = _masterCustomerLocationRepository.FindLocationsInCustomer(masterCustomer_Guid);
            return allLocationInCustomer;
        }

        public async Task<List<CustomerView>> GetCustomerBySiteGuid(Guid siteGuid)
        {
            return await _masterCustomerRepository.GetCustomerBySiteGuid(siteGuid);
        }
    }
}
