using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog
{
    public class CompareResult
    {
        public bool FlagIsChange { get { return ChangeInfoList != null && ChangeInfoList.Any(); } }
        public IEnumerable<ChangeInfo> ChangeInfoList { get; set; } = new List<ChangeInfo>();

        public ChangeInfo GetChangeInfo(string fieldName) => ChangeInfoList?.FirstOrDefault(e => e.FieldName == fieldName);
    }
}
