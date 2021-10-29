using Bgt.Ocean.Models.StandardTable;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using Bgt.Ocean.Service.Messagings.StandardTable.MachineSubServiceType;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{

    #region interface

    public interface IMachineSubServiceTypeService
    {
        ResponseQueryMachineSubServiceType GetMachineSubServiceTypeList(RequestQueryMachineSubServiceType request);
    }

    #endregion

    public class MachineSubServiceTypeService : IMachineSubServiceTypeService
    {
        private readonly IMachineSubServiceTypeRepository _machineSubServiceTypeRepository;

        public MachineSubServiceTypeService(IMachineSubServiceTypeRepository machineSubServiceTypeRepository)
        {
            _machineSubServiceTypeRepository = machineSubServiceTypeRepository;
        }
        
        public ResponseQueryMachineSubServiceType GetMachineSubServiceTypeList(RequestQueryMachineSubServiceType request)
        {
            var result = new ResponseQueryMachineSubServiceType();
            var requestRepo = new MachineSubServiceTypeView_Request
            {
                countryAbb = request.countryAbb,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _machineSubServiceTypeRepository.GetMachineSubServiceTypeList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<MachineSubServiceTypeView>, IEnumerable<ResponseQueryMachineSubServiceType_Main>>(resultRepo).ToList();

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }
    }
}
