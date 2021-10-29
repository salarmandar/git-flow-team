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

    public interface IProblemRepository : IRepository<object>
    {
        List<ProblemView> GetProblemList(ProblemView_Request request);
        List<ProblemView_SubProblem> GetSubProblemByProblemGuid(string problemGuid);
    }

    #endregion

    public class ProblemRepository : Repository<OceanDbEntities, object>, IProblemRepository
    {
        public ProblemRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public List<ProblemView> GetProblemList(ProblemView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<ProblemView>
                (
                    "Up_OceanOnlineMVC_API_GetProblemList",
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

        public List<ProblemView_SubProblem> GetSubProblemByProblemGuid(string problemGuid)
        {
            var result = DbContext.Database.Connection
                .Query<ProblemView_SubProblem>
                (
                    "Up_OceanOnlineMVC_API_GetSubProblemByProblem",
                    new
                    {
                        @ProblemGuid = problemGuid
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result;
        }
    }
}
