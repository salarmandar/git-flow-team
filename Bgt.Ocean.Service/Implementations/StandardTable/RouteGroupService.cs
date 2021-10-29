using Bgt.Ocean.Models.StandardTable;
using System;
using System.Collections.Generic;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;
using System.Linq;
using Bgt.Ocean.Service.Messagings.StandardTable.RouteGroup;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;

namespace Bgt.Ocean.Service.Implementations.StandardTable
{

    #region interface

    public interface IRouteGroupService
    {
        ResponseQueryRouteGroup GetRouteGroupList(RequestQueryRouteGroup request);
        IEnumerable<TblMasterRouteGroup> GetRouteGroupBySite(Guid siteGuid);
        RouteGroupAndGroupDetailView GetRouteGroupAndRouteDetailBySite(Guid siteGuid);
    }

    #endregion

    public class RouteGroupService : IRouteGroupService
    {
        private readonly IMasterRouteGroupRepository _masterRouteGroupRepository;
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterRouteGroupDetailRepository _masterRouteGroupDetailRepository;

        public RouteGroupService(
            IMasterRouteGroupRepository routeGroupRepository,
            IMasterCustomerRepository masterCustomerRepository,
            IMasterRouteGroupDetailRepository masterRouteGroupDetailRepository
        )
        {
            _masterRouteGroupRepository = routeGroupRepository;
            _masterCustomerRepository = masterCustomerRepository;
            _masterRouteGroupDetailRepository = masterRouteGroupDetailRepository;
        }

        public ResponseQueryRouteGroup GetRouteGroupList(RequestQueryRouteGroup request)
        {
            var result = new ResponseQueryRouteGroup();
            var requestRepo = new RouteGroupView_Request
            {
                countryAbb = request.countryAbb,
                createdDatetimeFrom = request.createdDatetimeFrom,
                createdDatetimeTo = request.createdDatetimeTo
            };

            var resultRepo = _masterRouteGroupRepository.GetRouteGroupList(requestRepo);
            var resultList = MapperService.Map<IEnumerable<RouteGroupView>, IEnumerable<ResponseQueryRouteGroup_Main>>(resultRepo).ToList();

            result.result = resultList;
            result.rows = resultList.Count;

            return result;
        }

        public IEnumerable<TblMasterRouteGroup> GetRouteGroupBySite(Guid siteGuid)
        {

            var companyGuid = _masterCustomerRepository.GetCompanyOnCustomerBySite(siteGuid);
            var routeDetailGuids = _masterRouteGroupDetailRepository.FindBySite(siteGuid)?.Select(x => x.MasterRouteGroup_Guid).ToList();
            if (routeDetailGuids == null || !routeDetailGuids.Any())
                return new List<TblMasterRouteGroup>();
            return _masterRouteGroupRepository.FindByCompanyGuid(companyGuid.Guid)?.Where(e => routeDetailGuids.Contains(e.Guid)).OrderBy(j => j.MasterRouteGroupName);
        }
        public RouteGroupAndGroupDetailView GetRouteGroupAndRouteDetailBySite(Guid siteGuid)
        {
            return _masterRouteGroupDetailRepository.FindRouteGroupAndGroupDetailBySite(siteGuid);
        }
    }
}
