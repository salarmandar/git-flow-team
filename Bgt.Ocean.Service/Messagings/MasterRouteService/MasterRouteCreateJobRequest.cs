using Bgt.Ocean.Models.MasterRoute;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class MasterRouteCreateJobRequest : RequestBase
    {
        public bool IsUpdate { get; set; }
        public Guid MasterJobHead_Guid { get; set; }
        public Guid? MasterRoute_Guid { get; set; }
        public int DayInVault { get; set; }
        public Guid? MasterCustomerContract_Guid { get; set; }
        public Guid? MasterSitePathHeader_Guid { get; set; }
        public Guid? MasterSubServiceType_Guid { get; set; }
        public int OnwardDestinationType { get; set; }
        public Guid? OnwardDestination_Guid { get; set; }
        public Guid? SystemLineOfBusiness_Guid { get; set; }
        public Guid? SystemServiceJobType_Guid { get; set; }
        public Guid? SystemStopType_Guid { get; set; }
        public Guid? SystemTripIndicator_Guid { get; set; }
        public bool FlagJobInterBranch { get; set; }
        public bool FlagJobMultiBranch { get; set; }
        public MasterRouteJobServiceStopLegsView Pickupleg { get; set; } = new MasterRouteJobServiceStopLegsView();
        public MasterRouteJobServiceStopLegsView Deliveryleg { get; set; } = new MasterRouteJobServiceStopLegsView();

        // stamp modify date
        public DateTime ClientDateTime_Local { get; set; }
        public DateTimeOffset UtcDateTimeModifyDefined { get; set; }
    }

    public class MasterRouteCreateJobResponse : BaseResponse
    {
        public DateTime DateTimeModify { get; set; }
        public List<UpdatedLegsView> data { get; set; } = new List<UpdatedLegsView>();
    }

    public class UpdatedLegsView
    {
        public Guid? LegGuid { get; set; }
        public Guid? MasterRouteGuid { get; set; }
        public Guid? RouteGroupDetailGuid { get; set; }
        public Guid? SiteGuid { get; set; }       
    }

}
