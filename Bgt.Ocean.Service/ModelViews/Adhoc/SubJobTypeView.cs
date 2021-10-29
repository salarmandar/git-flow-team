using System;

namespace Bgt.Ocean.Service.ModelViews.Adhoc
{
    public class SubJobTypeView
    {
        public Guid JobTypeGuid { get; set; }
        public int? ServiceJobTypeID { get; set; }
        public string JobTypeName { get; set; }
        public Guid SubJobTypeGuid { get; set; }
        public string SubServiceTypeName { get; set; }
    }
}
