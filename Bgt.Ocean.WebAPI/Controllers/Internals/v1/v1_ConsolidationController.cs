using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Service.Implementations.Consolidation;
using Bgt.Ocean.Service.Messagings.Consolidation;
using Bgt.Ocean.WebAPI.Models.Consolidation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_ConsolidationController : ApiControllerBase
    {
        private readonly IConsolidationService _consolidationService;

        public v1_ConsolidationController(IConsolidationService consolidationService)
        {
            _consolidationService = consolidationService;
        }

        [HttpPost]
        public async Task<ConsolidateInfoModel> GetConsolidateLocationInfo(ConsolidationMainRequest request)
        {
            var data = await _consolidationService.GetMainConsolidateLocation(request.workDate.ChangeFromStringToDate(request.userFormatDate).GetValueOrDefault(), request.siteGuid);
            ConsolidateInfoModel mainTable = new ConsolidateInfoModel();
            mainTable.LocationConsolidation = data.LocationConsolidation;
            return mainTable;
        }

        [HttpPost]
        public async Task<ConsolidateInfoModel> GetConsolidateRouteInfo(ConsolidationMainRequest request)
        {
            var data = await _consolidationService.GetMainConsolidateRoute(request.workDate.ChangeFromStringToDate(request.userFormatDate).GetValueOrDefault(), request.siteGuid);
            ConsolidateInfoModel mainTable = new ConsolidateInfoModel();
            mainTable.RouteConsolidation = data.RouteConsolidation;
            return mainTable;
        }

        [HttpPost]
        public async Task<ConsolidateInfoModel> GetConsolidateInterBranchInfo(ConsolidationMainRequest request)
        {
            var data = await _consolidationService.GetMainConsolidateInterBranch(request.workDate.ChangeFromStringToDate(request.userFormatDate).GetValueOrDefault(), request.siteGuid);
            ConsolidateInfoModel mainTable = new ConsolidateInfoModel();
            mainTable.InterBranchConsolidation = data.InterBranchConsolidation;
            return mainTable;
        }

        #region MULTI BRANCH CON
        [HttpPost]
        public async Task<ConsolidateInfoModel> GetConsolidateMultiBranchInfo(ConsolidationMainRequest request)
        {
            var data = await _consolidationService.GetMainConsolidateMultiBranch(request.workDate.ChangeFromStringToDate(request.userFormatDate).GetValueOrDefault(), request.siteGuid);
            ConsolidateInfoModel mainTable = new ConsolidateInfoModel();
            mainTable.MultiBranchConsolidation = data.MultiBranchConsolidation;
            return mainTable;
        }

        //Not use, we use model in Ocean Online
        [HttpPost]
        public ConMultiBranchEditResponse GetEditDetailMultiBranchConsolidation(ConMultiBranchEditModelRequest request)
        {
            ConMultiBranchEditResponse response = _consolidationService.GetEditDetailMultiBranchConsolidation(request);
            return response;
        }

        [HttpPost]
        public ConMultiBranchCreateUpdateResponse CreateConsolidate_MultiBranch(ConMultiBranchModel model)
        {
            string dateUserFormat = model.ConMultiBrModel.FormatDate;
            model.ConMultiBrModel.WorkDate = model.ConMultiBrModel.StrWorkDate.ChangeFromStringToDate(dateUserFormat).GetValueOrDefault();
            ConMultiBranchCreateUpdateRequest request = new ConMultiBranchCreateUpdateRequest();
            request.ItemSeals = model.ItemSeals;
            request.ItemCommodity = model.ItemCommodity;
            request.MasterID_Guid = model.ConMultiBrModel.MasterID_Guid;
            request.MasterSite_Guid = model.ConMultiBrModel.MasterSite_Guid;
            request.Destination_MasterSite_Guid = model.ConMultiBrModel.Destination_MasterSite_Guid;

            if (model.ConMultiBrModel.MasterCustomerLocation_Guid != null && model.ConMultiBrModel.MasterRouteGroup_Detail_Guid != null && model.ConMultiBrModel.MasterDailyRunResource_Guid != null)
            {
                request.MasterCustomerLocation_Guid = model.ConMultiBrModel.MasterCustomerLocation_Guid;
                request.MasterRouteGroup_Detail_Guid = null;
            }
            else
            {
                request.MasterCustomerLocation_Guid = model.ConMultiBrModel.MasterCustomerLocation_Guid;
                request.MasterRouteGroup_Detail_Guid = model.ConMultiBrModel.MasterRouteGroup_Detail_Guid;
            }

            request.MasterDailyRunResource_Guid = model.ConMultiBrModel.MasterDailyRunResource_Guid;
            request.OnwardDestination_Guid = model.ConMultiBrModel.OnwardDestination_Guid;
            request.MasterSitePathHeader_Guid = model.ConMultiBrModel.MasterSitePathHeader_Guid;
            request.WorkDate = model.ConMultiBrModel.WorkDate;
            request.StrWorkDate = model.ConMultiBrModel.StrWorkDate;
            request.FormatDate = model.ConMultiBrModel.FormatDate;
            request.MasterID = model.ConMultiBrModel.MasterID;
            request.reasonName = model.ConMultiBrModel.reasonName;
            request.remark = model.ConMultiBrModel.remark;
            request.FlagSealed = model.FlagSealed;
            request.FlagUnsealed = model.FlagUnsealed;

            ConMultiBranchCreateUpdateResponse response = _consolidationService.CreateConsolidate_MultiBranch(request);
            return response;
        }

        [HttpPost]
        public ConMultiBranchCreateUpdateResponse UpdateConsolidate_MultiBranch(ConMultiBranchModel model)
        {
            string dateUserFormat = model.ConMultiBrModel.FormatDate;
            model.ConMultiBrModel.WorkDate = model.ConMultiBrModel.StrWorkDate.ChangeFromStringToDate(dateUserFormat).GetValueOrDefault();
            ConMultiBranchCreateUpdateRequest request = new ConMultiBranchCreateUpdateRequest();
            request.ItemSeals = model.ItemSeals;
            request.ItemCommodity = model.ItemCommodity;
            request.MasterID_Guid = model.ConMultiBrModel.MasterID_Guid;
            request.MasterSite_Guid = model.ConMultiBrModel.MasterSite_Guid;
            request.Destination_MasterSite_Guid = model.ConMultiBrModel.Destination_MasterSite_Guid;

            if (model.ConMultiBrModel.MasterCustomerLocation_Guid != null && model.ConMultiBrModel.MasterRouteGroup_Detail_Guid != null && model.ConMultiBrModel.MasterDailyRunResource_Guid != null)
            {
                request.MasterCustomerLocation_Guid = model.ConMultiBrModel.MasterCustomerLocation_Guid;
                request.MasterRouteGroup_Detail_Guid = null;
            }
            else
            {
                request.MasterCustomerLocation_Guid = model.ConMultiBrModel.MasterCustomerLocation_Guid;
                request.MasterRouteGroup_Detail_Guid = model.ConMultiBrModel.MasterRouteGroup_Detail_Guid;
            }

            request.MasterDailyRunResource_Guid = model.ConMultiBrModel.MasterDailyRunResource_Guid;
            request.OnwardDestination_Guid = model.ConMultiBrModel.OnwardDestination_Guid;
            request.MasterSitePathHeader_Guid = model.ConMultiBrModel.MasterSitePathHeader_Guid;
            request.WorkDate = model.ConMultiBrModel.WorkDate;
            request.StrWorkDate = model.ConMultiBrModel.StrWorkDate;
            request.FormatDate = model.ConMultiBrModel.FormatDate;
            request.MasterID = model.ConMultiBrModel.MasterID;
            request.reasonName = model.ConMultiBrModel.reasonName;
            request.remark = model.ConMultiBrModel.remark;
            request.FlagSealed = model.FlagSealed;
            request.FlagUnsealed = model.FlagUnsealed;
            ConMultiBranchCreateUpdateResponse response = _consolidationService.UpdateConsolidate_MultiBranch(request);
            return response;
        }

        [HttpPost]
        public IEnumerable<DropdownSitePathView> GetSitePathHaveItemDDL(SitePathDDLRequest request)
        {
            string dateUserFormat = request.FormatDate;
            request.WorkDate = request.StrWorkDate.ChangeFromStringToDate(dateUserFormat).GetValueOrDefault();
            IEnumerable<DropdownSitePathView> items = _consolidationService.GetSitePathHaveItemDDL(request);
            return items;
        }

        [HttpPost]
        public ConGetItemResponse GetItemConMultiBranch_New(ItemAvailableConRequest request)
        {
            string dateUserFormat = request.FormatDate;
            request.WorkDate = request.StrWorkDate.ChangeFromStringToDate(dateUserFormat).GetValueOrDefault();
            ConGetItemResponse items = _consolidationService.GetItemConMultiBranch_New(request);
            return items;
        }

        [HttpPost]
        public Dictionary<string, ConsolidateAllItemView> GetItemConMultiBranch_Edit(ItemAvailableConRequest request)
        {
            string dateUserFormat = request.FormatDate;
            request.WorkDate = request.StrWorkDate.ChangeFromStringToDate(dateUserFormat).GetValueOrDefault();
            Dictionary<string, ConsolidateAllItemView> items = _consolidationService.GetItemConMultiBranch_Edit(request);
            return items;
        }
        #endregion
    }
}
