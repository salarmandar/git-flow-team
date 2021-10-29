using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.CustomerLocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation
{
    #region Interface
    public interface IMasterCustomerLocationInternalDepartmentRepository : IRepository<TblMasterCustomerLocation_InternalDepartment>
    {
        IEnumerable<CustomerLocationInternalDepartmentModel> GetInternalDeptOnwardDestination(Guid siteGuid);
        IEnumerable<CustomerLocationInternalDepartmentModel> GetInternalDeptRoom(Guid siteGuid);
        string GetInternalName(Guid OnwardDestination_Guid);
        IEnumerable<Bgt.Ocean.Models.InternalDepartments.InternalDepartmentView> GetInternalDepartmentsInCompany(Guid company_Guid, int typeID);
    }
    #endregion
    public class MasterCustomerLocationInternalDepartmentRepository : Repository<OceanDbEntities, TblMasterCustomerLocation_InternalDepartment>, IMasterCustomerLocationInternalDepartmentRepository
    {
        public MasterCustomerLocationInternalDepartmentRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<CustomerLocationInternalDepartmentModel> GetInternalDeptOnwardDestination(Guid siteGuid)
        {
            int[] intDeptID = { 2, 3 }; //2 : Pre-vault , 3 : Room
            var response = DbContext.TblMasterCustomerLocation_InternalDepartment.Where(o => !o.FlagDisable)
                .Join(DbContext.TblMasterCustomerLocation.Where(o => o.MasterSite_Guid == siteGuid),
                    I => I.MasterCustomerLocation_Guid,
                    L => L.Guid,
                    (I, L) => new { cusLocation = L, Internal = I })
                .Join(DbContext.TblSystemInternalDepartmentType.Where(o => intDeptID.Contains(o.InternalDepartmentID.Value)),
                    I => I.Internal.InternalDepartmentType,
                    T => T.Guid,
                    (I, T) => new CustomerLocationInternalDepartmentModel
                    {
                        id = I.Internal.Guid,
                        text = I.Internal.InterDepartmentName
                    });
            return response;
        }

        public IEnumerable<CustomerLocationInternalDepartmentModel> GetInternalDeptRoom(Guid siteGuid)
        {
            var FlagISA = (from Envi in DbContext.TblSystemEnvironmentMasterCountry.Where(e => e.AppKey == Infrastructure.Util.EnumGlobalEnvironment.appkey.FlagEnableISA)
                           join EnviValue in DbContext.TblSystemEnvironmentMasterCountryValue on Envi.Guid equals EnviValue.SystemEnvironmentMasterCountry_Guid
                           into LeftEnviValue
                           from EnviValue in LeftEnviValue.DefaultIfEmpty()
                           join Site in DbContext.TblMasterSite.Where(e => e.Guid == siteGuid) on EnviValue.MasterCountry_Guid equals Site.MasterCountry_Guid
                           select EnviValue.AppValue1?? "false").FirstOrDefault();

            var response = DbContext.TblMasterCustomerLocation_InternalDepartment.Where(o => !o.FlagDisable)
                .Join(DbContext.TblMasterCustomerLocation.Where(o => o.MasterSite_Guid == siteGuid),
                    I => I.MasterCustomerLocation_Guid,
                    L => L.Guid,
                    (I, L) => new { cusLocation = L, Internal = I })
                .Join(DbContext.TblSystemInternalDepartmentType.Where(o => o.InternalDepartmentID == 3),
                    I => I.Internal.InternalDepartmentType,
                    T => T.Guid,
                    (I, T) => new CustomerLocationInternalDepartmentModel
                    {
                        id = I.Internal.Guid,
                        text = I.Internal.InterDepartmentName        ,
                        FlagISA = (FlagISA.ToUpper()=="TRUE")? I.Internal.FlagIntegratedwithISA : false
                    }).OrderBy(o => o.text);
            return response;
        }

        public string GetInternalName(Guid OnwardDestination_Guid)
        {
            var internalDep = DbContext.TblMasterCustomerLocation_InternalDepartment.FirstOrDefault(f => f.Guid == OnwardDestination_Guid);
            if (internalDep != null) return internalDep.InterDepartmentName;

            var systemOnward = DbContext.TblSystemOnwardDestinationType.FirstOrDefault(f => f.Guid == OnwardDestination_Guid);
            return systemOnward.OnwardDestinationName;
        }

        public IEnumerable<Bgt.Ocean.Models.InternalDepartments.InternalDepartmentView> GetInternalDepartmentsInCompany(Guid company_Guid, int typeID)
        {
            using (IDbConnection db = new SqlConnection(DbContext.Database.Connection.ConnectionString))
            {
                string query = $@"	
	SELECT
		TblMasterCustomerLocation.Guid AS MasterCustomerLocation_Guid, 
		TblMasterCustomerLocation_InternalDepartment.[Guid] AS Guid, 
		TblMasterCustomerLocation_InternalDepartment.InterDepartmentName as Name,
		TblSystemInternalDepartmentType.InternalDepartmentID as InternalDepartmentTypeID,
		TblSystemInternalDepartmentType.InternalDepartmentTypeName as InternalDepartmentTypeName,
        TblMasterSite.SiteName
	FROM TblMasterCustomer 
		INNER JOIN TblMasterCustomerLocation ON TblMasterCustomer.Guid = TblMasterCustomerLocation.MasterCustomer_Guid 
		INNER JOIN TblMasterCustomerLocation_InternalDepartment ON TblMasterCustomerLocation.Guid = TblMasterCustomerLocation_InternalDepartment.MasterCustomerLocation_Guid 
		INNER JOIN TblSystemInternalDepartmentType on TblMasterCustomerLocation_InternalDepartment.InternalDepartmentType = TblSystemInternalDepartmentType.Guid
	    INNER JOIN TblMasterSite ON TblMasterCustomerLocation.MasterSite_Guid = TblMasterSite.Guid 
	WHERE TblMasterCustomer.Guid = '{company_Guid}'
		AND TblMasterCustomer.FlagDisable = 0
		AND TblMasterCustomerLocation_InternalDepartment.FlagDisable = 0 
		AND TblSystemInternalDepartmentType.InternalDepartmentID = {typeID}";
                var data = db.Query<Bgt.Ocean.Models.InternalDepartments.InternalDepartmentView>(query, null);
                return data.OrderBy(e => e.Name);
            }
        }
    }
}
