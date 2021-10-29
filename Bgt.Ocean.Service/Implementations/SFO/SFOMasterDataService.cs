using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.Problem;
using System;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    #region Interface

    public interface ISFOMasterDataService
    {
        ProblemView GetProblemInfo(Guid problemGuid);
    }

    #endregion

    public class SFOMasterDataService : ISFOMasterDataService
    {
        private readonly ISFOMasterProblemRepository _masterProblemRepository;

        public SFOMasterDataService(
                ISFOMasterProblemRepository masterProblemRepository
            )
        {
            _masterProblemRepository = masterProblemRepository;
        }

        public ProblemView GetProblemInfo(Guid problemGuid)
        {
            try
            {
                var masterProblem = _masterProblemRepository.FindById(problemGuid);
                return masterProblem.ConvertToProblemView();
            }
            catch (Exception err)
            {
                throw new ArgumentException($"ProblemGuid: {problemGuid} not found", err);
            }

        }
    }
}
