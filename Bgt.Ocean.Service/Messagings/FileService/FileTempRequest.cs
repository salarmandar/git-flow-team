using System;
using System.IO;

namespace Bgt.Ocean.Service.Messagings.FileService
{
    public class FileTempRequest
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// File type as 'pdf', 'xlsx'
        /// </summary>
        public string FileType { get; set; }
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
        /// <summary>
        /// File expired in 30 mins.
        /// </summary>
        public DateTime ExpiredDateTime { get; set; } = DateTime.Now.AddMinutes(30);
    }
}
