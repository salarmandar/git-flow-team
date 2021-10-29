
namespace Bgt.Ocean.Models.RunControl
{
    public class ValidateJobCannotCloseResponse
    {
        public MessageCloseRunResponseView Msg { get; set; } = new MessageCloseRunResponseView();

    }

    public class JobStatusCannotCloseRunView
    {
        public int MsgID { get; set; }
        public int? JobStatus { get; set; }
        public bool? FlagHasItem { get; set; } = null;
        public bool? FlagIntermediate { get; set; } = null;
        public bool? FlagActionIsD { get; set; } = null;
        public bool? FlagDiscrepancies { get; set; } = null;
        public bool? FlagIsDolphinReceive { get; set; } = null;
        public bool? FlagIsInTransit { get; set; } = null;
        public string JobNo { get; set; }
        public int? JobType { get; set; } = null;
        public bool Validate(JobStatusCannotCloseRunView rule, JobStatusCannotCloseRunView item)
        {

            return (rule.JobStatus == item.JobStatus || rule.JobStatus == null)
            && (rule.FlagHasItem == item.FlagHasItem || rule.FlagHasItem == null)
            && (rule.FlagIntermediate == item.FlagIntermediate || rule.FlagIntermediate == null)
            && (rule.FlagActionIsD == item.FlagActionIsD || rule.FlagActionIsD == null)
            && (rule.FlagDiscrepancies == item.FlagDiscrepancies || rule.FlagDiscrepancies == null)
            && (rule.FlagIsInTransit == item.FlagIsInTransit || rule.FlagIsInTransit == null);
        }
    }



    public class MessageCloseRunResponseView
    {
        public int MsgId { get; set; }
        public string MsgDetail { get; set; }
        public string MsgTitle { get; set; }
        public string JobNo { get; set; }
    }
}
