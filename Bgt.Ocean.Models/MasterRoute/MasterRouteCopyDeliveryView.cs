using System;
namespace Bgt.Ocean.Models.MasterRoute
{
    public class MasterRouteCopyDeliveryView
    {
        public Guid MasterRouteJobServiceStopLegGuid { get; set; }
        public string ServiceJobTypeNameAbb { get; set; }
        public string ActionNameAbbrevaition { get; set; }
        public string LocationName { get; set; }
        public int DayInVault { get; set; }
        public Guid? BrinksiteGuid { get; set; }
        public string BrinksiteName { get; set; }

        public Guid MasterRouteDeliveryLeg_Guid { get; set; }
        public Guid? MasterRouteJobHeader_Guid { get; set; }

        public int DayOfWeek_Sequence { get; set; }
        public Guid MasterDayOfweek_Guid { get; set; }
        public bool FlagHoliday { get; set; }

        public Guid MasterRoute_Guid { get; set; }
        public Guid? RouteGroupDetailGuid { get; set; }

        public string Original_MasterRouteName { get; set; }
        public Guid? Original_MasterRouteGuid { get; set; }
        public string Original_RouteGroupDetail { get; set; }
        public Guid? Original_RouteGroupDetailGuid { get; set; }

        public int? SequenceStop { get; set; }
        public int? JobOrder { get; set; }
    }
}
