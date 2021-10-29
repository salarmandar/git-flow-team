using System;

namespace Bgt.Ocean.Service.ModelViews.Customer
{
    public class AccountView
    {
        public Guid Guid { get; set; }

        /// <summary>
        /// Main Customer
        /// </summary>
        public Guid MasterCustomer_Guid { get; set; }
        public string MasterCustomer_Code { get; set; }

        /// <summary>
        /// TblMasterCustomer (Account)
        /// </summary>
        public Guid? Account_Guid { get; set; }

        /// <summary>
        /// TblMasterCustomer (Account)
        /// </summary>
        public string AccountCustomerName { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public Guid MasterCountry_Guid { get; set; }
    }
}
