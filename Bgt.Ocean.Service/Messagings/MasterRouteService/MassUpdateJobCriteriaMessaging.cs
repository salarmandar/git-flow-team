using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models.MasterRoute;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class MassUpdateJobResponse : BaseResponse
    {
        public IEnumerable<MasterrouteView> MasterRouteDetail { get; set; } = new List<MasterrouteView>();

    }
    public class MassUpdateJobCriteriaRequest : RequestBase
    {
        public SeviceStopLegViewModel PickUpLeg { get; set; }
        public SeviceStopLegViewModel DeliveryLeg { get; set; }
        public List<Guid> LobGuids { get; set; }
        public List<Guid> ServiceJobTypeGuids { get; set; }
        public List<Guid> DayOfWeekGuids { get; set; }
        /// <summary>
        /// True = include holiday.
        /// </summary>
        public bool FlagIncludeHoliday { get; set; }
        /// <summary>
        /// True = include everweek.
        /// </summary>
        public bool FlagEveryWeek { get; set; }
        /// <summary>
        /// True = include A Week.
        /// </summary>
        public bool FlagA_Week { get; set; }
        /// <summary>
        /// True = Include B Week.
        /// </summary>
        public bool FlagB_Week { get; set; }

        /// <summary>
        /// P = PickUp
        /// D = Delivery
        /// </summary>
        public string ActionReplace { get; set; }
        public SeviceStopLegViewModel DataForReplace { get; set; }
        public Guid? Langquage
        {
            get
            {
                return ApiSession.UserLanguage_Guid;
            }
        }
    }

    public class SeviceStopLegViewModel
    {
        public Guid MasterSiteGuid { get; set; }
        public Guid? CustomerGuid { get; set; }
        public Guid? CustomerLocationGuid { get; set; }


        public bool FlagIsCustomerLocation { get; set; }
    }

    public class MassUpdateDropdownView
    {
        public IEnumerable<DropDownLobView> LOBList { get; set; }
        public IEnumerable<DropDownDayOfWeekView> DayOfWeekList { get; set; }
    }

    public class DropDownCustomerView
    {
        public Guid CustomerGuid { get; set; }
        public string CustomerName { get; set; }
    }

    public class DropDownLocationView
    {
        public Guid LocationGuid { get; set; }
        public string LocationName { get; set; }
    }

    public class DropDownLobView
    {
        public Guid LobGuid { get; set; }
        public string LobName { get; set; }
    }

    public class DropDownDayOfWeekView
    {
        public Guid DayOfWeekGuid { get; set; }
        public string DayOfWeekName { get; set; }
        public int DayOfWeekSequence { get; set; }
    }

    public class ServiceTypeByLOBsRequest
    {
        public bool FlagPcustomer { get; set; }
        public bool FlagDcustomer { get; set; }
        public bool FlagSameSite { get; set; }
        public List<Guid> LobGuids { get; set; }

    }
}
