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
    
    public partial class TblSystemApiHistory
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> TokenID { get; set; }
        public string ApiMethodName { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeStart { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeStop { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }
}
