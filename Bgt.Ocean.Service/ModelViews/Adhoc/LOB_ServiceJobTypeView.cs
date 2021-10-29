using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.ModelViews.Adhoc
{
    public class Adhoc_LOB_ServiceJobTypeView
    {
        public List<Adhoc_LOB> list_LOB = new List<Adhoc_LOB>();

        public List<Adhoc_LOB_ServiceJobType> list_LOB_ServiceType = new List<Adhoc_LOB_ServiceJobType>();
    }


    public class Adhoc_LOB_ServiceJobType
    {
        public Guid LOBGuid { get; set; }
        //public int? ServiceJobTypeID { get; set; }
        //public string ServiceJobTypeName { get; set; }
        //public string ServiceJobTypeNameAbb { get; set; }
        public string TypeName_DisplayText { get; set; }
        //public string TypeNameAbb_DisplayText { get; set; }
        public Guid JobTypeGuid { get; set; }
    }

    public class Adhoc_LOB
    {
        public Guid LOBGuid { get; set; }

        public string LOBFullName { get; set; }
    }
}
