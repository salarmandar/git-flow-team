using System;

namespace Bgt.Ocean.Models.MasterRoute
{
    public class MasterRouteJobServiceStopLegsView
    {
        /// <summary>
        /// origin : false,
        /// destination : true
        /// </summary>
        public bool FlagDeliveryLegForTV { get; set; }
        /// <summary>
        /// origin : false,
        /// destination : true
        /// </summary>
        public bool FlagDestination { get; set; }
        /// <summary>
        /// origin : null,
        /// destination : selected Delivery Master Route Template
        /// </summary>
        public Guid? MasterRouteDeliveryLeg_Guid { get; set; }
        /// <summary>
        /// origin : 1
        /// destination : unknown
        /// </summary>
        public int SequenceStop { get; set; }
        /// <summary>
        /// origin : origin-dayofweeksequence,
        /// destination : origin-dayofweeksequence  + dayinvault
        /// </summary>
        public int DayOfWeek_Sequence { get; set; }


        public Guid? MasterCustomerLocation_Guid { get; set; }
        public int DayOfWeekCompletion_Sequence { get; set; }
        public bool FlagMultiplesDaysJob { get; set; }
        public bool FlagNonBillable { get; set; }
        public Guid? MasterRouteGroupDetail_Guid { get; set; }
        public Guid? MasterSite_Guid { get; set; }
        public int NumberOfDaysCompletionJob { get; set; }
        public string StrSchduleTime { get; set; }

        /// <summary>
        /// Not required
        /// </summary>
        public Guid? MasterRouteJobHeader_Guid { get; set; }
        /// <summary>
        /// Not required
        /// </summary>
        public Guid MasterRouteLeg_Guid { get; set; }
        /// <summary>
        /// Not required
        /// </summary>
        public int SeqIndex { get; set; }
        /// <summary>
        /// Not required
        /// </summary>
        public int JobOrder { get; set; }
        /// <summary>
        /// Not required
        /// </summary>
        public bool FlagDisableRoute { get; set; }
        /// <summary>
        /// Not required
        /// </summary>
        public Guid? CustomerLocationAction_Guid { get; set; }
        public Guid? MasterCustomer_Guid { get; set; }
    }
}
