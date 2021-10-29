using Bgt.Ocean.Models.StandardTable;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using Bgt.Ocean.Service.Messagings.StandardTable.RunResourceType;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{

    #region interface

    public interface IRunResourceTypeService
    {
        ResponseQueryRunResourceType GetRunResourceTypeList(RequestQueryRunResourceType request);
    }

    #endregion

    public class RunResourceTypeService : IRunResourceTypeService
    {
        private readonly IRunResourceTypeRepository _runResourceTypeRepository;

        public RunResourceTypeService(IRunResourceTypeRepository runResourceTypeRepository)
        {
            _runResourceTypeRepository = runResourceTypeRepository;
        }
        
        public ResponseQueryRunResourceType GetRunResourceTypeList(RequestQueryRunResourceType request)
        {
            var result = new ResponseQueryRunResourceType();
            var requestRepo = new RunResourceTypeView_Request
            {
                countryAbb = request.countryAbb,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _runResourceTypeRepository.GetRunResourceTypeList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<RunResourceTypeView>, IEnumerable<ResponseQueryRunResourceType_Main>>(resultRepo).ToList();

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }
    }
}
