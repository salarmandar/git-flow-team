using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.MasterRoute;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Bgt.Ocean.Infrastructure.Util.EnumMasterRoute;

namespace Bgt.Ocean.Service.Implementations.MasterRoute
{
    public interface IMasterRouteTransactionLogService
    {
        void InsertMasterRouteTransactionLogAsync(EnumMasterRouteLogCategory category, EnumMasterRouteProcess process, params MasterRouteLogRequest[] requestList);
    }
    public class MasterRouteTransactionLogService : IMasterRouteTransactionLogService
    {
        private readonly SystemService _systemService;

        public MasterRouteTransactionLogService(SystemService systemService)
        {
            _systemService = systemService;
        }
        #region Insert audit log : [TFS#47783] - master route job
        public void InsertMasterRouteTransactionLogAsync(EnumMasterRouteLogCategory category, EnumMasterRouteProcess process, params MasterRouteLogRequest[] requestList)
        {
            Task.Run(() => InsertMasterRouteTransactionLog(category, process, requestList));
        }
        private void InsertMasterRouteTransactionLog(EnumMasterRouteLogCategory category, EnumMasterRouteProcess process, params MasterRouteLogRequest[] requestList)
        {
            string desCategory = EnumHelper.GetDescription(category);
            string desProcess = EnumHelper.GetDescription(process);
            using (var dbContext = new OceanDbEntities())
            {
                try
                {
                    var categoryGuid = dbContext.SFOTblSystemLogCategory.FirstOrDefault(e => e.CategoryCode == desCategory)?.Guid;
                    var processGuid = dbContext.SFOTblSystemLogProcess.FirstOrDefault(e => e.ProcessCode == desProcess)?.Guid;

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
                        dbContext.TblMasterRouteTransactionLog.AddRange(newLog);
                    }
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    _systemService.CreateHistoryErrorAsync(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress,SystemHelper.ClientHostName);
                }
            }
        }

        #endregion
    }
}
