using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.Service.Messagings.Consolidation
{
    //ConAndDeconsolidationResponse


    #region ## MODEL EDIT DATA
    public class ConMultiBranchEditResponse
    {
        public SystemMessageView Message { get; set; } = new SystemMessageView();
        public ConsolidateMultiBranchView ConMultiBranchEditModel { get; set; } = new ConsolidateMultiBranchView();
    }

    public class ConsolidateMultiBranchView
    {
        public Guid MasterID_Guid { get; set; }
        public Guid? MasterSite_Guid { get; set; }
        public Guid? MasterSitePathHeader_Guid { get; set; }
        public Guid? Destination_MasterSite_Guid { get; set; }
        public Guid? MasterCustomerLocation_Guid { get; set; }
        public Guid? MasterRouteGroup_Detail_Guid { get; set; }
        public Guid? MasterDailyRunResource_Guid { get; set; }
        public Guid? OnwardDestination_Guid { get; set; }
        public Guid SystemCoAndDeSolidateStatus_Guid { get; set; }
        public int MinimumSealDigi { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string MasterID { get; set; }
        public string SitePathName { get; set; }
        public string LocationName { get; set; } //BranchName
        public string GroupDetailName { get; set; }
        public string DestinationSiteName { get; set; }
        public string InterDepartmentName { get; set; }
        public string FormatDate { get; set; }
        public string StrWorkDate { get; set; }
        public DateTime WorkDate { get; set; } //use for new
        public string reasonName { get; set; } //use for update con
        public string remark { get; set; } //use for update con
        public SystemMessageView Message { get; set; } = new SystemMessageView();

    }
    #endregion

    #region ## MODEL CREATE UPDATE RESPONSE
    public class ConMultiBranchCreateUpdateResponse
    {
        public SystemMessageView Message { get; set; } = new SystemMessageView();
        public Guid MasterID_Guid { get; set; }

        /// <summary>
        /// Old Name: StatusMasterID
        /// </summary>
        public string StatusName { get; set; }
    }
    #endregion

    #region
    public class ConGetItemResponse
    {
        public Dictionary<string, ConsolidateAllItemView> Items { get; set; } = new Dictionary<string, ConsolidateAllItemView>();
        public IEnumerable<DestRouteDropdownResponse> DestinationRoute { get; set; } = new List<DestRouteDropdownResponse>();
    }

    public class DestRouteDropdownResponse
    {
        public Guid? MasterRouteGroupDetail_Guid { get; set; }  
        public Guid? MasterDailyRunResource_Guid { get; set; }
        public string DestinationRoute { get; set; }
    }
    #endregion
}
