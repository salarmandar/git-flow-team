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

    public interface IProblemPriorityRepository : IRepository<object>
    {
        List<ProblemPriorityView> GetProblemPriorityList(ProblemPriorityView_Request request);
    }

    #endregion

    public class ProblemPriorityRepository : Repository<OceanDbEntities, object>, IProblemPriorityRepository
    {
        public ProblemPriorityRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public List<ProblemPriorityView> GetProblemPriorityList(ProblemPriorityView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<ProblemPriorityView>
                (
                    "Up_OceanOnlineMVC_API_GetProblemPriorityList",
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
