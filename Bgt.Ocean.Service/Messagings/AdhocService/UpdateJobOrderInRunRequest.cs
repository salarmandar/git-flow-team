using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{

    public class RunResourceUpdateJobOrder
    {
        public List<UpdateJobOrderInRunRequest> RunResource { get; set; } = new List<UpdateJobOrderInRunRequest>();
  
    }
    public class UpdateJobOrderInRunRequest
    {
        public Guid? MasterRouteGuid { get; set; } // Form MasterRoute
        public Guid RunDailyGuid { get; set; }
        public Guid SiteGuid { get; set; }          
        public bool FlagReorder { get; set; }

        public DateTime? ClientDateTime { get; set; }
        public string UserModified { get; set; } //added: 2018/10/12
        public Guid LanguageGuid { get; set; }

        public DateTime? WorkDate { get; set; } // Form MasterRoute 
        public List<Guid> JobHeadGuidList { get; set; } = new List<Guid>();
    }
  public  class PushToDolPhinRequest : UpdateJobOrderInRunRequest
    {
        
    }
}
