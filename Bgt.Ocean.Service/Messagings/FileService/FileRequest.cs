using System;
using System.IO;

namespace Bgt.Ocean.Service.Messagings.FileService
{
    public class FileRequest : RequestBase
    {
        public Guid? Guid { get; set; }
        public Guid Table_Guid { get; set; }
        public Guid SystemFileType_Guid { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public Stream Content { get; set; }
      
    }
}
