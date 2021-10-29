#region Using
using AutoMapper;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Customer;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Models.NemoConfiguration;
using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Models.ServiceRequest;
using Bgt.Ocean.Models.SiteNetwork;
using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Models.SystemConfigurationAdditional;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.BrinksService;
using Bgt.Ocean.Service.Messagings.CustomerService;
using Bgt.Ocean.Service.Messagings.MasterService;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using Bgt.Ocean.Service.Messagings.StandardTable.MachineServiceType;
using Bgt.Ocean.Service.Messagings.StandardTable.MachineSubServiceType;
using Bgt.Ocean.Service.Messagings.StandardTable.Problem;
using Bgt.Ocean.Service.Messagings.StandardTable.ProblemPriority;
using Bgt.Ocean.Service.Messagings.StandardTable.ReasonCode;
using Bgt.Ocean.Service.Messagings.StandardTable.RouteGroup;
using Bgt.Ocean.Service.Messagings.StandardTable.RunResourceType;
using Bgt.Ocean.Service.Messagings.UserService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.Adhoc;
using Bgt.Ocean.Service.ModelViews.Customer;
using Bgt.Ocean.Service.ModelViews.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.Machine;
using Bgt.Ocean.Service.ModelViews.Masters;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using Bgt.Ocean.Service.ModelViews.RunControls;
using Bgt.Ocean.Service.ModelViews.ServiceRequest;
using Bgt.Ocean.Service.ModelViews.SiteNetWork;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.ModelViews.Users;
using Bgt.Ocean.Service.ModelViews.SystemConfigurationAdditional;
using Bgt.Ocean.Services.Messagings.StandardTable.Machine;
using System;
using System.Linq;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.ModelViews.PreVault;
using Bgt.Ocean.Service.ModelViews.SitePath;
using Bgt.Ocean.Models.PricingRules;
using Bgt.Ocean.Models.MasterRoute;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;
using Bgt.Ocean.Models.Consolidation;
using Bgt.Ocean.Service.Messagings.AlarmHub;
using Bgt.Ocean.Models.Nemo.NemoSync;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using Bgt.Ocean.Models.OnHandRoute;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using Bgt.Ocean.Service.ModelViews.Monitoring;
using static Bgt.Ocean.Infrastructure.Util.EnumMonitoring;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance;
using Bgt.Ocean.Models.PreVault;
#endregion

namespace Bgt.Ocean.Service.Mapping
{
    public static class ServiceMapperBootstrapper
    {
        private static IMapper _mapper;

        public static IMapper MapperService
        {
            get
            {
                if (_mapper == null)
                    Configure();
                return _mapper;
            }
        }

        public static void Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                ConfigSystem(cfg);
                ConfigBrinks(cfg);
                ConfigRunControl(cfg);
                ConfigStandardTable(cfg);
                ConfigPricingRule(cfg);
                ConfigAdhoc(cfg);
                ConfigCustomer(cfg);
                Config_DomainEmail(cfg);
                Config_SystemEnviromentGlobal(cfg);
                Config_PreDefinedEmails(cfg);
                Config_NotificationConfigPeriods(cfg);
                Config_LogAudit(cfg);
                ConfigSiteNework(cfg);
                ConfigPreVault(cfg);
                ConfigAlarmHub(cfg);
                ConfigUser(cfg);

                #region SFO

                ConfigMachine(cfg);
                ConfigProblem(cfg);
                ConfigECash(cfg);
                ConfigTechMeet(cfg);
                ConfigServiceRequest(cfg);

                #endregion

                #region Nemo
                ConfigNemoConfig(cfg);
                ConfigNemoSync(cfg);
                #endregion

                #region Fleet maintenance
                ConfigFleetMaintenance(cfg);
                #endregion

                #region On Hand Route
                ConfigOnHandRoute(cfg);
                #endregion

                #region Smart Billing Monitoring
                ConfigSmartBillingMonitoring(cfg);
                #endregion

                #region Vault Balance
                ConfigVaultBalance(cfg);
                #endregion
            });

            _mapper = config.CreateMapper();
        }

        private static void ConfigUser(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblMasterUser, UserView>();
        }

        private static void ConfigAlarmHub(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblMasterDailyRunResource_Alarm, AlarmHubTriggeredResponse>()
                .ForMember(
                    src => src.LocationName,
                    opt => opt.MapFrom(e => e.TblMasterCustomerLocation != null ? e.TblMasterCustomerLocation.BranchName : null)
                );
        }

        private static void ConfigTechMeet(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SRCreateRequestTechMeet, TechMeetView>();
            cfg.CreateMap<SRCreateRequestFLM, SRCreateRequestTechMeet>();
        }

        private static void ConfigECash(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ECashAmount, ECashView>();
            cfg.CreateMap<SRCreateRequestFLM, SRCreateRequestECash>();
        }

        private static void ConfigSystem(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<MasterMenuDetailResult, MasterMenuDetailResponse>();
            cfg.CreateMap<CountryByUserResult, CountryByUserResponse>();

            cfg.CreateMap<TblSystemMessage, SystemMessageView>();
            cfg.CreateMap<TblSystemApplication, SystemApplicationResponse>();
            cfg.CreateMap<TblSystemDomainAiles, SystemDomainAilesResponse>();
            cfg.CreateMap<TblSystemDomainDC, SystemDomainDCResponse>();
            cfg.CreateMap<AuthenLoginResult, AuthenLoginResponse>();

            cfg.CreateMap<AuthenLoginResponse, DataStorage>().ForMember(
                dst => dst.UserGuid,
                opt => opt.MapFrom(s => s.GuidMasterUser)
            ).ForMember(
                dst => dst.UserLanguageGuid,
                opt => opt.MapFrom(s => s.SystemLanguage_Guid)
            );
            cfg.CreateMap<TblSystemGlobalUnit, SystemGlobalUnitView>();
            cfg.CreateMap<LineOfBusinessJobTypeByFlagAdhocJobResult, LineOfBusinessAndJobType>();
            cfg.CreateMap<TblSystemDayOfWeek, SystemDayOfWeekView>();
            cfg.CreateMap<TblSystemLineOfBusiness, LobView>();

            cfg.CreateMap<SystemMessageView, BaseResponse>()
                .ForMember(
                    dst => dst.Message,
                    opt => opt.MapFrom(e => e.MessageTextContent)
                )
                .ForMember(
                    dst => dst.Title,
                    opt => opt.MapFrom(e => e.MessageTextTitle)
                );
        }

        private static void ConfigBrinks(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblMasterDistrict, DistrictView>();

            cfg.CreateMap<TblMasterDistrict, MasterDistrictResponse>();

            cfg.CreateMap<TblMasterCurrency, CurrencyView>().ForMember(
                dst => dst.DateTimeCreated,
                opt => opt.MapFrom(s => s.DatetimeCreated.HasValue ? s.DatetimeCreated.Value.UtcDateTime : (DateTime?)null)
            ).ForMember(
                dst => dst.DateTimeModify,
                opt => opt.MapFrom(s => s.DatetimeModified.HasValue ? s.DatetimeModified.Value.UtcDateTime : (DateTime?)null)
            ).ForMember(
                dst => dst.UserModify,
                opt => opt.MapFrom(s => s.UserModifed)
            ).ForMember(
                dst => dst.UserCreated,
                opt => opt.MapFrom(s => s.UserCreated)
            );
            cfg.CreateMap<AuthenLoginResult, DataStorage>().ForMember(
                dst => dst.UserGuid,
                opt => opt.MapFrom(s => s.GuidMasterUser)
            ).ForMember(
                dst => dst.UserLanguageGuid,
                opt => opt.MapFrom(s => s.SystemLanguage_Guid)
            );

            cfg.CreateMap<TblMasterCurrency, Nautilus_CurrencyView>().ForMember(
                dst => dst.DateTimeCreated,
                opt => opt.MapFrom(s => s.DatetimeCreated.HasValue ? s.DatetimeCreated.Value.UtcDateTime : (DateTime?)null)
            ).ForMember(
                dst => dst.DateTimeModify,
                opt => opt.MapFrom(s => s.DatetimeModified.HasValue ? s.DatetimeModified.Value.UtcDateTime : (DateTime?)null)
            ).ForMember(
                dst => dst.UserModify,
                opt => opt.MapFrom(s => s.UserModifed)
            ).ForMember(
                dst => dst.UserCreated,
                opt => opt.MapFrom(s => s.UserCreated)
            ).ForMember(
                dst => dst.MasterCurrencyReportDisplay,
                opt => opt.MapFrom(s => s.MasterCurrencyReportDisplay)
            ).ForMember(
                dst => dst.MasterCurrencyDescription,
                opt => opt.MapFrom(s => s.MasterCurrencyDescription)
            );

            cfg.CreateMap<TblMasterSite, BrinksSiteView>();

            cfg.CreateMap<TblMasterCommodity, CommodityView>().ForMember(
                dst => dst.CommodityGroupName,
                opt => opt.MapFrom(s => s.TblMasterCommodityGroup == null ? "Old" : s.TblMasterCommodityGroup.CommodityGroupName)
            ).ForMember(
                dst => dst.ColumnInReport,
                opt => opt.MapFrom(s => s.ColumnInReport ?? "00")
            );

            cfg.CreateMap<TblMasterCommodityCountry, CommodityView>().ForMember(
                dst => dst.CommodityGroupName,
                opt => opt.MapFrom(s => s.TblMasterCommodity == null ? "Old" : s.TblMasterCommodity.TblMasterCommodityGroup != null ? s.TblMasterCommodity.TblMasterCommodityGroup.CommodityGroupName : "Old")
            ).ForMember(
                dst => dst.ColumnInReport,
                opt => opt.MapFrom(s => s.ColumnInReport ?? "00")
            ).ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => s.MasterCommodity_Guid)
            ).ForMember(
                dst => dst.CommodityName,
                opt => opt.MapFrom(s => s.CommodityNameLocal)
            );

            cfg.CreateMap<TblMasterCountry, CountryView>().ForMember(
                   dst => dst.MasterCountry_Guid,
                   opt => opt.MapFrom(s => s.Guid)
                );
        }

        private static void ConfigMachine(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Up_OceanOnlineMVC_SFO_SearchMachine_Get_Result, MachineModel>();
            cfg.CreateMap<MachineView_ServiceHour, ResponseQueryMachine_ServiceHour>();
            cfg.CreateMap<MachineView, ResponseQueryMachine_Main>().ForMember(
                dst => dst.slaDetail,
                opt => opt.MapFrom(s => new ResponseQueryMachine_SLA
                {
                    compuSafeSlaTime = s.compuSafeSlaTime,
                    eCashSlaTime = s.eCashSlaTime,
                    flmSlaTime = s.flmSlaTime
                })
            ).ForMember(
                dst => dst.timeZoneDetail,
                opt => opt.MapFrom(s => new ResponseQueryMachine_Timezone
                {
                    countryTimeZoneId = s.countryTimeZoneId,
                    timeZoneDisplayName = s.timeZoneDisplayName,
                    timeZoneId = s.timeZoneId,
                    timeZoneIdentifier = s.timeZoneIdentifier,
                    timeZoneStandardName = s.timeZoneStandardName
                })
            );
        }

        private static void ConfigProblem(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SFOTblMasterProblem, ModelViews.Problem.ProblemView>();
        }

        private static void ConfigServiceRequest(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<QueryServiceRequestView, ResponseQueryServiceRequest>();
            cfg.CreateMap<SRRescheduleResponse, BaseResponse>();
        }

        private static void ConfigRunControl(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ValidateCrewOnPortalView, ValidateCrewOnPortalResponse_Main>();
        }

        private static void ConfigStandardTable(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<MachineServiceTypeView, ResponseQueryMachineServiceType_Main>();
            cfg.CreateMap<MachineSubServiceTypeView, ResponseQueryMachineSubServiceType_Main>();
            cfg.CreateMap<ProblemView, ResponseQueryProblem_Main>();
            cfg.CreateMap<ProblemView_SubProblem, ResponseQueryProblem_SubProblem>();
            cfg.CreateMap<ProblemPriorityView, ResponseQueryProblemPriority_Main>();
            cfg.CreateMap<ReasonCodeView, ResponseQueryReasonCode_Main>();
            cfg.CreateMap<RouteGroupView, ResponseQueryRouteGroup_Main>();
            cfg.CreateMap<RunResourceTypeView, ResponseQueryRunResourceType_Main>();
            cfg.CreateMap<RunResourceTypeView, ResponseQueryRunResourceType_Main>();
            #region site Path
            cfg.CreateMap<Models.StandardTable.SitePathView, SitePathViewResponse>();

            cfg.CreateMap<SitePathViewRequest, Models.StandardTable.SitePathView>();

            cfg.CreateMap<TblMasterSitePathAuditLog, SitePathAuditLogView>().ForMember(
                dst => dst.AuditLogGuid,
                opt => opt.MapFrom(s => s.Guid)
            ).ForMember(
                dst => dst.MasterSitePathGuid,
                opt => opt.MapFrom(s => s.MasterSitePathHeader_Guid)
            ).ForMember(
                dst => dst.Message,
                opt => opt.MapFrom(s => s.MsgParameter)
            ).ForMember(
                dst => dst.MsgID,
                opt => opt.MapFrom(s => s.MsgID)
            ).ForMember(
                dst => dst.DatetimeCreated,
                opt => opt.MapFrom(s => s.DatetimeCreted)
            ).ForMember(
                dst => dst.UserCreated,
                opt => opt.MapFrom(s => s.UserCreated)
            );
            #endregion
        }

        private static void ConfigPricingRule(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblPricingRule, SyncPricingRuleView>().ForMember(
               dst => dst.TblChargeCategory,
               opt => opt.MapFrom(s => s.TblChargeCategory.ConvertToSyncChargeCategoryView())
            );
            cfg.CreateMap<TblChargeCategory, SyncChargeCategoryView>().ForMember(
               dst => dst.TblChargeCategory_Rule,
               opt => opt.MapFrom(s => s.TblChargeCategory_Rule.ConvertToSyncChargeCategory_RuleView())
            ).ForMember(
               dst => dst.TblChargeCategory_Action,
               opt => opt.MapFrom(s => s.TblChargeCategory_Action.ConvertToSyncChargeCategory_ActionView())
            );
            cfg.CreateMap<TblChargeCategory_Action, SyncChargeCategory_ActionView>().ForMember(
               dst => dst.TblChargeCategory_Action_Charge,
               opt => opt.MapFrom(s => s.TblChargeCategory_Action_Charge.ConvertToSyncChargeCategory_Action_ChargeView())
            );
            cfg.CreateMap<TblChargeCategory_Rule, SyncChargeCategory_RuleView>().ForMember(
               dst => dst.TblChargeCategory_Rule_Value,
               opt => opt.MapFrom(s => s.TblChargeCategory_Rule_Value.ConvertToSyncChargeCategory_Rule_ValueView())
            );
            cfg.CreateMap<TblChargeCategory_Rule_Value, SyncChargeCategory_Rule_ValueView>();
            cfg.CreateMap<TblChargeCategory_Action_Charge, SyncChargeCategory_Action_ChargeView>().ForMember(
               dst => dst.TblChargeCategory_Action_Charge_Condition,
               opt => opt.MapFrom(s => s.TblChargeCategory_Action_Charge_Condition.ConvertToSyncChargeCategory_Action_Charge_ConditionView())
            );
            cfg.CreateMap<TblChargeCategory_Action_Charge_Condition, SyncChargeCategory_Action_Charge_ConditionView>();


            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute, TblLeedToCashProduct_ProductAttribute>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            ).ForMember(
                dst => dst.IsCopyData,
                opt => opt.MapFrom(s => true)
            );

            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_Certification, TblLeedToCashProduct_ProductAttribute_Certification>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_Discrepancy, TblLeedToCashProduct_ProductAttribute_Discrepancy>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_GeoLocation_City, TblLeedToCashProduct_ProductAttribute_GeoLocation_City>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_Info, TblLeedToCashProduct_ProductAttribute_Info>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_Limit_Items, TblLeedToCashProduct_ProductAttribute_Limit_Items>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_Limit_Values, TblLeedToCashProduct_ProductAttribute_Limit_Values>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_Limit_Weight, TblLeedToCashProduct_ProductAttribute_Limit_Weight>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_ResponseTime, TblLeedToCashProduct_ProductAttribute_ResponseTime>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_ServiceWindows, TblLeedToCashProduct_ProductAttribute_ServiceWindows>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_TimeDeadLine, TblLeedToCashProduct_ProductAttribute_TimeDeadLine>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_TimeSendReport, TblLeedToCashProduct_ProductAttribute_TimeSendReport>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail, TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );

            cfg.CreateMap<TblLeedToCashProduct, TblLeedToCashProduct>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            ).ForMember(
                dst => dst.IsCopyData,
                opt => opt.MapFrom(s => true)
            ).ForMember(
                dst => dst.TblPricingRule,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.TblLeedToCashProduct_ProductAttribute,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.UserCreated,
                opt => opt.MapFrom(s => "Bot Copied")
            ).ForMember(
                dst => dst.UserModifed,
                opt => opt.MapFrom(s => "Bot Copied")
            );

            cfg.CreateMap<TblLeedToCashProduct_ServiceType, TblLeedToCashProduct_ServiceType>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );

            cfg.CreateMap<TblLeedToCashProduct_Clause, TblLeedToCashProduct_Clause>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );

            cfg.CreateMap<TblLeedToCashQuotation_Customer_Mapping, TblLeedToCashQuotation_Customer_Mapping>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );

            cfg.CreateMap<TblLeedToCashQuotation_Location_Mapping, TblLeedToCashQuotation_Location_Mapping>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );

            cfg.CreateMap<TblPricingRule, TblPricingRule>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            ).ForMember(
                dst => dst.UserCreated,
                opt => opt.MapFrom(s => "Bot Copied")
            ).ForMember(
                dst => dst.UserModified,
                opt => opt.MapFrom(s => "Bot Copied")
            ).ForMember(
                dst => dst.MasterPricingRule_Guid,
                opt => opt.MapFrom(s => s.MasterPricingRule_Guid)
            ).ForMember(
                dst => dst.TblLeedToCashContract_PricingRules,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.TblLeedToCashContract_PricingRules1,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.TblLeedToCashQuotation_PricingRule_Mapping,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.TblLeedToCashQuotation_PricingRule_Mapping1,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.TblLeedToCashQuotation_Customer_Mapping,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.TblLeedToCashQuotation_Location_Mapping,
                opt => opt.Ignore()
            ).ForMember(
                dst => dst.TblMasterCustomerContract_ServiceLocation,
                opt => opt.Ignore()
            );

            cfg.CreateMap<TblChargeCategory, TblChargeCategory>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblChargeCategory_Action, TblChargeCategory_Action>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblChargeCategory_Action_Charge, TblChargeCategory_Action_Charge>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblChargeCategory_Action_Charge_Condition, TblChargeCategory_Action_Charge_Condition>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblChargeCategory_Rule, TblChargeCategory_Rule>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );
            cfg.CreateMap<TblChargeCategory_Rule_Value, TblChargeCategory_Rule_Value>().ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(s => Guid.NewGuid())
            );

            cfg.CreateMap<TblPricingRule, PricingRuleView>().ForMember(
                dst => dst.ProductID,
                opt => opt.MapFrom(s => s.TblLeedToCashProduct.ProductID)
            ).ForMember(
                dst => dst.ProductName,
                opt => opt.MapFrom(s => s.TblLeedToCashProduct.ProductName)
            ).ForMember(
                dst => dst.Product_Guid,
                opt => opt.MapFrom(s => s.TblLeedToCashProduct.Guid)
            ).ForMember(
                dst => dst.ProductDescription,
                opt => opt.MapFrom(s => s.TblLeedToCashProduct.Description)
            ).ForMember(
                dst => dst.MasterCurrencyAbbreviation,
                opt => opt.MapFrom(s => s.TblMasterCurrency.MasterCurrencyAbbreviation)
            ).ForMember(
                dst => dst.MasterCurrency_Guid,
                opt => opt.MapFrom(s => s.MasterCurrency_Guid)
            ).ForMember(
                dst => dst.SystemLineOfBusiness_Guid,
                opt => opt.MapFrom(s => s.TblLeedToCashProduct.SystemLineOfBusiness_Guid)
            );

            cfg.CreateMap<TblLeedToCashQuotation_PricingRule_Mapping, QuotationMappingPricingView>().ForMember(
                dst => dst.PricingRuleView,
                opt => opt.MapFrom(s => s.TblPricingRule.ConvertToPricingRuleView())
            ).ForMember(
                dst => dst.FlagRateChanged,
                opt => opt.MapFrom(s => s.FlagRateChanged)
            ).ForMember(
                dst => dst.FlagExceed,
                opt => opt.MapFrom(s => s.FlagExcess)
            );

            cfg.CreateMap<TblChargeCategory, ChargeCategoryView>();
            cfg.CreateMap<TblChargeCategory_Rule, RuleView>().ForMember(
                dst => dst.ValueList,
                opt => opt.MapFrom(s => s.TblChargeCategory_Rule_Value.Select(e => new RuleValueView()
                {
                    SeqNo = e.SeqNo,
                    Value = e.Value
                }))
            );

            cfg.CreateMap<TblChargeCategory_Action, ActionView>().ForMember(
                dst => dst.Action_ChargeList,
                opt => opt.MapFrom(s => s.TblChargeCategory_Action_Charge.ConvertToAction_ChargeView().OrderBy(e => e.SeqNo))
            ).ForMember(
                dst => dst.ChargeCategory_Guid,
                opt => opt.MapFrom(s => s.ChargeCategory_Guid)
            );

            cfg.CreateMap<TblChargeCategory_Action_Charge, Action_ChargeView>().ForMember(
                dst => dst.Action_Charge_ConditionList,
                opt => opt.MapFrom(s => s.TblChargeCategory_Action_Charge_Condition.ConvertToAction_Charge_ConditionView().OrderBy(e => e.SeqNo))
            ).ForMember(
                dst => dst.Action_Guid,
                opt => opt.MapFrom(s => s.ChargeCategory_Action_Guid)
            );

            cfg.CreateMap<TblChargeCategory_Action_Charge_Condition, Action_Charge_ConditionView>().ForMember(
                dst => dst.Action_Charge_Guid,
                opt => opt.MapFrom(s => s.ChargeCategory_Action_Charge_Guid)
            );

            cfg.CreateMap<TblLeedToCashProduct, ProductTreeView>().ForMember(
                dst => dst.CashStatusID,
                opt => opt.MapFrom(s => s.TblSystemLeedToCashStatus.StatusID)
            ).ForMember(
                dst => dst.Description,
                opt => opt.MapFrom(s => s.Description)
            ).ForMember(
                dst => dst.ValidFrom,
                opt => opt.MapFrom(s => s.ValidFrom.ToString())
            ).ForMember(
                dst => dst.ValidTo,
                opt => opt.MapFrom(s => s.ValidTo.ToString())
            ).ForMember(
                dst => dst.BrinksCompany_Guid,
                opt => opt.MapFrom(s => s.MasterCustomer_Guid)
            ).ForMember(
                dst => dst.PricingRuleList,
                opt => opt.MapFrom(s => s.TblPricingRule.ConvertToPricingRuleView().OrderBy(e => e.SeqNo))
            ).ForMember(
                dst => dst.ParentProduct_Guid,
                opt => opt.MapFrom(s => s.Product_Guid)
            ).ForMember(
                dst => dst.SystemCashStatus_Guid,
                opt => opt.MapFrom(s => s.SystemLeedToCashStatus_Guid)
            ).ForMember(
                dst => dst.ServiceJobTypeGuids,
                opt => opt.MapFrom(s => s.TblLeedToCashProduct_ServiceType.Select(e => e.SystemServiceJobType_Guid))
            );
        }

        #region ## Adhoc config
        private static void ConfigAdhoc(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<AdhocCustomerResult, AdhocCustomerView>();
            cfg.CreateMap<AdhocLocationByCustomerResult, AdhocCustomerLocationView>();
            cfg.CreateMap<CustomerLocationInternalDepartmentModel, CustomerLocationInternalDepartmentView>();
            cfg.CreateMap<Models.RunControl.JobLegsView, JobLegsViewResult>();

            #region ##Cash Add
            cfg.CreateMap<CassetteModelView, TblMasterActualJobActualCount>().ForMember(
                dst => dst.MasterActualJobSumActualCount_Guid,
                opt => opt.MapFrom(s => s.SumHeadGuid)
            );
            cfg.CreateMap<CassetteModelView, TblMasterActualJobCashAdd>().ForMember(
                dst => dst.MasterActualJobSumCashAdd_Guid,
                opt => opt.MapFrom(s => s.SumHeadGuid)
            );
            cfg.CreateMap<CassetteModelView, TblMasterActualJobCashReturn>().ForMember(
                dst => dst.MasterActualJobSumCashReturn_Guid,
                opt => opt.MapFrom(s => s.SumHeadGuid)
            );
            cfg.CreateMap<CassetteModelView, TblMasterActualJobMachineReport>().ForMember(
                dst => dst.MasterActualJobSumMachineReport_Guid,
                opt => opt.MapFrom(s => s.SumHeadGuid)
            );

            #endregion
        }
        #endregion

        #region ## Customer cofign.
        private static void ConfigCustomer(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<RequestAdhocCustomer, AdhocCustomerRequest>().ReverseMap();
        }
        #endregion

        #region Nemo configuration
        private static void ConfigNemoConfig(IMapperConfigurationExpression cfg)
        {
            #region Convert To NemoCountryValueView
            cfg.CreateMap<TblMasterNemoCountryValue, NemoCountryValueView>()
            .ForMember(
                dst => dst.LunchTime,
                opt => opt.MapFrom(o => o.LunchTime.GetValueOrDefault().ToString("HH:mm"))
            ).ForMember(
                dst => dst.MaxLiability,
                opt => opt.MapFrom(o => o.MaxLiability.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxOverlapDistance,
                opt => opt.MapFrom(s => s.MaxOverlapDistance.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxCapacity,
                opt => opt.MapFrom(s => s.MaxCapacity.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxVolumn,
                opt => opt.MapFrom(s => s.MaxVolumn.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxDistanceBetweenStop,
                opt => opt.MapFrom(s => s.MaxDistanceBetweenStop.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxRunTotalDistance,
                opt => opt.MapFrom(s => s.MaxRunTotalDistance.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.TurnAroundLeadTime,
                opt => opt.MapFrom(s => s.TurnAroundLeadTime.GetValueOrDefault().ToString("N2"))
            );
            #endregion

            #region Convert To TblMasterNemoCountryValue
            cfg.CreateMap<NemoCountryValueView, TblMasterNemoCountryValue>()
            .ForMember(
                dst => dst.LunchTime,
                opt => opt.MapFrom(o => string.IsNullOrEmpty(o.LunchTime) ? "12:00".ToTimeDateTime() : o.LunchTime.ToTimeDateTime())
            ).ForMember(
                dst => dst.MaxLiability,
                opt => opt.MapFrom(o => decimal.Parse(o.MaxLiability))
            ).ForMember(
                dst => dst.MaxOverlapDistance,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxOverlapDistance) ? 0.00 : double.Parse(s.MaxOverlapDistance))
            ).ForMember(
                dst => dst.MaxCapacity,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxCapacity) ? 0.00 : double.Parse(s.MaxCapacity))
            ).ForMember(
                dst => dst.MaxVolumn,
                opt => opt.MapFrom(s => decimal.Parse(s.MaxVolumn))
            ).ForMember(
                dst => dst.MaxDistanceBetweenStop,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxDistanceBetweenStop) ? 0.00 : double.Parse(s.MaxDistanceBetweenStop))
            ).ForMember(
                dst => dst.MaxRunTotalDistance,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxRunTotalDistance) ? 0.00 : double.Parse(s.MaxRunTotalDistance))
            ).ForMember(
                dst => dst.TurnAroundLeadTime,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.TurnAroundLeadTime) ? 0.00 : double.Parse(s.TurnAroundLeadTime))
            );
            #endregion

            #region Convert to NemoSiteValueView
            cfg.CreateMap<TblMasterNemoSiteValue, NemoSiteValueView>()
            .ForMember(
                dst => dst.LunchTime,
                opt => opt.MapFrom(o => o.LunchTime.GetValueOrDefault().ToString("HH:mm"))
            ).ForMember(
                dst => dst.MaxLiability,
                opt => opt.MapFrom(o => o.MaxLiability.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxOverlapDistance,
                opt => opt.MapFrom(s => s.MaxOverlapDistance.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxCapacity,
                opt => opt.MapFrom(s => s.MaxCapacity.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxVolumn,
                opt => opt.MapFrom(s => s.MaxVolumn.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxDistanceBetweenStop,
                opt => opt.MapFrom(s => s.MaxDistanceBetweenStop.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.MaxRunTotalDistance,
                opt => opt.MapFrom(s => s.MaxRunTotalDistance.GetValueOrDefault().ToString("N2"))
            ).ForMember(
                dst => dst.TurnAroundLeadTime,
                opt => opt.MapFrom(s => s.TurnAroundLeadTime.GetValueOrDefault().ToString("N2"))
            );
            #endregion

            #region Convert to TblMasterNemoSiteValue
            cfg.CreateMap<NemoSiteValueView, TblMasterNemoSiteValue>()
            .ForMember(
                dst => dst.LunchTime,
                opt => opt.MapFrom(o => string.IsNullOrEmpty(o.LunchTime) ? "12:00".ToTimeDateTime() : o.LunchTime.ToTimeDateTime())
            ).ForMember(
                dst => dst.MaxLiability,
                opt => opt.MapFrom(o => decimal.Parse(o.MaxLiability))
            ).ForMember(
                dst => dst.MaxOverlapDistance,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxOverlapDistance) ? 0.00 : double.Parse(s.MaxOverlapDistance))
            ).ForMember(
                dst => dst.MaxCapacity,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxCapacity) ? 0.00 : double.Parse(s.MaxCapacity))
            ).ForMember(
                dst => dst.MaxVolumn,
                opt => opt.MapFrom(s => decimal.Parse(s.MaxVolumn))
            ).ForMember(
                dst => dst.MaxDistanceBetweenStop,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxDistanceBetweenStop) ? 0.00 : double.Parse(s.MaxDistanceBetweenStop))
            ).ForMember(
                dst => dst.MaxRunTotalDistance,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.MaxRunTotalDistance) ? 0.00 : double.Parse(s.MaxRunTotalDistance))
            ).ForMember(
                dst => dst.TurnAroundLeadTime,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.TurnAroundLeadTime) ? 0.00 : double.Parse(s.TurnAroundLeadTime))
            );
            #endregion

            #region Convert to NemoTrafficFactorValueView
            cfg.CreateMap<TblMasterNemoTrafficFactorValue, NemoTrafficFactorValueView>()
            .ForMember(
                dst => dst.StartTime,
                opt => opt.MapFrom(o => o.StartTime.GetValueOrDefault().ToString("HH:mm"))
            ).ForMember(
                dst => dst.EndTime,
                opt => opt.MapFrom(o => o.EndTime.GetValueOrDefault().ToString("HH:mm"))
            ).ForMember(
                dst => dst.TrafficMultiplier,
                opt => opt.MapFrom(s => s.TrafficMultiplier.GetValueOrDefault().ToString("N2"))
            );

            cfg.CreateMap<NemoTrafficFactorValueView, TblMasterNemoTrafficFactorValue>().ForMember(
                dst => dst.DayOfWeekName,
                opt => opt.MapFrom(o => o.DayOfWeekName.ToString())
            ).ForMember(
                dst => dst.StartTime,
                opt => opt.MapFrom(o => string.IsNullOrEmpty(o.StartTime) ? "00:00".ToTimeDateTime() : o.StartTime.ToTimeDateTime())
            ).ForMember(
                dst => dst.EndTime,
                opt => opt.MapFrom(o => string.IsNullOrEmpty(o.EndTime) ? "00:00".ToTimeDateTime() : o.EndTime.ToTimeDateTime())
            ).ForMember(
                dst => dst.TrafficMultiplier,
                opt => opt.MapFrom(s => string.IsNullOrEmpty(s.TrafficMultiplier) ? 0.00 : double.Parse(s.TrafficMultiplier))
            );
            #endregion

            cfg.CreateMap<NemoApplyToSiteRequest, NemoApplyToSiteView>();
        }
        #endregion

        #region Nemo Sync
        private static void ConfigNemoSync(IMapperConfigurationExpression cfg)
        {
            #region Service Job Types
            cfg.CreateMap<TblSystemServiceJobType, SyncSystemServiceJobTypeRequest>()
            .ForMember(
                dst => dst.SystemServiceJobTypeGuid,
                opt => opt.MapFrom(o => o.Guid)
            ).ForMember(
                dst => dst.Name,
                opt => opt.MapFrom(o => o.ServiceJobTypeName)
            ).ForMember(
                dst => dst.Code,
                opt => opt.MapFrom(s => s.ServiceJobTypeNameAbb)
            );
            #endregion
        }
        #endregion

        #region System Configuration Additional
        private static void Config_DomainEmail(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblSystemDomainEmailWhiteList, EmailDomainsView>().ForMember(
                    dst => dst.AllowedDomain_Guid,
                    opt => opt.MapFrom(s => s.Guid)
            ).ForMember(
                    dst => dst.AllowedDomain,
                    opt => opt.MapFrom(s => s.DomainEmail)
            );

            cfg.CreateMap<EmailDomainsView, TblSystemDomainEmailWhiteList>().ForMember(
                    dst => dst.Guid,
                    opt => opt.MapFrom(s => s.AllowedDomain_Guid)
            ).ForMember(
                    dst => dst.DomainEmail,
                    opt => opt.MapFrom(s => s.AllowedDomain)
            );

            cfg.CreateMap<EmailDomainsView, SystemEmailDomainsView>().ForMember(
                    dst => dst.AllowedDomain_Guid,
                    opt => opt.MapFrom(s => s.AllowedDomain_Guid)
            ).ForMember(
                    dst => dst.AllowedDomain,
                    opt => opt.MapFrom(s => s.AllowedDomain)
            );

            cfg.CreateMap<TblSystemDomainEmailWhiteList, SystemEmailDomainsView>().ForMember(
                    dst => dst.AllowedDomain_Guid,
                    opt => opt.MapFrom(s => s.Guid)
            ).ForMember(
                    dst => dst.AllowedDomain,
                    opt => opt.MapFrom(s => s.DomainEmail)
            );
        }

        private static void Config_SystemEnviromentGlobal(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblSystemEnvironment_Global, SystemEnvironmentView>()
                .ForMember(
                    dst => dst.SystemEnvironmentGuid,
                    opt => opt.MapFrom(s => s.Guid)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppKey,
                    opt => opt.MapFrom(s => s.AppKey)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppDescription,
                    opt => opt.MapFrom(s => s.AppDescription)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue1,
                    opt => opt.MapFrom(s => s.AppValue1)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue2,
                    opt => opt.MapFrom(s => s.AppValue2)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue3,
                    opt => opt.MapFrom(s => s.AppValue3)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue4,
                    opt => opt.MapFrom(s => s.AppValue4)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue5,
                    opt => opt.MapFrom(s => s.AppValue5)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue6,
                    opt => opt.MapFrom(s => s.AppValue6)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue7,
                    opt => opt.MapFrom(s => s.AppValue7)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue8,
                    opt => opt.MapFrom(s => s.AppValue8)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue9,
                    opt => opt.MapFrom(s => s.AppValue9)
            );

            cfg.CreateMap<SystemEnvironmentView, TblSystemEnvironment_Global>()
                .ForMember(
                    dst => dst.Guid,
                    opt => opt.MapFrom(s => s.SystemEnvironmentGuid)
                ).ForMember(
                    dst => dst.AppKey,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppKey)
                ).ForMember(
                    dst => dst.AppDescription,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppDescription)
                ).ForMember(
                    dst => dst.AppValue1,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue1 == "" ? null : s.SystemEnvironmentAppValue1)
                ).ForMember(
                    dst => dst.AppValue2,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue2 == "" ? null : s.SystemEnvironmentAppValue2)
                ).ForMember(
                    dst => dst.AppValue3,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue3 == "" ? null : s.SystemEnvironmentAppValue3)
                ).ForMember(
                    dst => dst.AppValue4,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue4 == "" ? null : s.SystemEnvironmentAppValue4)
                ).ForMember(
                    dst => dst.AppValue5,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue5 == "" ? null : s.SystemEnvironmentAppValue5)
                ).ForMember(
                    dst => dst.AppValue6,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue6 == "" ? null : s.SystemEnvironmentAppValue6)
                ).ForMember(
                    dst => dst.AppValue7,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue7 == "" ? null : s.SystemEnvironmentAppValue7)
                ).ForMember(
                    dst => dst.AppValue8,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue8 == "" ? null : s.SystemEnvironmentAppValue8)
                ).ForMember(
                    dst => dst.AppValue9,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue9 == "" ? null : s.SystemEnvironmentAppValue9)
            );

            cfg.CreateMap<SystemEnvironmentView, SystemEnvironmentGlobalView>()
                .ForMember(
                    dst => dst.SystemEnvironmentGuid,
                    opt => opt.MapFrom(s => s.SystemEnvironmentGuid)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppKey,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppKey)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppDescription,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppDescription)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue1,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue1 == "" ? null : s.SystemEnvironmentAppValue1)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue2,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue2 == "" ? null : s.SystemEnvironmentAppValue2)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue3,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue3 == "" ? null : s.SystemEnvironmentAppValue3)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue4,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue4 == "" ? null : s.SystemEnvironmentAppValue4)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue5,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue5 == "" ? null : s.SystemEnvironmentAppValue5)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue6,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue6 == "" ? null : s.SystemEnvironmentAppValue6)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue7,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue7 == "" ? null : s.SystemEnvironmentAppValue7)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue8,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue8 == "" ? null : s.SystemEnvironmentAppValue8)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue9,
                    opt => opt.MapFrom(s => s.SystemEnvironmentAppValue9 == "" ? null : s.SystemEnvironmentAppValue9)
            );

            cfg.CreateMap<TblSystemEnvironment_Global, SystemEnvironmentGlobalView>()
                .ForMember(
                    dst => dst.SystemEnvironmentGuid,
                    opt => opt.MapFrom(s => s.Guid)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppKey,
                    opt => opt.MapFrom(s => s.AppKey)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppDescription,
                    opt => opt.MapFrom(s => s.AppDescription)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue1,
                    opt => opt.MapFrom(s => s.AppValue1)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue2,
                    opt => opt.MapFrom(s => s.AppValue2)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue3,
                    opt => opt.MapFrom(s => s.AppValue3)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue4,
                    opt => opt.MapFrom(s => s.AppValue4)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue5,
                    opt => opt.MapFrom(s => s.AppValue5)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue6,
                    opt => opt.MapFrom(s => s.AppValue6)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue7,
                    opt => opt.MapFrom(s => s.AppValue7)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue8,
                    opt => opt.MapFrom(s => s.AppValue8)
                ).ForMember(
                    dst => dst.SystemEnvironmentAppValue9,
                    opt => opt.MapFrom(s => s.AppValue9)
            );

        }

        private static void Config_PreDefinedEmails(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblMasterUserAccessGroupCountryEmail, PreDefinedEmailsView>().ForMember(
                    dst => dst.UserAccessGroupCountryEmailGuid,
                    opt => opt.MapFrom(s => s.Guid)
            ).ForMember(
                    dst => dst.CountryGuid,
                    opt => opt.MapFrom(s => s.MasterCountry_Guid)
            ).ForMember(
                    dst => dst.AllowedEmailList,
                    opt => opt.MapFrom(s => s.EmailList)
            );

            cfg.CreateMap<PreDefinedEmailsView, TblMasterUserAccessGroupCountryEmail>().ForMember(
                    dst => dst.Guid,
                    opt => opt.MapFrom(s => s.UserAccessGroupCountryEmailGuid)
            ).ForMember(
                    dst => dst.MasterCountry_Guid,
                    opt => opt.MapFrom(s => s.CountryGuid)
            ).ForMember(
                    dst => dst.EmailList,
                    opt => opt.MapFrom(s => s.AllowedEmailList)
            );

            cfg.CreateMap<PreDefinedEmailsView, SystemPreDefinedEmailsView>().ForMember(
                    dst => dst.UserAccessGroupCountryEmailGuid,
                    opt => opt.MapFrom(s => s.UserAccessGroupCountryEmailGuid)
            ).ForMember(
                    dst => dst.CountryGuid,
                    opt => opt.MapFrom(s => s.CountryGuid)
            ).ForMember(
                    dst => dst.AllowedEmailList,
                    opt => opt.MapFrom(s => s.AllowedEmailList)
            );

            cfg.CreateMap<TblMasterUserAccessGroupCountryEmail, SystemPreDefinedEmailsView>().ForMember(
                    dst => dst.UserAccessGroupCountryEmailGuid,
                    opt => opt.MapFrom(s => s.Guid)
            ).ForMember(
                    dst => dst.CountryGuid,
                    opt => opt.MapFrom(s => s.MasterCountry_Guid)
            ).ForMember(
                    dst => dst.AllowedEmailList,
                    opt => opt.MapFrom(s => s.EmailList)
            );
        }

        private static void Config_NotificationConfigPeriods(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SystemNotificationConfigPeriodsView, NotificationConfigPeriodsView>().ForMember(
                    dst => dst.NotificationConfigPeriodsGuid,
                    opt => opt.MapFrom(s => s.NotificationConfigPeriodsGuid)
            ).ForMember(
                    dst => dst.InitialDate,
                    opt => opt.MapFrom(s => s.InitialDate)
            ).ForMember(
                    dst => dst.FinalDate,
                    opt => opt.MapFrom(s => s.FinalDate)
            ).ForMember(
                    dst => dst.DaysBeforeDueDate,
                    opt => opt.MapFrom(s => s.DaysBeforeDueDate)
            ).ForMember(
                    dst => dst.NotifyBeforeDueDate,
                    opt => opt.MapFrom(s => s.NotifyBeforeDueDate)
            ).ForMember(
                    dst => dst.Country,
                    opt => opt.MapFrom(s => s.Country)
            ).ForMember(
                    dst => dst.PeriodTitle,
                    opt => opt.MapFrom(s => s.PeriodTitle)
            ).ForMember(
                    dst => dst.UserCreated,
                    opt => opt.MapFrom(s => s.UserCreated)
            ).ForMember(
                    dst => dst.UserModified,
                    opt => opt.MapFrom(s => s.UserModifed)
            ).ForMember(
                    dst => dst.FlagDisable,
                    opt => opt.MapFrom(s => s.FlagDisable)
             ).ForMember(
                    dst => dst.SystemEnvironmentMasterCountry_Guid,
                    opt => opt.MapFrom(s => s.SystemEnvironmentMasterCountry_Guid)
            );

            cfg.CreateMap<TblSystemNotificationConfigPeriods, SystemNotificationConfigPeriodsView>().ForMember(
                    dst => dst.NotificationConfigPeriodsGuid,
                    opt => opt.MapFrom(s => s.Guid)
            ).ForMember(
                    dst => dst.InitialDate,
                    opt => opt.MapFrom(s => s.StartDate)
            ).ForMember(
                    dst => dst.FinalDate,
                    opt => opt.MapFrom(s => s.EndDate)
            ).ForMember(
                    dst => dst.Country,
                    opt => opt.MapFrom(s => s.MasterCountry_Guid)
            ).ForMember(
                    dst => dst.PeriodTitle,
                    opt => opt.MapFrom(s => s.PeriodTitle)
            ).ForMember(
                    dst => dst.UserCreated,
                    opt => opt.MapFrom(s => s.UserCreated)
            ).ForMember(
                    dst => dst.UserModifed,
                    opt => opt.MapFrom(s => s.UserModifed)
            ).ForMember(
                    dst => dst.FlagDisable,
                    opt => opt.MapFrom(s => s.FlagDisable)
             ).ForMember(
                    dst => dst.SystemEnvironmentMasterCountry_Guid,
                    opt => opt.MapFrom(s => s.SystemEnvironmentMasterCountry_Guid)
            );

            cfg.CreateMap<NotificationConfigPeriodsUsers, TblSystemNotificationConfigPeriodsUsers>().ForMember(
                    dst => dst.Guid,
                    opt => opt.MapFrom(s => s.GlobalUserGuid)
            ).ForMember(
                    dst => dst.SystemNotificationConfigPeriods_Guid,
                    opt => opt.MapFrom(s => s.SystemNotificationConfigPeriodsGuid)
            ).ForMember(
                    dst => dst.EscalationEmail,
                    opt => opt.MapFrom(s => s.Email)
            ).ForMember(
                    dst => dst.UserCreated,
                    opt => opt.MapFrom(s => s.UserCreated)
            ).ForMember(
                    dst => dst.FlagDisable,
                    opt => opt.MapFrom(s => s.FlagDisable)
            ).ForMember(
                    dst => dst.UserName,
                    opt => opt.MapFrom(s => s.UserName)
            ).ForMember(
                    dst => dst.ExternalUser,
                    opt => opt.MapFrom(s => s.IsExternalGlobalAdmin)
            );
        }

        private static void Config_LogAudit(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblSystemConfigurationAuditLog, ConfigurationAuditLogView>()
                .ForMember(
                    dst => dst.LogGuid,
                    opt => opt.MapFrom(s => s.Guid)
                ).ForMember(
                    dst => dst.EventID,
                    opt => opt.MapFrom(s => s.SystemEvet_ID)
                ).ForMember(
                    dst => dst.EventType,
                    opt => opt.MapFrom(s => s.Event)
                ).ForMember(
                    dst => dst.UserName,
                    opt => opt.MapFrom(s => s.UseID)
                ).ForMember(
                    dst => dst.ConfigType,
                    opt => opt.MapFrom(s => s.ConfigType)
                ).ForMember(
                    dst => dst.LogAppKey,
                    opt => opt.MapFrom(s => s.AppKey)
                ).ForMember(
                    dst => dst.LogAppDescription,
                    opt => opt.MapFrom(s => s.AppDescription)
                ).ForMember(
                    dst => dst.LogAppValue1,
                    opt => opt.MapFrom(s => s.AppValue1)
                ).ForMember(
                    dst => dst.LogAppValue2,
                    opt => opt.MapFrom(s => s.AppValue2)
                ).ForMember(
                    dst => dst.LogAppValue3,
                    opt => opt.MapFrom(s => s.AppValue3)
                ).ForMember(
                    dst => dst.LogAppValue4,
                    opt => opt.MapFrom(s => s.AppValue4)
                ).ForMember(
                    dst => dst.LogAppValue5,
                    opt => opt.MapFrom(s => s.AppValue5)
                ).ForMember(
                    dst => dst.LogAppValue6,
                    opt => opt.MapFrom(s => s.AppValue6)
                ).ForMember(
                    dst => dst.LogAppValue7,
                    opt => opt.MapFrom(s => s.AppValue7)
                ).ForMember(
                    dst => dst.LogAppValue8,
                    opt => opt.MapFrom(s => s.AppValue8)
                ).ForMember(
                    dst => dst.LogAppValue9,
                    opt => opt.MapFrom(s => s.AppValue9)
                ).ForMember(
                    dst => dst.Date,
                    opt => opt.MapFrom(s => s.DateTimeCreated)
            );
        }
        #endregion

        #region SiteNetwork
        private static void ConfigSiteNework(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SiteNetworkMemberView, SiteNetworkViewResponse>()
                .ForMember(
                    dst => dst.SiteGuid,
                    opt => opt.MapFrom(s => s.SiteGuid)
                ).ForMember(
                    dst => dst.SiteName,
                    opt => opt.MapFrom(s => s.SiteName)
                ).ForMember(
                    dst => dst.MasterCountryGuid,
                    opt => opt.MapFrom(s => s.MasterCountryGuid)
                ).ForMember(
                    dst => dst.MasterCountryName,
                    opt => opt.MapFrom(s => s.MasterCountryName)
                ).ForMember(
                    dst => dst.FlagDisable,
                    opt => opt.MapFrom(s => s.FlagDisable)
                ).ForMember(
                    dst => dst.BrinksSitelist,
                    opt => opt.MapFrom(s => s.BrinksSitelist)
                ).ForMember(
                    dst => dst.UserCreated,
                    opt => opt.MapFrom(s => s.UserCreated)
                ).ForMember(
                    dst => dst.DatetimeCreated,
                    opt => opt.MapFrom(s => s.DatetimeCreated)
                ).ForMember(
                    dst => dst.UserModified,
                    opt => opt.MapFrom(s => s.UserModified)
                ).ForMember(
                dst => dst.DatetimeModified,
                opt => opt.MapFrom(s => s.DatetimeModified)
            );

            cfg.CreateMap<TblMasterSiteNetworkAuditLog, SiteNetworkAuditLogView>()
                .ForMember(
                    dst => dst.AuditLogGuid,
                    opt => opt.MapFrom(s => s.Guid)
                ).ForMember(
                    dst => dst.MasterSiteNetworkGuid,
                    opt => opt.MapFrom(s => s.MasterSiteNetwork_Guid)
                ).ForMember(
                    dst => dst.MsgID,
                    opt => opt.MapFrom(s => s.MsgID)
                ).ForMember(
                    dst => dst.Message,
                    opt => opt.MapFrom(s => s.MsgParameter)
                ).ForMember(
                    dst => dst.UserCreated,
                    opt => opt.MapFrom(s => s.UserCreated)
                ).ForMember(
                    dst => dst.DatetimeCreated,
                    opt => opt.MapFrom(s => s.DatetimeCreated)
            );
        }
        #endregion

        #region Pre-Vault
        private static void ConfigPreVault(IMapperConfigurationExpression cfg)
        {
            #region Convert To InternalDepartmentView
            cfg.CreateMap<CustomerLocationInternalDepartmentModel, CheckOutDeptInternalDeptModelView>()
                .ForMember(
                    dst => dst.Id,
                    opt => opt.MapFrom(o => o.id)
                ).ForMember(
                    dst => dst.InternalDeptName,
                    opt => opt.MapFrom(o => o.text)
                );
            cfg.CreateMap<ConAvailableItemView, ConsolidateItemView>().ForMember(
                    dst => dst.Liability,
                    opt => opt.MapFrom(s => s.Liability ?? 0.00)
                ).ForMember(
                    dst => dst.Qty,
                    opt => opt.MapFrom(s => s.Qty ?? 0)
                );
            #endregion
        }

        #endregion

        #region Fleet maintenance
        private static void ConfigFleetMaintenance(IMapperConfigurationExpression cfg)
        {
            #region Convert To TblMasterRunResourceGasoline
            cfg.CreateMap<FleetGasolineDataRequest, TblMasterRunResource_GasolineExpense>()
            .ForMember(
                dst => dst.Guid,
                opt => opt.MapFrom(o => Guid.NewGuid())
            ).ForMember(
                dst => dst.CurrencyAmount_Guid,
                opt => opt.MapFrom(o => o.CurrencyUnit_Guid)
            ).ForMember(
                dst => dst.DocumentRef,
                opt => opt.MapFrom(o => o.DocumentRef.Trim())
            );
            #endregion
        }
        #endregion

        #region On Hand Route
        private static void ConfigOnHandRoute(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<JobDetailOnRunView, OnHandJobOnRunView>();
        }
        #endregion

        #region Smart Billing Monitoring
        private static void ConfigSmartBillingMonitoring(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TblMasterHistory_ReportPushToSmart, SmartBillingGenerateStatusView>()
            .ForMember(
                dst => dst.SmartBillingStatusGuid,
                opt => opt.MapFrom(s => s.Guid)
            ).ForMember(
                dst => dst.FileGenerating_Status,
                opt => opt.MapFrom(s => s.FlagAutoGen ? s.FlagAutoGenResult.SmartBillingStatusText() : SmartBillingGenerateStatus.NA)
            ).ForMember(
                dst => dst.EmailSending_Status,
                opt => opt.MapFrom(s => s.FlagEmailSending ? s.FlagEmailSendingResult.SmartBillingStatusText() : SmartBillingGenerateStatus.NA)
            ).ForMember(
                dst => dst.FileDropping_Status,
                opt => opt.MapFrom(s => s.FlagFileDroping ? s.FlagFileDropingResult.SmartBillingStatusText() : SmartBillingGenerateStatus.NA)
            ).ForMember(
                dst => dst.ReportName,
                opt => opt.MapFrom(s => s.AutoGenFileName ?? string.Empty)
            ).ForMember(
                dst => dst.Email,
                opt => opt.MapFrom(s => s.Email ?? string.Empty)
            ).ForMember(
                dst => dst.DropFilePath,
                opt => opt.MapFrom(s => s.DropFilePath ?? string.Empty)
            ).ForMember(
                dst => dst.Date,
                opt => opt.MapFrom(s => s.DatetimeCreated.GetValueOrDefault())
            ).ForMember(
                dst => dst.FlagGenerateStatus,
                opt => opt.MapFrom(s => s.FlagAutoGenResult)
            ).ForMember(
                dst => dst.FlagEmailSendingStatus,
                opt => opt.MapFrom(s => s.FlagEmailSendingResult)
            ).ForMember(
                dst => dst.FlagDroppingStatus,
                opt => opt.MapFrom(s => s.FlagFileDropingResult)
            );
        }

        private static string SmartBillingStatusText(this bool statusResult)
        {
            return statusResult ? SmartBillingGenerateStatus.Success : SmartBillingGenerateStatus.Failed;
        }
        #endregion

        #region Vault balance
        private static void ConfigVaultBalance(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<VaultBalanceSealModel, VaultBalanceSealModelView>();
        }
        #endregion

    }
}
