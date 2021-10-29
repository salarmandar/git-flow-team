using System;

namespace Bgt.Ocean.Models.MasterRoute
{
    public class MasterRouteLogModel
    {
        public System.Guid Guid { get; set; }
        public string CategoryName { get; set; }
        public string LocationName { get; set; }
        public string Description { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? CreatedDatetime { get; set; }
        public string SystemMsgID { get; set; }
        public string JSONValue { get; set; }
        public int? SeqIndexShow { get; set; }
    }

    public class MasterRouteLogRequest
    {
        public System.Guid ReferenceValue_Guid { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string SystemMsgID { get; set; }
        public string JSONValue { get; set; }
    }
}
