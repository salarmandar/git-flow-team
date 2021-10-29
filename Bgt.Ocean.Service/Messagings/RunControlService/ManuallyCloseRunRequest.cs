using System;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class ManuallyCloseRunRequest
    {
        public Guid dailyRun { get; set; }
        public Guid languageGuid { get; set; }
        public Guid userGuid { get; set; }
        public string strClientDate { get; set; }         /* Required! Format 'MM/dd/yyyy' */
    }
}
