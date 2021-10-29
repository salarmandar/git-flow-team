using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.WebAPI.Controllers.Internals.v1.AuditLog;
using Bgt.Ocean.WebAPI.Models.AuditLog.CustomerLocation;
using System;
using System.Web.Http;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.ModelViews.GenericLog;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_AuditLogCustomerLocationController : BaseAuditLogController
    {

        public v1_AuditLogCustomerLocationController(IObjectComparerService objectComparerService, ICustomerLocationAuditLogService customerLocationAuditLogService, ISystemService systemService) : base(objectComparerService, customerLocationAuditLogService, systemService)
        {
        }

        [HttpPost]
        public BaseResponse CreateDetail(AuditLogCustomerLocationDetailModel request)
            => CreateLogUpdateData<AuditLogCustomerLocationDetailModel, TblMasterCustomerLocation, TblMasterCustomerLocation>(request);

        [HttpPost]
        public BaseResponse CreateAction(AuditLogCustomerLocationActionModel request)
        {
            var genericLogList = new TransactionGenericLogModel();
            genericLogList.DateTimeCreated = DateTime.UtcNow;
            genericLogList.JSONValue = Helpers.JsonHelper.GetJSONStringByArray<string>(request.LocationName, request.Action);
            genericLogList.LabelIndex = null;
            genericLogList.ReferenceValue = request.ReferenceValue;
            genericLogList.SystemLogCategory_Guid = SFOLogCategoryHelper.STD_CUSLOC_DETAIL.ToGuid();
            genericLogList.SystemLogProcess_Guid = SFOProcessHelper.STD_CustomerLocation.ToGuid();
            genericLogList.SystemMsgID = "2104";
            genericLogList.UserCreated = request.UserCreated;
            _logService.InsertTransactionGenericLog(genericLogList);

            return GetSuccessMessage("Log Complete");
        }
    }
}