using System;

namespace Bgt.Ocean.Service.ModelViews.GenericLog
{
    public class TransactionGenericLogModel
    {
        public Guid SystemLogCategory_Guid { get; set; }
        public Guid SystemLogProcess_Guid { get; set; }
        public string ReferenceValue { get; set; }
        public string Description { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public string JSONValue { get; set; }
        public string SystemMsgID { get; set; }
        public string LabelIndex { get; set; }
    }
}
