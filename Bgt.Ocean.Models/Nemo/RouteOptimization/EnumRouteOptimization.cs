
namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class EnumRouteOptimization
    {
        public enum TypeOptimization
        {
            Create = 0,
            Reorder = 1,
            AddMore = 2,
            Import = 3,
            Resequence = 4,
            DynamicRoute = 5,
            Schedule = 6
        }

        public enum OptimizationStatus
        {
            Optimizing = 1,
            Optimized = 2,
            Accepted = 3,
            Cancelled = 4,
            Errors = 5
        }

        public enum NemoUrlType
        {
            Authentication = 0,
            Optmization = 1
        }

    }
}
