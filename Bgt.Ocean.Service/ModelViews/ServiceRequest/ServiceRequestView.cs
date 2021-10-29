using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.ServiceRequest
{
    public class ServiceRequestView
    {
        public Guid CountryGuid { get; set; }
        public Guid BrinksCompanyGuid { get; set; }
        public string BrinkSiteCode { get; set; }
        public Guid BrinkSiteGuid { get; set; }
        public Guid CustomerGuid { get; set; }
        public string DateTimeNotifiedTime { get; set; }
        public Guid MachineGuid { get; set; }
        public DateTime? DateTimeServiceDate { get; set; }
        public Guid ServiceTypeGuid { get; set; }
        public int ServiceTypeId { get; set; }
        public int StatusId { get; set; }
        public string TicketNumber { get; set; }

        public Guid ProblemGuid { get; set; }


        #region Information

        public DateTime DateTimeNotified { get; set; }
        public Guid TicketStatusGuid { get; set; }
        public Guid? PriorityGuid { get; set; }
        public Guid? OpenSourceGuid { get; set; }
        public Guid? CloseChannelGuid { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public string ResponderRefNum { get; set; }
        public string ReportedIncidentDescription { get; set; }

        public bool? FlagNotify { get; set; }
        public DateTime? DateTimeETA { get; set; }
        public DateTime? DateTimeDown { get; set; }
        public string PACNCode { get; set; }
        public bool? FlagClosedOnFirstContact { get; set; }
        public string BrinksLog { get; set; }
        public bool? FlagNonBillableToCustomer { get; set; }
        public bool? FlagNonBillableToBranch { get; set; }
        public bool? FlagSpecialCharges { get; set; }
        public decimal? ChargeAmount { get; set; }
        public bool? FlagReScheduledApproved { get; set; }
        public string ReasonName { get; set; }
        public string ResolveBy { get; set; }
        public bool? FlagStampDispatch { get; set; }
        public bool? FlagStampAccept { get; set; }
        public bool? FlagStampArrive { get; set; }
        public bool? FlagStampRestore { get; set; }
        public bool? FlagStampDepart { get; set; }
        public bool? FlagStampReturn { get; set; }
        public string ResponderName { get; set; }
        public string ReportedDescription { get; set; }
        public bool? FlagStampReportedOnsite { get; set; }
        public bool? FlagStampETA { get; set; }
        public string RescheduleReason { get; set; }
        public string ResponderEmail { get; set; }
        public string ResponderShift { get; set; }
        public bool FlagATM { get; set; }
        public bool FlagFLM { get; set; }
        public bool FlagCompuSafe { get; set; }
        public string MassRequestID { get; set; }
        public bool FlagEcash { get; set; }
        public int EnvBranchTypeId { get; set; }
        public Guid? MachineServiceTypeGuid { get; set; }
        public bool FlagOnHold { get; set; }

        #endregion


        /// <summary>
        /// TechMeet Information
        /// </summary>
        public TechMeetView TechMeetInformation { get; set; }
        public IEnumerable<ECashView> ECashViewList { get; set; }

        #region SFI Information

        public SFIModelView SFIModelView { get; set; }

        #endregion

    }

    #region Techmeet

    public class TechMeetView
    {

        public string TechMeetName { get; set; }
        public string TechMeetPhone { get; set; }
        public string TechMeetCompanyName { get; set; }
        public string TechMeetReason { get; set; }
        public bool TechMeetSecurityRequired { get; set; }

    }
    #endregion

    #region ECASH

    public class ECashView
    {
        public string Currency { get; set; }
        public string Denomination { get; set; }
        public double? DenominationValue { get; set; }
        public int? Amount { get; set; }
        public int? DenominationQuantity { get; set; }
        public int? Unit { get; set; }
    }

    #endregion

    #region SFI
    
    public class SFIModelView
    {
        public string FileReference { get; set; }
        public string CustomerCommentIN { get; set; }
        public string NetworkID { get; set; }
    }

    #endregion
}