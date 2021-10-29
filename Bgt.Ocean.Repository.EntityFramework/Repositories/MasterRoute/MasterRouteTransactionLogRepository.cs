using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.MasterRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumMasterRoute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute
{
    public interface IMasterRouteTransactionLogRepository : IRepository<TblMasterRouteTransactionLog>
    {
       void InsertMasterRouteTransactionLog(List<MasterRouteLogRequest> requestList, EnumMasterRouteLogCategory category, EnumMasterRouteProcess process);
        void MasterRouteTransactionLog_Create(MasterRouteLogRequest request, EnumMasterRouteLogCategory category, EnumMasterRouteProcess process);

    }
    public class MasterRouteTransactionLogRepository : Repository<OceanDbEntities, TblMasterRouteTransactionLog>, IMasterRouteTransactionLogRepository
    {
        public MasterRouteTransactionLogRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        #region Insert audit log : [TFS#47783] - master route job
        public void InsertMasterRouteTransactionLog(List<MasterRouteLogRequest> requestList, EnumMasterRouteLogCategory category, EnumMasterRouteProcess process)
        {
            string desCategory = EnumHelper.GetDescription(category);
            string desProcess = EnumHelper.GetDescription(process);
            var categoryGuid = DbContext.SFOTblSystemLogCategory.FirstOrDefault(e => e.CategoryCode == desCategory)?.Guid;
            var processGuid = DbContext.SFOTblSystemLogProcess.FirstOrDefault(e => e.ProcessCode == desProcess)?.Guid;

            if (requestList.Any() && categoryGuid.HasValue && processGuid.HasValue)
            {
                var newLog = requestList.Select(o => new TblMasterRouteTransactionLog()
                {
                    Guid = Guid.NewGuid(),
                    SystemLogCategory_Guid = categoryGuid ?? Guid.Empty,
                    SystemLogProcess_Guid = processGuid ?? Guid.Empty,
                    ReferenceValue_Guid = o.ReferenceValue_Guid,
                    UserCreated = o.UserName,
                    DatetimeCreated = o.DatetimeCreated,
                    UniversalDatetimeCreated = o.UniversalDatetimeCreated,
                    Remark = o.Remark,
                    SystemMsgID = o.SystemMsgID,
                    JSONValue = o.JSONValue
                });
                DbContext.TblMasterRouteTransactionLog.AddRange(newLog);
            }
            DbContext.SaveChanges();
        }
        public void MasterRouteTransactionLog_Create(MasterRouteLogRequest request, EnumMasterRouteLogCategory category, EnumMasterRouteProcess process)
        {
                string desCategory = EnumHelper.GetDescription(category);
                string desProcess = EnumHelper.GetDescription(process);
                var categoryGuid = DbContext.SFOTblSystemLogCategory.FirstOrDefault(e => e.CategoryCode == desCategory)?.Guid;
                var processGuid = DbContext.SFOTblSystemLogProcess.FirstOrDefault(e => e.ProcessCode == desProcess)?.Guid;

                if (request != null && categoryGuid.HasValue && processGuid.HasValue)
                {
                    var newLog = new TblMasterRouteTransactionLog()
                    {
                        Guid = Guid.NewGuid(),
                        SystemLogCategory_Guid = categoryGuid ?? Guid.Empty,
                        SystemLogProcess_Guid = processGuid ?? Guid.Empty,
                        ReferenceValue_Guid = request.ReferenceValue_Guid,
                        UserCreated = request.UserName,
                        DatetimeCreated = request.DatetimeCreated,
                        UniversalDatetimeCreated = request.UniversalDatetimeCreated,
                        Remark = request.Remark,
                        SystemMsgID = request.SystemMsgID,
                        JSONValue = request.JSONValue
                    };
                    DbContext.TblMasterRouteTransactionLog.Add(newLog);
                }
                DbContext.SaveChanges();
        }
        #endregion
    }
}
