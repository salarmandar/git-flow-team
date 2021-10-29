using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using System;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    #region Interface

    public interface IGenericLogService : IBaseAuditLogService
    {
       
    }

    #endregion

    public class GenericLogService : BaseAuditLogService<ISFOTransactionGenericLogRepository, SFOTblTransactionGenericLog, SFOLogDbEntities>, IGenericLogService
    {
        public GenericLogService(ISFOTransactionGenericLogRepository logRepository, ISystemLog_HistoryErrorRepository systemLogHistoryErrorRepository, IUnitOfWork<SFOLogDbEntities> uowLogDb) : base(logRepository, systemLogHistoryErrorRepository, uowLogDb)
        {
        }

        protected override SFOTblTransactionGenericLog GetNewLogModel(TransactionGenericLogModel transactionGenericLogModel)
        {
            SFOTblTransactionGenericLog logModel = new SFOTblTransactionGenericLog();
            logModel.Guid = Guid.NewGuid();
            logModel.SystemLogCategory_Guid = transactionGenericLogModel.SystemLogCategory_Guid;
            logModel.SystemLogProcess_Guid = transactionGenericLogModel.SystemLogProcess_Guid;
            logModel.ReferenceValue = transactionGenericLogModel.ReferenceValue;
            logModel.Description = transactionGenericLogModel.Description;
            logModel.DateTimeCreated = transactionGenericLogModel.DateTimeCreated ?? DateTime.Now;
            logModel.UniversalDatetimeCreated = DateTime.UtcNow;
            logModel.UserCreated = transactionGenericLogModel.UserCreated;
            logModel.JSONValue = transactionGenericLogModel.JSONValue;
            logModel.SystemMsgID = transactionGenericLogModel.SystemMsgID;
            logModel.LabelIndex = transactionGenericLogModel.LabelIndex;
            return logModel;
        }        
    }
}
