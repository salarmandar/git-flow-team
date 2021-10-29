using Bgt.Ocean.Models.BaseModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Models.FleetMaintenance
{
    public class VehicleTypeView
    {
        private Guid Vehicle_Guid { get; set; }
        private string Vehicle_Name { get; set; }
    }

    public class VehicleBrandView
    {
        private Guid VehicleBrand_Guid { get; set; }

        private string VehicleBrand_Name { get; set; }
    }

    public class AccidentInfoView
    {
        public Guid Guid { get; set; }
        public DateTime? DateOfAccident { get; set; }
        public DateTime? TimeOfAccident { get; set; }
        public string BrinksDriver { get; set; }
        public string CounterParty { get; set; }
        public string Parties_DriverLicenseID { get; set; }
        public string MasterRunResourceBrandName { get; set; }
        public string MasterRunResourceTypeName { get; set; }
        public string Parties_RunResourceModel { get; set; }

        public bool FlagDisable { get; set; }

        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public DateTime? DatetimeModified { get; set; }
    }

    public class AccidentInfoViewRequest : PagingBase
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        [Required]
        public Guid RunResourceGuid { get; set; }
        [Required]
        public Guid SiteGuid { get; set; }
        public Guid? RunResourceBrandGuid { get; set; }
        public Guid? RunResourceTypeGuid { get; set; }
        public bool FlagBrinksIsFault { get; set; }
        public bool FlagShowAll { get; set; }
        public string CounterParty { get; set; }
    }

    public class AccidentBrinksDriverView
    {
        public Guid Guid { get; set; }
        public string DriverName { get; set; }
    }


}

