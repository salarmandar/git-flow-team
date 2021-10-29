using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.ModelViews.Machine;
using Bgt.Ocean.Services.Messagings.StandardTable.Machine;
using System;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumOTC;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{

    #region interface

    public interface IMachineService
    {
        MachineModel GetMachineByGuid(Guid machineGuid);
        MachineModel GetMachineByMachineID(string machineId, Guid masterCoutryGuid);
        ResponseQueryMachine GetMachineList(RequestQueryMachine request);

        bool IsMachineHasCryptoLock(params Guid[] machineGuids);
    }

    #endregion

    public class MachineService : IMachineService
    {
        private readonly ISFOMasterMachineRepository _machineRepository;
        private readonly ISFOMasterMachineLockTypeRepository _machineLockTypeRepository;
        private readonly ISFOSystemLockTypeRepository _systemLockTypeRepository;

        public MachineService(
                ISFOMasterMachineRepository machineRepository,
                ISFOMasterMachineLockTypeRepository machineLockTypeRepository,
                ISFOSystemLockTypeRepository systemLockTypeRepository
            )
        {
            _machineRepository = machineRepository;
            _machineLockTypeRepository = machineLockTypeRepository;
            _systemLockTypeRepository = systemLockTypeRepository;
        }

        public MachineModel GetMachineByGuid(Guid machineGuid)
        {
            try
            {
                var machine = _machineRepository.GetMachineByGuid(machineGuid);
                return machine.ConvertToMachineModel();
            }
            catch (Exception err)
            {
                throw new ArgumentException($"MachineGuid: {machineGuid} not found", err);
            }
        }

        public MachineModel GetMachineByMachineID(string machineId, Guid masterCoutryGuid)
        {
            try
            {
                var machine = _machineRepository.GetMachineByMachineID(machineId, masterCoutryGuid);
                return machine.ConvertToMachineModel();
            }
            catch (Exception err)
            {

                throw new ArgumentException($"MachineID: {machineId} not found", err);
            }
        }

        public ResponseQueryMachine GetMachineList(RequestQueryMachine request)
        {
            var result = new ResponseQueryMachine();
            var requestRepo = new MachineView_Request
            {
                countryAbb = request.countryAbb,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _machineRepository.GetMachineList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<MachineView>, IEnumerable<ResponseQueryMachine_Main>>(resultRepo).ToList();

            // Service hour
            foreach (var item in resultList)
            {
                item.serviceHourDetail.AddRange(GetServiceHourByMachineGuid(item.guid));
            }

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }

        private List<ResponseQueryMachine_ServiceHour> GetServiceHourByMachineGuid(string machineGuid)
        {
            var resultRepo = _machineRepository.GetServiceHourByMachineGuid(machineGuid);
            var result = MapperService.Map<IEnumerable<MachineView_ServiceHour>, IEnumerable<ResponseQueryMachine_ServiceHour>>(resultRepo).ToList();

            return result;
        }

        public bool IsMachineHasCryptoLock(params Guid[] machineGuids)
        {
            var lockType = _systemLockTypeRepository.FindOne(e => e.LockTypeID == LockTypeID.Crypto);
            var cryptoGuid = lockType.Guid;

            var query = (from lt in _machineLockTypeRepository.FindAllAsQueryable()
                         join m in machineGuids on lt.SFOMasterMachine_Guid equals m
                         where lt.SFOSystemLockType_Guid == cryptoGuid
                         select 1).Any();

            return query;
        }
    }
}
