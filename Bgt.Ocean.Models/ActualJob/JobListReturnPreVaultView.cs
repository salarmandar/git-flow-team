using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Models.ActualJob
{
    public class JobListReturnPreVaultView
    {
        public List<Guid> JobNonDeliveryList { get; set; }
        public List<Guid> JobPartialList { get; set; }
    }
}
