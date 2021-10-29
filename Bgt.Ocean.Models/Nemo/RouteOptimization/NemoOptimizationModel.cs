using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationModel
    {
        /// <summary>
        /// Gets o sets the list of daily Run Resources.
        /// </summary>
        public List<Guid> DailyRunResources { get; set; } = new List<Guid>();


        /// <summary>
        /// Gets o sets the list of master routes.
        /// </summary>
        public List<RouteGroupDetailByRouteResult> MasterRouteResources { get; set; } = new List<RouteGroupDetailByRouteResult>();

        /// <summary>
        /// Gets o sets the list of UnassignedStops.
        /// </summary>
        public List<OptimizeNode> UnassignedStops { get; set; } = new List<OptimizeNode>();

        /// <summary>
        /// Gets o sets the RouteOptimizationType, single or multiple.
        /// </summary>
        public int RouteOptimizationType { get; set; }

        /// <summary>
        /// Gets o sets the user name who makes the request.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets o sets the Language of user name who makes the request.
        /// </summary>
        public Guid LanguageGuid { get; set; }

        /// <summary>
        /// Gets o sets the list of daily Run Resources with Jobs.
        /// This list should have a quantity lesss or equal than the quantity of item in the property DailyRunResources
        /// </summary>
        public List<NemoRouteOptimizationObject> DailyRunWithJobs { get; set; } = new List<NemoRouteOptimizationObject>();

        /// <summary>
        /// Gets o sets the corresponding Master Site for the request.
        /// </summary>
        public Guid MasterSiteGuid { get; set; }
    }
}
