using System;

namespace Bgt.Ocean.Service.ModelViews.Systems
{
    public class LineOfBusinessAndJobType
    {
        public Guid LOBGuid { get; set; }
        public int? InternalID { get; set; }
        public string LOBFullName { get; set; }
        public string LOBAbbrevaitionName { get; set; }
        public Guid JobTypeGuid { get; set; }
        public int? ServiceJobTypeID { get; set; }
        public string JobTypeName { get; set; }
        public string JobTypeNameAbb { get; set; }
        public string DisplayTextJobTypeName { get; set; }
        public string DisplayTextTypeNameAbb { get; set; }
    }
}
