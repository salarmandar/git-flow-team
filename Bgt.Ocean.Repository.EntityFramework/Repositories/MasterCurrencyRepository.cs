using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterCurrencyRepository : IRepository<TblMasterCurrency>
    {
        IEnumerable<TblMasterCurrency> FindByCountryGuid(Guid countryGuid);

        IEnumerable<TblMasterCurrency> GetCurrencyList();

        IEnumerable<TblMasterCurrency_ExchangeRate> GetCurrencyExchangeList(Guid? siteGuid);
    }

    public class MasterCurrencyRepository : Repository<OceanDbEntities, TblMasterCurrency>, IMasterCurrencyRepository
    {
        public MasterCurrencyRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterCurrency_ExchangeRate> GetCurrencyExchangeList(Guid? siteGuid)
        {
            var masterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == siteGuid)?.MasterCountry_Guid;
            var global_exchange = DbContext.TblMasterCurrency_ExchangeRate.Where(o => !o.FlagDisable && o.MasterCountry_Guid == null).ToList();
            var local_exchange = DbContext.TblMasterCurrency_ExchangeRate.Where(o => !o.FlagDisable && o.MasterCountry_Guid == masterCountry_Guid).ToList();


            var include_global = global_exchange.Where(o => !local_exchange.Any(t => t.MasterCurrencySource_Guid == o.MasterCurrencySource_Guid && t.MasterCurrencyTarget_Guid == o.MasterCurrencyTarget_Guid))
                                                .Select(o => { o.MasterCountry_Guid = masterCountry_Guid; return o; });

            var merge_exchange = local_exchange.Union(include_global);

            return merge_exchange;
        }

        public IEnumerable<TblMasterCurrency> FindByCountryGuid(Guid countryGuid)
        {
            var findInCountry = DbContext.TblMasterCurrencyDependOnCountry.Where(e => e.MasterCountry_Guid == countryGuid).Select(e => e.MasterCurrency_Guid).ToList();
            var allCurrencyInCountry = DbContext.TblMasterCurrency.Where(e => findInCountry.Contains(e.Guid) && !e.FlagDisable);
            return allCurrencyInCountry;
        }

        public IEnumerable<TblMasterCurrency> GetCurrencyList()
        {
            return DbContext.TblMasterCurrency.Where(o => o.FlagDisable == false).OrderBy(o => o.MasterCurrencyAbbreviation).ToList();
        }
    }
}
