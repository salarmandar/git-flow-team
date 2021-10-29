using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.OTCManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.ModelViews.RunControls
{
    public class JobLegsView
    {
        public Guid JobGuid { get; set; }
        public Guid JobLegGuid { get; set; }
        public int StatusID { get; set; }
        public string ScheduleTime { get; set; }
        public string ActionJob { get; set; } // P // D
        public int ServiceJobTypeId { get; set; } // Type Job ID
        public string ServiceJobTypeNameAbb { get; set; } // Type Job
        public bool FlagRouteBalance { get; set; }
    }

    public class JobOtcRequest : RequestBase
    {
        [Required]
        public Guid JobHeadGuid { get; set; }
        public string UserDateFormat { get; set; }
        public Guid MasterSite_Guid { get; set; }
        
        public Guid UserLangquege
        {
            get { return ApiSession.UserLanguage_Guid.GetValueOrDefault(); }
        }

        public IEnumerable<GetJobListByRunRequest> JobListRequest { get; set; } = new List<GetJobListByRunRequest>();

    }

    public class JobLegsViewResult
    {
        public Guid JobGuid { get; set; }
        public Guid JobLegGuid { get; set; }
        public string JobNo { get; set; }
        public string CustomerLocateionName { get; set; }
    }
}
