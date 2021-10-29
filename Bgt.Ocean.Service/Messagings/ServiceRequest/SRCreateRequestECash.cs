using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRCreateRequestECash : SRCreateRequestFLM
    {
        public IEnumerable<ECashAmount> ECashViewList { get; set; }
    }

    public class SRCreateRequestECashWithSFI : SRCreateRequestECash
    {
        public SRCreateRequestSFI SFIModel { get; set; }
    }

    #region Child Class


    public class ECashAmount
    {
        public double? DenominationValue { get; set; }
        public int? Amount { get; set; }
        public int? DenominationQuantity { get; set; }
        public int? Unit { get; set; }
        public string Currency { get; set; }
        public string Denomination { get; set; }
    }

    #endregion
}
