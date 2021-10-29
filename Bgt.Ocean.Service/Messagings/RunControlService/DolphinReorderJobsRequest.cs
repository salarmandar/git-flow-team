using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class DolphinReorderJobsRequest
    {
        public DolphinAuthen authen { get; set; }
        public string action { get; set; }
        public string datetime { get; set; }
        public string routeName { get; set; }
        public string userAction { get; set; }
        public Guid? runResourceDailyGuid { get; set; }
        public IEnumerable<ReorderJobsView> reOrder { get; set; }
    }
    public class DolphinAuthen
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class ReorderJobsView
    {
        public Guid? legGuid { get; set; }
        public int? jobOrder { get; set; }
    }

    public class UpdateStatusSyncToDolphinRequest
    {
        public IEnumerable<Guid?> ListJobGuid { get; set; }
        public int SyncStatusDolphin { get; set; }
        public DateTime ClientDateTime { get; set; }
        public string UserModified { get; set; }
    }

    public class SyncToDolphinRequest : UpdateStatusSyncToDolphinRequest
    {
        public int MsgId { get; set; }
        public Guid? DailyRunGuid { get; set; }
        public bool Success { get; set; }
    }

}
