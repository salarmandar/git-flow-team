
using Bgt.Ocean.Models;
using System.Data.Entity;

namespace DataSoruce.Test
{
    /*
   *  --query for create datasource
  ;WITH DS AS(
  SELECT * FROM TblMasterCurrency WHERE Guid = '82011826-B47D-4747-A533-A89F42DECF24'
  ),JSON_DATA AS(
  SELECT ROW_NUMBER() OVER(ORDER BY Guid ASC) AS Row# ,STRING_ESCAPE(v._ROW, 'JSON') Data# FROM DS s
  OUTER APPLY(
  SELECT (SELECT top 1 * FROM DS

  d WHERE d.Guid = s.Guid FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)

  AS _ROW) v
  )
  SELECT
  CASE WHEN Row# = 1										THEN 'result = result.AddDbSet<T,{EntityName}>("' + Data# + '"'
  WHEN Row# = (SELECT MAX(s.Row#) FROM JSON_DATA s)	THEN ',"' + Data# +'");'
  ELSE  ',"' + Data# + '"' END AS  Data#
  FROM JSON_DATA
       */
    public static class Mock53385<T> where T : class
    {

        public static DbSet<T> Data
        {
            get
            {
                DbSet<T> result = null;
                result = result.AddDbSet<T, TblMasterCountry>("{\"Guid\":\"777C133E-BC5E-4A5E-965C-F4B12B4CAD54\",\"SystemGlobalRegion_Guid\":\"D4FABDB0-C180-4B89-84A4-5BDABE7790BD\",\"MasterCountryName\":\"THAILAND\",\"MasterCountryAbbreviation\":\"TH\",\"FlagInputCityManual\":false,\"FlagDisable\":false,\"UniversalDatetimeCreated\":\"2013-07-23T03:35:26.8770000Z\",\"UserModifed\":\"taungkac\",\"DatetimeModified\":\"2020-12-24T13:09:46+07:00\",\"UniversalDatetimeModified\":\"2020-12-24T06:09:47.1868466Z\",\"FlagSortCityByIndex\":true,\"FlagHaveState\":false,\"MobilePrefixCode\":\"66\",\"SystemLanguage_Guid\":\"C9B0264E-F645-4C14-991D-08EA96E782C2\",\"ReferenceId\":\"777C133E\"}");

                result = result.AddDbSet<T, TblMasterSite>("{\"Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"MasterCountry_Guid\":\"777C133E-BC5E-4A5E-965C-F4B12B4CAD54\",\"SiteName\":\"Tum_Test\",\"SiteCode\":\"0010\",\"FlagDisable\":false,\"FlagSendSignature\":false,\"UserCreated\":\"YuensooS\",\"DatetimeCreated\":\"2017-08-07T09:57:25+07:00\",\"UniversalDatetimeCreated\":\"2017-08-07T02:57:25.1090153Z\",\"UserModifed\":\"chaikheas\",\"DatetimeModified\":\"2020-07-08T15:46:26+07:00\",\"UniversalDatetimeModified\":\"2020-07-08T08:47:17.3733225Z\",\"TimeZoneID\":76,\"MasterCustomer_Guid\":\"B89535EE-FE90-47DD-B00C-AA522D55DDD2\",\"MasterSiteHub_Guid\":\"22F50127-C528-46B8-8677-31B133FC69E5\",\"FixedCostPerMonth\":0.00,\"TermConditionBody\":\"PAPAhio22\",\"FlagIntegrationSite\":false,\"SiteAbbrevaitionName\":\"2T\",\"FlagOTC\":false,\"OTCTime\":\"1900-01-01T00:00:00\",\"MaxTimeOTC\":4290,\"ReferenceId\":\"742346F7\",\"FlagAutoSendDailyPlanReport\":true}");

                result = result.AddDbSet<T, TblMasterCustomerLocation>("{\"Guid\":\"26DE6458-985D-4C0D-AA83-A6B655F9CB59\",\"MasterCustomer_Guid\":\"A46B88FF-3B0E-48E5-B0C7-8284CBDC2229\",\"SystemCustomerOfType_Guid\":\"D103DC53-4600-49FB-AE83-043D0EED73EF\",\"SystemCustomerLocationType_Guid\":\"38FDD8F7-C6BE-4031-BC7D-A94AA703DC7C\",\"MasterSite_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"MasterCity_Guid\":\"41FE6E0F-DEEC-4C08-A729-BDA92930EA41\",\"MasterDistrict_Guid\":\"7CB5F2AC-9FE1-483B-84F7-09011AA3CA39\",\"BranchCodeReference\":\"loveM\",\"BranchName\":\"Love ManU\",\"BranchAbbrevaitionName\":\"Love ManU\",\"BranchProgramName\":\"Love ManU\",\"Address\":\"xxa151\",\"Postcode\":\"12423\",\"Email\":\"supakit.yuensook@brinksglabal.com\",\"FlagDefaultServiceHours\":false,\"FlagSmokeBox\":false,\"FlagOutOfTown\":false,\"SystemGlobalUnit_CeilingHeight_Guid\":\"3B2DFBE3-35DE-416B-B8A5-27FCD3E09B19\",\"FlagExchangeMoney\":false,\"FlagHaveAirport\":false,\"PeriodofServiceDay\":0,\"PeriodofServiceMonth\":0,\"FlagDisable\":false,\"UserCreated\":\"supakit\",\"DatetimeCreated\":\"2019-06-25T18:12:30\",\"UniversalDatetimeCreated\":\"2019-06-25T11:12:30.0858693Z\",\"UserModifed\":\"supakitb\",\"DatetimeModified\":\"2019-08-08T11:24:03.323\",\"UniversalDatetimeModified\":\"2019-08-08T04:24:03.2675706Z\",\"FlagPrintReceive\":false,\"FlagSendPodEmail\":true,\"FlagDefaultCommodity\":false,\"FlagSFOBranch\":false,\"FlagNonBillable\":false,\"FlagCommentOnDPNotReq\":false,\"FlagSendMissedStop\":false,\"FlagSftp_POD\":true,\"FlagGenUnableToServiceReport\":false,\"FlagUseSftpPubKey\":false,\"ReferenceId\":\"26DE6458\",\"FlagShowLayoutSealOnly\":false,\"FlagPrintReceiptEachSeal\":false,\"PavementLimit\":0.00}");

                result = result.AddDbSet<T, TblMasterCustomer>("{\"Guid\":\"A46B88FF-3B0E-48E5-B0C7-8284CBDC2229\",\"MasterCountry_Guid\":\"777C133E-BC5E-4A5E-965C-F4B12B4CAD54\",\"MasterCity_Guid\":\"41FE6E0F-DEEC-4C08-A729-BDA92930EA41\",\"StateName\":\"Bangkok\",\"MasterDistrict_Guid\":\"4D67CAF4-4433-4B70-88BB-73216AD04B78\",\"SystemCustomerOfType_Guid\":\"D103DC53-4600-49FB-AE83-043D0EED73EF\",\"CustomerFullName\":\"Love Yourself\",\"CustomerReportName\":\"Love Yourself\",\"CustomerEmail\":\"supakit.yuensook@brinksglabal.com\",\"Flag3Party\":false,\"FlagChkInventory\":false,\"FlagExchangeFund\":false,\"FlagRequiredtoInputSTC\":false,\"FlagEmailApplyAll\":true,\"FlagChkTransactionFromPickupDate\":false,\"FlagChkCustomer\":true,\"FlagATMCustomer\":false,\"FlagDisable\":false,\"UserCreated\":\"YuensooS\",\"DatetimeCreated\":\"2017-08-28T10:21:27\",\"UniversalDatetimeCreated\":\"2020-07-21T06:54:11.1298666Z\",\"UserModifed\":\"supakit\",\"DatetimeModified\":\"2020-08-05T12:35:15\",\"FlagDefaultSystem\":false,\"FlagSendSignature\":false,\"FlagSftp_POD\":true,\"Path_Sftp\":\"155.140.79.157\",\"Username_Sftp\":\"brnkcsftp\",\"Password_Sftp\":\"BGLtest\",\"Port_Sftp\":22,\"FlagGenUnableToServiceReport\":false,\"ReferenceId\":\"A46B88FF\",\"FlagShowLayoutSealOnly\":false,\"PavementLimit\":0.00}");

                result = result.AddDbSet<T, SFOTblMasterMachine_LockType>("{\"Guid\":\"139CC7DA-CEDB-479A-938F-8DC822442DF9\",\"SFOMasterMachine_Guid\":\"26DE6458-985D-4C0D-AA83-A6B655F9CB59\",\"SFOSystemLockType_Guid\":\"59884F00-0EB8-4E7F-8591-19671EC57C8A\",\"FlagDisable\":false,\"UserCreated\":\"supakitb\",\"DatetimeCreated\":\"2019-08-08T11:24:03.5062736+07:00\",\"UniversalDatetimeCreated\":\"2019-08-08T04:24:03Z\",\"UserModified\":\"supakitb\",\"DatetimeModified\":\"2019-08-08T11:24:03.5062736+07:00\",\"UniversalDatetimeModified\":\"2019-08-08T04:24:03Z\",\"LockSeq\":1,\"SerailNumber\":\"1234521\",\"MachineLockID\":\"11521522\",\"SFOSystemUserLockMode_Guid\":\"2933594E-941D-4E58-B6A6-265470697923\",\"ReferenceId\":\"139CC7DA\"}"
                        , "{\"Guid\":\"66E922BC-424D-49A7-AD8B-E326D171ED01\",\"SFOMasterMachine_Guid\":\"26DE6458-985D-4C0D-AA83-A6B655F9CB59\",\"SFOSystemLockType_Guid\":\"59884F00-0EB8-4E7F-8591-19671EC57C8A\",\"FlagDisable\":false,\"UserCreated\":\"supakitb\",\"DatetimeCreated\":\"2019-08-08T11:24:03.5223601+07:00\",\"UniversalDatetimeCreated\":\"2019-08-08T04:24:03Z\",\"UserModified\":\"supakitb\",\"DatetimeModified\":\"2019-08-08T11:24:03.5223601+07:00\",\"UniversalDatetimeModified\":\"2019-08-08T04:24:03Z\",\"LockSeq\":2,\"SerailNumber\":\"9784512\",\"MachineLockID\":\"62123\",\"SFOSystemUserLockMode_Guid\":\"2933594E-941D-4E58-B6A6-265470697923\",\"ReferenceId\":\"66E922BC\"}");

                result = result.AddDbSet<T, SFOTblMasterMachine>("{\"Guid\":\"26DE6458-985D-4C0D-AA83-A6B655F9CB59\",\"MachineID\":\"456721354521\",\"CustomerMachineID\":\"7541222122\",\"SFOMasterMachineServiceType_Guid\":\"B603B2C5-E5EB-49DA-BBEE-235C8A995556\",\"SFOMasterMachineModelType_Guid\":\"446E679C-897F-4E5A-BD9E-F9D0824BA5C1\",\"MasterVendor_Guid\":\"12669F65-09E3-410F-A2A1-38593CEB770F\",\"FlagDailyCreditIndicator\":false,\"FlagRemoteSafeManagementIndicator\":false,\"FlagDeviceDashBoardIndicator\":false,\"DateEntered\":\"2019-08-08T00:00:00\",\"FlagDisable\":false,\"UserCreated\":\"supakitb\",\"DatetimeCreated\":\"2019-08-08T11:24:03.5062736+07:00\",\"UniversalDatetimeCreated\":\"2019-08-08T04:24:03Z\",\"SFOMasterCountryTimeZone_Guid\":\"0A8B66F4-18AF-4430-A820-9B9D6C75D7F3\",\"FLMSLATime\":2520,\"SFOMasterMachineBrand_Guid\":\"0E4DE1F8-AEA9-4A7E-8EBB-462EF019A38B\",\"SFOServicingBranch_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"SFOAssignedBranch_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"SFOFLMBranch_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"SFOArmoredBranch_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"SFOCashBranch_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"FlagEcash\":false,\"FlagNoneLock\":false,\"FlagSuspension\":false,\"SystemMachineOwner_Guid\":\"0C8FDAE6-857A-4B19-9180-7D381D50C0BA\",\"SystemMachineType_Guid\":\"033F4213-78E1-473D-8242-477D03457E81\",\"MaximumCassettes\":10,\"ReferenceId\":\"26DE6458\"}");

                result = result.AddDbSet<T, TblMasterActualJobServiceStopLegs>("{\"Guid\":\"1BAE61AB-8D4F-43D9-94AD-1A61FCE74D00\",\"MasterActualJobHeader_Guid\":\"46A570FC-B12B-426A-B4FD-C8B57BC99769\",\"SequenceStop\":2,\"CustomerLocationAction_Guid\":\"F6BF9DD1-FC98-4DF8-9FC6-D4E9ECC6FF15\",\"MasterCustomerLocation_Guid\":\"239E2E3C-E89A-48E6-A4E4-EFB6BDCBAACF\",\"ServiceStopTransectionDate\":\"2021-02-26T00:00:00\",\"WindowsTimeServiceTimeStart\":\"1900-01-01T00:00:00\",\"WindowsTimeServiceTimeStop\":\"1900-01-01T00:00:00\",\"FlagCancelLeg\":false,\"JobOrder\":0,\"MasterSite_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"FlagNextStop\":false,\"SeqIndex\":0,\"FlagNonBillable\":false,\"FlagDestination\":true,\"UnassignedDatetime\":\"2021-02-23T18:27:53\",\"UnassignedBy\":\"chaikheas\",\"flagHaveSignature\":false,\"FlagHaveImage\":false,\"NoOfImage\":0}"
                        , "{\"Guid\":\"8493D3B4-D9FA-4159-A9F1-E35107C51B88\",\"MasterActualJobHeader_Guid\":\"46A570FC-B12B-426A-B4FD-C8B57BC99769\",\"SequenceStop\":1,\"CustomerLocationAction_Guid\":\"DB81AEE9-9A66-4D69-9D6F-5A407C1B7069\",\"MasterCustomerLocation_Guid\":\"26DE6458-985D-4C0D-AA83-A6B655F9CB59\",\"ServiceStopTransectionDate\":\"2021-02-26T00:00:00\",\"WindowsTimeServiceTimeStart\":\"1900-01-01T00:00:00\",\"WindowsTimeServiceTimeStop\":\"1900-01-01T00:00:00\",\"FlagCancelLeg\":false,\"JobOrder\":1,\"PrintedReceiptNumber\":\"202102260010love35361\",\"MasterSite_Guid\":\"742346F7-F2E8-4347-AD07-77A2811EE636\",\"FlagNextStop\":false,\"SeqIndex\":1,\"FlagNonBillable\":false,\"FlagDestination\":false,\"UnassignedDatetime\":\"2021-02-23T18:27:53\",\"UnassignedBy\":\"chaikheas\",\"flagHaveSignature\":false,\"FlagHaveImage\":false,\"NoOfImage\":0}");

                return result;
            }
        }
    }
}
