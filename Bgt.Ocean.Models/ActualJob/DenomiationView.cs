using System;

namespace Bgt.Ocean.Models.ActualJob
{
    public class DenominationView
    {
        public Guid Guid { get; set; }
        public Guid? MasterCurrency_Guid { get; set; }
        public double? DenominationValue { get; set; }
        public string DenominationText { get; set; }

    }
    public class DenominationModel
    {
        public Guid Guid { get; set; }
        public Guid? MasterCurrency_Guid { get; set; }
        public double? DenominationValue { get; set; }
        public string DenominationName { get; set; }
        public int? DenominationTypeID { get; set; }
        public string DenominationType { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
    }

}
