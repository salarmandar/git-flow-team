using System;

namespace Bgt.Ocean.Service.ModelViews.Reports
{
    public class ReportProductivityCollection
    {
        public string FileType { get; set; }
        public int ReportStyleId { get; set; }
        public Guid? BrinkSiteGuid { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string UserName { get; set; }
    }
}
