using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Customer;
using Bgt.Ocean.Models.Nemo.NemoSync;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterCustomerRepository : IRepository<TblMasterCustomer>
    {
        IEnumerable<BrinksCompanyResult> Func_BrinksCompany_Get(Guid? userGuid, int roleUser, Guid? siteGuid, Guid? countryGuid, Guid? languageGuid);
        IEnumerable<CustomerGeneralResult> Func_Customer_General_Get(Guid? siteGuid, Guid countryGuid, Guid? masterUserGuid, int roleType);
        IEnumerable<AdhocCustomerResult> Func_AdhocCustomer_Get(AdhocCustomerRequest request);
        TblMasterCustomer GetCompanyOnCustomerBySite(Guid? siteGuid);
        TblSystemCustomerOfType GetCustomerOfTypeByID(int? customerTypeID);
        IEnumerable<CustomerByCompanyResult> Func_CustomerByCompany_Get(Guid company_Guid);

        Task<List<SyncMasterCustomerRequest>> FindByLastNemoSyncAsync(DateTime dateTime);
        Task<List<CustomerView>> GetCustomerBySiteGuid(Guid siteGuid);
    }

    public class MasterCustomerRepository : Repository<OceanDbEntities, TblMasterCustomer>, IMasterCustomerRepository
    {
        public MasterCustomerRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<BrinksCompanyResult> Func_BrinksCompany_Get(Guid? userGuid, int roleUser, Guid? siteGuid, Guid? countryGuid, Guid? languageGuid)
        {
            return DbContext.Up_OceanOnlineMVC_BrinksCompany_Get(userGuid, siteGuid, countryGuid, languageGuid, roleUser);
        }

        public IEnumerable<CustomerGeneralResult> Func_Customer_General_Get(Guid? siteGuid, Guid countryGuid, Guid? masterUserGuid, int roleType)
        {
            return DbContext.Up_OceanOnlineMVC_Customer_General_Get(masterUserGuid, roleType, siteGuid, countryGuid);
        }
        public IEnumerable<AdhocCustomerResult> Func_AdhocCustomer_Get(AdhocCustomerRequest request)
        {
            var WorkDate = request.strWorkDate.ChangeFromStringToDate(request.DateTimeFormat);
            Guid? DayOfWeek = null;
            return DbContext.Up_OceanOnlineMVC_Adhoc_Customer_Get(request.UserGuid, request.RoleUser, request.SiteGuid, DayOfWeek, request.JobType_Guid, request.LobGuid, WorkDate, request.FlagDestination, request.CustomerLocaitonPK, request.SiteDelGuid, request.subServiceType_Guid).OrderBy(e => e.CustomerFullName);
        }

        public TblMasterCustomer GetCompanyOnCustomerBySite(Guid? siteGuid)
        {
            var company = DbContext.TblMasterCustomerLocation.Where(e => e.MasterSite_Guid == siteGuid && e.FlagDisable == false)
                .Join(DbContext.TblMasterCustomer.Where(e => e.FlagDisable == false && e.FlagChkCustomer == false),
                CL => CL.MasterCustomer_Guid,
                C => C.Guid,
               (CL, C) => new { CusLocation = CL, Customer = C }).Select(x => x.Customer).FirstOrDefault();

            return company;
        }

        public TblSystemCustomerOfType GetCustomerOfTypeByID(int? customerTypeID)
        {
            return DbContext.TblSystemCustomerOfType.FirstOrDefault(o => o.SystemCustomerOfTypeID == customerTypeID);
        }

        public IEnumerable<CustomerByCompanyResult> Func_CustomerByCompany_Get(Guid company_Guid)
        {
            return DbContext.Up_OceanOnlineMVC_CustomerByCompany_Get(company_Guid);
        }

        public async Task<List<SyncMasterCustomerRequest>> FindByLastNemoSyncAsync(DateTime dateTime)
        {
            using (OceanDbEntities context = NewDbContext())
            {
                return await context.TblMasterCustomer.Where(o => o.FlagDisable.HasValue && !o.FlagDisable.Value && (!o.LastNemoSync.HasValue || o.LastNemoSync < dateTime))
                            .Join(context.TblMasterCustomerLocation.Where(o => !o.FlagDisable), customer => customer.Guid, location => location.MasterCustomer_Guid, (customer, location) => new { Customer = customer, Location = location })
                            .Join(context.TblMasterCustomerLocation_BrinksSite.Where(o => o.FlagDefaultBrinksSite), location => location.Location.Guid, defaultSite => defaultSite.MasterCustomerLocation_Guid, (location, defaultSite) => new { Customer = location.Customer, Location = location.Location, DefaultSite = defaultSite })
                            .Join(context.TblMasterSite.Where(o => !o.FlagDisable), joinData => joinData.DefaultSite.MasterSite_Guid, site => site.Guid, (joinData, site) => new { Customer = joinData.Customer, Location = joinData.Location, Site = site })
                            .Join(context.TblMasterCountry.Where(o => !o.FlagDisable), joinData => joinData.Site.MasterCountry_Guid, country => country.Guid, (joinData, country) => new { Customer = joinData.Customer, Location = joinData.Location, Site = joinData.Site, Country = country })
                            .GroupJoin(context.TblMasterDistrict.Where(o => !o.FlagDisable), joinData => joinData.Customer.MasterDistrict_Guid, district => district.Guid, (joinData, district) => new { Customer = joinData.Customer, Location = joinData.Location, Site = joinData.Site, Country = joinData.Country, District = district })
                            .SelectMany(o => o.District.DefaultIfEmpty(), (joinData, district) => new { Customer = joinData.Customer, Location = joinData.Location, Site = joinData.Site, Country = joinData.Country, District = district })
                            .GroupJoin(context.TblMasterCountry_State.Where(o => !o.FlagDisable), joinData => joinData.Customer.MasterCountry_State_Guid, state => state.Guid, (joinData, state) => new { Customer = joinData.Customer, Location = joinData.Location, Site = joinData.Site, Country = joinData.Country, District = joinData.District, State = state })
                            .SelectMany(o => o.State.DefaultIfEmpty(), (joinData, state) => new { Customer = joinData.Customer, Location = joinData.Location, Site = joinData.Site, Country = joinData.Country, District = joinData.District, State = state })
                            .Select(o => new SyncMasterCustomerRequest()
                            {
                                CountryCode = o.Country.MasterCountryAbbreviation,
                                MasterSiteGuid = o.Site.Guid,
                                BranchCode = o.Site.SiteCode,
                                CustomerGuid = o.Customer.Guid,
                                Code = o.Customer.CustomerCodeExternalReference,
                                Name = o.Customer.CustomerFullName,
                                Address = (string.IsNullOrEmpty(o.Customer.CustomerAddress) ? "N/A" : o.Customer.CustomerAddress),
                                Address2 = "N/A",
                                City = (o.District == null || string.IsNullOrEmpty(o.District.MasterDistrictName) ? "N/A" : o.District.MasterDistrictName),
                                State = (o.State == null || string.IsNullOrEmpty(o.State.MasterStateName) ? "N/A" : o.State.MasterStateName),
                                Postcode = (string.IsNullOrEmpty(o.Customer.PostelCode) ? "N/A" : o.Customer.PostelCode),
                                Email = (string.IsNullOrEmpty(o.Customer.CustomerEmail) ? "N/A" : o.Customer.CustomerEmail),
                                Telephone = (string.IsNullOrEmpty(o.Customer.TelephoneNo) ? "N/A" : o.Customer.TelephoneNo),
                                Contact = "N/A",
                                Icon = "N/A"
                            }).Distinct().ToListAsync();
            }
        }

        public async Task<List<CustomerView>> GetCustomerBySiteGuid(Guid siteGuid)
        {
            using (OceanDbEntities context = NewDbContext())
            {
                return await context.TblMasterCustomerLocation_BrinksSite.Where(o => o.FlagDefaultBrinksSite && o.MasterSite_Guid == siteGuid)
                                .Join(context.TblMasterCustomerLocation.Where(o => !o.FlagDisable), locSite => locSite.MasterCustomerLocation_Guid, loc => loc.Guid, (locSite, loc) => loc)
                                .Join(context.TblMasterCustomer.Where(o => o.FlagDisable.HasValue && !o.FlagDisable.Value && o.FlagChkCustomer.HasValue && o.FlagChkCustomer.Value), loc => loc.MasterCustomer_Guid, cus => cus.Guid, (loc, cus) => cus)
                                .Select(o => new CustomerView { Guid = o.Guid, Code = o.CustomerCodeExternalReference, Name = o.CustomerFullName }).Distinct().OrderBy(o => o.Name).ToListAsync();
            }
        }
    }
}
