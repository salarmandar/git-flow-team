using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.FileService;
using System;
using System.IO;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.File
{
    public interface IFileService
    {
        void SaveFileTemp(FileTempRequest request);
        void SaveFile(FileRequest request);
        MasterImageView GetFile(Guid table_guid);
        void RemoveFileTemp(Guid guid);
        MasterImageView GetFileById(Guid guid);
    }
    public class FileService : IFileService
    {
        private readonly IMasterImageTempRepository _masterImageTempRepository;
        private readonly IMasterImageRepository _masterImageRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;

        public FileService(
            IMasterImageTempRepository masterImageTempRepository,
            IMasterImageRepository masterImageRepository,
            IUnitOfWork<OceanDbEntities> uow
         )
        {
            _masterImageTempRepository = masterImageTempRepository;
            _masterImageRepository = masterImageRepository;
            _uow = uow;
        }

        public void SaveFileTemp(FileTempRequest request)
        {
            BinaryReader b = new BinaryReader(request.FileStream);
            byte[] binData = b.ReadBytes((int)request.FileStream.Length);

            TblMasterImageTemp imageTemp = new TblMasterImageTemp();
            imageTemp.Guid = request.Guid;
            imageTemp.FileName = request.FileName;
            imageTemp.FileType = request.FileType;
            imageTemp.Stream = binData;
            imageTemp.ExpiredDatatime = request.ExpiredDateTime;
            _masterImageTempRepository.Create(imageTemp);
            _uow.Commit();
        }

        public void RemoveFileTemp(Guid guid)
        {
            TblMasterImageTemp MasterImageTemp = _masterImageTempRepository.FindById(guid);
            if (MasterImageTemp != null)
            {
                _masterImageTempRepository.Remove(MasterImageTemp);
            }
            _uow.Commit();
        }

        public void SaveFile(FileRequest request)
        {
            TblMasterImageTemp MasterImageTemp = _masterImageTempRepository.FindById(request.Table_Guid);
            BinaryReader b = new BinaryReader(request.Content);
            byte[] bytes = MasterImageTemp != null ? MasterImageTemp.Stream : b.ReadBytes((int)request.Content.Length);
            TblMasterImage MasterImage = _masterImageRepository.FindAll(x => x.Table_Guid == request.Table_Guid && x.SystemFileType_Guid == request.SystemFileType_Guid).FirstOrDefault();
            if (MasterImage != null)
            {
                MasterImage.Content = bytes;
                MasterImage.ContentLength = bytes.Length;
                MasterImage.FileType = MasterImageTemp != null ? MasterImageTemp.FileType : request.FileType;
                MasterImage.FlagDisable = false;
                MasterImage.UserModified = request.UserName;
                MasterImage.DatetimeModified = request.ClientDateTime.DateTime;
                MasterImage.UniversalDatetimeModified = request.ClientDateTime;
                _masterImageRepository.Modify(MasterImage);
            }
            else
            {
                MasterImage = new TblMasterImage()
                {
                    Guid = Guid.NewGuid(),
                    SystemFileType_Guid = request.SystemFileType_Guid,
                    Table_Guid = request.Table_Guid,
                    FileType = MasterImageTemp != null ? MasterImageTemp.FileType : request.FileType,
                    FileName = request.FileName,
                    Content = bytes,
                    ContentLength = bytes.Length,
                    UserCreated = request.UserName,
                    FlagDisable = false,
                    UniversalDatetimeCreated = request.ClientDateTime,
                    DatetimeCreated = request.ClientDateTime.DateTime
                };
                _masterImageRepository.Create(MasterImage);
            }

            if(MasterImageTemp != null)
            {
                _masterImageTempRepository.Remove(MasterImageTemp);
            }
            _uow.Commit();
        }

        public MasterImageView GetFile(Guid table_guid)
        {
            return _masterImageRepository.FindByTableID(table_guid).ConvertToMasterImageView();
        }
        public MasterImageView GetFileById(Guid guid)
        {
            return _masterImageRepository.FindById(guid).ConvertToMasterImageView();
        }
    }
}
