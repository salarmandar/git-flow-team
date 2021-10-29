
using Bgt.Ocean.Models.Denomination;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.Denomination
{
    public class GetDenominationRequest
    {
        public Guid? LiabilityGuid { get; set; }
        public Guid? CurrencyGuid { get; set; }
    }

    public class SetDenominationRequest
    {
        public IEnumerable<DenominationHeaderView> DenominationHeaderList { get; set; }

    }

    public class SetDenominationAsyncRequest : SetDenominationRequest
    {
        //From Session
        public Guid LanguageGuid { get; set; }
        public String UserName { get; set; }
        public DateTime ClientDateTime { get; set; }
        public DateTimeOffset UniversalDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        public String FormatDateTime { get; set; }
        public String FormatDate { get; set; }
    }
  

    public class DenominationResponse : BaseResponse
    {
        public IEnumerable<DenominationDetailView> DenominationList { get; set; }
    }

    

}
