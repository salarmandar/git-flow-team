using System;

namespace Bgt.Ocean.Service.ModelViews.Problem
{
    public class ProblemView
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterSite_Guid { get; set; }
        public string ProblemID { get; set; }
        public string ProblemName { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<System.Guid> SFOMasterPriority_Guid { get; set; }
        public Nullable<System.Guid> SFOMasterCategory_Guid { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public bool FlagNoneSLA { get; set; }
        public Nullable<int> SLATime { get; set; }
        public Nullable<System.Guid> SFOMasterMachineServiceType_Guid { get; set; }
        public string SFOMasterMachineServiceTypeName { get; set; }
    }
}
