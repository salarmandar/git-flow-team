using Bgt.Ocean.Models.StandardTable;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using Bgt.Ocean.Service.Messagings.StandardTable.Problem;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{

    #region interface

    public interface IProblemService
    {
        ResponseQueryProblem GetProblemList(RequestQueryProblem request);
    }

    #endregion

    public class ProblemService : IProblemService
    {
        private readonly IProblemRepository _problemRepository;

        public ProblemService(IProblemRepository problemRepository)
        {
            _problemRepository = problemRepository;
        }
        
        public ResponseQueryProblem GetProblemList(RequestQueryProblem request)
        {
            var result = new ResponseQueryProblem();
            var requestRepo = new ProblemView_Request
            {
                countryAbb = request.countryAbb,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _problemRepository.GetProblemList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<ProblemView>, IEnumerable<ResponseQueryProblem_Main>>(resultRepo).ToList();

            // Sub problem
            foreach (var item in resultList)
            {
                item.subProblem.AddRange(GetSubProblemByProblemGuid(item.guid));
            }

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }

        private List<ResponseQueryProblem_SubProblem> GetSubProblemByProblemGuid(string problemGuid)
        {
            var result = new List<ResponseQueryProblem_SubProblem>();
            var resultRepo = _problemRepository.GetSubProblemByProblemGuid(problemGuid);

            result = MapperService.Map<IEnumerable<ProblemView_SubProblem>, IEnumerable<ResponseQueryProblem_SubProblem>>(resultRepo).ToList();

            return result;
        }
    }
}
