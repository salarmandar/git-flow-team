using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Models.Reports
{
    public class ExportFileResponse
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] ReportData { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessge { get; set; }
    }
}
