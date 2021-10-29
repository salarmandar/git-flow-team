using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterCountryRepository : IRepository<TblMasterCountry>
    {
        IEnumerable<CountryByUserResult> Func_System_CountryHandleByUser(string userName);
        IEnumerable<BrinksCompanyResult> FindCountryFromBrinksHandle(Guid masterUser_Guid, int userRoleId, Guid? language_Guid);
    }

    public class MasterCountryRepository : Repository<OceanDbEntities, TblMasterCountry>, IMasterCountryRepository
    {
        public MasterCountryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<CountryByUserResult> Func_System_CountryHandleByUser(string userName)
        {
            return DbContext.Up_OceanOnlineMVC_StandardTable_CountryByUser_Get(userName);
        }

        public IEnumerable<BrinksCompanyResult> FindCountryFromBrinksHandle(Guid masterUser_Guid, int userRoleId, Guid? language_Guid)
        {
            return DbContext.Up_OceanOnlineMVC_BrinksCompany_Get(masterUser_Guid, null, null, language_Guid, userRoleId)
                .Distinct((x, y) => x.MasterCountry_Guid == y.MasterCountry_Guid);
        }
      
    }
}
