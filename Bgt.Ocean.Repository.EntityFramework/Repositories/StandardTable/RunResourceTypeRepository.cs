using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Configuration;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    #region Interface

    public interface IRunResourceTypeRepository : IRepository<object>
    {
        List<RunResourceTypeView> GetRunResourceTypeList(RunResourceTypeView_Request request);

        IEnumerable<TblMasterRunResourceType> GetRunResourceTypeDdl(Guid customer_Guid);

        IEnumerable<TblMasterRunResourceBrand> GetRunResourceBrandDdl(Guid? customer_Guid);
    }

    #endregion

    public class RunResourceTypeRepository : Repository<OceanDbEntities, object>, IRunResourceTypeRepository
    {
        public RunResourceTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public List<RunResourceTypeView> GetRunResourceTypeList(RunResourceTypeView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<RunResourceTypeView>
                (
                    "Up_OceanOnlineMVC_API_GetRunResourceTypeList",
                    new
                    {
                        @MaxRow = WebConfigurationManager.AppSettings["MaxRow"],
                        @CountryAbb = request.countryAbb,
                        @CreatedFrom = request.createdDatetimeFrom,
                        @CreatedTo = request.createdDatetimeTo
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result;
        }

        public IEnumerable<TblMasterRunResourceType> GetRunResourceTypeDdl(Guid customer_Guid)
        {
            // Customer Guid = Brinks Company (Guid from TblMasterCustomer) FlagChkCustomer = 0
            return DbContext.TblMasterRunResourceType.Where(o => o.MasterCustomer_Guid == customer_Guid 
                                                                    && o.FlagDisable == false);

        }

        public IEnumerable<TblMasterRunResourceBrand> GetRunResourceBrandDdl(Guid? customer_Guid) {
            var response = DbContext.TblMasterRunResourceBrand.Where(o => o.MasterCustomer_Guid == customer_Guid 
                                                                            && o.FlagDisable == false);
            return response;
        }
    }
}
