using System;

namespace Bgt.Ocean.Models.Customer
{
    public class CustomerView
    {
        public Guid Guid { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class CustomerJobHideView
    {
        public Guid SystemJobHideScreen_Guid { get; set; }
        public Guid? SystemJobHideField_Guid { get; set; }
    }

}
