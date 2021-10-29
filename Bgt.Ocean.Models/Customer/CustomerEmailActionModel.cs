using Bgt.Ocean.Infrastructure.Util;
using System;

namespace Bgt.Ocean.Models.Customer
{
    public class CustomerEmailActionModel
    {
        public System.Guid Guid { get; set; }
        public string ActionName { get; set; }
        public bool FlagDisable { get; set; }
        public Nullable<System.Guid> SystemDisplayTextControls_Guid { get; set; }
        public EnumEmailAction ActionID { get; set; }
    }
}
