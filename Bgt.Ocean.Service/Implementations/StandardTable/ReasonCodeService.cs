using Bgt.Ocean.Models.StandardTable;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using Bgt.Ocean.Service.Messagings.StandardTable.ReasonCode;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{

    #region interface

    public interface IReasonCodeService
    {
        ResponseQueryReasonCode GetReasonCodeList(RequestQueryReasonCode request);
    }

    #endregion

    public class ReasonCodeService : IReasonCodeService
    {
        private readonly IReasonCodeRepository _reasonCodeRepository;

        public ReasonCodeService(IReasonCodeRepository reasonCodeRepository)
        {
            _reasonCodeRepository = reasonCodeRepository;
        }
        
        public ResponseQueryReasonCode GetReasonCodeList(RequestQueryReasonCode request)
        {
            var result = new ResponseQueryReasonCode();
            var requestRepo = new ReasonCodeView_Request
            {
                countryAbb = request.countryAbb,
                reasonTypeCategoryId = request.reasonTypeCategoryId,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _reasonCodeRepository.GetReasonCodeList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<ReasonCodeView>, IEnumerable<ResponseQueryReasonCode_Main>>(resultRepo).ToList();

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }
    }
}
