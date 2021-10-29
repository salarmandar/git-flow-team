using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using System.Data.Entity;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    public interface IBaseAuditLogService
    {
        /// <summary>
        /// Add generic log (This method will commit DbContext automatically)
        /// </summary>
        /// <param name="transactionGenericLogModel"></param>
        void InsertTransactionGenericLog(TransactionGenericLogModel transactionGenericLogModel);
        void BulkInsertTransactionGenericLog(IEnumerable<TransactionGenericLogModel> transactionGenericLogModelList);
    }

    public abstract class BaseAuditLogService<TLogRepository, TEntity, TDbContext> : IBaseAuditLogService
        where TEntity : class, new()
        where TDbContext : DbContext
        where TLogRepository : IRepository<TEntity>
    {
        private readonly TLogRepository _logRepository;
        private readonly ISystemLog_HistoryErrorRepository _systemLogHistoryErrorRepository;
        private readonly IUnitOfWork<TDbContext> _uowLogDb;

        public BaseAuditLogService(
                TLogRepository logRepository,
                ISystemLog_HistoryErrorRepository systemLogHistoryErrorRepository,
                IUnitOfWork<TDbContext> uowLogDb
            )
        {
            _logRepository = logRepository;
            _systemLogHistoryErrorRepository = systemLogHistoryErrorRepository;
            _uowLogDb = uowLogDb;
        }

        public void InsertTransactionGenericLog(TransactionGenericLogModel transactionGenericLogModel)
        {
            try
            {
                var newLogModel = GetNewLogModel(transactionGenericLogModel);
                _logRepository.Create(newLogModel);
                _uowLogDb.Commit();
            }
            catch (Exception err)
            {
                _systemLogHistoryErrorRepository.CreateHistoryError(err);
            }
        }

        public void BulkInsertTransactionGenericLog(IEnumerable<TransactionGenericLogModel> transactionGenericLogModelList)
        {
            try
            {
                var logList = transactionGenericLogModelList.Select(GetNewLogModel);
                _logRepository.CreateRange(logList);
                _uowLogDb.Commit();
            }
            catch (Exception err)
            {
                _systemLogHistoryErrorRepository.CreateHistoryError(err);
            }
        }

        protected abstract TEntity GetNewLogModel(TransactionGenericLogModel transactionGenericLogModel);
        
    }
}
