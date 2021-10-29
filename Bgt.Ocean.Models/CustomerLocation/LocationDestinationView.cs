using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.CustomerLocation
{
    public class MultiBrDetailView
    {
        public IEnumerable<DropdownListSiteView> SiteList { get; set; } = new List<DropdownListSiteView>();
        public IEnumerable<DropdownCustomerView> CustomerList { get; set; } = new List<DropdownCustomerView>();
        public IEnumerable<DropdownLocationView> LocationList { get; set; } = new List<DropdownLocationView>();
        public IEnumerable<InternalDepartmentView> InternalDepartmentList { get; set; } = new List<InternalDepartmentView>();
        public IEnumerable<DropdownSitePathView> SitePathList { get; set; } = new List<DropdownSitePathView>();
    }

    public class DropdownCustomerView
    {
        public Guid? SiteGuid { get; set; }
        public string SiteName { get; set; }
        public Guid? CustomerGuid { get; set; }
        public string CustomerName { get; set; }
    }

    public class DropdownLocationView
    {
        public Guid? SiteGuid { get; set; }
        public string SiteName { get; set; }
        public Guid? CustomerGuid { get; set; }
        public string CustomerName { get; set; }
        public Guid? LocationGuid { get; set; }
        public string LocationName { get; set; }
        public bool FlagNonBillable { get; set; }
    }

    public class DropdownListSiteView
    {
        public Guid? SiteGuid { get; set; }
        public string SiteName { get; set; }
        public Guid? CompanyGuid { get; set; }
    }

    public class DropdownSitePathView
    {
        public Guid? SiteGuid { get; set; }
        public string SiteName { get; set; }
        public Guid? CustomerGuid { get; set; }
        public string CustomerName { get; set; }
        public Guid? LocationGuid { get; set; }
        public string LocationName { get; set; }
        public Guid? SitePath_Guid { get; set; }
        public string SitePathName { get; set; }
        public Guid? InternalDepartment_Guid { get; set; }
        public Guid? InternalDepartmentName { get; set; }
    }

    public class InternalDepartmentView
    {
        public int OnwardDestinationTypeID { get; set; }
        public Guid? SiteGuid { get; set; }
        public string SiteName { get; set; }
        public Guid? InternalDepartment_Guid { get; set; }
        public string InternalDepartmentName { get; set; }

    }    
}
