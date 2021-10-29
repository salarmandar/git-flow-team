using Bgt.Ocean.Models;
using Bgt.Ocean.Models.SiteNetwork;
using Bgt.Ocean.Service.ModelViews.SiteNetWork;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class SiteNetworkMapper
    {
        public static IEnumerable<SiteNetworkViewResponse> ConvertToSiteNetworkViewResponseList(this IEnumerable<SiteNetworkMemberView> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SiteNetworkMemberView>, IEnumerable<SiteNetworkViewResponse>>(model);
        }

        public static SiteNetworkViewResponse ConvertToSiteNetworkViewResponseView(this SiteNetworkMemberView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<SiteNetworkMemberView, SiteNetworkViewResponse>(model);
        }

        public static IEnumerable<SiteNetworkAuditLogView> ConvertToSiteNetworkAuditLogViewList(this IEnumerable<TblMasterSiteNetworkAuditLog> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<TblMasterSiteNetworkAuditLog>, IEnumerable<SiteNetworkAuditLogView>>(model);
        }
    }
}
