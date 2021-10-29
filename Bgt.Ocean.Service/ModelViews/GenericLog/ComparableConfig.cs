using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog
{

    public class ComparableConfig
    {       
        public string ModelNameSource { get; set; }
        public string ModelNameTarget { get; set; }

        public virtual IEnumerable<ComparableConfigProperty> ComparableConfigPropertyList { get; set; }
       
    }

}
