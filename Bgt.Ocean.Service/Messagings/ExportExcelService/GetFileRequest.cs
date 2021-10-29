using System;

namespace Bgt.Ocean.Service.Messagings.ExportExcelService
{
    public class GetFileRequest
    {
        /// <summary>
        /// File type as 'pdf', 'xlsx'
        /// </summary>
        public string FileType { get; set; }
        
        public int ReportStyleId { get; set; }
        public Guid? BrinkSiteGuid { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string UserName { get; set; }
    }
}
