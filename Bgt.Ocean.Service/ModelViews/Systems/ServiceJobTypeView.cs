using System;

namespace Bgt.Ocean.Service.ModelViews.Systems
{
    public class ServiceJobTypeView
    {
        public Guid Guid { get; set; }
        public int? ServiceJobTypeID { get; set; }
        public string ServiceJobTypeName { get; set; }
        public string ServiceJobTypeNameAbb { get; set; }
        public string TypeName_DisplayText { get; set; }
        public string TypeNameAbb_DisplayText { get; set; }
        public Guid SystemLineOfBusiness_Guid { get; set; }
    }
}
