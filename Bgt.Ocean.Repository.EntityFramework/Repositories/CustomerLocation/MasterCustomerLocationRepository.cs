using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Models.Nemo.NemoSync;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation
{
    #region Interface
    public interface IMasterCustomerLocationRepository : IRepository<TblMasterCustomerLocation>
    {
        IEnumerable<AdhocLocationByCustomerResult> Func_AdhocLocationByCustomer_Get(Guid? customerGuid, Guid? siteGuid, Guid? dayOfWeek_Guid, Guid? jobType_Guid, Guid? lineOfBusiness_Guid, DateTime? workDate, bool flagDestination, Guid? customerLocaitonPK, Guid? siteGuid_Del, Guid? subServiceType_Guid);
        Guid GetCompanyGuidBySite(Guid siteGuid);
        Guid GetLocationByCodeReference(string codeReference, Guid country);
        IEnumerable<LocationView> FindLocationsInCustomer(Guid masterCustomer_Guid);

        Task<List<SyncMasterCustomerLocationRequest>> FindByLastNemoSyncAsync(DateTime dateTime, List<Guid> customerGuids);
        Task<List<LocationEmailActionView>> GetLocationsForEmailAction(LocationEmailRequest request);
        Task<List<LocationEmailActionView>> GetLocationsForEmailActionByCustomerGuid(Guid customerGuid);
        IQueryable<TblMasterCustomerLocation> FindLocationByListGuid(IEnumerable<Guid> cusLocGuids);
        IQueryable<TblMasterCustomerLocation> GetLocationByCustomer(Guid customerGuid, bool includeDisable = false);
        IEnumerable<T> ExectueQuery<T>(string queryStr, Dictionary<string, object> param);
    }
    #endregion

    public class MasterCustomerLocationRepository : Repository<OceanDbEntities, TblMasterCustomerLocation>, IMasterCustomerLocationRepository
    {
        public MasterCustomerLocationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        public IEnumerable<AdhocLocationByCustomerResult> Func_AdhocLocationByCustomer_Get(Guid? customerGuid, Guid? siteGuid, Guid? dayOfWeek_Guid, Guid? jobType_Guid, Guid? lineOfBusiness_Guid, DateTime? workDate, bool flagDestination, Guid? customerLocaitonPK, Guid? siteGuid_Del, Guid? subServiceType_Guid)
        {
            return DbContext.Up_OceanOnlineMVC_Adhoc_LocationByCustomer_Get(customerGuid, siteGuid, dayOfWeek_Guid, jobType_Guid, lineOfBusiness_Guid, subServiceType_Guid, workDate, flagDestination, customerLocaitonPK, siteGuid_Del);
        }

        public Guid GetLocationByCodeReference(string codeReference, Guid country)
        {
            var data = (from location in DbContext.TblMasterCustomerLocation
                        join customer in DbContext.TblMasterCustomer on location.MasterCustomer_Guid equals customer.Guid
                        where location.BranchCodeReference == codeReference
                        select location).ToList();

            return data.FirstOrDefault().Guid;
        }

        public Guid GetCompanyGuidBySite(Guid siteGuid)
        {
            return DbContext.TblMasterCustomerLocation.Where(e => e.MasterSite_Guid == siteGuid)
                        .Join(DbContext.TblMasterCustomer.Where(e => e.FlagChkCustomer == false),
                            CL => CL.MasterCustomer_Guid,
                            C => C.Guid,
                            (CL, C) => new { CL }).Select(o => o.CL.Guid).FirstOrDefault();
        }

        public IEnumerable<LocationView> FindLocationsInCustomer(Guid masterCustomer_Guid)
        {
            string sqlCmd = $@"SELECT DISTINCT 
TblMasterCustomerLocation.Guid as Guid,
TblMasterCustomerLocation.MasterCustomer_Guid,
TblMasterCustomer.CustomerFullName,
TblMasterCustomerLocation.MasterCity_Guid,
TblMasterCustomerLocation.MasterDistrict_Guid,
CASE WHEN TblMasterCountry.FlagHaveState = 1 
    THEN TblMasterCountry_State.MasterStateName 
    ElSE  CASE WHEN TblMasterCountry.FlagInputCityManual = 1 THEN  TblMasterCustomerLocation.StateName ELSE TblMasterCity.MasterCityName END END AS StateName,
CASE WHEN TblMasterCountry.FlagHaveState = 1 AND TblMasterCountry.FlagInputCityManual = 0 
    THEN  TblMasterDistrict.MasterDistrictName 
    ElSE  CASE WHEN TblMasterCountry.FlagInputCityManual = 1 THEN  TblMasterCustomerLocation.CitryName ELSE TblMasterDistrict.MasterDistrictName END END AS CityName,		
TblMasterCustomerLocation.MasterServiceHour_Guid,
TblMasterCustomerLocation.BranchCodeReference as LocationCode,
TblMasterCustomerLocation.BranchName as LocationName,
TblMasterCustomerLocation.Address,
TblMasterCustomerLocation.Postcode,
TblMasterCustomerLocation.Phone,
TblMasterCustomerLocation.Fax,
TblMasterCustomerLocation.Email,
TblMasterCustomerLocation.Remark,
TblMasterCustomerLocation.FlagDisable,
TblMasterCustomerLocation.UserCreated,
TblMasterCustomerLocation.DatetimeCreated,
TblMasterCustomerLocation.UniversalDatetimeCreated,
TblMasterCustomerLocation.UserModifed as UserModified,
TblMasterCustomerLocation.DatetimeModified,
TblMasterCustomerLocation.UniversalDatetimeModified
FROM TblMasterCustomerLocation		
INNER JOIN TblMasterCustomer ON TblMasterCustomerLocation.MasterCustomer_Guid = TblMasterCustomer.Guid
INNER JOIN TblMasterCountry ON TblMasterCustomer.MasterCountry_Guid = TblMasterCountry.Guid
LEFT JOIN TblMasterCountry_State ON TblMasterCustomerLocation.MasterCountry_State_Guid = TblMasterCountry_State.Guid
LEFT JOIN TblMasterCity ON TblMasterCustomerLocation.MasterCity_Guid = TblMasterCity.Guid
LEFT JOIN TblMasterDistrict ON TblMasterCustomerLocation.MasterDistrict_Guid = TblMasterDistrict.Guid
WHERE	
TblMasterCustomerLocation.MasterCustomer_Guid = '{masterCustomer_Guid}' AND TblMasterCustomerLocation.FlagDisable = 0";

            using (IDbConnection db = new SqlConnection(DbContext.Database.Connection.ConnectionString))
            {
                var data = db.Query<LocationView>(sqlCmd, null);
                return data;
            }
        }

        public async Task<List<SyncMasterCustomerLocationRequest>> FindByLastNemoSyncAsync(DateTime dateTime, List<Guid> customerGuids)
        {
            using (OceanDbEntities context = NewDbContext())
            {
                return await context.TblMasterCustomer.Where(o => o.FlagDisable.HasValue && !o.FlagDisable.Value && (!o.LastNemoSync.HasValue || o.LastNemoSync < dateTime))
                            .Join(customerGuids, customer => customer.Guid, customerGuid => customerGuid, (customer, customerGuid) => customer)
                            .Join(context.TblMasterCustomerLocation.Where(o => !o.FlagDisable), customer => customer.Guid, location => location.MasterCustomer_Guid, (customer, location) => new { Customer = customer, Location = location })
                            .Join(context.TblMasterCustomerLocation_BrinksSite.Where(o => o.FlagDefaultBrinksSite), location => location.Location.Guid, defaultSite => defaultSite.MasterCustomerLocation_Guid, (location, defaultSite) => new { location.Customer, location.Location, DefaultSite = defaultSite })
                            .Join(context.TblMasterSite.Where(o => !o.FlagDisable), joinData => joinData.DefaultSite.MasterSite_Guid, site => site.Guid, (joinData, site) => new { joinData.Customer, joinData.Location, Site = site })
                            .Join(context.TblMasterCountry.Where(o => !o.FlagDisable), joinData => joinData.Site.MasterCountry_Guid, country => country.Guid, (joinData, country) => new { joinData.Customer, joinData.Location, joinData.Site, Country = country })
                            .GroupJoin(context.TblMasterDistrict.Where(o => !o.FlagDisable), joinData => joinData.Location.MasterDistrict_Guid, district => district.Guid, (joinData, district) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, District = district })
                            .SelectMany(o => o.District.DefaultIfEmpty(), (joinData, district) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, District = district })
                            .GroupJoin(context.TblMasterCountry_State.Where(o => !o.FlagDisable), joinData => joinData.Location.MasterCountry_State_Guid, state => state.Guid, (joinData, state) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, joinData.District, State = state })
                            .SelectMany(o => o.State.DefaultIfEmpty(), (joinData, state) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, joinData.District, State = state })
                            .GroupJoin(context.TblMasterCustomerLocation_ServiceHour, joinData => joinData.Location.Guid, serviceHour => serviceHour.MasterCustomerLocation_Guid, (joinData, serviceHour) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, joinData.District, joinData.State, ServiceHour = serviceHour })
                            .SelectMany(o => o.ServiceHour.DefaultIfEmpty(), (joinData, serviceHour) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, joinData.District, joinData.State, ServiceHour = serviceHour })
                            .GroupJoin(context.TblSystemServiceJobType.Where(o => o.FlagDisable.HasValue && !o.FlagDisable.Value), joinData => joinData.ServiceHour.SystemServiceJobType_Guid, jobType => jobType.Guid, (joinData, jobType) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, joinData.District, joinData.State, joinData.ServiceHour, JobType = jobType })
                            .SelectMany(o => o.JobType.DefaultIfEmpty(), (joinData, jobType) => new { joinData.Customer, joinData.Location, joinData.Site, joinData.Country, joinData.District, joinData.State, joinData.ServiceHour, JobType = jobType })
                            .GroupBy(o => new
                            {
                                LocationGuid = o.Location.Guid,
                                CountryCode = o.Country.MasterCountryAbbreviation,
                                MasterSiteGuid = o.Site.Guid,
                                BranchCode = o.Site.SiteCode,
                                CustomerGuid = o.Customer.Guid,
                                CustomerCode = o.Customer.CustomerCodeExternalReference,
                                Code = o.Location.BranchCodeReference,
                                Name = o.Location.BranchName,
                                o.Location.Latitude,
                                o.Location.Longitude,
                                o.Location.Address,
                                City = o.District == null ? "N/A" : o.District.MasterDistrictName,
                                State = o.State == null ? "N/A" : o.State.MasterStateName,
                                o.Location.Postcode,
                                WaitTime = o.Location.WaitingMinute,
                                ServiceTime = o.Location.ServiceDuration,
                                ServiceTypeCode = o.JobType.ServiceJobTypeNameAbb
                            }, o => o.ServiceHour, (joinData, serviceHour) => new
                            {
                                joinData.LocationGuid,
                                joinData.CountryCode,
                                joinData.MasterSiteGuid,
                                joinData.BranchCode,
                                joinData.CustomerGuid,
                                joinData.CustomerCode,
                                joinData.Code,
                                joinData.Name,
                                joinData.Latitude,
                                joinData.Longitude,
                                joinData.Address,
                                joinData.City,
                                joinData.State,
                                joinData.Postcode,
                                joinData.WaitTime,
                                joinData.ServiceTime,
                                joinData.ServiceTypeCode,
                                ServiceHour = serviceHour.ToList()
                            })
                            .Select(o => new SyncMasterCustomerLocationRequest()
                            {
                                MasterCustomerLocationGuid_Sync = o.LocationGuid,
                                CountryCode = o.CountryCode,
                                MasterSiteGuid = o.MasterSiteGuid,
                                BranchCode = o.BranchCode,
                                CustomerGuid = o.CustomerGuid,
                                CustomerCode = o.CustomerCode,
                                Code = o.Code,
                                Name = o.Name,
                                Latitude = o.Latitude,
                                Longitude = o.Longitude,
                                Address = (string.IsNullOrEmpty(o.Address) ? "N/A" : o.Address),
                                Address2 = "N/A",
                                City = o.City,
                                State = o.State,
                                Postcode = (string.IsNullOrEmpty(o.Postcode) ? "N/A" : o.Postcode),
                                ServiceTypeCode = o.ServiceTypeCode,
                                ServiceStartDate = o.ServiceHour.Any(x => x.ServiceHourStart.HasValue) ? o.ServiceHour.Where(x => x.ServiceHourStart.HasValue).Min(x => x.ServiceHourStart.Value) : new DateTime(1901, 1, 1, 0, 0, 0),
                                ServiceEndDate = o.ServiceHour.Any(x => x.ServiceHourStop.HasValue) ? o.ServiceHour.Where(x => x.ServiceHourStop.HasValue).Max(x => x.ServiceHourStop.Value) : new DateTime(1901, 1, 1, 23, 59, 59),
                                WaitTime = o.WaitTime.HasValue ? o.WaitTime.ToString() : "10",
                                ServiceTime = o.ServiceTime.HasValue ? o.ServiceTime.Value : 10,
                            }).ToListAsync();
            }
        }


        public async Task<List<LocationEmailActionView>> GetLocationsForEmailAction(LocationEmailRequest request)
        {
            using (OceanDbEntities context = NewDbContext())
            {
                return await context.TblMasterCustomerLocation_BrinksSite.Where(o => o.FlagDefaultBrinksSite && o.MasterSite_Guid == request.SiteGuid)
                                .Join(context.TblMasterCustomerLocation.Where(o => !o.FlagDisable), locSite => locSite.MasterCustomerLocation_Guid, loc => loc.Guid, (locSite, loc) => loc)
                                .Join(request.CustomerGuid, loc => loc.MasterCustomer_Guid, cus => cus, (loc, cus) => loc)
                                .Join(context.TblMasterCustomer, loc => loc.MasterCustomer_Guid, cus => cus.Guid, (loc, cus) => new { Customer = cus, Location = loc })
                                .Join(context.TblMasterCustomerLocation_EmailAction.Where(o => o.SystemEmailAction_Guid == request.EmailActionGuid), joinData => joinData.Location.Guid, email => email.MasterCustomerLocation_Guid, (joinData, email)
                                    => new LocationEmailActionView
                                    {
                                        Guid = email.Guid,
                                        CustomerGuid = joinData.Customer.Guid,
                                        CustomerName = joinData.Customer.CustomerFullName,
                                        LocationGuid = joinData.Location.Guid,
                                        LocationName = joinData.Location.BranchName,
                                        EmailActionGuid = email.SystemEmailAction_Guid,
                                        Email = email.Email
                                    }).Distinct().ToListAsync();
            }
        }

        public async Task<List<LocationEmailActionView>> GetLocationsForEmailActionByCustomerGuid(Guid customerGuid)
        {
            using (OceanDbEntities context = NewDbContext())
            {
                return await (from loc_email in DbContext.TblMasterCustomerLocation_EmailAction
                              join eAction in DbContext.TblSystemEmailAction on loc_email.SystemEmailAction_Guid equals eAction.Guid
                              //Left join
                              join loc in DbContext.TblMasterCustomerLocation on loc_email.MasterCustomerLocation_Guid equals loc.Guid into tmpLoc
                              from leftLoc in tmpLoc.DefaultIfEmpty()
                                  //Left join
                              join temp in DbContext.TblMasterUserEmailTemplate on loc_email.MasterUserEmailTemplate_Guid equals temp.Guid into tmpTemp
                              from leftTemp in tmpTemp.DefaultIfEmpty()
                              where (loc_email.MasterCustomer_Guid == customerGuid || (loc_email.MasterCustomer_Guid == customerGuid && loc_email.MasterCustomerLocation_Guid == null))
                              select new LocationEmailActionView
                              {
                                  Guid = loc_email.Guid,
                                  CustomerGuid = loc_email.MasterCustomer_Guid,
                                  LocationGuid = leftLoc.Guid,
                                  LocationName = loc_email.MasterCustomerLocation_Guid == null ? "All" : leftLoc.BranchName,
                                  EmailActionGuid = loc_email.SystemEmailAction_Guid,
                                  EmailActionName = eAction.ActionName,
                                  Email = loc_email.Email,
                                  EmailTemplateGuid = leftTemp.Guid,
                                  EmailTemplateName = leftTemp.EmailTemplateName
                              }).ToListAsync();

            }
        }
        public IQueryable<TblMasterCustomerLocation> FindLocationByListGuid(IEnumerable<Guid> cusLocGuids)
        {
            return DbContext.TblMasterCustomerLocation.Where(w => cusLocGuids.Any(a => a == w.Guid));
        }

        public IQueryable<TblMasterCustomerLocation> GetLocationByCustomer(Guid customerGuid,bool includeDisable = false)
        {
            if (includeDisable)
            {
                return DbContext.TblMasterCustomerLocation.Where(w => w.MasterCustomer_Guid == customerGuid && w.FlagDisable == includeDisable);
            }
            else
            {
                return DbContext.TblMasterCustomerLocation.Where(w => w.MasterCustomer_Guid == customerGuid);
            }
        
        }

        public IEnumerable<T> ExectueQuery<T>(string queryStr, Dictionary<string, object> param)
        {
            DynamicParameters dyParam;

            if (param.Any())
            {
                dyParam = new DynamicParameters();
                foreach (var item in param)
                {
                    dyParam.Add(item.Key, item.Value);
                }
                var result = DbContextConnection.Query<T>(queryStr, dyParam);
                return result;
            }

            return DbContextConnection.Query<T>(queryStr).ToList();
        }
    }
}
