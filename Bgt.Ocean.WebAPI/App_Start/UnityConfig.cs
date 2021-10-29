using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Implementations.Consolidation;
using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Implementations.Monitoring;
using Bgt.Ocean.Service.Implementations.Prevault;
using Bgt.Ocean.Service.Implementations.PricingRules;
using Bgt.Ocean.Service.Implementations.PushToDolphin;
using Bgt.Ocean.Service.Implementations.RunControl;
using Bgt.Ocean.Service.Implementations.AuditLog.ServiceRequest;
using Bgt.Ocean.Service.Implementations.StandardTable;
using System;
using Unity;
using Bgt.Ocean.Service.Implementations.File;
using Bgt.Ocean.Service.Implementations.Configuration;
using Microsoft.AspNet.SignalR;
using Bgt.Ocean.Service.Implementations.Hubs;
using Unity.Injection;
using Microsoft.AspNet.SignalR.Infrastructure;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.WebAPI.Hubs;
using Bgt.Ocean.Service.Implementations.Nemo;
using Bgt.Ocean.Service.Implementations.Nemo.NemoSync;
using Bgt.Ocean.Service.Implementations.Systems;
using Bgt.Ocean.Service.Implementations.Nemo.RouteOptimization;
using Bgt.Ocean.Service.Implementations.Report;
using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Implementations.Email;
using Bgt.Ocean.Service.Implementations.TruckLiabilityLimit;
using Bgt.Ocean.Service.Implementations.Denomination;
using Bgt.Ocean.Service.Implementations.PreVault;

namespace Bgt.Ocean.WebAPI
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

        private static DefaultDependencyResolver SignalRResolver;
        public static void RegisterSignalRResolver(DefaultDependencyResolver resolver) => SignalRResolver = resolver;

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
            // register sharing repository
            Bgt.Ocean.DependencyResolver.UnityConfig.RegisterTypes(container);

            // Register service here
            container.RegisterType<IActualJobHeaderService, ActualJobHeaderService>();
            container.RegisterType<IAdhocService, AdhocService>();
            container.RegisterType<IBrinksService, BrinksService>();
            container.RegisterType<ICheckOutDepartmentService, CheckOutDepartmentService>();
            container.RegisterType<IConsolidationService, ConsolidationService>();
            container.RegisterType<IContractService, ContractService>();
            container.RegisterType<ICrewService, CrewService>();
            container.RegisterType<ICustomerLocationAuditLogService, CustomerLocationAuditLogService>();
            container.RegisterType<IExportExcelService, ExportExcelService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IMasterCustomerLocationService, MasterCustomerLocationService>();
            container.RegisterType<IMasterCustomerService, MasterCustomerService>();
            container.RegisterType<IMasterRouteService, MasterRouteService>();
            container.RegisterType<IMasterRouteTransactionLogService, MasterRouteTransactionLogService>();
            container.RegisterType<IMasterUserAccessGroupCountryEmailService, MasterUserAccessGroupCountryEmailService>();
            container.RegisterType<IMonitoringService, MonitoringService>();
            container.RegisterType<INemoAuthenticationService, NemoAuthenticationService>();
            container.RegisterType<INemoConfigurationService, NemoConfigurationService>();
            container.RegisterType<INemoSyncService, NemoSyncService>();
            container.RegisterType<IRouteOptimizationService, RouteOptimizationService>();
            container.RegisterType<IOTCRunControlService, OTCRunControlService>();
            container.RegisterType<IPricingRuleService, PricingRuleService>();
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<IPushToDolphinService, PushToDolphinService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IQuotationService, QuotationService>();
            container.RegisterType<IRouteGroupService, RouteGroupService>();
            container.RegisterType<IRunControlService, RunControlService>();
            container.RegisterType<ISaleService, SaleService>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<ISiteNetworkAuditoLogService, SiteNetworkAuditoLogService>();
            container.RegisterType<ISiteNetworkService, SiteNetworkService>();
            container.RegisterType<ISitePathService, SitePathService>();
            container.RegisterType<ISystemConfigurationAuditLogService, SystemConfigurationAuditLogService>();
            container.RegisterType<ISystemDomainEmailService, SystemDomainEmailService>();
            container.RegisterType<ISystemEmailActionService, SystemEmailActionService>();
            container.RegisterType<ISystemEnvironmentService, SystemEnvironmentService>();
            container.RegisterType<ISystemNotificationPeriodsService, SystemNotificationPeriodsService>();
            container.RegisterType<ISystemService, SystemService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IFleetSummaryService, FleetSummaryService>();
            container.RegisterType<IFleetMaintenanceService, FleetMaintenanceService>();
            container.RegisterType<IFleetGasolineService, FleetGasolineService>();
            container.RegisterType<IFleetAccidentService, FleetAccidentService>();
            container.RegisterType<IFleetMainService, FleetMainService>();
            container.RegisterType<IBaseRequest, BaseRequest>();
            container.RegisterType<IMoniteringNetWorkService, MoniteringNetWorkService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IManageMasterJobService, ManageMasterJobService>();
            container.RegisterType<ITruckLiabilityLimitService, TruckLiabilityLimitService>();
            container.RegisterType<IOnHandRouteService, OnHandRouteService>();
            container.RegisterType<IDenominationService, DenominationService>();
            container.RegisterType<IVaultBalanceService, VaultBalanceService>();
            container.RegisterType<IPrevaultManagementService, PrevaultManagementService>();
            container.RegisterType<IDiscrepancyManagementService, DiscrepancyManagementService>();

            // sfo service
            container.RegisterType<IAlarmHubService, AlarmHubService>();
            container.RegisterType<IGenericLogService, GenericLogService>();
            container.RegisterType<IMachineService, MachineService>();
            container.RegisterType<IObjectComparerService, ObjectComparerService>();
            container.RegisterType<IOTCManagementService, OTCManagementService>();
            container.RegisterType<IServiceRequestCreatorService, ServiceRequestCreatorService>();
            container.RegisterType<IServiceRequestEditorService, ServiceRequestEditorService>();
            container.RegisterType<IServiceRequestReaderService, ServiceRequestReaderService>();
            container.RegisterType<ISFOApiUserService, SFOApiUserService>();
            container.RegisterType<ISFOMasterDataService, SFOMasterDataService>();
            container.RegisterType<ISFOPushToDolphinService, SFOPushToDolphinService>();



            RegisterSignalRDI(container);
        }

        private static void RegisterSignalRDI(IUnityContainer container)
        {
            // register dependency that using signalr connection context
            container.RegisterType<IAlarmHubBroadcastService>(new InjectionFactory(c =>
            {
                var hubConnection = SignalRResolver.Resolve<IConnectionManager>();
                var groupRepo = container.Resolve<IMasterGroupRepository>();
                var masterDailyRunAlarm = container.Resolve<IMasterDailyRunResourceAlarmRepository>();
                var logRepo = container.Resolve<ISystemLog_HistoryErrorRepository>();

                return new AlarmHubBroadcastService(
                    hubConnection.GetHubContext<AlarmHub>().Clients,
                    groupRepo,
                    masterDailyRunAlarm,
                    logRepo
                );
            }));

            container.RegisterType<IRouteOptimizationBroadcastService>(new InjectionFactory(c =>
            {
                var hubConnection = SignalRResolver.Resolve<IConnectionManager>();

                return new RouteOptimizationBroadcastService(
                    hubConnection.GetHubContext<RouteOptimizationHub>().Clients
                );
            }));

        }
    }
}