using System;

namespace Bgt.Ocean.Models.Systems
{
    public class SystemApplicationView
    {
        public Guid Guid { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public Nullable<Guid> TokenID { get; set; }
    }
}
