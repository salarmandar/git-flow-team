using Bgt.Ocean.Models;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{
    public class AdhocJobResponse : SystemMessageView
    {
        public AdhocJobResponse(TblSystemMessage tblmsg) : base(tblmsg)
        {
        }

        public Guid JobGuid { get; set; }
        public string JobNo { get; set; }
        public bool FlagDolphin { get; set; } // true == รถ Dispatch และเป็นรถ Dolphin
        public bool FlagDuplicate { get; set; } // true == have data same day
        public bool FlagJobDupValidate { get; set; }// Falg Country Option

        public bool FlagIsThereEmployeeCanDoOTC { get; set; } //is there employee that can do request OTC in run
        public SystemMessageView AlertMaxNumberJobs { get; set; }

    }
}
