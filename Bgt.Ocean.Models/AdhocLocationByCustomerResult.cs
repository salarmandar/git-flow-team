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
    
    public partial class AdhocLocationByCustomerResult
    {
        public System.Guid Guid { get; set; }
        public string BranchName { get; set; }
        public Nullable<System.Guid> SystemCustomerOfType_Guid { get; set; }
        public string SystemCustomerOfTypeName { get; set; }
        public Nullable<System.Guid> SystemCustomerLocationType_Guid { get; set; }
        public string CustomerLocationTypeName { get; set; }
        public Nullable<System.Guid> SiteGuid { get; set; }
        public string SiteCodeName { get; set; }
        public string ServiceHour { get; set; }
        public Nullable<bool> FlagLocationDestination { get; set; }
        public Nullable<bool> FlagNonBillable { get; set; }
    }
}
