using System;

namespace Bgt.Ocean.Models.OTCManagement
{
  public  class GetOTCRequestModel
    {
        public Guid MasterActualJobHeader_Guid { get; set; }
        public string ReferenceCode { get; set; }
        public string MachineLockID { get; set; }
        public string SerialNumber { get; set; }
        public Guid? MasterEmployee_Guid { get; set; }
        public Guid? MasterEmployee_Guid2 { get; set; }
        public string LockMode { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string DateStr { get; set; }
        public Nullable<int> Hour { get; set; }
        public Nullable<int> TimeBlock { get; set; }
    }
}
