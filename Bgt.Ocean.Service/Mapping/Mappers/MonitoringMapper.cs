using Bgt.Ocean.Models;
using Bgt.Ocean.Service.ModelViews.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class MonitoringMapper
    {
        public static IEnumerable<SmartBillingGenerateStatusView> ConvertToSmartBillingGenerateStatusView(this IEnumerable<TblMasterHistory_ReportPushToSmart> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterHistory_ReportPushToSmart>,IEnumerable<SmartBillingGenerateStatusView>>(model);
        }
    }
}
