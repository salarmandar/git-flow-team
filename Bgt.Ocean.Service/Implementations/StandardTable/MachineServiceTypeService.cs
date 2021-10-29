using Bgt.Ocean.Models.StandardTable;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using Bgt.Ocean.Service.Messagings.StandardTable.MachineServiceType;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{

    #region interface

    public interface IMachineServiceTypeService
    {
        ResponseQueryMachineServiceType GetMachineServiceTypeList(RequestQueryMachineServiceType request);
    }

    #endregion

    public class MachineServiceTypeService : IMachineServiceTypeService
    {
        private readonly IMachineServiceTypeRepository _machineServiceTypeRepository;

        public MachineServiceTypeService(IMachineServiceTypeRepository machineServiceTypeRepository)
        {
            _machineServiceTypeRepository = machineServiceTypeRepository;
        }
        
        public ResponseQueryMachineServiceType GetMachineServiceTypeList(RequestQueryMachineServiceType request)
        {
            var result = new ResponseQueryMachineServiceType();
            var requestRepo = new MachineServiceTypeView_Request
            {
                countryAbb = request.countryAbb,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _machineServiceTypeRepository.GetMachineServiceTypeList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<MachineServiceTypeView>, IEnumerable<ResponseQueryMachineServiceType_Main>>(resultRepo).ToList();

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }
    }
}
