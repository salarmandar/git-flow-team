using System;

namespace Bgt.Ocean.Models.InternalDepartments
{
    /// <summary>
    /// Model for internal department
    /// </summary>
    public class InternalDepartmentView
    {
        /// <summary>
        /// TblMasterCustomerLocation.Guid 
        /// </summary>
        public Guid MasterCustomerLocation_Guid { get; set; }

        /// <summary>
        /// TblMasterCustomerLocation_InternalDepartment.[Guid]
        /// Also is Vault_Guid
        /// </summary>
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string SiteName { get; set; }
        public int InternalDepartmentTypeID { get; set; }
        public string InternalDepartmentTypeName { get; set; }
    }
}
