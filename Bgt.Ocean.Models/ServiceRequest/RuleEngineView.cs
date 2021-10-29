using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.ServiceRequest
{
    public class RuleEngineView
    {
        public Guid? MasterCountry_Guid { get; set; }
        public Guid? SystemJobStatus_Guid { get; set; }
        public Guid? SystemServiceRequestState_Guid { get; set; }
        public string SystemServiceRequestState_Name { get; set; }
        public List<RuleDetail> RuleDetail { get; set; }
    }
    public class RuleDetail
    {
        public string TargetField { get; set; }
        public string Operator { get; set; }
        public string TargetValue { get; set; }
    }
}
