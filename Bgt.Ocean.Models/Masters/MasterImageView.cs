using System;

namespace Bgt.Ocean.Models.Masters
{
    public class MasterImageView
    {
        public System.Guid Guid { get; set; }
        public System.Guid Table_Guid { get; set; }
        public System.Guid SystemFileType_Guid { get; set; }
        public byte[] Content { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<int> ContentLength { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}
