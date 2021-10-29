using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.PreVault
{
    public class CheckOutDepartmentResponse
    {
        public SystemMessageView MsgReponse { get; set; }
        public Guid TempReportGuid { get; set; }
        public bool Flagconfirmcheckout { get; set; } = false;        
        public List<CheckOutDepartmentCustomerLocationListModel> CustomerLocationList { get; set; }
    }

    public class CheckOutDepartmentCustomerLocationListModel
    {
        public string BranchCodeReference { get; set; }
        public string BranchName { get; set; }
    }


}
