using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Configuration;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    #region Interface

    public interface IMachineServiceTypeRepository : IRepository<object>
    {
        List<MachineServiceTypeView> GetMachineServiceTypeList(MachineServiceTypeView_Request request);
    }

    #endregion

    public class MachineServiceTypeRepository : Repository<OceanDbEntities, object>, IMachineServiceTypeRepository
    {
        public MachineServiceTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public List<MachineServiceTypeView> GetMachineServiceTypeList(MachineServiceTypeView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<MachineServiceTypeView>
                (
                    "Up_OceanOnlineMVC_API_GetMachineServiceTypeList",
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
    }
}
