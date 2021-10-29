using Bgt.Ocean.Models.Masters;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RouteOptimization
{
    public class DdlViewModel
    {
        public Guid Value { get; set; }
        public string Text { get; set; }
    }

    public class RequestManagementResponse
    {
        public IEnumerable<RequestManagementView> RequestManagementList { get; set; }
        public int Total { get; set; }
    }
    public class RequestManagementView
    {
        public Guid Guid { get; set; }
        public string RequestID { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public string RouteTypeCode { get; set; }
        public string RouteType { get; set; }
        public string RequestType { get; set; }
        public DateTime? RequestDateTime { get; set; }
        public string RequestUser { get; set; }
    }

    public class RoadnetFileViewModel
    {
        public IEnumerable<RouteOptimizeRequestIdAndCountry> RouteOptimizeRequestIdAndCountry { get; set; }
        public IEnumerable<RoadnetFileDataModel> FileData { get; set; }
    }
    public class RouteOptimizeRequestIdAndCountry
    {
        public Guid RequestGuid { get; set; }
        public string RequestId { get; set; }
        public Guid CountryGuid { get; set; }
        public Guid? SiteGuid { get; set; }
        public string DirectoryPath { get; set; }
        public string FileName { get; set; }
    }
    public class RoadnetFileDataModel
    {
        public string Request_ID { get; set; }
        public string Request_Item_ID { get; set; }
        public string Request_Type { get; set; }
        public string Request_Scope { get; set; }
        public string Location_Sequence_Original { get; set; }
        public string Location_Sequence_RoadNet { get; set; }
        public string Week_Type { get; set; }
        public string Site_Code { get; set; }
        public string Site_Name { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public string WeekDay { get; set; }
        public string Route_Day { get; set; }
        public string Ready_Status { get; set; }
        public string Balanced_Status { get; set; }
        public string Route_Start_Time { get; set; }
        public string StopLocationCode { get; set; }
        public string StopLocationName { get; set; }
        public string Job_ID { get; set; }
        public string Job_Type { get; set; }
        public string Stop_Premise_Time { get; set; }
        public string Job_Premise_Time { get; set; }
        public string STC { get; set; }
        public string Place_Truck_Stop { get; set; }
        public string Address { get; set; }
        public string city { get; set; }
        public string StateName { get; set; }
        public string postal { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string WorkDate { get; set; }
        public string Week_Start_Sunday { get; set; }
        public string Week_End_Saturday { get; set; }
        public string TblMasterCustomerLocation_Guid { get; set; }

    }
    public class OptAdvMsgModel
    {
        public string Request_ID { get; set; }
        public string Request_Type { get; set; }
        public string Request_Date { get; set; }
        public string Request_File { get; set; }
        public string Requester_Name { get; set; }
        public string Request_Status { get; set; }
        public string Request_Comment { get; set; }
    }

    public class OptAdvResponseModel
    {
        public string Request_ID { get; set; }
        public string Request_Type { get; set; }
        public string Request_Date { get; set; }
        public string Request_File { get; set; }
        public string Requester_Name { get; set; }
        public string Request_Status { get; set; }
        public string Request_Comment { get; set; }
    }
    public class RoadnetDailyRouteRequestModel
    {
        public TblTransactionRouteOptimizationHeader_Detail_Item RouteOptimizeItem { get; set; }
        public string StrDayOfweek { get; set; }
        public ActualJobDetailViewModel ActualJobHeader { get; set; }
        public decimal Stc { get; set; }
        public string CurrencyAbb { get; set; }
        public string StrFirstDow { get; set; }
        public string StrLastDow { get; set; }
        public string MasterRouteGroupName { get; set; }
    }

    public class CommodityRequestModel
    {
        public Guid JobHeaderGuid { get; set; }
        public Guid SiteGuid { get; set; }

    }

    public class ActualJobDetailViewModel
    {
        public Guid JobHeaderGuid { get; set; }
        public string JobNo { get; set; }

    }

    public class StcViewModel
    {
        public Guid JobHeaderGuid { get; set; }
        public decimal Stc { get; set; }
    }

    public class RouteOptimizeSearchModel
    {
        public List<Guid> ActualJobHeaderGuid { get; set; } = new List<Guid>();
        public List<Guid> JobLegGuid { get; set; } = new List<Guid>();
        public List<Guid> RunResourceGuid { get; set; } = new List<Guid>();
        public List<MasterRouteGuidModel> MasterRouteModel { get; set; } = new List<MasterRouteGuidModel>();
        public List<Guid> CustomerGuid { get; set; } = new List<Guid>();
        public List<Guid> CustomerLocationGuid { get; set; } = new List<Guid>();
        public List<Guid> RouteGroupGuid { get; set; } = new List<Guid>();
        public List<Guid> RouteGroupDetailGuid { get; set; } = new List<Guid>();
    }

    public class MasterRouteGuidModel
    {
        public Guid MasterRouteGuid { get; set; }
        public List<Guid> RouteGroupDetailGuid { get; set; }
    }

    public class RouteOptimizeSearchResultModel
    {
        public bool FlaglockProcess { get; set; } = false;
        public List<Guid> LockedGuid { get; set; }
        public string ProcessStep { get; set; }
        public string returnMessage { get; set; }
    }
    public class RoadNetResModel
    {
        public string FileName { get; set; }
        public List<RoadnetFileDataModel> Transaction { get; set; }
    }
    public class RoadNetCurrencyView
    {
        public Guid CurrencyGuid { get; set; }
        public string CurrencyAbb { get; set; }
    }
    public class RoadNetCurrencyExchangeRateModel : RoadNetCurrencyView
    {

        public bool FlagValidateTrukLimit { get; set; }
    }
    public class RoadNetCurrencyOnRunresourceView : RoadNetCurrencyView
    {
        public Guid DailyRunGuid { get; set; }
    }
    public class RoadNetCurrencyExchangeRateRequestModel
    {
        public IEnumerable<RoadNetCurrencyView> LiabilityCurrency { get; set; }
        public bool FlagValidateTruckLimit { get; set; }
        public Guid DefaultCurrencyGuid { get; set; }
        public string DefaultCurrencyAbb { get; set; }


    }

    public class RoadNetCountryAndSiteRequest
    {
        public Guid CountryGuid { get; set; }
        public Guid SiteGuid { get; set; }
    }
    public class RoadNetExchangeTemplateView
    {
        public List<RoadNetCommodityView> Template { get; set; } = new List<RoadNetCommodityView>();
        public List<TblMasterCurrency_ExchangeRate> CurrentExchange { get; set; } = new List<TblMasterCurrency_ExchangeRate>();
    }
    public class RoadNetCurrencyExchangeConfig
    {
        public Guid CountryGuid { get; set; }
        public bool FlagValidateRunLiabilityLimit { get; set; }

    }
    public class DefaultUserCurrencyView : RoadNetCurrencyView
    {
        public string UserName { get; set; }
    }
    public class RoadNetCurrencyJobView : RoadNetCurrencyView
    {
        public Guid JobGuid { get; set; }
    }

    public class RoadNetBuildFileRequest
    {
        public TblTransactionRouteOptimizationHeader_Detail_Item OptimizeDetailItem { get; set; }
        public List<CustomerLocationCityStateModel> ListStateAndCity { get; set; }
        public string MasterRouteGroupName { get; set; }
        public string DayOfweek { get; set; }
        public string StrFirstDow { get; set; }
        public string StrLastDow { get; set; }
        
    }

    public class CustomerLocationCityStateModel
    {
        public Guid CustomerLocationGuid { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}
