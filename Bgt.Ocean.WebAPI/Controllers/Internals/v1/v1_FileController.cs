using Bgt.Ocean.Service.Implementations.File;
using Bgt.Ocean.Service.Messagings.FileService;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_FileController : ApiControllerBase
    {
        // GET: v1_File
        private readonly IFileService _fileService;
        public v1_FileController(
            IFileService fileService)
        {
            _fileService = fileService;
        }
        /// <summary>
        /// Save temp file
        /// </summary>
        /// <param name="inputFileName">Name of input file</param>
        /// <returns></returns>
        [HttpPost]
        public Guid SaveFileTemp(string inputFileName)
        {
            FileTempRequest fileRequest = new FileTempRequest();
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var httpPostedFile = HttpContext.Current.Request.Files[inputFileName];
                if (httpPostedFile != null)
                {
                    fileRequest.FileName = httpPostedFile.FileName;
                    fileRequest.FileType = httpPostedFile.ContentType;
                    fileRequest.FileStream = httpPostedFile.InputStream;
                    _fileService.SaveFileTemp(fileRequest);
                }
            }
            return fileRequest.Guid;
        }
        /// <summary>
        /// for remove temp file
        /// </summary>
        /// <param name="guid">guid of image temp table</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage RemoveFileTemp(Guid guid)
        {
            _fileService.RemoveFileTemp(guid);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet]
        public HttpResponseMessage DownloadFileByTableId(Guid tableGuid)
        {
            var masterImage = _fileService.GetFile(tableGuid);
            FileResponse response = new FileResponse();
            response.ContentBase64 = Convert.ToBase64String(masterImage.Content);
            response.FileName = masterImage.FileName;
            response.FileType = masterImage.FileType;
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage DownloadFile(Guid guid)
        {
            var masterImage = _fileService.GetFileById(guid);
            FileResponse response = new FileResponse();
            response.ContentBase64 = Convert.ToBase64String(masterImage.Content);
            response.FileName = masterImage.FileName;
            response.FileType = masterImage.FileType;
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}