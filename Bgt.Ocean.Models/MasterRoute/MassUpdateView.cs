using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.MasterRoute
{
    public class MassUpdateView
    {
        public class BrinksNameView
        {
            public string BrinksCompanyName { get; set; }
            public string BrinksSiteName { get; set; }
        }

        public class DropDownServiceTypeView
        {
            public Guid ServiceTypeGuid { get; set; }
            public string ServiceTypeName { get; set; }
        }
    }

    public class LobView
    {
        /// <summary>
        /// Can be LOB Guid or Sub LOB Guid
        /// </summary>
        public Guid Guid { get; set; }
        public string LOBFullName { get; set; }

        /// <summary>
        /// In SystemLineOfBusiness not sub this is 'null'
        /// In sub lob this is main relate
        /// </summary>
        public Guid? SystemLineOfBusiness_Guid { get; set; }

    }
    public class MassUpdateDataView
    {
        public IEnumerable<MasterrouteView> Masterroute { get; set; }
        public MessageResponseView Msg { get; set; } 
    }
    public class MasterrouteView
    {
        public Guid MasterRouteGuid { get; set; }
        public Guid? RouteGroupDetailGuid { get; set; }
    }
    public class MessageResponseView
    {
        public int MsgId { get; set; }
        public string MsgDetail { get; set; }
    }
    public class ValidateRouteUnderOptimizeModel
    {
        public Guid MasterRoutGuid { get; set; }
        public string MasterRouteName { get; set; }
        public Guid RouteGroupDetailGuid { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public string RouteOptimizationStatusName { get; set; }

    }

}
