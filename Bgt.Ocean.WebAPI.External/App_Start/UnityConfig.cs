#region Using
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework;
using Bgt.Ocean.Repository.EntityFramework.Core;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Customer;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Products;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Implementations.Monitoring;
using Bgt.Ocean.Service.Implementations.Prevault;
using Bgt.Ocean.Service.Implementations.RunControl;
using Bgt.Ocean.Service.Implementations.SFO;
using Bgt.Ocean.Service.Implementations.StandardTable;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;
#endregion

namespace Bgt.Ocean.WebAPI.External
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            #region Configuration
            container.RegisterType<IDbFactory<OceanDbEntities>, OceanDbFactory>(new PerThreadLifetimeManager());
            container.RegisterType<IUnitOfWork<OceanDbEntities>, OceanUnitOfWork>();
            container.RegisterType<IDbFactory<SFOLogDbEntities>, SFOLogDbFactory>(new PerThreadLifetimeManager());
            container.RegisterType<IUnitOfWork<SFOLogDbEntities>, SFOLogDbUnitOfWork>();
            #endregion

            #region Service
            // OO Service
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<ISystemService, SystemService>();
            container.RegisterType<IBrinksService, BrinksService>();
            container.RegisterType<IExportExcelService, ExportExcelService>();
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<IActualJobHeaderService, ActualJobHeaderService>();
            container.RegisterType<IOTCRunControlService, OTCRunControlService>();
            container.RegisterType<ICrewService, CrewService>();
            container.RegisterType<IReasonCodeService, ReasonCodeService>();
            container.RegisterType<IRouteGroupService, RouteGroupService>();
            container.RegisterType<IRunResourceTypeService, RunResourceTypeService>();
            container.RegisterType<INemoConfigurationService, NemoConfigurationService>();
            container.RegisterType<IMasterRouteService, MasterRouteService>();
            container.RegisterType<IMonitoringService, MonitoringService>();
            container.RegisterType<IConsolidationService, ConsolidationService>();
            container.RegisterType<ICheckOutDepartmentService, CheckOutDepartmentService>();

            // SFO Service
            container.RegisterType<IServiceRequestService, ServiceRequestService>();
            container.RegisterType<IGenericLogService, GenericLogService>();
            container.RegisterType<IMachineService, MachineService>();
            container.RegisterType<ISFOMasterDataService, SFOMasterDataService>();
            container.RegisterType<IMachineServiceTypeService, MachineServiceTypeService>();
            container.RegisterType<IMachineSubServiceTypeService, MachineSubServiceTypeService>();
            container.RegisterType<IProblemService, ProblemService>();
            container.RegisterType<IProblemPriorityService, ProblemPriorityService>();
            #endregion

            #region Repository
            // OO Repository
            container.RegisterType<IWebAPIUser_TokenRepository, WebAPIUser_TokenRepository>();
            container.RegisterType<IWebAPIUserRepository, WebAPIUserRepository>();
            container.RegisterType<IMasterMenuDetailRepository, MasterMenuDetailRepository>();
            container.RegisterType<IMasterUserRepository, MasterUserRepository>();
            container.RegisterType<ISystemEnvironment_GlobalRepository, SystemEnvironment_GlobalRepository>();
            container.RegisterType<IMasterLogVerifyKeyRepository, MasterLogVerifyKeyRepository>();
            container.RegisterType<ISystemDisplayTextControlsLanguageRepository, SystemDisplayTextControlsLanguageRepository>();
            container.RegisterType<ISystemMessageRepository, SystemMessageRepository>();
            container.RegisterType<ISystemServiceJobTypeLOBRepository, SystemServiceJobTypeLOBRepository>();
            container.RegisterType<ISystemServiceJobTypeRepository, SystemServiceJobTypeRepository>();
            container.RegisterType<IMasterCityRepository, MasterCityRepository>();
            container.RegisterType<IMasterCountry_StateRepository, MasterCountry_StateRepository>();
            container.RegisterType<IMasterCountryRepository, MasterCountryRepository>();
            container.RegisterType<IMasterCurrencyRepository, MasterCurrencyRepository>();
            container.RegisterType<IMasterCustomerContractRepository, MasterCustomerContractRepository>();
            container.RegisterType<IMasterCustomerRepository, MasterCustomerRepository>();
            container.RegisterType<IMasterDistrictRepository, MasterDistrictRepository>();
            container.RegisterType<ISystemFormat_NumberCurrencyRepository, SystemFormat_NumberCurrencyRepository>();
            container.RegisterType<ISystemLog_ActivityRepository, SystemLog_ActivityRepository>();
            container.RegisterType<ISystemLog_AttemptToLoginRepository, SystemLog_AttemptToLoginRepository>();
            container.RegisterType<ISystemLog_HistoryErrorRepository, SystemLog_HistoryErrorRepository>();
            container.RegisterType<IMasterReportRepository, MasterReportRepository>();
            container.RegisterType<ISystemJobStatusRepository, SystemJobStatusRepository>();
            container.RegisterType<ISystemLanguageRepository, SystemLanguageRepository>();
            container.RegisterType<ISystemDomainsRepository, SystemDomainsRepository>();
            container.RegisterType<ILeedToCashProductRepository, LeedToCashProductRepository>();
            container.RegisterType<IMasterActualJobHeaderRepository, MasterActualJobHeaderRepository>();
            container.RegisterType<ISystemJobActionsRepository, SystemJobActionsRepository>();
            container.RegisterType<IMasterCustomerContractServiceLocationRepository, MasterCustomerContractServiceLocationRepository>();
            container.RegisterType<IMasterCustomerLocationRepository, MasterCustomerLocationRepository>();
            container.RegisterType<IMasterActualJobServiceStopSpecialCommandRepository, MasterActualJobServiceStopSpecialCommandRepository>();
            container.RegisterType<IMasterActualJobServiceStopsRepository, MasterActualJobServiceStopsRepository>();
            container.RegisterType<IMasterHistoryActualJobRepository, MasterHistoryActualJobRepository>();
            container.RegisterType<IMasterDailyRunResourceRepository, MasterDailyRunResourceRepository>();
            container.RegisterType<ISystemRunningValueGlobalRepository, SystemRunningValueGlobalRepository>();
            container.RegisterType<ISystemServiceStopTypesRepository, SystemServiceStopTypesRepository>();
            container.RegisterType<IMasterSiteRepository, MasterSiteRepository>();
            container.RegisterType<IMasterActualJobServiceStopLegsRepository, MasterActualJobServiceStopLegsRepository>();
            container.RegisterType<IMasterHistoryActualJobOnDailyRunResourceRepository, MasterHistoryActualJobOnDailyRunResourceRepository>();
            container.RegisterType<IMasterActualJobHeaderOTCRepository, MasterActualJobHeaderOTCRepository>();
            container.RegisterType<ISystemTimeZoneRepository, SystemTimeZoneRepository>();
            container.RegisterType<ISFOTransactionGenericLogRepository, SFOTransactionGenericLogRepository>();
            container.RegisterType<IReasonCodeRepository, ReasonCodeRepository>();
            container.RegisterType<IMasterRouteGroupRepository, MasterRouteGroupRepository>();
            container.RegisterType<IRunResourceTypeRepository, RunResourceTypeRepository>();
            container.RegisterType<IMasterNemoCountryValueRepository, MasterNemoCountryValueRepository>();
            container.RegisterType<IMasterNemoSiteValueRepository, MasterNemoSiteValueRepository>();
            container.RegisterType<IMasterNemoTrafficFactorValueRepository,MasterNemoTrafficFactorValueRepository>();
            container.RegisterType<ISystemGlobalUnitRepository,SystemGlobalUnitRepository>();
            container.RegisterType<IMasterRouteJobHeaderRepository, MasterRouteJobHeaderRepository>();
            container.RegisterType<IMasterPlaceRepository, MasterPlaceRepository>();
            container.RegisterType<IMasterHistoryErrorPodByServiceRepository, MasterHistoryErrorPodByServiceRepository>();
            container.RegisterType<IMasterHistoryLogPodByServiceRepository, MasterHistoryLogPodByServiceRepository>();
            container.RegisterType<IMasterHistory_DailyRunResourceRepository, MasterHistory_DailyRunResourceRepository>();
            container.RegisterType<IMasterActualJobItemsSealRepository, MasterActualJobItemsSealRepository>();
            container.RegisterType<IMasterActualJobItemsCommodityRepository, MasterActualJobItemsCommodityRepository>();


            // SFO Repository
            container.RegisterType<ISFOMasterCountryTimeZoneRepository, SFOMasterCountryTimeZoneRepository>();
            container.RegisterType<ISFOMasterJournalRepository, SFOMasterJournalRepository>();
            container.RegisterType<ISFOMasterMachineLockTypeRepository, SFOMasterMachineLockTypeRepository>();
            container.RegisterType<ISFOMasterMachineRepository, SFOMasterMachineRepository>();
            container.RegisterType<ISFOMasterMachineServiceTypeRepository, SFOMasterMachineServiceTypeRepository>();
            container.RegisterType<ISFOMasterProblemRepository, SFOMasterProblemRepository>();
            container.RegisterType<ISFOMasterSolutionRepository, SFOMasterSolutionRepository>();
            container.RegisterType<ISFOMasterSourceRepository, SFOMasterSourceRepository>();
            container.RegisterType<ISFOSystemDataConfigurationRepository, SFOSystemDataConfigurationRepository>();
            container.RegisterType<ISFOSystemEnvironmentGlobalRepository, SFOSystemEnvironmentGlobalRepository>();
            container.RegisterType<ISFOSystemFunctionRepository, SFOSystemFunctionRepository>();
            container.RegisterType<ISFOSystemLockTypeRepository, SFOSystemLockTypeRepository>();
            container.RegisterType<ISFOTransactionServiceRequestEcashRepository, SFOTransactionServiceRequestEcashRepository>();
            container.RegisterType<ISFOTransactionServiceRequestInfoRepository, SFOTransactionServiceRequestInfoRepository>();
            container.RegisterType<ISFOTransactionServiceRequestJournalRepository, SFOTransactionServiceRequestJournalRepository>();
            container.RegisterType<ISFOTransactionServiceRequestProblemRepository, SFOTransactionServiceRequestProblemRepository>();
            container.RegisterType<ISFOTransactionServiceRequestRepository, SFOTransactionServiceRequestRepository>();
            container.RegisterType<ISFOTransactionServiceRequestSolutionRepository, SFOTransactionServiceRequestSolutionRepository>();
            container.RegisterType<ISystemDayOfWeekRepository, SystemDayOfWeekRepository>();
            container.RegisterType<IMachineServiceTypeRepository, MachineServiceTypeRepository>();
            container.RegisterType<IMachineSubServiceTypeRepository, MachineSubServiceTypeRepository>();
            container.RegisterType<IProblemRepository, ProblemRepository>();
            container.RegisterType<IProblemPriorityRepository, ProblemPriorityRepository>();
            #endregion

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}