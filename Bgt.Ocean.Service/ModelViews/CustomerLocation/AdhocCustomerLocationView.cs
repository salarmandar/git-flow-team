using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.CustomerLocation
{
    public class AdhocCustomerLocationRequest
    {
        public Guid? CustomerGuid { get; set; }
        public Guid? SiteGuid { get; set; }
        public Guid? DayOfWeek_Guid { get; set; }
        public Guid? JobType_Guid { get; set; }

        public Guid? subServiceType_Guid { get; set; }
        public Guid? LineOfBusiness_Guid { get; set; }
        public string strWorkDate { get; set; }
        public bool FlagDestination { get; set; }
        public Guid? CustomerLocaitonPK { get; set; }
        public Guid? SiteGuid_Del { get; set; }

        public string DateTimeFormat { get; set; }

    }
    public class AdhocCustomerLocationView
    {
        public Guid Guid { get; set; }
        public string BranchName { get; set; }
        public Guid? SystemCustomerOfType_Guid { get; set; }
        public string SystemCustomerOfTypeName { get; set; }
        public Guid? SystemCustomerLocationType_Guid { get; set; }
        public string CustomerLocationTypeName { get; set; }
        public Guid SiteGuid { get; set; }
        public string SiteCodeName { get; set; }
        public string ServiceHour { get; set; }
        public bool? FlagLocationDestination { get; set; }
        public bool FlagNonBillable { get; set; }
    }


}
