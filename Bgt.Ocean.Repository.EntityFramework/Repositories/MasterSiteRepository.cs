using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Nemo.NemoSync;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static Bgt.Ocean.Models.MasterRoute.MassUpdateView;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    #region Interface

    public interface IMasterSiteRepository : IRepository<TblMasterSite>
    {
        BrinksNameView GetBrinksCompanyAndSiteName(Guid siteGuid);
        IQueryable<TblMasterCustomerLocation_BrinksSite> getCustomerLocation_BrinksSite();
        IEnumerable<BrinksSiteByUserResult> Func_BrinksSite_Get(Guid? userGuid, int? roletypeID, Guid? companyGuid, bool flagDestinationLocation, bool flagDestinationInternal, Guid? cusLocationPKGuid, bool flagHubBrinksite = false, Guid? siteGuid = null, bool isChangedHubBrinksite = false);
        Guid GetCountryGuidByMasterSiteGuid(Guid siteGuid);

        Task<List<SyncMasterSiteRequest>> FindByLastNemoSyncAsync(DateTime dateTime);
    }

    #endregion

    public class MasterSiteRepository : Repository<OceanDbEntities, TblMasterSite>, IMasterSiteRepository
    {
        public MasterSiteRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }


        public BrinksNameView GetBrinksCompanyAndSiteName(Guid siteGuid)
        {
            BrinksNameView data = new BrinksNameView();
            var site = DbContext.TblMasterSite.FirstOrDefault(w => w.Guid == siteGuid);
            data.BrinksSiteName = site.SiteCode + " - " + site.SiteName;
            data.BrinksCompanyName = DbContext.TblMasterCustomer.FirstOrDefault(e => site.MasterCustomer_Guid == e.Guid).CustomerFullName;
            return data;
        }

        public IQueryable<TblMasterCustomerLocation_BrinksSite> getCustomerLocation_BrinksSite()
        {
            return DbContext.TblMasterCustomerLocation_BrinksSite;
        }

        public IEnumerable<BrinksSiteByUserResult> Func_BrinksSite_Get(Guid? userGuid, int? roletypeID, Guid? companyGuid, bool flagDestinationLocation, bool flagDestinationInternal, Guid? cusLocationPKGuid, bool flagHubBrinksite = false, Guid? siteGuid = default(Guid?), bool isChangedHubBrinksite = false)
        {
            return DbContext.Up_OceanOnlineMVC_BrinksSiteByUser_Get(userGuid, roletypeID, companyGuid, flagDestinationLocation, flagDestinationInternal, cusLocationPKGuid, flagHubBrinksite, siteGuid, isChangedHubBrinksite);
        }
        public Guid GetCountryGuidByMasterSiteGuid(Guid siteGuid)
        {
            return DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == siteGuid).MasterCountry_Guid;
        }

        public async Task<List<SyncMasterSiteRequest>> FindByLastNemoSyncAsync(DateTime dateTime)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                return await context.TblMasterSite.Where(o => !o.FlagDisable && (!o.LastNemoSync.HasValue || o.LastNemoSync < dateTime))
                            .Join(context.TblMasterCountry, site => site.MasterCountry_Guid, country => country.Guid, (site, country) => new SyncMasterSiteRequest()
                            {
                                CountryCode = country.MasterCountryAbbreviation,
                                BranchCode = site.SiteCode,
                                BranchName = site.SiteName,
                                MasterSiteSync_Guid = site.Guid,
                                TimeZoneID = (site.TimeZoneID.HasValue ? site.TimeZoneID.ToString() : string.Empty)
                            })
                            .ToListAsync();
            }
        }
    }
}
