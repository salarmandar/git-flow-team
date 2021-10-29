using System;

namespace Bgt.Ocean.Service.Messagings.PreVault
{
    public class InternalDepartmentResponse
    {
        public Guid InternalDepartment_Guid { get; set; }
        public string InternalDepartmentName { get; set; }
        public Guid InternalDepartmentType { get; set; }    
        public string InternalDepartmentTypeName { get; set; }
        public int? InternalDepartmentID { get; set; }  
    }
}
