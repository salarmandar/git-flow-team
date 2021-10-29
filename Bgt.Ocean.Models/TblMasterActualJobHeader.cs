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
    
    public partial class TblMasterActualJobHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblMasterActualJobHeader()
        {
            this.SFOTblTransactionOTC = new HashSet<SFOTblTransactionOTC>();
            this.SFOTblTransactionServiceRequest = new HashSet<SFOTblTransactionServiceRequest>();
            this.TblMasterActualJobHeader_Capability = new HashSet<TblMasterActualJobHeader_Capability>();
            this.TblMasterActualJobHeader_OTC = new HashSet<TblMasterActualJobHeader_OTC>();
            this.TblMasterActualJobHideScreenMapping = new HashSet<TblMasterActualJobHideScreenMapping>();
            this.TblMasterActualJobMCSBulkDepositReport = new HashSet<TblMasterActualJobMCSBulkDepositReport>();
            this.TblMasterActualJobMCSBulkJammed = new HashSet<TblMasterActualJobMCSBulkJammed>();
            this.TblMasterActualJobMCSBulkRetract = new HashSet<TblMasterActualJobMCSBulkRetract>();
            this.TblMasterActualJobMCSBulkSuspectFake = new HashSet<TblMasterActualJobMCSBulkSuspectFake>();
            this.TblMasterActualJobMCSCITDelivery = new HashSet<TblMasterActualJobMCSCITDelivery>();
            this.TblMasterActualJobMCSCoinBulkNoteCollect = new HashSet<TblMasterActualJobMCSCoinBulkNoteCollect>();
            this.TblMasterActualJobMCSCoinCashAdd = new HashSet<TblMasterActualJobMCSCoinCashAdd>();
            this.TblMasterActualJobMCSCoinCashReturn = new HashSet<TblMasterActualJobMCSCoinCashReturn>();
            this.TblMasterActualJobMCSCoinMachineBalance = new HashSet<TblMasterActualJobMCSCoinMachineBalance>();
            this.TblMasterActualJobMCSCoinSuspectFake = new HashSet<TblMasterActualJobMCSCoinSuspectFake>();
            this.TblMasterActualJobMCSItemSeal = new HashSet<TblMasterActualJobMCSItemSeal>();
            this.TblMasterActualJobMCSRecyclingActualCount = new HashSet<TblMasterActualJobMCSRecyclingActualCount>();
            this.TblMasterActualJobMCSRecyclingCashRecycling = new HashSet<TblMasterActualJobMCSRecyclingCashRecycling>();
            this.TblMasterActualJobMCSRecyclingMachineReport = new HashSet<TblMasterActualJobMCSRecyclingMachineReport>();
            this.TblMasterActualJobSumActualCount = new HashSet<TblMasterActualJobSumActualCount>();
            this.TblMasterActualJobSumCashAdd = new HashSet<TblMasterActualJobSumCashAdd>();
            this.TblMasterActualJobSumCashReturn = new HashSet<TblMasterActualJobSumCashReturn>();
            this.TblMasterActualJobSumMachineReport = new HashSet<TblMasterActualJobSumMachineReport>();
            this.TblMasterCapturedCard = new HashSet<TblMasterCapturedCard>();
            this.TblMasterHistory_DolphinAssignToAnotherRun = new HashSet<TblMasterHistory_DolphinAssignToAnotherRun>();
            this.TblVaultBalanceSealAndMaster = new HashSet<TblVaultBalanceSealAndMaster>();
            this.TblVaultBalance_Discrepancy = new HashSet<TblVaultBalance_Discrepancy>();
        }
    
        public System.Guid Guid { get; set; }
        public string JobNo { get; set; }
        public System.Guid SystemStopType_Guid { get; set; }
        public Nullable<System.Guid> SystemLineOfBusiness_Guid { get; set; }
        public Nullable<System.Guid> SystemServiceJobType_Guid { get; set; }
        public Nullable<double> SaidToContain { get; set; }
        public Nullable<System.Guid> MasterCurrency_Guid { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.Guid> MasterCustomerContract_Guid { get; set; }
        public Nullable<System.Guid> MasterRouteJobHeader_Guid { get; set; }
        public Nullable<int> DayInVault { get; set; }
        public Nullable<int> SystemStatusJobID { get; set; }
        public Nullable<bool> FlagCancelAll { get; set; }
        public Nullable<System.DateTime> InformTime { get; set; }
        public Nullable<System.DateTime> TransectionDate { get; set; }
        public Nullable<bool> FlagJobClose { get; set; }
        public Nullable<bool> FlagJobReadyToVault { get; set; }
        public Nullable<bool> FlagJobProcessDone { get; set; }
        public Nullable<int> OnwardDestinationType { get; set; }
        public Nullable<System.Guid> OnwardDestination_Guid { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<bool> FlagJobDiscrepancies { get; set; }
        public Nullable<System.Guid> MasterReasonType_Guid { get; set; }
        public string ResonCancelText { get; set; }
        public Nullable<bool> FlagMissingStop { get; set; }
        public Nullable<bool> FlagNonDelivery { get; set; }
        public Nullable<System.DateTime> DateNewSchedule { get; set; }
        public Nullable<System.DateTime> TimeNewSchedule { get; set; }
        public Nullable<bool> FlagPickupAlready { get; set; }
        public Nullable<System.Guid> JobNoRef_Guid { get; set; }
        public string External_Code_Ref { get; set; }
        public string OTC_Code { get; set; }
        public string OTC_Code_Encrypt { get; set; }
        public Nullable<int> OTC_Time_Block { get; set; }
        public Nullable<System.DateTime> OTC_Effective_DateTime { get; set; }
        public Nullable<System.DateTime> OTC_Expire_DateTime { get; set; }
        public Nullable<int> NoOfItems { get; set; }
        public Nullable<byte> FlagSyncToMobile { get; set; }
        public Nullable<System.DateTime> DateTimeSyncToMobile { get; set; }
        public Nullable<System.Guid> ReasonUnableToService_Guid { get; set; }
        public Nullable<System.DateTime> ReasonUnableToServiceDateTime { get; set; }
        public Nullable<bool> FlagJobAdditional { get; set; }
        public string MobileDeviceID { get; set; }
        public string TransferName { get; set; }
        public Nullable<bool> FlagBgsAirport { get; set; }
        public Nullable<int> SystemStatusJobIDPrevious { get; set; }
        public Nullable<bool> FlagPickedupAfterInterchange { get; set; }
        public Nullable<int> SeqOutSite { get; set; }
        public Nullable<System.Guid> SystemTripIndicator_Guid { get; set; }
        public string ConsigneeName { get; set; }
        public Nullable<decimal> TotalValue { get; set; }
        public bool FlagTotalValuePerJob { get; set; }
        public Nullable<System.Guid> JobBCP_Ref_Guid { get; set; }
        public bool FlagExportedToSMART { get; set; }
        public string UserExportedToSMART { get; set; }
        public Nullable<System.DateTime> DateTimeExportedToSMART { get; set; }
        public Nullable<System.Guid> BCD_MasterCustomerLocation_InternalDepartment_Guid { get; set; }
        public Nullable<System.DateTime> DatePickupInternalDeptForBCD { get; set; }
        public Nullable<System.Guid> PushToSmart_Guid { get; set; }
        public Nullable<System.Guid> MasterSubServiceType_Guid { get; set; }
        public bool FlagJobInterBranch { get; set; }
        public bool FlagChkOutInterBranchComplete { get; set; }
        public Nullable<System.Guid> HistoryGenerateJob_Guid { get; set; }
        public Nullable<bool> FlagUpdateJobManual { get; set; }
        public Nullable<bool> FlagRequireOpenLock { get; set; }
        public Nullable<bool> SFOFlagRequiredTechnician { get; set; }
        public string SFOTechnicianID { get; set; }
        public string SFOTechnicianName { get; set; }
        public Nullable<int> SFOMaxWorkingTime { get; set; }
        public Nullable<int> SFOMaxTechnicianWaitingTime { get; set; }
        public Nullable<System.Guid> SFOMasterSolution_guid { get; set; }
        public Nullable<int> EESolutionType_id { get; set; }
        public Nullable<System.Guid> BCD_MasterRoute_Guid { get; set; }
        public bool FlagIntermediate { get; set; }
        public Nullable<bool> FlagCompleteServiceByQR { get; set; }
        public Nullable<System.Guid> LeadToCashProduct_PricingRule_Guid { get; set; }
        public Nullable<System.Guid> BankCleanOutJobDelivery_Temp_Guid { get; set; }
        public bool FlagJobSFO { get; set; }
        public Nullable<System.DateTime> BCD_DesiredDeliveryDate { get; set; }
        public Nullable<System.DateTime> BCD_DefaultDesiredDeliveryDate { get; set; }
        public Nullable<System.Guid> BCD_MasterRouteJobHeader_Guid { get; set; }
        public bool FlagCalculated { get; set; }
        public string TicketNumber { get; set; }
        public bool FlagGenJobsReturnFromUnable { get; set; }
        public string HAWB { get; set; }
        public string LockState { get; set; }
        public string CommodityName { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public string TotalWeightUnit { get; set; }
        public Nullable<bool> FlagRequestSLM { get; set; }
        public Nullable<bool> FlagUseTranferSafe { get; set; }
        public bool FlagAccepted { get; set; }
        public bool FlagDeliverCardMainBank { get; set; }
        public bool FlagDeliverCardBankBranch { get; set; }
        public bool FlagMultiBranch { get; set; }
        public Nullable<int> CurrentStopLeg { get; set; }
        public Nullable<System.Guid> MasterSitePathHeader_Guid { get; set; }
        public bool FlagJobChage { get; set; }
        public bool FlagJobReOrder { get; set; }
        public Nullable<System.DateTime> AcceptedDatetime { get; set; }
        public Nullable<System.DateTime> RejectedDatetime { get; set; }
        public Nullable<System.Guid> MasterMachineServiceType_Guid { get; set; }
        public Nullable<System.Guid> MasterProblem_Guid { get; set; }
        public string SLMComment { get; set; }
        public Nullable<int> CITDeliveryStatus { get; set; }
        public string ReferenceId { get; set; }
        public Nullable<bool> FlagStartServiceByQR { get; set; }
        public Nullable<System.Guid> UUID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SFOTblTransactionOTC> SFOTblTransactionOTC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SFOTblTransactionServiceRequest> SFOTblTransactionServiceRequest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobHeader_Capability> TblMasterActualJobHeader_Capability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobHeader_OTC> TblMasterActualJobHeader_OTC { get; set; }
        public virtual TblMasterSitePathHeader TblMasterSitePathHeader { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobHideScreenMapping> TblMasterActualJobHideScreenMapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSBulkDepositReport> TblMasterActualJobMCSBulkDepositReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSBulkJammed> TblMasterActualJobMCSBulkJammed { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSBulkRetract> TblMasterActualJobMCSBulkRetract { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSBulkSuspectFake> TblMasterActualJobMCSBulkSuspectFake { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSCITDelivery> TblMasterActualJobMCSCITDelivery { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSCoinBulkNoteCollect> TblMasterActualJobMCSCoinBulkNoteCollect { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSCoinCashAdd> TblMasterActualJobMCSCoinCashAdd { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSCoinCashReturn> TblMasterActualJobMCSCoinCashReturn { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSCoinMachineBalance> TblMasterActualJobMCSCoinMachineBalance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSCoinSuspectFake> TblMasterActualJobMCSCoinSuspectFake { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSItemSeal> TblMasterActualJobMCSItemSeal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSRecyclingActualCount> TblMasterActualJobMCSRecyclingActualCount { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSRecyclingCashRecycling> TblMasterActualJobMCSRecyclingCashRecycling { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobMCSRecyclingMachineReport> TblMasterActualJobMCSRecyclingMachineReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobSumActualCount> TblMasterActualJobSumActualCount { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobSumCashAdd> TblMasterActualJobSumCashAdd { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobSumCashReturn> TblMasterActualJobSumCashReturn { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterActualJobSumMachineReport> TblMasterActualJobSumMachineReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterCapturedCard> TblMasterCapturedCard { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMasterHistory_DolphinAssignToAnotherRun> TblMasterHistory_DolphinAssignToAnotherRun { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblVaultBalanceSealAndMaster> TblVaultBalanceSealAndMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblVaultBalance_Discrepancy> TblVaultBalance_Discrepancy { get; set; }
    }
}
