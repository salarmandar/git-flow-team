using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using static Bgt.Ocean.Infrastructure.Helpers.SystemHelper;
using System.Collections.Generic;
using Bgt.Ocean.Models.Systems;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemEnvironmentMasterCountryRepository : IRepository<TblSystemEnvironmentMasterCountry>
    {
        bool IsUseDolphin(Guid? siteGuid, Guid dailyRunGuid);
        bool IsCloseRunWithOutCheckIn(Guid siteGuid);
        bool FindAppkeyValueByEnumAppkeyName(Guid? Site_Guid, string EnumAppkeyName);
        CountryOptionResult FindCountryOptionByEnumAppkeyName(Guid? Site_Guid, string EnumAppkeyName);
        TblSystemEnvironmentMasterCountryValue GetValueByAppKey(string appKey, Guid countryGuid, Guid siteGuid);
        IEnumerable<TblSystemEnvironmentMasterCountryValue> GetValueByAppKeyAndCountry(string appKey, IEnumerable<Guid> countryGuid);
        IEnumerable<TblSystemEnvironmentMasterCountryValue> GetValueByAppKeyAllCountry(string appKey);
        IEnumerable<CountryOptionView> FindListCountryOptionByAppKey(Guid? siteGuid, Guid? countryGuid, List<string> appkey);
    }
    public class SystemEnvironmentMasterCountryRepository : Repository<OceanDbEntities, TblSystemEnvironmentMasterCountry>, ISystemEnvironmentMasterCountryRepository
    {
        public SystemEnvironmentMasterCountryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public bool IsUseDolphin(Guid? siteGuid, Guid dailyRunGuid)
        {
            var roleIdUsedMobile = DbContext.Up_OceanOnlineMVC_CountryOption_Get("RoleIDUsedMobile", siteGuid, null).FirstOrDefault().AppValue1;
            var splitRoleIds = roleIdUsedMobile.Split(',').ConvertToInts();
            var dailyEmployeeByRoles = DbContext.TblMasterDailyEmployee.Where(
                e => e.MasterDailyRunResource_Guid == dailyRunGuid && splitRoleIds.Contains(e.RoleInRunResourceID));

            var hasMobilePIN = dailyEmployeeByRoles.Any(
                e => DbContext.TblMasterEmployee.FirstOrDefault(o => o.Guid == e.MasterEmployee_Guid).MobileLoginPIN.Length > 0);

            return hasMobilePIN;
        }

        public bool IsCloseRunWithOutCheckIn(Guid siteGuid)
        {
            bool flgWithOutCheckIn = false;
            var configWithOutChkIn = DbContext.TblSystemEnvironmentMasterSiteValue.Where(x => x.MasterSite_Guid == siteGuid)
                        .Join(DbContext.TblSystemEnvironmentMasterCountry.Where(x => x.AppKey == "FlagAllowDolphinCloseRunWithoutCheckIn")
                        , envs => envs.SystemEnvironmentMasterCountry_Guid
                        , envc => envc.Guid
                        , (envs, envc) => envs.AppValue1).FirstOrDefault();
            return Boolean.TryParse(configWithOutChkIn, out flgWithOutCheckIn);
        }

        public bool FindAppkeyValueByEnumAppkeyName(Guid? Site_Guid, string EnumAppkeyName)
        {

            var MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == Site_Guid)?.MasterCountry_Guid;
            return DbContext.Up_OceanOnlineMVC_CountryOption_Get(EnumAppkeyName, Site_Guid, MasterCountry_Guid)
                                            .FirstOrDefault().AppValue1.ToLower() == "true";
        }

        public CountryOptionResult FindCountryOptionByEnumAppkeyName(Guid? Site_Guid, string EnumAppkeyName)
        {

            var MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == Site_Guid)?.MasterCountry_Guid;
            return DbContext.Up_OceanOnlineMVC_CountryOption_Get(EnumAppkeyName, Site_Guid, MasterCountry_Guid)
                                            .FirstOrDefault();
        }

        public IEnumerable<CountryOptionView> FindListCountryOptionByAppKey(Guid? siteGuid, Guid? countryGuid,List<string> appkey)
        {
            if (siteGuid.HasValue)
            {
                // Check on site level first
                var siteItems = DbContext.TblSystemEnvironmentMasterCountry.Where(o => appkey.Contains(o.AppKey))
                                   .Join(DbContext.TblSystemEnvironmentMasterSiteValue.Where(o => o.MasterSite_Guid == siteGuid.Value),
                                        env => env.Guid,
                                        value => value.SystemEnvironmentMasterCountry_Guid,
                                        (env, value) => new { env.AppKey,value})
                                        .AsEnumerable()
                                        .Select(obj=> new CountryOptionView
                                        {
                                            Guid = obj.value.Guid,
                                            SystemEnvironmentMasterCountry_Guid = obj.value.SystemEnvironmentMasterCountry_Guid,
                                            MasterSite_Guid = siteGuid.HasValue ? siteGuid.Value.ToString() : "",
                                            MasterCountry_Guid = "",
                                            Appkey = obj.AppKey,
                                            AppValue1 = obj.value.AppValue1,
                                            AppValue2 = obj.value.AppValue2,
                                            AppValue3 = obj.value.AppValue3,
                                            AppValue4 = obj.value.AppValue4,
                                            AppValue5 = obj.value.AppValue5,
                                            AppValue6 = obj.value.AppValue6,
                                            AppValue7 = obj.value.AppValue7,
                                            AppValue8 = obj.value.AppValue8,
                                            AppValue9 = obj.value.AppValue9,
                                            FlagDataDefault = false
                                        });
                                     

                if (siteItems.Any())
                {
                  return siteItems;
                }
                else {
                    var MasterCountry_Guid = DbContext.TblMasterSite.FirstOrDefault(o => o.Guid == siteGuid.Value)?.MasterCountry_Guid;
                    return DbContext.TblSystemEnvironmentMasterCountry.Where(o => appkey.Contains(o.AppKey))
                            .Join(DbContext.TblSystemEnvironmentMasterCountryValue.Where(o => o.MasterCountry_Guid == MasterCountry_Guid),
                            env => env.Guid, 
                            value => value.SystemEnvironmentMasterCountry_Guid,
                            (env, value) => new { env.AppKey, value })
                            .AsEnumerable()
                            .Select(obj => new CountryOptionView {
                                Guid = obj.value.Guid,
                                SystemEnvironmentMasterCountry_Guid = obj.value.SystemEnvironmentMasterCountry_Guid,
                                MasterSite_Guid = siteGuid.HasValue ? siteGuid.Value.ToString() : "",
                                MasterCountry_Guid = obj.value.MasterCountry_Guid.ToString(),
                                Appkey = obj.AppKey,
                                AppValue1 = obj.value.AppValue1,
                                AppValue2 = obj.value.AppValue2,
                                AppValue3 = obj.value.AppValue3,
                                AppValue4 = obj.value.AppValue4,
                                AppValue5 = obj.value.AppValue5,
                                AppValue6 = obj.value.AppValue6,
                                AppValue7 = obj.value.AppValue7,
                                AppValue8 = obj.value.AppValue8,
                                AppValue9 = obj.value.AppValue9,
                                FlagDataDefault = obj.value.FlagDataDefault
                            });
                }
            }
            else {
                return DbContext.TblSystemEnvironmentMasterCountry.Where(o => appkey.Contains(o.AppKey))
                        .Join(DbContext.TblSystemEnvironmentMasterCountryValue,
                        env=>env.Guid,
                        value => value.SystemEnvironmentMasterCountry_Guid, 
                        (env,value) => new { env.AppKey, value })
                        .AsEnumerable()
                        .Select(obj => new CountryOptionView { 
                             Guid = obj.value.Guid,
                             SystemEnvironmentMasterCountry_Guid = obj.value.SystemEnvironmentMasterCountry_Guid,
                             MasterSite_Guid = siteGuid.HasValue ? siteGuid.Value.ToString(): "",
                             MasterCountry_Guid = obj.value.MasterCountry_Guid.ToString(),
                             Appkey = obj.AppKey,
                             AppValue1 = obj.value.AppValue1,
                             AppValue2 = obj.value.AppValue2,
                             AppValue3 = obj.value.AppValue3,
                             AppValue4 = obj.value.AppValue4,
                             AppValue5 = obj.value.AppValue5,
                             AppValue6 = obj.value.AppValue6,
                             AppValue7 = obj.value.AppValue7,
                             AppValue8 = obj.value.AppValue8,
                             AppValue9 = obj.value.AppValue9,
                             FlagDataDefault = obj.value.FlagDataDefault
                        }).Where(o=>countryGuid.HasValue && o.MasterCountry_Guid.Equals((Guid)countryGuid));
            }
        }

        public TblSystemEnvironmentMasterCountryValue GetValueByAppKey(string appKey, Guid countryGuid, Guid siteGuid)
        {
            return DbContext.TblSystemEnvironmentMasterCountry.Where(o => o.AppKey == appKey)
                    .Join(DbContext.TblSystemEnvironmentMasterCountryValue, env => env.Guid, value => value.SystemEnvironmentMasterCountry_Guid, (env, value) => value).FirstOrDefault();
        }

        public IEnumerable<TblSystemEnvironmentMasterCountryValue> GetValueByAppKeyAndCountry(string appKey, IEnumerable<Guid> countryGuid)
        {
            return DbContext.TblSystemEnvironmentMasterCountry.Where(o => o.AppKey == appKey)
                    .Join(DbContext.TblSystemEnvironmentMasterCountryValue.Where(w => countryGuid.Any(a => a == w.MasterCountry_Guid))
                    , env => env.Guid, value => value.SystemEnvironmentMasterCountry_Guid, (env, value) => value);

        }

        public IEnumerable<TblSystemEnvironmentMasterCountryValue> GetValueByAppKeyAllCountry(string appKey)
        {
            return DbContext.TblSystemEnvironmentMasterCountry.Where(o => o.AppKey == appKey)
                    .Join(DbContext.TblSystemEnvironmentMasterCountryValue
                    , env => env.Guid, value => value.SystemEnvironmentMasterCountry_Guid, (env, value) => value);
        }
    }
}
