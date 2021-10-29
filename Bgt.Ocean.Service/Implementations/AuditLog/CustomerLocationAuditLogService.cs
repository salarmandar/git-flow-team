using System;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.AuditLog;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    #region Interface

    public interface ICustomerLocationAuditLogService : IBaseAuditLogService
    {

    }

    #endregion

    public class CustomerLocationAuditLogService : BaseAuditLogService<ISystemCustomerLocationAuditLogRepository, TblSystemCustomerLocation_Audit_Log, SFOLogDbEntities>, ICustomerLocationAuditLogService
    {
        public CustomerLocationAuditLogService(ISystemCustomerLocationAuditLogRepository logRepository, ISystemLog_HistoryErrorRepository systemLogHistoryErrorRepository, IUnitOfWork<SFOLogDbEntities> uowLogDb) : base(logRepository, systemLogHistoryErrorRepository, uowLogDb)
        {
        }

        protected override TblSystemCustomerLocation_Audit_Log GetNewLogModel(TransactionGenericLogModel transactionGenericLogModel)
        {
            TblSystemCustomerLocation_Audit_Log logModel = new TblSystemCustomerLocation_Audit_Log();
            logModel.Guid = Guid.NewGuid();
            logModel.SystemLogCategory_Guid = transactionGenericLogModel.SystemLogCategory_Guid;
            logModel.SystemLogProcess_Guid = transactionGenericLogModel.SystemLogProcess_Guid;
            logModel.CustomerLocation_Guid = transactionGenericLogModel.ReferenceValue;
            logModel.Description = transactionGenericLogModel.Description;
            logModel.DateTimeCreated = DateTime.Now;
            logModel.UniversalDatetimeCreated = DateTime.UtcNow;
            logModel.UserCreated = transactionGenericLogModel.UserCreated;

            return logModel;
        }
    }
}
