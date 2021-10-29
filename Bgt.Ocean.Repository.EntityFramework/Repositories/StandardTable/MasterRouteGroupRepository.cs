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

    public interface IMasterRouteGroupRepository : IRepository<TblMasterRouteGroup>
    {
        List<RouteGroupView> GetRouteGroupList(RouteGroupView_Request request);
        IEnumerable<TblMasterRouteGroup> FindByCompanyGuid(Guid brinksSite);
    }

    #endregion

    public class MasterRouteGroupRepository : Repository<OceanDbEntities, TblMasterRouteGroup>, IMasterRouteGroupRepository
    {
        public MasterRouteGroupRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public List<RouteGroupView> GetRouteGroupList(RouteGroupView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<RouteGroupView>
                (
                    "Up_OceanOnlineMVC_API_GetRouteGroupList",
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

        public IEnumerable<TblMasterRouteGroup> FindByCompanyGuid(Guid compGuid)
        {
            return DbContext.TblMasterRouteGroup.Where(e => e.MasterCustomer_Guid == compGuid && !e.FlagDisable);
        }

    }
}
