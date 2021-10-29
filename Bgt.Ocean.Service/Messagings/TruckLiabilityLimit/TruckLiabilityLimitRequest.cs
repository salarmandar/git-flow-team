

using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Models.RunControl.LiabilityLimitModel;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.TruckLiabilityLimit
{

    public class LiabilityLimitExistsRunRequest
    {
        public LiabilityLimitRunsActionModel RunsActionModel { get; set; }
    }

    public class LiabilityLimitExistsJobsRequest
    {
        public LiabilityLimitJobsActionModel JobsActionModel { get; set; }
    }

    public class LiabilityLimitNoExistsItemsRequest
    {
        public LiabilityLimitItemsActionModel ItemsActionModel { get; set; }
    }

    public class LiabilityLimitNoExistsJobsRequest
    {
        public LiabilityLimitNoJobsActionModel NoJobsActionModel { get; set; }
    }

    public class LiabilityLimitResponse
    {
        public SystemMessageView Message { get; set; }
        public TruckLibilityLimitResult TruckLimitDetail { get; set; }
    }

    public class PercentageLiabilityLimitAlertResponse
    {
        public SystemMessageView Message { get; set; }
        public IEnumerable<PercentageLiabilityLimitAlertResult> RunDetail { get; set; }
    }

    public class ConvertBankCleanOutTotalLiabilityRequest
    {
        public ConvertBankCleanOutLiabilityModel JobModel { get; set; }
    }
    public class ConvertBankCleanOutTotalLiabilityResponse
    {
        public SystemMessageView Message { get; set; }
        public double Total_STC { get; set; }
        public double TotalLiabilities_STC { get; set; }
        public double TotalCommodities_STC { get; set; }
        public string Target_CurrencyAbb { get; set; }
    }


    public class JobWithSTCRequest
    {
        public IEnumerable<Guid> JobList { get; set; }
        public Guid SiteGuid { get; set; }
        public Guid DailyRunGuid { get; set; }
        public bool FlagCalExchageRate { get; set; }
        public Guid UserGuid { get; set; }
    }
    public class JobWithSTCResponse
    {
        public IEnumerable<JobWithStcView> JobWithSTCOnHand { get; set; }
    }
}
