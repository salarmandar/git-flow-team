//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bgt.Ocean.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblMasterNemoQueueRouteOptimization_Detail
    {
        public System.Guid Guid { get; set; }
        public System.Guid NemoQueueRouteOptimization_Guid { get; set; }
        public System.Guid MasterCustomerLocation_Guid { get; set; }
        public string MasterCustomerLocation_Code { get; set; }
        public int JobOrder { get; set; }
        public Nullable<int> JobOrderOptmized { get; set; }
        public Nullable<System.Guid> MasterJob_Guid { get; set; }
        public Nullable<System.DateTime> ScheduleTime { get; set; }
        public Nullable<System.DateTime> ScheduleTimeOptimized { get; set; }
        public Nullable<bool> FlagBrinksLocation { get; set; }
    }
}
