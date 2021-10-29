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
    
    public partial class SFOTblTransactionServiceRequest_Problem
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterSite_Guid { get; set; }
        public System.Guid SFOTransactionServiceRequest_Guid { get; set; }
        public System.Guid SFOMasterProblem_Guid { get; set; }
        public string ProblemID { get; set; }
        public string ProblemName { get; set; }
        public string ProblemDescription { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<System.Guid> SFOMasterCategory_Guid { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public Nullable<bool> FlagNoneSLA { get; set; }
        public Nullable<int> SLATime { get; set; }
        public Nullable<System.Guid> SFOMasterMachineServiceType_Guid { get; set; }
        public string MachineServiceTypeID { get; set; }
        public string MachineServiceTypeName { get; set; }
        public Nullable<System.Guid> SFOMasterSubProblemLevel1_Guid { get; set; }
        public string SFOMasterSubProblemLevel1_ID { get; set; }
        public string SFOMasterSubProblemLevel1_Name { get; set; }
        public Nullable<System.Guid> SFOMasterSubProblemLevel2_Guid { get; set; }
        public string SFOMasterSubProblemLevel2_ID { get; set; }
        public string SFOMasterSubProblemLevel2_Name { get; set; }
        public Nullable<System.Guid> SFOMasterSubProblemLevel3_Guid { get; set; }
        public string SFOMasterSubProblemLevel3_ID { get; set; }
        public string SFOMasterSubProblemLevel3_Name { get; set; }
        public Nullable<System.Guid> SFOMasterSubProblemLevel4_Guid { get; set; }
        public string SFOMasterSubProblemLevel4_ID { get; set; }
        public string SFOMasterSubProblemLevel4_Name { get; set; }
    
        public virtual SFOTblMasterMachineServiceType SFOTblMasterMachineServiceType { get; set; }
        public virtual SFOTblMasterProblem SFOTblMasterProblem { get; set; }
        public virtual SFOTblTransactionServiceRequest SFOTblTransactionServiceRequest { get; set; }
        public virtual TblMasterSite TblMasterSite { get; set; }
    }
}
