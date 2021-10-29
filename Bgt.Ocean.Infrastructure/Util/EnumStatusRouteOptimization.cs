
namespace Bgt.Ocean.Infrastructure.Util
{
    public static class EnumStatusRouteOptimization
    {
        public enum StatusName
        {
            [EnumDescription("Pending Sync Nemo")]
            PendingSyncNemo = 1,
            [EnumDescription("Task Nemo Error")]
            TaskNemoError = 2,
            [EnumDescription("Task Nemo Success")]
            TaskNemoSuccess = 3,
            [EnumDescription("Proposed Solution Saved")]
            ProposedSolutionSaved = 4,
            [EnumDescription("Proposed Solution Discarded")]
            ProposedSolutionDiscarded = 5
        }
    }
}
