using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Service.ModelViews.SitePath;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class SitePathMapper
    {
        public static IEnumerable<SitePathViewResponse> ConvertToSitePathViewResponseList(this IEnumerable<SitePathView> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SitePathView>, IEnumerable<SitePathViewResponse>>(model);
        }

        public static SitePathViewResponse ConvertToSitePathViewResponse(this SitePathView model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<SitePathView, SitePathViewResponse>(model);
        }

        public static SitePathView ConvertToSitePathView(this SitePathViewRequest model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<SitePathViewRequest, SitePathView>(model);
        }

        public static IEnumerable<SitePathAuditLogView> ConvertToSitePathAuditLogViewList(this IEnumerable<TblMasterSitePathAuditLog> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map< IEnumerable<TblMasterSitePathAuditLog>, IEnumerable<SitePathAuditLogView>>(model);
        }
    }
}
