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
    
    public partial class RoutesDataResult
    {
        public System.Guid DailyGuid { get; set; }
        public System.Guid BranchGuid { get; set; }
        public string ServiceJobType { get; set; }
        public string BranchCode { get; set; }
        public Nullable<System.Guid> CountryGuid { get; set; }
        public string CountryCode { get; set; }
        public System.Guid ShiftGuid { get; set; }
        public System.DateTime ShiftServiceStart { get; set; }
        public Nullable<System.DateTime> ShiftServiceEnd { get; set; }
        public System.DateTime DateStart { get; set; }
        public string TimeZone { get; set; }
        public string customerLocCode { get; set; }
        public Nullable<System.Guid> JobGuid { get; set; }
        public string ServiceTypes { get; set; }
        public Nullable<System.DateTime> ServiceTimeStart { get; set; }
        public string ParentNodes { get; set; }
        public Nullable<decimal> Volumes { get; set; }
        public Nullable<decimal> Capacities { get; set; }
        public Nullable<decimal> Liabilities { get; set; }
        public int Orders { get; set; }
    }
}
