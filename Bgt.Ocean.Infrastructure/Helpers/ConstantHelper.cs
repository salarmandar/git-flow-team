using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumGlobalEnvironment;

namespace Bgt.Ocean.Infrastructure.Helpers
{
    public static class SystemTimeZoneHelper
    {
        /// <summary>
        /// UTC TimezoneID = 36
        /// </summary>
        public const int UTC = 36;
    }

    public static class LineOfBusinessHelper
    {
        public const string BGSGuid = "86d88499-a0f1-4e1f-aabf-2e161c2b3447";
        public const string ATMGuid = "a6d0d672-1b43-407e-b738-31d65baf9115";
        public const string CompuSafeGuid = "5f59b9b8-218b-40d3-aba7-7db035e54487";
        public const string OtherGuid = "b19f1624-f083-4f97-88b2-920d05ba0de2";
        public const string CITGuid = "db65f1db-8c61-436d-b762-cc585df27547";
    }


    public static class ServiceJobTypeHelper
    {
        public const int ServiceJobTypeFLM = 6;
        public const int ServiceJobTypeSLM = 7;
        public const int ServiceJobTypeTechmeetID = 12;
        public const int ServiceJobTypeMCS = 13;
        public const int ServiceJobTypeEcashID = 9;
        public const int ServiceJobTypePendingID = 8;

        public const string ServiceJobTypeFLMGuid = "df8a1b04-e90b-4613-af3b-7d2764709861";
        public const string ServiceJobTypeSLMGuid = "5319167f-a482-427e-8bef-d5b1a127956f";
        public const string ServiceJobTypePENGuid = "f46f8459-31d2-4830-9d54-f5f27e3e8b91";
        public const string ServiceJobTypeECASHGuid = "0dc2c6be-6074-4ae7-8eae-6d3d7534477c";
        public const string ServiceJobTypeTMGuid = "64c94bff-001d-4d9a-b894-ef3dd9b7eb85";
        public const string ServiceJobTypeMCSGuid = "960f9752-66b9-49f2-bc19-f717ff203593";
    }

    /// <summary>
    /// For example Cash Add
    /// </summary>
    public static class SubServiceType
    {
        /// <summary>
        /// LOB ATM > Cash Add
        /// </summary>
        public static Guid SubServiceTypeATM_CashAdd
        {
            get
            {
                return Guid.Parse("91D2096B-EFC4-4D21-8117-F31D01092EB3");
            }
        }

        /// <summary>
        /// LOB Compusafe > Cash Add
        /// </summary>
        public static Guid SubServiceTypeCPS_CashAdd
        {
            get
            {
                return Guid.Parse("E494BD90-4659-4CCD-B76D-CC3560C39C39");
            }
        }
    }

    public static class JobStatusHelper
    {
        public const int Open = 1;
        public const int OnTruck = 2;
        public const int OnTheWay = 3;
        public const int PickedUp = 4;
        public const int Process = 5;
        public const int ReadyToInPreVault = 6;
        public const int InPreVault = 7;
        public const int Delivered = 8;
        public const int Closed = 9;
        public const int WaitingPickUp = 10;
        public const int InDepartment = 11;
        public const int ReturnToPreVault = 12;
        public const int NonDelivery = 13;
        public const int CancelledJob = 14;
        public const int MissingStop = 15;
        public const int UnableToService = 16;
        public const int PartialDelivery = 17;
        public const int Unrealized = 20;
        public const int VisitWithStamp = 21;
        public const int VisitWithoutStamp = 22;
        public const int NoArrived = 23;
        public const int OnTruckPickUp = 24;
        public const int OnTruckDelivery = 25;
        public const int OnTheWayPickUp = 26;
        public const int OnTheWayDelivery = 27;
        public const int InPreVaultPickUp = 28;
        public const int InPreVaultDelivery = 29;
        public const int CancelAndReturnToPreVault = 30;
        public const int DeliverToPreVault = 31;
        public const int WaitingforApproveCancel = 32;
        public const int WaitingforApproveScheduleTime = 33;
        public const int WaitingforDolphinReceive = 34;
        public const int UnableToServiceInPreVault = 35;
        public const int CrewBreakStart = 90;
        public const int CrewBreakEnd = 91;
        public const int Pending = 103;
        public const int Dispatched = 104;

        [Obsolete("Use CancelledJob Instead", false)]
        public const int Cancelled = 105;
        public const int Reactivated = 106;
        public const int Planned = 107;
        public const int Reopen = 108;
        public const int OnHold = 109;

        public const string DispatchStr = "Dispatch";
        public const string OpenStr = "Open";
        public const string PlanStr = "Plan";

        public const string OpenGuid = "b79a7b54-c618-41e0-a037-923a224272c2";
        public const string OnTruckGuid = "a2b4576a-a6f5-4411-8669-93bef089d04d";
        public const string OnTheWayGuid = "be508ddf-5fd1-4742-89d9-6f2aea97120c";
        public const string PickedUpGuid = "d5bb834f-91bc-48cd-b128-6a28e999eba2";
        public const string ProcessGuid = "e9d68d91-d418-4bf8-9a59-911abaab3dba";
        public const string ReadyToInPreVaultGuid = "66cd8fd5-e271-44f7-b6f2-0b02717a365f";
        public const string InPreVaultGuid = "fd7d60e6-570f-4e26-9991-02f819fc5ae8";
        public const string DeliveredGuid = "b5be11b2-efef-49fd-a769-2aedec163a2b";
        public const string ClosedGuid = "2401c373-f36b-448f-bb2b-a9865d44b4bb";
        public const string WaitingPickUpGuid = "b0e29318-cc2c-45d4-a7dc-9066021dbc7e";
        public const string InDepartmentGuid = "cdd1ddd3-b021-4be1-983f-e05345b3cf35";
        public const string ReturnToPreVaultGuid = "d6a8cc69-186e-4692-89f1-7afcc8b2ed88";
        public const string NonDeliveryGuid = "6bcf5246-3938-4c2a-868b-0e233895cffc";
        public const string CancelledJobGuid = "08670529-e589-46a1-8ee0-6096778797a3";
        public const string MissingStopGuid = "f21ce96b-0a35-46a1-b0cd-8568f612fa69";
        public const string UnableToServiceGuid = "6e2e8163-0d2a-4198-8598-fd1ada01839f";
        public const string PartialDeliveryGuid = "dd86fc44-139d-4413-8ecc-1195224fc0f0";
        public const string UnrealizedGuid = "1efe1c80-e108-4fdf-a87c-fe65b77c2675";
        public const string VisitWithStampGuid = "614197d8-88c5-4574-8eed-0b127fd9f13e";
        public const string VisitWithoutStampGuid = "981eae50-51b4-412a-be7a-b0563fe5f22a";
        public const string NoArrivedGuid = "210e0d67-2864-40f6-b0f9-d872c8f8b190";
        public const string OnTruckPickUpGuid = "071751f8-a9b6-4000-a214-cc62d06aa9e7";
        public const string OnTruckDeliveryGuid = "ab259d50-714a-478a-b208-6d43632be9c2";
        public const string OnTheWayPickUpGuid = "13553902-4315-4801-9bab-de443592ae29";
        public const string OnTheWayDeliveryGuid = "493f531c-0e21-4f30-8991-626cbbc82d98";
        public const string InPreVaultPickUpGuid = "f1f1e516-324d-4ed9-99a2-ddcfb9b97606";
        public const string InPreVaultDeliveryGuid = "29f1c22a-187e-4871-a259-8581e9b32641";
        public const string CancelAndReturnToPreVaultGuid = "02bd6127-b64a-4179-95e4-a2f1a1309831";
        public const string DeliverToPreVaultGuid = "268bc33c-d709-4e52-835a-71d29bf25703";
        public const string WaitingforApproveCancelGuid = "56c89331-fcca-43d3-a433-e35c455597cc";
        public const string WaitingforApproveScheduleTimeGuid = "9e8db9b1-caa6-4a13-8c54-5a4e93ab5b2c";
        public const string WaitingforDolphinReceiveGuid = "9ffbc66f-348e-4c15-b0e9-4c8722a63a31";
        public const string UnableToServiceInPreVaultGuid = "ab1135c7-9d75-4d73-ab52-25aa765ed458";
        public const string CrewBreakStartGuid = "ca0f631e-69d6-45a6-94c8-62096ad3f661";
        public const string CrewBreakEndGuid = "c2369a86-df94-42bb-be10-eaa507e95f9c";
        public const string PendingGuid = "66308efd-db0c-44cf-b5dc-296ec5aaf0c9";
        public const string DispatchedGuid = "a46002fb-a975-45fd-bdd4-828d6dbc9e2a";
        public const string CancelledGuid = "ce8f921f-07c1-4f99-a4ba-9de1387533ed";
        public const string ReactivatedGuid = "8f62dce9-62ff-487f-a8a9-93b75a9e85f4";
        public const string PlannedGuid = "4e851173-c577-429c-9d06-86c0ec1041c6";
        public const string ReopenGuid = "aa8405da-028b-4584-999e-26654369de95";
        public const string OnHoldGuid = "31e839ee-9ec0-4ba6-a35d-b5c4a96fd820";

        /// <summary>
        /// Get Status ID by status guid
        /// </summary>
        /// <param name="jobStatusGuid"></param>
        /// <returns></returns>
        public static int GetStatusIDByGuid(Guid jobStatusGuid)
        {
            Dictionary<string, int> dictStatus = new Dictionary<string, int>();
            dictStatus.Add("b79a7b54-c618-41e0-a037-923a224272c2", 1); //Open
            dictStatus.Add("a2b4576a-a6f5-4411-8669-93bef089d04d", 2); //On Truck
            dictStatus.Add("be508ddf-5fd1-4742-89d9-6f2aea97120c", 3); //On The Way
            dictStatus.Add("d5bb834f-91bc-48cd-b128-6a28e999eba2", 4); //Picked Up
            dictStatus.Add("e9d68d91-d418-4bf8-9a59-911abaab3dba", 5); //Process
            dictStatus.Add("66cd8fd5-e271-44f7-b6f2-0b02717a365f", 6); //Ready To In Pre-Vault
            dictStatus.Add("fd7d60e6-570f-4e26-9991-02f819fc5ae8", 7); //In Pre-Vault
            dictStatus.Add("b5be11b2-efef-49fd-a769-2aedec163a2b", 8); //Delivered
            dictStatus.Add("2401c373-f36b-448f-bb2b-a9865d44b4bb", 9); //Closed
            dictStatus.Add("b0e29318-cc2c-45d4-a7dc-9066021dbc7e", 10); //Waiting Pick Up
            dictStatus.Add("cdd1ddd3-b021-4be1-983f-e05345b3cf35", 11); //In Department
            dictStatus.Add("d6a8cc69-186e-4692-89f1-7afcc8b2ed88", 12); //Return To Pre-Vault
            dictStatus.Add("6bcf5246-3938-4c2a-868b-0e233895cffc", 13); //Non Delivery
            dictStatus.Add("08670529-e589-46a1-8ee0-6096778797a3", 14); //Cancelled Job
            dictStatus.Add("f21ce96b-0a35-46a1-b0cd-8568f612fa69", 15); //Missing Stop
            dictStatus.Add("6e2e8163-0d2a-4198-8598-fd1ada01839f", 16); //Unable To Service
            dictStatus.Add("dd86fc44-139d-4413-8ecc-1195224fc0f0", 17); //Partial Delivery
            dictStatus.Add("1efe1c80-e108-4fdf-a87c-fe65b77c2675", 20); //Unrealized
            dictStatus.Add("614197d8-88c5-4574-8eed-0b127fd9f13e", 21); //Visit With Stamp
            dictStatus.Add("981eae50-51b4-412a-be7a-b0563fe5f22a", 22); //Visit Without Stamp
            dictStatus.Add("210e0d67-2864-40f6-b0f9-d872c8f8b190", 23); //No Arrived
            dictStatus.Add("071751f8-a9b6-4000-a214-cc62d06aa9e7", 24); //On Truck - Pick Up
            dictStatus.Add("ab259d50-714a-478a-b208-6d43632be9c2", 25); //On Truck - Delivery
            dictStatus.Add("13553902-4315-4801-9bab-de443592ae29", 26); //On The Way - Pick Up
            dictStatus.Add("493f531c-0e21-4f30-8991-626cbbc82d98", 27); //On The Way - Delivery
            dictStatus.Add("f1f1e516-324d-4ed9-99a2-ddcfb9b97606", 28); //In Pre-Vault - Pick Up
            dictStatus.Add("29f1c22a-187e-4871-a259-8581e9b32641", 29); //In Pre-Vault - Delivery
            dictStatus.Add("02bd6127-b64a-4179-95e4-a2f1a1309831", 30); //Cancel And Return To Pre-Vault
            dictStatus.Add("268bc33c-d709-4e52-835a-71d29bf25703", 31); //Deliver To Pre-Vault
            dictStatus.Add("56c89331-fcca-43d3-a433-e35c455597cc", 32); //Waiting for Approve Cancel
            dictStatus.Add("9e8db9b1-caa6-4a13-8c54-5a4e93ab5b2c", 33); //Waiting for Approve Schedule Time
            dictStatus.Add("9ffbc66f-348e-4c15-b0e9-4c8722a63a31", 34); //Waiting for Dolphin Receive
            dictStatus.Add("ab1135c7-9d75-4d73-ab52-25aa765ed458", 35); //Waiting for Deconsolidate
            dictStatus.Add("f7aabdf9-afd3-40e7-b603-e6ef4fcd2145", 36); //In Department and Waiting for Deconsolidate
            dictStatus.Add("d7ffbb3e-7485-4247-8117-e10bfffaac88", 37); //Partial In Pre-Vault Delivery
            dictStatus.Add("ca0f631e-69d6-45a6-94c8-62096ad3f661", 90); //Crew Break Start
            dictStatus.Add("c2369a86-df94-42bb-be10-eaa507e95f9c", 91); //Crew Break End
            dictStatus.Add("66308efd-db0c-44cf-b5dc-296ec5aaf0c9", 103); //Pending
            dictStatus.Add("a46002fb-a975-45fd-bdd4-828d6dbc9e2a", 104); //Dispatched
            dictStatus.Add("ce8f921f-07c1-4f99-a4ba-9de1387533ed", 105); //Cancelled
            dictStatus.Add("8f62dce9-62ff-487f-a8a9-93b75a9e85f4", 106); //Reactivated
            dictStatus.Add("4e851173-c577-429c-9d06-86c0ec1041c6", 107); //Planned
            dictStatus.Add("aa8405da-028b-4584-999e-26654369de95", 108); //Reopen
            dictStatus.Add("31e839ee-9ec0-4ba6-a35d-b5c4a96fd820", 109); //OnHold
            return dictStatus[jobStatusGuid.ToString()];
        }

    }

    public static class AppSetting
    {
        public static string[] AppFormat { get { return EnvironmentSetting.SystemFormatDate.Split('|'); } }
    }

    public static class ConstFlagSyncToMobile
    {
        public const int CreateToMobile = 0;
        public const int AlreadySyncToMobile = 1;
        public const int UpdateToMobile = 7;
        public const int CancelBeforeMobileReceive = 88;
    }

    public static class FlagSyncToMobile
    {
        private static Dictionary<int, string> _statusMobileList { get; set; }
        private static void InitialValues()
        {
            Dictionary<int, string> statusList = new Dictionary<int, string>();
            statusList.Add(0, "job created");
            statusList.Add(1, "mobile receive");
            statusList.Add(2, "job update");
            statusList.Add(3, "sicop update get current status");
            statusList.Add(4, "reorder push from ocean");
            statusList.Add(5, "job canceled");
            statusList.Add(6, "job interchange");
            statusList.Add(77, "mobile receive 2 legs(TV) (EE)");
            statusList.Add(88, "job cancel before mobile receive (EE)");
            statusList.Add(99, "mobile getJob(T) only 1  leg");

            _statusMobileList = statusList;
        }

        public static string GetTextByFlagSync(int flagSyncId)
        {
            if (_statusMobileList == null)
                InitialValues();

            return _statusMobileList.FirstOrDefault(e => e.Key == flagSyncId).Value;
        }
    }

    public static class DenominationType
    {
        public const int Note = 1;
        public const int Coin = 2;
    }

    public static class DenominationUnit
    {
        public const string Note = "Note";
    }

    public static class SubServiceTypeHelper
    {
        public const int CashAdd = 1;

    }

    public static class ApplicationKey
    {
        public readonly static Guid OceanOnline = Guid.Parse("57DEF301-431D-4AAE-880C-E9F9A8EAA897");
        public readonly static Guid StarfishOnline = Guid.Parse("4A7318D2-E41E-43F9-AE5A-E81D21766A6F");
        public readonly static Guid OceanIntegrationAPI = Guid.Parse("52B105C7-70B5-4713-94E4-5703D98EC5C9");
    }

    #region SFO Helper

    public static class MachineTypeHelper
    {
        public const string ATMMachineGuid = "033f4213-78e1-473d-8242-477d03457e81";
        public const string CompuSafeMachineGuid = "fa2530a8-5362-41e0-a1b0-93e74ffb3f15";
    }

    public static class EnvBranchTypeHelper
    {
        public const int ServicingBranchID = 1;
        public const int FLMBranchID = 2;
    }

    public static class SFOLogCategoryHelper
    {
        public const string ServiceRequestDetailsGuid = "7f701ff0-b018-4c16-985f-6a8b078b4ec8";
        public const string STD_CUSLOC_DETAIL = "DB1E735A-ED09-4E1C-8455-7FC16A377559";

        public const string ADM_USER_MGR_USER_PROFILE = "DA593FC9-481C-4D0F-8123-757E4FAE604B";
        public const string ADM_USER_MGR_USER_ACCOUNT = "2C52CBB9-7A60-4505-8669-B1210B4716B6";
        public const string ADM_USER_MGR_PASSWORD = "ABA557BC-A7F4-47BF-8E28-E07357668339";
    }

    public static class SFOProcessHelper
    {
        public const string ServiceRequestGuid = "9f1e0ef8-0cf6-423a-8136-18905682a608";
        public const string STD_CustomerLocation = "39547FFA-0CAD-445F-8964-1FD7A39D4DA4";

        public const string ADM_USER_MGR = "FB8291BD-7990-493D-BB0F-242F61917FA6";
    }

    public static class SFOSystemDataConfigurationDataKeyHelper
    {
        public const string SFO_API_USER = "SFO_API_USER";
    }

    public static class LockStatusCode
    {
        public const string InActive = "00";
        public const string Active = "01";
        public const string Close = "02";
        public const string CloseFail = "03";
        public const string ErrorFail = "04";
    }

    public static class DailyRunStatus
    {
        public const int Ready = 1;
        public const int DispatchRun = 2;
        public const int ClosedRun = 3;
        public const int CrewBreak = 4;
    }

    #endregion
}
