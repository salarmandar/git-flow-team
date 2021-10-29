using Bgt.Ocean.Models.PreVault;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance;
using Bgt.Ocean.Service.Messagings.PreVault;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.PreVault
{
    #region #### INTERFACE ####
    public interface IPrevaultManagementService
    {
        IEnumerable<InternalDepartmentResponse> GetPrevaultBySite(Guid siteGuid);
    }
    #endregion

    public class PrevaultManagementService : IPrevaultManagementService
    {
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly ISystemInternalDepartmentTypesRepository _systemInternalDepartmentTypesRepository;
        private readonly IMasterCustomerLocationInternalDepartmentRepository _masterCustomerLocationInternalDepartmentRepository;
        private readonly IVaultBalanceDiscrepancyRepository _vaultBalanceDiscrepancyRepository;

        #region #### DEPENDENCY INJECTION ####
        public PrevaultManagementService(
            IMasterCustomerLocationRepository masterCustomerLocationRepository,
            ISystemInternalDepartmentTypesRepository systemInternalDepartmentTypesRepository,
            IMasterCustomerLocationInternalDepartmentRepository masterCustomerLocationInternalDepartmentRepository,
            IVaultBalanceDiscrepancyRepository vaultBalanceDiscrepancyRepository
            )
        {
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _systemInternalDepartmentTypesRepository = systemInternalDepartmentTypesRepository;
            _masterCustomerLocationInternalDepartmentRepository = masterCustomerLocationInternalDepartmentRepository;
            _vaultBalanceDiscrepancyRepository = vaultBalanceDiscrepancyRepository;
        }
        #endregion

        /// <summary>
        /// USE IN DROPDOWN VAULT BALANCE
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        public IEnumerable<InternalDepartmentResponse> GetPrevaultBySite(Guid siteGuid)
        {

            var internalDepartment = _masterCustomerLocationInternalDepartmentRepository.FindAllAsQueryable(e => !e.FlagDisable);
            var location = _masterCustomerLocationRepository.FindAllAsQueryable(e => e.MasterSite_Guid.Value.Equals(siteGuid));
            var departType = _systemInternalDepartmentTypesRepository.FindAllAsQueryable(e => e.InternalDepartmentID.Value.Equals(2));

            var depart = internalDepartment.Join(location,
                                inter => inter.MasterCustomerLocation_Guid,
                                lo => lo.Guid, (inter, lo) => new { internalD = inter, l = lo, })
                                                                .Join(departType,
                                                                            dpt => dpt.internalD.InternalDepartmentType,
                                                                            interD => interD.Guid, (dpt, interD) => new { dpt, interD })
                                                                .OrderBy(o => o.dpt.internalD.InterDepartmentName)
                                                                .Select(result => new InternalDepartmentResponse
                                                                {
                                                                    InternalDepartment_Guid = result.dpt.internalD.Guid,
                                                                    InternalDepartmentName = result.dpt.internalD.InterDepartmentName,
                                                                    InternalDepartmentID = result.interD.InternalDepartmentID,
                                                                    InternalDepartmentType = result.dpt.internalD.InternalDepartmentType,
                                                                    InternalDepartmentTypeName = result.interD.InternalDepartmentTypeName
                                                                }).OrderBy(o => o.InternalDepartmentName).AsEnumerable();
            return depart;
        }
    }
}
