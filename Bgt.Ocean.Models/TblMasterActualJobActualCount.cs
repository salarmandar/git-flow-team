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
    
    public partial class TblMasterActualJobActualCount
    {
        public System.Guid Guid { get; set; }
        public System.Guid MasterActualJobSumActualCount_Guid { get; set; }
        public System.Guid MasterDenomination_Guid { get; set; }
        public System.Guid MasterCurrency_Guid { get; set; }
        public decimal DenominationValue { get; set; }
        public string CurrencyAbbr { get; set; }
        public int Count { get; set; }
        public int Reject { get; set; }
        public int Diff { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public int CassetteSequence { get; set; }
    
        public virtual TblMasterActualJobSumActualCount TblMasterActualJobSumActualCount { get; set; }
        public virtual TblMasterCurrency TblMasterCurrency { get; set; }
        public virtual TblMasterDenomination TblMasterDenomination { get; set; }
    }
}
