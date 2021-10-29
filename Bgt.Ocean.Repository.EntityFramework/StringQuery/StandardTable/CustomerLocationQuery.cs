using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.StringQuery.StandardTable
{
    public class CustomerLocationQuery
    {
        public string GetQueryUpdateChangedCustomer
        {
            get
            {
                string query = $@"
                    			        IF OBJECT_ID (N'tempdb..#TmpMasterRouteJobHead', N'U') IS NOT NULL
	                    DROP TABLE #TmpMasterRouteJobHead;
                    BEGIN TRY
                    BEGIN TRAN
                    DECLARE 
	                    @NewLocationGuid    uniqueidentifier = NEWID(),
	                    @OldLocationName	nvarchar(500),
	                    @OldMachineId		nvarchar(500),
	                    @OldCustomerName	nvarchar(500),
	                    @NewCustomerName	nvarchar(500)


                    --***************1. Get customer location
	                    INSERT INTO TblMasterCustomerLocation
		                    (
		                    Guid
		                    ,MasterCustomer_Guid
		                    ,SystemCustomerOfType_Guid
		                    ,SystemCustomerLocationType_Guid
		                    ,MasterSite_Guid
		                    ,MasterCity_Guid
		                    ,StateName
		                    ,MasterDistrict_Guid
		                    ,CitryName
		                    ,MasterPlace_Guid
		                    ,MasterServiceHour_Guid
		                    ,BranchCodeReference
		                    ,BranchName
		                    ,BranchAbbrevaitionName
		                    ,BranchProgramName
		                    ,BranchReportName
		                    ,Address
		                    ,Postcode
		                    ,Phone
		                    ,Fax
		                    ,ContractName
		                    ,Email
		                    ,Remark
		                    ,Restrictions
		                    ,CardReturnBranch
		                    ,QR_Code
		                    ,Latitude
		                    ,Longitude
		                    ,ServiceDuration
		                    ,FlagDefaultServiceHours
		                    ,FlagSmokeBox
		                    ,EmergencyProcedures
		                    ,AccessRequirments
		                    ,AlarmActivationCode
		                    ,AlarmDuressCode
		                    ,FlagOutOfTown
		                    ,Distance
		                    ,SystemGlobalUnit_Distance_Guid
		                    ,CeilingHeight
		                    ,SystemGlobalUnit_CeilingHeight_Guid
		                    ,WaitingMinute
		                    ,ReportOrderPriority
		                    ,FlagExchangeMoney
		                    ,FlagHaveAirport
		                    ,ExternalID
		                    ,StartofService
		                    ,EndofService
		                    ,PeriodofServiceDay
		                    ,PeriodofServiceMonth
		                    ,FlagDisable
		                    ,UserCreated
		                    ,DatetimeCreated
		                    ,UniversalDatetimeCreated
							,UserModifed
							,DatetimeModified
							,UniversalDatetimeModified
		                    ,OnwardDestinationType
		                    ,OnwardDestination_Guid
		                    ,CustomerLocationExtrnalID
		                    ,MasterCountry_State_Guid
		                    ,FlagPrintReceive
		                    ,FlagSendPodEmail
		                    ,TimeZoneID
		                    ,SystemTimezone_Guid
		                    ,FlagDefaultCommodity
		                    ,PdfPassword
		                    ,ExtBranchID
		                    ,ExtBranchName
		                    ,MasterNonBillable_Guid
		                    ,StartServiceDate
		                    ,EndServiceDate
		                    ,SFOCashBranch_Guid
		                    ,SFOServicingBranch_Guid
		                    ,SFOFLMBranch_Guid
		                    ,SFOMasterFLMZone_Guid
		                    ,SFOMasterECashZone_Guid
		                    ,SFOMasterCompuSafeZone_Guid
		                    ,SFOCountryTimeZone_Guid
		                    ,FlagSFOBranch
		                    ,FlagNonBillable
		                    ,FlagCommentOnDPNotReq
		                    ,MasterLocationReportGroup_Guid
		                    ,Branch
		                    ,UpperLock
		                    ,LowerLock
		                    ,LockMode
		                    ,LockUser
		                    ,FlagSendMissedStop
		                    ,EmailUnableServiceNotification
		                    ,PremiseTime
		                    ,RiskLevelRating
		                    ,MasterGeoZone_Guid
		                    ,SystemAirportType_Guid
		                    ,LocationAddress3
		                    ,LocationAddress4
		                    ,FlagSftp_POD
		                    ,FlagGenUnableToServiceReport
		                    ,FlagUseSftpPubKey
		                    ,PODReference
		                    ,LastNemoSync
		                    ,SystemReportStyle_Guid
		                    ,FlagShowLayoutSealOnly
		                    ,MasterPlaceRelated_Guid
		                    ,FlagPrintReceiptEachSeal
		                    ,PavementLimit
		                    ,PavementLimitCurrency_Guid
		                    ,ISAClientCode
		                    ,FlagObsolete
	                    )
	                    SELECT @NewLocationGuid AS Guid
		                    ,@NewCustomerGuid AS MasterCustomer_Guid
		                    ,SystemCustomerOfType_Guid
		                    ,SystemCustomerLocationType_Guid
		                    ,MasterSite_Guid
		                    ,MasterCity_Guid
		                    ,StateName
		                    ,MasterDistrict_Guid
		                    ,CitryName
		                    ,MasterPlace_Guid
		                    ,MasterServiceHour_Guid
		                    ,BranchCodeReference
		                    ,BranchName
		                    ,BranchAbbrevaitionName
		                    ,BranchProgramName
		                    ,BranchReportName
		                    ,Address
		                    ,Postcode
		                    ,Phone
		                    ,Fax
		                    ,ContractName
		                    ,Email
		                    ,Remark
		                    ,Restrictions
		                    ,CardReturnBranch
		                    ,QR_Code
		                    ,Latitude
		                    ,Longitude
		                    ,ServiceDuration
		                    ,FlagDefaultServiceHours
		                    ,FlagSmokeBox
		                    ,EmergencyProcedures
		                    ,AccessRequirments
		                    ,AlarmActivationCode
		                    ,AlarmDuressCode
		                    ,FlagOutOfTown
		                    ,Distance
		                    ,SystemGlobalUnit_Distance_Guid
		                    ,CeilingHeight
		                    ,SystemGlobalUnit_CeilingHeight_Guid
		                    ,WaitingMinute
		                    ,ReportOrderPriority
		                    ,FlagExchangeMoney
		                    ,FlagHaveAirport
		                    ,ExternalID
		                    ,StartofService
		                    ,EndofService
		                    ,PeriodofServiceDay
		                    ,PeriodofServiceMonth
		                    ,FlagDisable
							,UserCreated
		                    ,DatetimeCreated
		                    ,UniversalDatetimeCreated
		                    ,@UserCreated
		                    ,@ClientDate
		                    ,@UtcDate
		                    ,OnwardDestinationType
		                    ,OnwardDestination_Guid
		                    ,CustomerLocationExtrnalID
		                    ,MasterCountry_State_Guid
		                    ,FlagPrintReceive
		                    ,FlagSendPodEmail
		                    ,TimeZoneID
		                    ,SystemTimezone_Guid
		                    ,FlagDefaultCommodity
		                    ,PdfPassword
		                    ,ExtBranchID
		                    ,ExtBranchName
		                    ,MasterNonBillable_Guid
		                    ,StartServiceDate
		                    ,EndServiceDate
		                    ,SFOCashBranch_Guid
		                    ,SFOServicingBranch_Guid
		                    ,SFOFLMBranch_Guid
		                    ,SFOMasterFLMZone_Guid
		                    ,SFOMasterECashZone_Guid
		                    ,SFOMasterCompuSafeZone_Guid
		                    ,SFOCountryTimeZone_Guid
		                    ,FlagSFOBranch
		                    ,FlagNonBillable
		                    ,FlagCommentOnDPNotReq
		                    ,MasterLocationReportGroup_Guid
		                    ,Branch
		                    ,UpperLock
		                    ,LowerLock
		                    ,LockMode
		                    ,LockUser
		                    ,FlagSendMissedStop
		                    ,EmailUnableServiceNotification
		                    ,PremiseTime
		                    ,RiskLevelRating
		                    ,MasterGeoZone_Guid
		                    ,SystemAirportType_Guid
		                    ,LocationAddress3
		                    ,LocationAddress4
		                    ,FlagSftp_POD
		                    ,FlagGenUnableToServiceReport
		                    ,FlagUseSftpPubKey
		                    ,PODReference
		                    ,LastNemoSync
		                    ,SystemReportStyle_Guid
		                    ,FlagShowLayoutSealOnly
		                    ,MasterPlaceRelated_Guid
		                    ,FlagPrintReceiptEachSeal
		                    ,PavementLimit
		                    ,PavementLimitCurrency_Guid
		                    ,ISAClientCode
		                    ,FlagObsolete
	                    FROM TblMasterCustomerLocation
	                    WHERE Guid = @LocationGuid
	
                      --***************TblMasterCustomerLocation_BGS
                      INSERT INTO TblMasterCustomerLocation_BGS
                      (
	                      Guid
                          ,MasterCustomerLocation_Guid
                          ,SystemBGSConfigurationImportType_Guid
                          ,SystemBGSConfigurationExportType_Guid
                          ,MasterLocationAirport_Guid
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemBGSConfigurationImportType_Guid
                          ,SystemBGSConfigurationExportType_Guid
                          ,MasterLocationAirport_Guid
                      FROM TblMasterCustomerLocation_BGS
                      WHERE MasterCustomerLocation_Guid = @LocationGuid
                      --***************
                       INSERT INTO TblMasterCustomerLocation_BrinksSite (
	                      Guid
                          ,MasterCustomerLocation_Guid
                          ,MasterSite_Guid
                          ,FlagDefaultBrinksSite
	                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterSite_Guid
                          ,FlagDefaultBrinksSite
                      FROM TblMasterCustomerLocation_BrinksSite
                      WHERE MasterCustomerLocation_Guid = @LocationGuid
                      --***************
                      INSERT INTO TblMasterCustomerLocation_Commodity
                      (	Guid
	                    ,MasterCustomerLocation_Guid
                        ,MasterCommodity_Guid
                        ,FlagDefault)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterCommodity_Guid
                          ,FlagDefault
                      FROM TblMasterCustomerLocation_Commodity
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_Document
                      (
	                      Guid
                          ,MasterCustomerLocation_Guid
                          ,BranchDocumentDescription
                          ,FlagDisable
                          ,UserCreated
                          ,DatetimeCreated
                          ,UniversalDatetimeCreated    
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,BranchDocumentDescription
                          ,FlagDisable
                          ,@UserCreated
                          ,@ClientDate
                          ,@UtcDate      
                      FROM TblMasterCustomerLocation_Document
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_DocumentFile
                      (	  Guid
	                      ,MasterCustomerLocation_Guid
                          ,MasterCustomerLocation_Document_Guid
                          ,DocumentFile
                          ,DocumentFileReference
                          ,DocumentFileDescription
                          ,PathFile
                          ,DocumentFileExtention
                          ,FlagDisable
                          ,UserCreated
                          ,DatetimeCreated
                          ,UniversalDatetimeCreated  )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterCustomerLocation_Document_Guid
                          ,DocumentFile
                          ,DocumentFileReference
                          ,DocumentFileDescription
                          ,PathFile
                          ,DocumentFileExtention
                          ,FlagDisable
                          ,@UserCreated
                          ,@ClientDate
                          ,@UtcDate      
                      FROM TblMasterCustomerLocation_DocumentFile
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_EmailAction
                      (	  Guid
                          ,MasterCustomerLocation_Guid
                          ,SystemEmailAction_Guid
                          ,Email
                          ,MasterUserEmailTemplate_Guid
                          ,MasterCustomer_Guid
	                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemEmailAction_Guid
                          ,Email
                          ,MasterUserEmailTemplate_Guid
                          ,@NewCustomerGuid
                      FROM TblMasterCustomerLocation_EmailAction
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_Equipment
                      (   Guid
	                      ,MasterCustomerLocation_Guid
	                      ,MasterEquipment_Guid)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterEquipment_Guid
                      FROM TblMasterCustomerLocation_Equipment
                      WHERE MasterCustomerLocation_Guid = @LocationGuid 

                      --***************
                      INSERT INTO TblMasterCustomerLocation_LOB 
                      (	Guid
	                    ,MasterLocation_Guid
	                    ,MasterLineOfBusiness_Guid
                      )
                      SELECT NEWID()
                          ,@LocationGuid
                          ,MasterLineOfBusiness_Guid
                      FROM TblMasterCustomerLocation_LOB
                      WHERE MasterLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_LocationDestination
                      (
                          Guid
	                      ,MasterCustomerLocation_Guid
                          ,MasterCustomerLocationDes_Guid
                          ,FlagDefault
                          ,MasterCustomerLocation_InternalDepartment_Guid
                          ,MasterCommodity_Guid
                          ,MasterSitePathHeader_Guid
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterCustomerLocationDes_Guid
                          ,FlagDefault
                          ,MasterCustomerLocation_InternalDepartment_Guid
                          ,MasterCommodity_Guid
                          ,MasterSitePathHeader_Guid
                      FROM TblMasterCustomerLocation_LocationDestination
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      UPDATE TblMasterCustomerLocation_LocationDestination 
                      SET	MasterCustomerLocationDes_Guid = @NewLocationGuid
                      WHERE MasterCustomerLocationDes_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_MachineDetail
                      (
	                      Guid
	                      ,MasterCustomerLocation_Guid
	                      ,SFOMasterZone_Guid
                          ,SFOMasterMachineModel_Guid
                          ,SFOMasterMachineLock_Guid
                          ,SFOMasterMachineGrade_Guid
                          ,SlaTime
                          ,LockID
                          ,KeyID
                          ,FlagDisable
                          ,UserCreated
                          ,DatetimeCreated
                          ,UniversalDatetimeCreated    
                          ,BillingCode
                          ,SealCode
                          ,SystemSFO_MachineStatus_Guid
                          ,CustomerMachineID
                          ,VendorMachineID
                          ,ArmoredSiteGuid
                          ,ServiceSiteGuid
                          ,FLMBranchGuid
                          ,ServiceTypeGuid
                          ,SubServiceTypeGuid
                          ,SystemSFO_DaylightTime_Guid
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SFOMasterZone_Guid
                          ,SFOMasterMachineModel_Guid
                          ,SFOMasterMachineLock_Guid
                          ,SFOMasterMachineGrade_Guid
                          ,SlaTime
                          ,LockID
                          ,KeyID
                          ,FlagDisable
                          ,@UserCreated
                          ,@ClientDate
                          ,@UtcDate    
                          ,BillingCode
                          ,SealCode
                          ,SystemSFO_MachineStatus_Guid
                          ,CustomerMachineID
                          ,VendorMachineID
                          ,ArmoredSiteGuid
                          ,ServiceSiteGuid
                          ,FLMBranchGuid
                          ,ServiceTypeGuid
                          ,SubServiceTypeGuid
                          ,SystemSFO_DaylightTime_Guid
                      FROM TblMasterCustomerLocation_MachineDetail
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_MachineDetail_AdditionalInfo  (  Guid,MasterCustomerLocation_Guid  ,SystemSFO_MachineAdditionalInfo_Guid)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemSFO_MachineAdditionalInfo_Guid
                      FROM TblMasterCustomerLocation_MachineDetail_AdditionalInfo
                        WHERE MasterCustomerLocation_Guid = @LocationGuid

                       --***************
                       INSERT INTO TblMasterCustomerLocation_MachineDetail_Kiosk ( Guid,MasterCustomerLocation_Guid,SystemSFO_MachineKiosk_Guid)
                       SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemSFO_MachineKiosk_Guid
                      FROM TblMasterCustomerLocation_MachineDetail_Kiosk
                        WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_Reports (Guid ,MasterCustomerLocation_Guid,SystemReportStyle_Guid)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemReportStyle_Guid
                      FROM TblMasterCustomerLocation_Reports
                      WHERE MasterCustomerLocation_Guid = @LocationGuid
 
                      --***************
                      INSERT INTO TblMasterCustomerLocation_ServiceHour
                      (
		                    Guid,
		                    MasterCustomerLocation_Guid
                          ,SystemServiceJobType_Guid
                          ,SystemDayOfWeek_Guid
                          ,ServiceHourStart
                          ,ServiceHourStop
                          ,FlagSpotTime
                          ,MasterLineOfBusiness_Guid
                          ,FlagDefaultFromCustomer
                          ,MasterRouteJobHeader_Guid
                          ,MasterRouteGroupDetail_Guid
                          ,LastNemoSync  )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemServiceJobType_Guid
                          ,SystemDayOfWeek_Guid
                          ,ServiceHourStart
                          ,ServiceHourStop
                          ,FlagSpotTime
                          ,MasterLineOfBusiness_Guid
                          ,FlagDefaultFromCustomer
                          ,MasterRouteJobHeader_Guid
                          ,MasterRouteGroupDetail_Guid
                          ,LastNemoSync      
                      FROM TblMasterCustomerLocation_ServiceHour
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_ServiceJobType (Guid,MasterCustomerLocation_Guid,SystemServiceJobType_Guid)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemServiceJobType_Guid
                      FROM TblMasterCustomerLocation_ServiceJobType
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_SpecialCommand (Guid ,MasterCustomerLocation_Guid,MasterSpecialCommand_Guid)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterSpecialCommand_Guid
                      FROM TblMasterCustomerLocation_SpecialCommand
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_Sync
                      (		Guid,
		                    MasterCustomerLocation_Guid,
		                    NemoSync,
		                    UserCreated,
		                    DatetimeCreated,
		                    UniversalDatetimeCreated,
		                    NemoErrorCount
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,NemoSync
                          ,@UserCreated
                          ,@ClientDate
                          ,@UtcDate      
                          ,NemoErrorCount
                      FROM TblMasterCustomerLocation_Sync
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      INSERT INTO TblMasterCustomerLocation_Telephone
                      (
		                    Guid,
		                    MasterCustomerLocation_Guid
		                    ,SystemTelephoneTypeID
		                    ,PhoneNumber
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SystemTelephoneTypeID
                          ,PhoneNumber
                      FROM TblMasterCustomerLocation_Telephone
                      WHERE MasterCustomerLocation_Guid = @LocationGuid
                      --***************
                      INSERT INTO TblMasterCustomerLocation_UserDefineField
                      (		
		                    Guid,
		                    MasterCustomerLocation_Guid
		                    ,MasterMainUserDefineField_Guid
		                    ,UDFValue
		                    ,UDFDescription
	                    )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterMainUserDefineField_Guid
                          ,UDFValue
                          ,UDFDescription
                      FROM TblMasterCustomerLocation_UserDefineField
                      WHERE MasterCustomerLocation_Guid = @LocationGuid

                      --***************
                      /*
                         Machine  
                      */
                      INSERT INTO SFO.SFOTblMasterMachine
                      (
                           Guid	
                          ,MachineID
                          ,CustomerMachineID
                          ,VendorMachineID
                          ,SFOMasterMachineServiceType_Guid
                          ,SFOMasterMachineSubServiceType_Guid
                          ,SFOMasterMachineModelType_Guid
                          ,SystemCustomerLocationType_Guid
                          ,MasterVendor_Guid
                          ,ServiceCode
                          ,FlagDailyCreditIndicator
                          ,FlagRemoteSafeManagementIndicator
                          ,FlagDeviceDashBoardIndicator
                          ,SerialNumber
                          ,AccountReceivable
                          ,MachineEDI
                          ,DateEntered
                          ,StartDate
                          ,FlagDisable
                          ,UserCreated
                          ,DatetimeCreated
                          ,UniversalDatetimeCreated
						  ,UserModified
						  ,DatetimeModified
						  ,UniversalDatetimeModified
                          ,SFOMasterCountryTimeZone_Guid
                          ,FLMSLATime
                          ,ECashSLATime
                          ,CompuSafeSLATime
                          ,SFOMasterECashType_Guid
                          ,SafeSerialNumber
                          ,SFOMasterMachineBrand_Guid
                          ,LockSerialNumber
                          ,SFOSystemLockType_Guid
                          ,SFOServicingBranch_Guid
                          ,SFOAssignedBranch_Guid
                          ,SFOFLMBranch_Guid
                          ,SFOArmoredBranch_Guid
                          ,SFOCashBranch_Guid
                          ,SFOMasterFLMZone_Guid
                          ,SFOMasterECashZone_Guid
                          ,SFOMasterCompuSafeZone_Guid
                          ,SFOMasterBillingType_Guid
                          ,Run
                          ,FlagEcash
                          ,Address2
                          ,CombinationCode
                          ,FlagNoneLock
                          ,SFOMasterAlarmSystem_Guid
                          ,MonitoringNetwork
                          ,StopDate
                          ,MachineReason
                          ,FlagSuspension
                          ,SFOTblMasterMonitoringNetwork_Guid
                          ,SystemMachineOwner_Guid
                          ,SystemMachineType_Guid
                          ,MaximumCassettes
                          ,SystemATMDistance_Guid
                          ,SystemGlobalUnit_Guid
                          ,DistanceSiteToMachine
                          ,TotalATM
                          ,TotalAmountStay  
                      )
                      SELECT @NewLocationGuid
                          ,MachineID
                          ,CustomerMachineID
                          ,VendorMachineID
                          ,SFOMasterMachineServiceType_Guid
                          ,SFOMasterMachineSubServiceType_Guid
                          ,SFOMasterMachineModelType_Guid
                          ,SystemCustomerLocationType_Guid
                          ,MasterVendor_Guid
                          ,ServiceCode
                          ,FlagDailyCreditIndicator
                          ,FlagRemoteSafeManagementIndicator
                          ,FlagDeviceDashBoardIndicator
                          ,SerialNumber
                          ,AccountReceivable
                          ,MachineEDI
                          ,DateEntered
                          ,StartDate
                          ,FlagDisable
						  ,UserCreated
                          ,DatetimeCreated
                          ,UniversalDatetimeCreated
                          ,@UserCreated
                          ,@ClientDate
                          ,@UtcDate
                          ,SFOMasterCountryTimeZone_Guid
                          ,FLMSLATime
                          ,ECashSLATime
                          ,CompuSafeSLATime
                          ,SFOMasterECashType_Guid
                          ,SafeSerialNumber
                          ,SFOMasterMachineBrand_Guid
                          ,LockSerialNumber
                          ,SFOSystemLockType_Guid
                          ,SFOServicingBranch_Guid
                          ,SFOAssignedBranch_Guid
                          ,SFOFLMBranch_Guid
                          ,SFOArmoredBranch_Guid
                          ,SFOCashBranch_Guid
                          ,SFOMasterFLMZone_Guid
                          ,SFOMasterECashZone_Guid
                          ,SFOMasterCompuSafeZone_Guid
                          ,SFOMasterBillingType_Guid
                          ,Run
                          ,FlagEcash
                          ,Address2
                          ,CombinationCode
                          ,FlagNoneLock
                          ,SFOMasterAlarmSystem_Guid
                          ,MonitoringNetwork
                          ,StopDate
                          ,MachineReason
                          ,FlagSuspension
                          ,SFOTblMasterMonitoringNetwork_Guid
                          ,SystemMachineOwner_Guid
                          ,SystemMachineType_Guid
                          ,MaximumCassettes
                          ,SystemATMDistance_Guid
                          ,SystemGlobalUnit_Guid
                          ,DistanceSiteToMachine
                          ,TotalATM
                          ,TotalAmountStay      
                      FROM SFO.SFOTblMasterMachine
                      WHERE Guid = @LocationGuid

                      --***************
                      INSERT INTO SFO.SFOTblMasterMachine_AssociatedMachine (Guid,SFOMasterMachine_Guid,SFOMasterAssociatedMachine_Guid)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SFOMasterAssociatedMachine_Guid
                      FROM SFO.SFOTblMasterMachine_AssociatedMachine
                      WHERE SFOMasterMachine_Guid = @LocationGuid
 
                      --***************
                      INSERT INTO SFO.SFOTblMasterMachine_Capabilties (Guid,MasterMachine_Guid,MasterMachineModelType_Capability_Guid)
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterMachineModelType_Capability_Guid
                      FROM SFO.SFOTblMasterMachine_Capabilties
                      WHERE MasterMachine_Guid = @LocationGuid

                      --***************
                      INSERT INTO SFO.SFOTblMasterMachine_Cassette 
                      (
	                      Guid
	                      ,SFOTblMasterMachine_Guid
                          ,SFOTblMasterCassette_Guid
                          ,TblMasterCurrency_Guid
                          ,TblMasterDenomination_Guid
                          ,CassetteSequence      
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SFOTblMasterCassette_Guid
                          ,TblMasterCurrency_Guid
                          ,TblMasterDenomination_Guid
                          ,CassetteSequence      
                      FROM SFO.SFOTblMasterMachine_Cassette
                      WHERE  SFOTblMasterMachine_Guid= @LocationGuid

                      --***************
                      INSERT INTO SFO.SFOTblMasterMachine_ECash
                      (
	                       Guid
	                      ,SFOMasterMachine_Guid
                          ,MasterCurrency_Guid
                          ,MasterDenomination_Guid
                          ,Amount
                          ,FlagDisable
                          ,UserCreated
                          ,DatetimeCreated
                          ,UniversalDatetimeCreated
                          ,DenominationValue
                          ,UnitValue
                          ,InputValue        
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,MasterCurrency_Guid
                          ,MasterDenomination_Guid
                          ,Amount
                          ,FlagDisable
                          ,@UserCreated
                          ,@ClientDate
                          ,@UtcDate
                          ,DenominationValue
                          ,UnitValue
                          ,InputValue      
                      FROM SFO.SFOTblMasterMachine_ECash
                      WHERE  SFOMasterMachine_Guid= @LocationGuid

                      --***************
                      INSERT INTO SFO.SFOTblMasterMachine_LockType
                      (
	                       Guid
	                      ,SFOMasterMachine_Guid
                          ,SFOSystemLockType_Guid
                          ,FlagDisable
                          ,UserCreated
                          ,DatetimeCreated
                          ,UniversalDatetimeCreated
                          ,CombinationCode
                          ,ReferenceCode
                          ,LockSeq
                          ,SerailNumber
                          ,MachineLockID
                          ,SFOSystemUserLockMode_Guid
                      )
                      SELECT NEWID()
                          ,@NewLocationGuid
                          ,SFOSystemLockType_Guid
                          ,FlagDisable
                          ,@UserCreated
                          ,@ClientDate
                          ,@UtcDate
                          ,CombinationCode
                          ,ReferenceCode
                          ,LockSeq
                          ,SerailNumber
                          ,MachineLockID
                          ,SFOSystemUserLockMode_Guid
      
                      FROM SFO.SFOTblMasterMachine_LockType
                      WHERE  SFOMasterMachine_Guid = @LocationGuid

                      DELETE FROM SFO.SFOTblMasterMachine_LockType
                      WHERE  SFOMasterMachine_Guid = @LocationGuid
 
                      /*
		                    Audit log
                      */ 
                      SELECT @OldLocationName = BranchName FROM TblMasterCustomerLocation WHERE Guid = @LocationGuid
                      SELECT @OldCustomerName = CustomerFullName FROM TblMasterCustomer WHERE Guid IN (SELECT MasterCustomer_Guid FROM TblMasterCustomerLocation WHERE Guid = @LocationGuid)
                      SELECT @NewCustomerName = CustomerFullName FROM TblMasterCustomer WHERE Guid = @NewCustomerGuid
                      SELECT @OldMachineId = MachineID FROM SFO.SFOTblMasterMachine WHERE Guid = @LocationGuid


                      -- ****************Log disabled Old Location.
                      INSERT INTO TblSystemCustomerLocation_Audit_Log
                               (Guid
                               ,SystemLogCategory_Guid
                               ,SystemLogProcess_Guid
                               ,CustomerLocation_Guid
                               ,FlagDisable
                               ,Description
                               ,UserCreated
                               ,DateTimeCreated
                               ,UniversalDatetimeCreated           
                               ,FlagNewData
                               ,SystemMsgID
                               ,JSONValue)
                         VALUES
                               (NEWID()
                               ,'DB1E735A-ED09-4E1C-8455-7FC16A377559'
                               ,'39547FFA-0CAD-445F-8964-1FD7A39D4DA4'
                               ,@LocationGuid
                               ,0
                               ,'Customer location has been disabled.'
                               ,@UserCreated
                               ,@ClientDate
                               ,@UtcDate          
                               ,1
                               ,6129
                               ,'[""'+@OldLocationName+'"",""'+@OldCustomerName+'"",""'+@NewCustomerName+'""]')

                        -- * ****************log created new location

                         INSERT INTO TblSystemCustomerLocation_Audit_Log
                               (Guid
                               , SystemLogCategory_Guid
                               , SystemLogProcess_Guid
                               , CustomerLocation_Guid
                               , FlagDisable
                               , Description
                               , UserCreated
                               , DateTimeCreated
                               , UniversalDatetimeCreated
                               , FlagNewData
                               , SystemMsgID
                               , JSONValue)
                         VALUES
                               (NEWID()
                               , 'DB1E735A-ED09-4E1C-8455-7FC16A377559'
                               , '39547FFA-0CAD-445F-8964-1FD7A39D4DA4'
                               , @NewLocationGuid
                               , 0
                               , 'Customer location has been move to new Customer.'
                               , @UserCreated
                               , @UtcDate
                               , @UtcDate
                               , 1
                               , 6130
                               , '[""' + @OldLocationName + '"",""' + @OldCustomerName + '"",""' + @NewCustomerName + '""]')
                      -- * ********************Machine audit log
                       IF(@OldMachineId IS NOT NULL)
                       BEGIN
                            -- old machine
                            ; WITH TmpLogCategory AS(
                                 SELECT TOP 1 Guid AS SystemCategoryGuid, SystemLogProcess_Guid 
                                 FROM SFO.SFOTblSystemLogCategory 
                                 WHERE CategoryCode = 'STD_MCN_DIS' AND FlagDisable = 0
                             )

                            INSERT INTO SFO.SFOTblTransactionGenericLog
                               (Guid
                               , SystemLogCategory_Guid
                               , SystemLogProcess_Guid
                               , ReferenceValue
                               , FlagDisable
                               , UserCreated
                               , DateTimeCreated
                               , UniversalDatetimeCreated
                               , Description
                               , SystemMsgID
                               , JSONValue)
                            SELECT
                                NEWID()
                               ,SystemCategoryGuid
                               ,SystemLogProcess_Guid
                               ,@LocationGuid
                               ,0
                               ,@UserCreated
                               ,@UtcDate
                               ,@UtcDate
                               ,'Disabled affter move to new Customer.'
                               ,6131
                               ,'[""' + @OldMachineId + '"",""' + @OldCustomerName + '"",""' + @NewCustomerName + '""]'
                            FROM TmpLogCategory
                            ; WITH TmpLogCategory AS(
                                 SELECT TOP 1 Guid AS SystemCategoryGuid, SystemLogProcess_Guid
 
                                 FROM SFO.SFOTblSystemLogCategory
 
                                 WHERE CategoryCode = 'STD_MCN_CRT' AND FlagDisable = 0
                             )

                            INSERT INTO SFO.SFOTblTransactionGenericLog
                               (Guid
                               , SystemLogCategory_Guid
                               , SystemLogProcess_Guid
                               , ReferenceValue
                               , FlagDisable
                               , UserCreated
                               , DateTimeCreated
                               , UniversalDatetimeCreated
                               , Description
                               , SystemMsgID
                               , JSONValue)

                            SELECT
                                NEWID()
                               ,SystemCategoryGuid
                               ,SystemLogProcess_Guid
                               ,@NewLocationGuid
                               ,0
                               ,@UserCreated
                               ,@UtcDate
                               ,@UtcDate
                               ,'Create affter move to new Customer.'
                               ,6132
                               ,'[""' + @OldMachineId + '"",""' + @OldCustomerName + '"",""' + @NewCustomerName + '""]'

                            FROM TmpLogCategory

                            --New machine.
                      END
                     /*
                      Update old data
                    */
                     UPDATE TblMasterCustomerLocation
                     SET   FlagDisable = 1,
		                    FlagObsolete = 1,
		                    UserModifed = @UserCreated,
		                    DatetimeModified = @ClientDate,
		                    UniversalDatetimeModified = @UtcDate
                      WHERE Guid = @LocationGuid

                     UPDATE SFO.SFOTblMasterMachine 
                     SET FlagNoneLock = 1 ,
	                     UserModified = @UserCreated,
	                     DatetimeModified = @ClientDate,
	                     UniversalDatetimeModified = @UtcDate
                     WHERE Guid = @LocationGuid
                      /*
	                    Update master job.
                      */
                      SELECT DISTINCT MasterRouteJobHeader_Guid
                      INTO #TmpMasterRouteJobHead
                      FROM TblMasterRouteJobServiceStopLegs L
                      INNER JOIN TblMasterRouteJobHeader H ON L.MasterRouteJobHeader_Guid = H.Guid
                      WHERE H.FlagDisable != 1 AND MasterCustomerLocation_Guid = @LocationGuid

                      UPDATE TblMasterRouteJobServiceStopLegs
                      SET MasterCustomerLocation_Guid = @NewLocationGuid
                      WHERE MasterRouteJobHeader_Guid IN(select MasterRouteJobHeader_Guid FROM #TmpMasterRouteJobHead )
                      AND MasterCustomerLocation_Guid = @LocationGuid

                      UPDATE TblMasterRouteJobHeader
                      SET   UserModifed = @UserCreated,
                            DatetimeModified = @ClientDate,
                            UniversalDatetimeModified = @UtcDate
                      WHERE Guid IN(select MasterRouteJobHeader_Guid FROM #TmpMasterRouteJobHead)

		
                   COMMIT TRAN
 --ROLLBACK TRAN
                    SELECT 0 AS MsgID
                END TRY

                 BEGIN CATCH
	                 ROLLBACK TRAN
	                 SELECT -186 AS MsgID
	                 INSERT INTO TblSystemLog_HistoryError
		                (GUID ,ErrorDescription ,FunctionName ,PageName ,InnerError,ClientIP ,ClientName ,DatetimeCreated,FlagSendEmail)
	                SELECT								
		                Guid = NEWID(),
		                ErrorDescription  =ERROR_MESSAGE(),
		                FunctionName = 'SaveChangeCustomer ',
		                PageName = 'StandardTable >> CustomerLocation >> ChangeCustomer',
		                InnerError =CONCAT('Error : [Id : ',ERROR_NUMBER(),' Line : ',ERROR_LINE(),' Message : ',ERROR_MESSAGE() ,']'),
		                ClientIP = @@SERVERNAME,
		                ClientName = '',
		                DatetimeCreated =GETUTCDATE(),
		                FlagSendEmail = 1
				
                 END CATCH
                    IF OBJECT_ID (N'tempdb..#TmpMasterRouteJobHead', N'U') IS NOT NULL
		                DROP TABLE #TmpMasterRouteJobHead;";
                return query;
            }
        }
    }
}
