using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.WebAPI.Models.AuditLog;
using System.Linq;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1.AuditLog
{
    public abstract class BaseAuditLogController : ApiControllerBase
    {
        protected readonly IBaseAuditLogService _logService;
        protected readonly IObjectComparerService _objectComparerService;
        protected readonly ISystemService _systemService;

        public BaseAuditLogController(
                IObjectComparerService objectComparerService,
                IBaseAuditLogService logService,
                ISystemService systemService
            )
        {
            _objectComparerService = objectComparerService;
            _logService = logService;
            _systemService = systemService;
        }
        protected virtual BaseResponse CreateLogUpdateData<TRequest, TSource, TTarget>(TRequest request)
            where TSource : class
            where TTarget : class
            where TRequest : AuditLogUpdateModel<TSource, TTarget>
        {
            var result = _objectComparerService.GetCompareResult(request.SourceModel, request.TargetModel, request.ConfigKey);

            if (result.FlagIsChange)
            {
                var msgChange = _systemService.GetMessageByMsgId(663).MessageTextContent;
                var msgEmpty = $"({_systemService.GetMessageByMsgId(674).MessageTextContent})";

                _logService.BulkInsertTransactionGenericLog(result.ChangeInfoList
                                                .Where(e => e.LogCategoryGuid.HasValue 
                                                && e.LogProcessGuid.HasValue)
                                                .Select(e => new Service.ModelViews.GenericLog.TransactionGenericLogModel
                {
                    Description =
                        string.Format(msgChange, request.PrefixDescription, e.LabelKey,
                            e.OldValue.IsEmpty() ? msgEmpty : e.OldValue,
                            e.NewValue.IsEmpty() ? msgEmpty : e.NewValue),
                    ReferenceValue = request.ReferenceValue,
                    SystemLogCategory_Guid = e.LogCategoryGuid.Value,
                    SystemLogProcess_Guid = e.LogProcessGuid.Value,
                    UserCreated = request.UserCreated
                }));
            }

            return GetSuccessMessage("Create Log Complete");
        }

        protected BaseResponse GetSuccessMessage(string msg) => new BaseResponse { IsSuccess = true, Message = msg };
    }
}