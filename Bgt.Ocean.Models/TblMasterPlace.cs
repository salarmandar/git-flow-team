//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bgt.Ocean.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblMasterPlace
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblMasterPlace()
        {
            this.TblMasterCustomerLocation = new HashSet<TblMasterCustomerLocation>();
        }
    
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public Nullable<System.Guid> MasterCity_Guid { get; set; }
        public string MasterCity_Name { get; set; }
        public string BuildingName { get; set; }
        public string BuildingAddress { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTimeOffset> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<System.Guid> MasterCountry_State_Guid { get; set; }
        public Nullable<System.Guid> MasterDistrict_Guid { get; set; }
        public string Postcode { get; set; }
        public string StateName { get; set; }
        public string CitryName { get; set; }
        public bool FlagBulkProcesses { get; set; }
        public string MaximumTime { get; set; }
        public Nullable<System.Guid> MasterGeoZone_Guid { get; set; }
        public bool FlagLocken { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string PlaceID { get; set; }
        public string ReferenceId { get; set; }
        public Nullable<System.Guid> SystemPlaceType_Guid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterCustomerLocation> TblMasterCustomerLocation { get; set; }
    }
}
