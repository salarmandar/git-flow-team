
namespace Bgt.Ocean.Service.Messagings.FileService
{
    public class FileResponse : RequestBase
    {
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string ContentBase64 { get; set; }
      
    }
}
