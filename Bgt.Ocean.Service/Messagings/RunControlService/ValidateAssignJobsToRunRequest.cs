using Bgt.Ocean.Models.RunControl;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class ValidateAssignJobsToRunRequest
    {
        public IEnumerable<ValidateJobsInRunView> AssignJobList { get; set; } = new List<ValidateJobsInRunView>();
    }

    public class ValidateAssignJobsToRunResponse : BaseResponse
    {
    }
}
