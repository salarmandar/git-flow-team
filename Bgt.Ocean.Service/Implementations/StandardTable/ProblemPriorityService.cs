using Bgt.Ocean.Models.StandardTable;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using Bgt.Ocean.Service.Messagings.StandardTable.ProblemPriority;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{

    #region interface

    public interface IProblemPriorityService
    {
        ResponseQueryProblemPriority GetProblemPriorityList(RequestQueryProblemPriority request);
    }

    #endregion

    public class ProblemPriorityService : IProblemPriorityService
    {
        private readonly IProblemPriorityRepository _problemPriorityRepository;

        public ProblemPriorityService(IProblemPriorityRepository problemPriorityRepository)
        {
            _problemPriorityRepository = problemPriorityRepository;
        }
        
        public ResponseQueryProblemPriority GetProblemPriorityList(RequestQueryProblemPriority request)
        {
            var result = new ResponseQueryProblemPriority();
            var requestRepo = new ProblemPriorityView_Request
            {
                countryAbb = request.countryAbb,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _problemPriorityRepository.GetProblemPriorityList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<ProblemPriorityView>, IEnumerable<ResponseQueryProblemPriority_Main>>(resultRepo).ToList();

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }
    }
}
