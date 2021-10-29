using Bgt.Ocean.Service.Messagings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.ModelViews.Monitoring
{
    public class SmartBillingConfigView
    {
        public Guid SiteGuid { get; set; }
        public string Email { get; set; }
        public string DropFilePath { get; set; }
        public bool FlagAutoGenerate { get; set; }
        public List<Guid> DaysGuid { get; set; }
    }
}
