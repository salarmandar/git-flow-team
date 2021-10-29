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

    public interface IMachineSubServiceTypeRepository : IRepository<object>
    {
        List<MachineSubServiceTypeView> GetMachineSubServiceTypeList(MachineSubServiceTypeView_Request request);
    }

    #endregion

    public class MachineSubServiceTypeRepository : Repository<OceanDbEntities, object>, IMachineSubServiceTypeRepository
    {
        public MachineSubServiceTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public List<MachineSubServiceTypeView> GetMachineSubServiceTypeList(MachineSubServiceTypeView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<MachineSubServiceTypeView>
                (
                    "Up_OceanOnlineMVC_API_GetMachineSubServiceTypeList",
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
