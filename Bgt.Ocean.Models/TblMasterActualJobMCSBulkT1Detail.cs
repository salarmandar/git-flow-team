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
    
    public partial class TblMasterActualJobMCSBulkT1Detail
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterActualJobMCSBulkRetract_Guid { get; set; }
        public string ReasonName { get; set; }
        public int NumberOfNote { get; set; }
        public Nullable<System.Guid> MasterReasonType_Guid { get; set; }
    
        public virtual TblMasterActualJobMCSBulkRetract TblMasterActualJobMCSBulkRetract { get; set; }
        public virtual TblMasterReasonType TblMasterReasonType { get; set; }
    }
}
