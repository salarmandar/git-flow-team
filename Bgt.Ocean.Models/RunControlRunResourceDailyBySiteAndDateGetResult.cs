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
    
    public partial class RunControlRunResourceDailyBySiteAndDateGetResult
    {
        public Nullable<System.Guid> Guid { get; set; }
        public Nullable<System.Guid> MasterRunResource_Guid { get; set; }
        public Nullable<int> MasterRunResourceShift { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleNumberFullName { get; set; }
        public Nullable<System.Guid> MasterRouteGroup_Detail_Guid { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public Nullable<System.Guid> MasterGroupGuid { get; set; }
        public string MasterRouteGroupName { get; set; }
        public Nullable<System.Guid> ModeOfTransportGuid { get; set; }
        public string ModeOfTransport { get; set; }
        public Nullable<int> ModeOfTransportID { get; set; }
        public Nullable<int> RunResourceDailyStatusID { get; set; }
        public string RunResourceDailyStatusName { get; set; }
        public Nullable<int> TotalJobInRun { get; set; }
        public Nullable<int> TotalJobNotSync { get; set; }
        public Nullable<System.DateTime> WorkDate { get; set; }
        public string strWorDate { get; set; }
        public Nullable<int> DailyStatusID { get; set; }
        public string DailyStatusName { get; set; }
        public string PathPicModeOfTransport { get; set; }
        public int FlagUseMobile { get; set; }
        public Nullable<bool> FlagOverNight { get; set; }
        public string DetailRun { get; set; }
        public Nullable<bool> FlagRouteBalanceDone { get; set; }
        public Nullable<bool> FlagAlarmLock { get; set; }
        public Nullable<System.Guid> LiabilityLimitCurrency_Guid { get; set; }
        public string LiabilityLimitCurrencyAbb { get; set; }
    }
}
