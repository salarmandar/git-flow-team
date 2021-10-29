using Bgt.Ocean.Infrastructure.CustomAttributes;
using System;

namespace Bgt.Ocean.Service.ModelViews.FleetMaintenanceViewModel.FleetAccidentViewModel
{
    public class AccidentDetailModelView
    {
        // Accident date
        public DateTime DateOfAccident { get; set; }
        //Accident time
        public string TimeOfAccident { get; set; }
        //brink's driver
        public Guid EmployeeGuid { get; set; }
        //Brink's is at fault
        public bool? FlagBrinksIsFault { get; set; }
        //Personal injury
        public bool? FlagPersonalInjury { get; set; }
        //Insurance Covered
        public decimal? Indemnity { get; set; }
        public Guid? CurrencyIndemnityGuid { get; set; }
        //Total Liability / Cost to Brink's
        public decimal? ClaimExpense { get; set; }
        public string ClaimReference { get; set; }
        public Guid? CurrencyClaimExpenseGuid { get; set; }
        public string DescriptionOfAccident { get; set; }
        public string StrDateOfAccident { get; set; }

    }
    public class CounterpartyDetailModelView
    {
        public Guid TitleNameGuid { get; set; }
        public string PartiesFirstName { get; set; }
        public string PartiesMiddleName { get; set; }
        public string PartiesLastName { get; set; }
        //Driver License ID
        public string DriverLicenseID { get; set; }
        //License Plate
        public string EmployeeRegistrationID { get; set; }
        //Car Registration ID
        public string CarRegistrationDocumentNumber { get; set; }
        //Vehicle Type
        public Guid? RunResourceBrandGuid { get; set; }
        //Vehicle Brand
        public Guid? RunResourceTypeGuid { get; set; }
        public string RunResourceModel { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string InsuranceReferenceIDNumber { get; set; }
        public DateTime? InsuranceValidityDate { get; set; }

        public string StrInsuranceValidityDate { get; set; }
    }

    public class AccidentListDetailDamagedModelView
    {
        public Guid? Guid { get; set; }
        public Guid AccidentGuid { get; set; }
        public string AccidentItemsDetail { get; set; }
    }
    public class FleetMaintenanceAccidentImagesModelView
    {
        public Guid Guid { get; set; }
        public Guid AccidentGuid { get; set; }
        public string PathImage { get; set; }

    }
    public class ImageRunResourceView
    {
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public Guid? Guid { get; set; }
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public Guid MasterRunResourceImages_Guid { get; set; }
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public int IndexOrder { get; set; }
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public string PathImage { get; set; }
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public string FileName { get; set; }
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public Guid RunResource_FileImageType { get; set; }
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public string RunResourceImageBase64 { get; set; }
        [AvoidAuditLog(Description = "Disable"), IgnoreJsonSerialize]
        public string FileType { get; set; }
        public byte[] ItemFileByte { get; set; }

    }

    public class AccidentImageModelView
    {
        public Guid ImageGuid { get; set; }
        /// <summary>
        /// True = Data From TblMasterRunResource_Accident_Images
        /// False = Data From TblMasterImageTemp
        /// </summary>
        public bool IsActualImage { get; set; }
    }
}

