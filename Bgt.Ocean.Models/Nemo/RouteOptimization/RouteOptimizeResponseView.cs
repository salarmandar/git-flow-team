using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class RouteOptimizeResponseView
    {
        public NemoOptimizationResponse NemoOptimizationResponse { get; set; } = new NemoOptimizationResponse();

        public int TransactionID { get; set; }
        public Guid NemoTaskGuid { get; set; }
    }
}
