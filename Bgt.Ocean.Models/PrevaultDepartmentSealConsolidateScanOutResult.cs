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
    
    public partial class PrevaultDepartmentSealConsolidateScanOutResult
    {
        public Nullable<System.Guid> JobGuid { get; set; }
        public string JobNo { get; set; }
        public string Location { get; set; }
        public Nullable<System.Guid> CustomerLocationGuid { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public string SealNo { get; set; }
        public Nullable<System.Guid> OnwardDestinationGuid { get; set; }
        public string OnwardDestinationName { get; set; }
        public Nullable<System.Guid> LiabilityCommodityGuid { get; set; }
        public Nullable<System.Guid> MappingInternalDeptGuid { get; set; }
        public Nullable<System.Guid> MappingCommodityGuid { get; set; }
        public Nullable<System.Guid> MasterRunResourceDaily_Guid { get; set; }
        public Nullable<int> ServiceJobTypeID { get; set; }
        public string ServiceJobTypeName { get; set; }
        public string ServiceJobTypeNameAbb { get; set; }
        public string ActionNameAbbrevaition { get; set; }
        public Nullable<System.Guid> ConAndDeconsolidateHeaderGuid { get; set; }
        public Nullable<int> ConsolidationTypeID { get; set; }
        public Nullable<System.Guid> Con_SiteGuid { get; set; }
        public Nullable<int> Con_DesSiteGuid { get; set; }
        public Nullable<System.Guid> LiabilityGuid { get; set; }
        public Nullable<bool> FlagMixJobType { get; set; }
        public Nullable<bool> FlagTVD { get; set; }
        public string STC { get; set; }
        public Nullable<bool> FlagMultiCur { get; set; }
        public Nullable<System.DateTime> WorkDate { get; set; }
        public string DailyRunResource { get; set; }
    }
}
