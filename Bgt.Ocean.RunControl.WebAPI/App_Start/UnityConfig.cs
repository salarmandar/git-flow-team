using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using Bgt.Ocean.Repository.EntityFramework;
using Bgt.Ocean.Repository.EntityFramework.Core;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.AuditLog;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Customer;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Mobile;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring;
using Bgt.Ocean.Repository.EntityFramework.Repositories.NemoDynamicRoute;
using Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Products;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SiteNetwork;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable.SitePath;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.TempReport;
using System;
using Unity;
using Unity.Lifetime;

namespace Bgt.Ocean.RunControl.WebAPI
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var registry = new UnityContainer();
              RegisterTypes(registry);
              return registry;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // configuration
            container.RegisterType<IDbFactory<OceanDbEntities>, OceanDbFactory>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitOfWork<OceanDbEntities>, OceanUnitOfWork>();
            container.RegisterType<IDbFactory<SFOLogDbEntities>, SFOLogDbFactory>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitOfWork<SFOLogDbEntities>, SFOLogDbUnitOfWork>(new HierarchicalLifetimeManager());

            // Register repository here 
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
            container.RegisterType<ISystemLanguageRepository, SystemLanguageRepository>();
            container.RegisterType<ISystemJobStatusRepository, SystemJobStatusRepository>();
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
            container.RegisterType<IChargeCategoryRepository, ChargeCategoryRepository>();
            container.RegisterType<IPricingRuleRepository, PricingRuleRepository>();
            container.RegisterType<ISystemGlobalUnitRepository, SystemGlobalUnitRepository>();
            container.RegisterType<IMasterCommodityRepository, MasterCommodityRepository>();
            container.RegisterType<IMasterCommodityCountryRepository, MasterCommodityCountryRepository>();
            container.RegisterType<IMasterRouteGroupRepository, MasterRouteGroupRepository>();
            container.RegisterType<IMasterRouteGroupDetailRepository, MasterRouteGroupDetailRepository>();
            container.RegisterType<IMasterNemoCountryValueRepository, MasterNemoCountryValueRepository>();
            container.RegisterType<IMasterNemoSiteValueRepository, MasterNemoSiteValueRepository>();
            container.RegisterType<IMasterNemoTrafficFactorValueRepository, MasterNemoTrafficFactorValueRepository>();
            container.RegisterType<ISystemGlobalUnitRepository, SystemGlobalUnitRepository>();
            container.RegisterType<IMasterRouteJobHeaderRepository, MasterRouteJobHeaderRepository>();
            container.RegisterType<ISystemDayOfWeekRepository, SystemDayOfWeekRepository>();
            container.RegisterType<IQuotationRepository, QuotationRepository>();
            container.RegisterType<ISystemOnwardDestinationTypesRepository, SystemOnwardDestinationTypesRepository>();
            container.RegisterType<IMasterCustomerLocationBrinksSiteRepository, MasterCustomerLocationBrinksSiteRepository>();
            container.RegisterType<IMasterClauseRepository, MasterClauseRepository>();
            container.RegisterType<ISystemInternalDepartmentTypesRepository, SystemInternalDepartmentTypesRepository>();
            container.RegisterType<IMasterCustomerLocationInternalDepartmentRepository, MasterCustomerLocationInternalDepartmentRepository>();
            container.RegisterType<IMasterSpecialCommandRepository, MasterSpecialCommandRepository>();
            container.RegisterType<ISystemTripIndicatorRepository, SystemTripIndicatorRepository>();
            container.RegisterType<IMasterSubServiceTypeRepository, MasterSubServiceTypeRepository>();
            container.RegisterType<IMasterDenominationRepository, MasterDenominationRepository>();
            container.RegisterType<ISFOMasterMachineCassetteRepository, SFOMasterMachineCassetteRepository>();
            container.RegisterType<ISystemATMScreenRepository, SystemATMScreenRepository>();
            container.RegisterType<ISystemEnvironmentMasterCountryRepository, SystemEnvironmentMasterCountryRepository>();
            container.RegisterType<IMasterRouteGroupDetailRepository, MasterRouteGroupDetailRepository>();
            container.RegisterType<ISystemCustomerLocationTypeRepository, SystemCustomerLocationTypeRepository>();
            container.RegisterType<ISFOMasterOTCLockModeRepository, SFOMasterOTCLockModeRepository>();
            container.RegisterType<ISFOSystemOTCLockModeRepository, SFOSystemOTCLockModeRepository>();
            container.RegisterType<IMasterCustomerLocation_LocationDestinationRepository, MasterCustomerLocation_LocationDestinationRepository>();
            container.RegisterType<IQuotation_PricingRule_MappingRepository, Quotation_PricingRule_MappingRepository>();
            container.RegisterType<ISFOTblTransactionOTCRepository, SFOTblTransactionOTCRepository>();
            container.RegisterType<ISystemLineOfBusinessRepository, SystemLineOfBusinessRepository>();
            container.RegisterType<IMasterDetailRouteOptimizationRepository, MasterDetailRouteOptimizationRepository>();
            container.RegisterType<IMasterErrorRouteOptimizationRepository, MasterErrorRouteOptimizationRepository>();
            container.RegisterType<IMasterQueueRouteOptimizationRepository, MasterQueueRouteOptimizationRepository>();
            container.RegisterType<IMasterPlaceRepository, MasterPlaceRepository>();
            container.RegisterType<IMasterHistoryErrorPodByServiceRepository, MasterHistoryErrorPodByServiceRepository>();
            container.RegisterType<IMasterHistoryLogPodByServiceRepository, MasterHistoryLogPodByServiceRepository>();
            container.RegisterType<IMasterActualJobActualCountRepository, MasterActualJobActualCountRepository>();
            container.RegisterType<IMasterActualJobCashAddRepository, MasterActualJobCashAddRepository>();
            container.RegisterType<IMasterActualJobCashReturnRepository, MasterActualJobCashReturnRepository>();
            container.RegisterType<IMasterActualJobMachineReportRepository, MasterActualJobMachineReportRepository>();
            container.RegisterType<IMasterActualJobSumActualCountRepository, MasterActualJobSumActualCountRepository>();
            container.RegisterType<IMasterActualJobSumCashAddRepository, MasterActualJobSumCashAddRepository>();
            container.RegisterType<IMasterActualJobSumCashReturnRepository, MasterActualJobSumCashReturnRepository>();
            container.RegisterType<IMasterActualJobSumMachineReportRepository, MasterActualJobSumMachineReportRepository>();
            container.RegisterType<IMasterUserAccessGroupCountryEmailRepository, MasterUserAccessGroupCountryEmailRepository>();
            container.RegisterType<ISystemEmailDomainsRepository, SystemEmailDomainsRepository>();
            container.RegisterType<ISystemNotificationConfigPeriodsRepository, SystemNotificationConfigPeriodsRepository>();
            container.RegisterType<ISystemNotificationConfigPeriodsUsersRepository, SystemNotificationConfigPeriodsUsersRepository>();
            container.RegisterType<ISystemEnvironmentMasterCountryValueRepository, SystemEnvironmentMasterCountryValueRepository>();
            container.RegisterType<IMasterActualJobHeaderCapabilityRepository, MasterActualJobHeaderCapabilityRepository>();
            container.RegisterType<ISystemConfigurationAuditLogRepository, SystemConfigurationAuditLogRepository>();
            container.RegisterType<IMobileATMCheckListEERepository, MobileATMCheckListEERepository>();
            container.RegisterType<IMasterHistory_DailyRunResourceRepository, MasterHistory_DailyRunResourceRepository>();
            container.RegisterType<IMasterSiteNetworkRepository, MasterSiteNetworkRepository>();
            container.RegisterType<IMasterConAndDeconsolidate_HeaderRepository, MasterConAndDeconsolidate_HeaderRepository>();
            container.RegisterType<ISystemConAndDeconsolidateStatusRepository, SystemConAndDeconsolidateStatusRepository>();
            container.RegisterType<IMasterSiteNetworkMemberRepository, MasterSiteNetworkMemberRepository>();
            container.RegisterType<IMasterSiteNetworkAuditLogRepository, MasterSiteNetworkAuditLogRepository>();
            container.RegisterType<IMasterSitePathHeaderRepository, MasterSitePathHeaderRepository>();
            container.RegisterType<IMasterActualJobItemsSealRepository, MasterActualJobItemsSealRepository>();
            container.RegisterType<IMasterSitePathRepository, MasterSitePathRepository>();
            container.RegisterType<IMasterSitePathDetailRepository, MasterSitePathDetailRepository>();            
            container.RegisterType<ISystemCustomerLocationAuditLogRepository, SystemCustomerLocationAuditLogRepository>();
            container.RegisterType<IMasterCustomerLocation_LocationDestinationRepository, MasterCustomerLocation_LocationDestinationRepository>();
            container.RegisterType<IMasterCustomer_AccountRepository, MasterCustomer_AccountRepository>();
            container.RegisterType<IMasterActualJobItemsCommodityRepository, MasterActualJobItemsCommodityRepository>();
            container.RegisterType<IMasterActualJobItemsSeal_ScanHistoryRepository, MasterActualJobItemsSeal_ScanHistoryRepository>();
            container.RegisterType<IMasterHistory_SealRepository, MasterHistory_SealRepository>();
            container.RegisterType<IMasterActualJobItemsCommodity_ScanHistoryRepository, MasterActualJobItemsCommodity_ScanHistoryRepository>();
            container.RegisterType<ITempMainPrevaultReportRepository, TempMainPrevaultReportRepository>();
            container.RegisterType<ITempPrevaultNonBarcodeReportRepository, TempPrevaultNonBarcodeReportRepository>();
            container.RegisterType<ITempPrevaultSealReportRepository, TempPrevaultSealReportRepository>();
            container.RegisterType<ISystemReportStyleRepository, SystemReportStyleRepository>();
            container.RegisterType<IMasterHistory_ActualJobRepository, MasterHistory_ActualJobRepository>();
            container.RegisterType<IMasterSitePathDestinationRepository, MasterSitePathDestinationRepository>();
            container.RegisterType<IMasterSitePathAuditLogRepository, MasterSitePathAuditLogRepository>();
            container.RegisterType<IMasterRouteJobServiceStopLegsRepository, MasterRouteJobServiceStopLegsRepository>();

            // Register service here
            //container.RegisterType<IUserService, UserService>();
           
            // sfo
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
            container.RegisterType<ISFOSystemModelConfigRepository, SFOSystemModelConfigRepository>();
            container.RegisterType<IReasonCodeRepository, ReasonCodeRepository>();
            container.RegisterType<ISFOSystemEscalationRuleHeaderRepository, SFOSystemEscalationRuleHeaderRepository>();
            container.RegisterType<ISFOSystemEscalationRuleDetailRepository, SFOSystemEscalationRuleDetailRepository>();
            container.RegisterType<ISFOSystemEscalationRuleHeader_DetailRepository, SFOSystemEscalationRuleHeader_DetailRepository>();
            container.RegisterType<ISFOSystemServiceRequestStateRepository, SFOSystemServiceRequestStateRepository>();
            container.RegisterType<ISFOMasterEscalationHeaderRepository, SFOMasterEscalationHeaderRepository>();
            container.RegisterType<ISFOMasterEscalationDetailRepository, SFOMasterEscalationDetailRepository>();
            container.RegisterType<ISFOMasterEscalationDetail_PositionRepository, SFOMasterEscalationDetail_PositionRepository>();
            container.RegisterType<ISFOMasterEscalationDetail_EmailRepository, SFOMasterEscalationDetail_EmailRepository>();
            container.RegisterType<IUserEmailTemplateRepository, UserEmailTemplateRepository>();
            container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            container.RegisterType<IPositionRepository, PositionRepository>();

        }
    }
}