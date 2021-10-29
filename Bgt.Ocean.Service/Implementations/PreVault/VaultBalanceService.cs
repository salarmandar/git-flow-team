

using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Domain;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.VaultBalance;
using Bgt.Ocean.Service.Messagings;

namespace Bgt.Ocean.Service.Implementations.PreVault
{
    public partial interface IVaultBalanceService
    {

    }
    public partial class VaultBalanceService : RequestBase, IVaultBalanceService
    {
        private readonly IMasterConAndDeconsolidate_HeaderRepository _masterConAndDeconsolidate_HeaderRepository;
        private readonly IMasterActualJobItemDiscrapenciesRepository _masterActualJobItemDiscrapenciesRepository;
        private readonly ISystemConAndDeconsolidateStatusRepository _systemConAndDeconsolidateStatusRepository;
        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegsRepository;
        private readonly IMasterActualJobItemsCommodityRepository _masterActualJobItemsCommodityRepository;
        private readonly IVaultBalanceSealAndMasterRepository _vaultBalanceSealAndMasterRepository;
        private readonly IMasterActualJobItemUnknowRepository _masterActualJobItemUnknowRepository;
        private readonly IMasterActualJobItemsSealRepository _masterActualJobItemsSealRepository;
        private readonly IVaultBalanceDiscrepancyRepository _vaultBalanceDiscrepancyRepository;
        private readonly IVaultBalanceNonbarcodeRepository _vaultBalanceNonbarcodeRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IVaultBalanceHeaderRepository _vaultBalanceHeaderRepository;
        private readonly IVaultBalanceDetailRepository _vaultBalanceDetailRepository;
        private readonly IMasterRouteGroupRepository _masterRouteGroupRepository;
        private readonly IMasterCommodityRepository _masterCommodityRepository;
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly ISystemService _systemService;
        private readonly IUserService _userService;
        private readonly IDnsWrapper _dnsWrapper;
        private readonly IBaseRequest _req;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        public VaultBalanceService(
              IMasterConAndDeconsolidate_HeaderRepository masterConAndDeconsolidate_HeaderRepository
            , IMasterActualJobItemDiscrapenciesRepository masterActualJobItemDiscrapenciesRepository
            , ISystemConAndDeconsolidateStatusRepository systemConAndDeconsolidateStatusRepository
            , IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegsRepository
            , IMasterActualJobItemsCommodityRepository masterActualJobItemsCommodityRepository
            , IVaultBalanceSealAndMasterRepository vaultBalanceSealAndMasterRepository
            , IMasterActualJobItemUnknowRepository masterActualJobItemUnknowRepository
            , IMasterActualJobItemsSealRepository masterActualJobItemsSealRepository
            , IVaultBalanceDiscrepancyRepository vaultBalanceDiscrepancyRepository
            , IVaultBalanceNonbarcodeRepository vaultBalanceNonbarcodeRepository
            , IMasterActualJobHeaderRepository masterActualJobHeaderRepository
            , IVaultBalanceHeaderRepository vaultBalanceHeaderRepository
            , IVaultBalanceDetailRepository vaultBalanceDetailRepository
            , IMasterRouteGroupRepository masterRouteGroupRepository
            , IMasterCommodityRepository masterCommodityRepository
            , IMasterCustomerRepository masterCustomerRepository
            , ISystemMessageRepository systemMessageRepository
            , IMasterSiteRepository masterSiteRepository
            , ISystemService systemService
            , IUserService userService
            , IBaseRequest req
            , IDnsWrapper dnsWrapper
            , IUnitOfWork<OceanDbEntities> uow)
        {
            _masterConAndDeconsolidate_HeaderRepository = masterConAndDeconsolidate_HeaderRepository;
            _masterActualJobItemDiscrapenciesRepository = masterActualJobItemDiscrapenciesRepository;
            _systemConAndDeconsolidateStatusRepository = systemConAndDeconsolidateStatusRepository;
            _masterActualJobServiceStopLegsRepository = masterActualJobServiceStopLegsRepository;
            _masterActualJobItemsCommodityRepository = masterActualJobItemsCommodityRepository;
            _vaultBalanceSealAndMasterRepository = vaultBalanceSealAndMasterRepository;
            _masterActualJobItemUnknowRepository = masterActualJobItemUnknowRepository;
            _masterActualJobItemsSealRepository = masterActualJobItemsSealRepository;
            _vaultBalanceDiscrepancyRepository = vaultBalanceDiscrepancyRepository;
            _vaultBalanceNonbarcodeRepository = vaultBalanceNonbarcodeRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _vaultBalanceDetailRepository = vaultBalanceDetailRepository;
            _vaultBalanceHeaderRepository = vaultBalanceHeaderRepository;
            _masterRouteGroupRepository = masterRouteGroupRepository;
            _masterCommodityRepository = masterCommodityRepository;
            _masterCustomerRepository = masterCustomerRepository;
            _systemMessageRepository = systemMessageRepository;
            _masterSiteRepository = masterSiteRepository;
            _systemService = systemService;
            _userService = userService;
            _dnsWrapper = dnsWrapper;
            _req = req;
            _uow = uow;
        }
    }
}
