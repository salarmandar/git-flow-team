
namespace Bgt.Ocean.Infrastructure.Util
{
    public class EnumRoute
    {
        /// <summary>
        /// State : บอกสถานะของการ Update Data In Grid
        /// </summary>
        public static class State
        {
            public const string RowState_Update = "Update";
            public const string RowState_Add = "Add";
            public const string RowState_Edit = "Edit";
            public const string RowState_Delete = "Delete";
        }
        public static class IntTypeJob
        {
            public const int P = 0;
            public const int D = 1;
            public const int TV = 2;
            public const int T = 3;
            public const int AC = 4;
            public const int AE = 5;
            public const int FLM = 6;
            public const int FSLM = 7;  //added: 2018/01/19 -> Second Line Maintenance
            public const int PEN = 8;   //added: 2018/01/19 -> Service Request Pending
            public const int ECash = 9; //added: 2018/01/19 -> Service Request Emergency Cash
            public const int BCP = 10;
            public const int BCD = 11;
            public const int TM = 12;   //added: 2018/01/19 -> Tech Meet
            public const int MCS = 13;// cash add
            /*For support multi branch [OO 6.2.0]*/
            public const int P_MultiBr = 14;  //Pickup Multi Branch
            public const int TV_MultiBr = 15; //Tranfer Vault Multi Branch
            public const int BCD_MultiBr = 16; //BCD Multi Branch
            public const int KP = 17; //Key pickup.
            public const int PM = 18; //Preventive Maintenance
        }
        public static class IntServiceStop
        {
            public const int FistStop = 1;
            public const int SecondStop = 2;
            public const int ThirdStop = 3;
            public const int Fourth = 4;
        }
        public static class JobActionAbb
        {
            public const string StrPickUp = "P";
            public const string StrDelivery = "D";
        }

        public static class JobManualType
        {
            public const int MaskAsCacel = 1;
            public const int MaskAsComplete = 2;

        }

        public static class ItemState
        {
            public const int Added = 1;
            public const int Deleted = -1;
            public const int Unchanged = 0;
            public const int Modified = 2;
        }
        public static class IntStatusJob
        {
            public const int intNoStatus = -1; //2018/04/27 -> changed from 0 to check about job is missing because status is 0.
            public const int Open = 1;
            public const int OnTruck = 2;
            public const int OnTheWay = 3;
            public const int PickedUp = 4;
            public const int Process = 5;
            public const int ReadyToPreVault = 6;
            public const int InPreVault = 7;
            public const int Delivered = 8;
            public const int Closed = 9;
            public const int WaitingPickUp = 10;
            public const int Department = 11;
            public const int ReturnToPreVault = 12;
            public const int NonDelivery = 13;
            public const int CancelledJob = 14;
            public const int MissingStop = 15;
            public const int UnableToService = 16;
            public const int PartialDelivery = 17;
            public const int Unrealized = 20;
            public const int VisitWithStamp = 21;
            public const int VisitWithOutStamp = 22;
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
            public const int WaitingforDeconsolidate = 35;
            public const int InDepartmentandWaitingforDeconsolidate = 36;
            public const int PartialInPrevaultDelivered = 37;
            public const int IntransitInterBranch = 38;
            public const int PartialInDepartment = 39;
            public const int CrewBreakStart = 90;
            public const int CrewBreakEND = 91;
        }

        public static class UnableToServiceCatergory
        {
            public const int Unrealized = 1;
            public const int Visitwithstamp = 2;
            public const int NoArrived = 3;
            public const int VisitWithoutSstamp = 4;
            public const int VisitwithstampVisitwithoutstamp = 5;
            public const int PartailDelivery = 6;
        }
        public static class FixStringPageName
        {
            public const string RUN_CONTROL = "Run Control";
            public const string JOB_SEARCH = "Job Search";
            public const string FAST_DATA_ENTRY = "Fast Data Entry";
            public const string FAST_UPDATE_JOB = "Fast Update Job";
        }

        public static class FixStringRoute
        {
            public const string True = "True";
            public const string JobNo = "JobNo";
            public const string OnTruckWaitingForPickUp = "On Truck (Waiting for Pick up)";
            public const string PageRunControl = "Route/RunControl";
            public const string RunControl_JobInRun = "RunControl_JobInRun";
            public const string RunControl_ServiceStop = "RunControl_ServiceStop";
            public const string RunControl_Vehicle = "RunControl_Vehicle";
            public const string RunControl_Unassigned = "RunControl_Unassigned";
            public const string job = "job";
            public const string service = "service";
            public const string vehicle = "vehicle";
            public const string Unassigned = "Unassigned";
            public const string JobInrun = "jobInrun";
            public const string JobUnassign = "jobUnassign";
            public const string DocumentRef = "DocumentRef";
            public const string Currency = "Currency";
            public const string Comodity = "Comodity";
            public const string ArrivalPickupTime = "Arrival Pick up Time";
            public const string ActualPickupTime = "Actual Pick up Time";
            public const string DeparturePickupTime = "Departure Pick up Time";
            public const string ArrivalDeliveryTime = "Arrival Delivery Time";
            public const string ActualDeliveryTime = "Actual Delivery Time";
            public const string DepartureDeliveryTime = "Departure Delivery Time";
            public const string Cancel = "cancel";
            public const string AnotherRun = "anotherRun";
            public const string Reorder = "reorder";
            public const string DisAlarm = "DisAlarm";
            public const string HHmm = "HH:mm";
            public const string BCD = "BCD";
            public const string MaintenanceID = "MaintenanceID";
            public const string NA = "N/A";
            public const string Liability = "Liability";
            public const string Seal = "Seal";
            public const string SealInLiablity = "Seal in liablity";
        }
        public static class FixMessageRoute
        {
            public const string Message_ChangejobstatustoWaitingforApproveCancel = "Change job status to \"Waiting for Approve Cancel\"";
            public const string ChangejobstatustoOnthewaytoPickedUp = "Change job status to \"On the way to Picked Up\"";
            public const string ChangejobstatustoOnthewayPickedUptoPickedUp = "Change job status to \"On the way Picked Up to Picked Up\"";
            public const string ChangejobstatustoDeconsolidatetoInPreVault = "Change job status to \"Waiting for Deconsolidate to In Pre-Vault\"";
            public const string ChangejobstatustoDeconsolidatetoInPreVaultPickUp = "Change job status to \"Waiting for Deconsolidate to In Pre-Vault - Pick Up\"";
            public const string UpdateDataJobNo = "Update data Job No.";
            public const string create = "create";
            public const string update = "update";
            public const string TimeDefault0000 = "00:00";
        }

        public static class PartialAdhoc
        {
            public const string PickUpPartial = "_PickUpPartial";
            public const string TransferVaultPartial = "_TransferVaultPartial";
            public const string TransferPartial = "_TransferPartial";
            public const string DeliveryPartial = "_DeliveryPartial";
            public const string CheckAll = "Check All";
            public const string getJobHeader = "getJobHeader";
            public const string getJobLeg = "getJobLeg";
        }

        public static class JobSearchControl
        {
            public const string Site = "Site";
            public const string WorkdateFrom = "WorkdateFrom";
            public const string WorkdateTo = "WorkdateTo";
            public const string SFOOption = "SFOOption";
            public const string Customers = "Customers";
            public const string Locations = "Locations";
            public const string RouteGroupDetails = "RouteGroupDetails";
            public const string RunResources = "RunResources";
            public const string SealNos = "SealNos";
            public const string JobStatuses = "JobStatuses";
            public const string DailyEmps = "DailyEmps";
            public const string Airport = "Airport";
            public const string FlagDocReceived = "FlagDocReceived";
            public const string FlagShowAll = "FlagShowAll";
            public const string JobNos = "JobNos";
            public const string LOB = "LOB";
            public const string ServiceType = "Service Type";
        }

        public static class UserSearchControl
        {
            public const string Country = "Country";
            public const string Company = "Company";
            public const string Site = "Site";
            public const string User = "User";
            public const string Status = "Status";
            public const string UserLock = "UserLock";
            public const string UserAccessGroup = "UserAccessGroup";
            public const string PeriodPasswordExpire = "PeriodPasswordExpire";
            public const string PeriodLastLogin = "PeriodLastLogin";
        }

        public enum CITDeliveryStatus
        {
            [EnumDescription("In progress")]
            Inprogress = 0,
            [EnumDescription("Delivered")]
            Delivered = 1,
            [EnumDescription("Not Deliver")]
            Notdeliver = 2
        }

        public enum LOB
        {
            [EnumDescription("ATM")]
            ATM,
            [EnumDescription("BGS")]
            BGS,
            [EnumDescription("CIT")]
            CIT
        }
        public enum CustomerLocationType
        {
            [EnumDescription("ATM Machine")]
            ATM_Machine = 1,
            [EnumDescription("Location")]
            Location = 2,
            [EnumDescription("CompuSafe Machine")]
            CompuSafe_Machine = 3,
            [EnumDescription("Airport")]
            Airport = 4,
            [EnumDescription("Railway")]
            Railway = 5,
            [EnumDescription("eCash")]
            eCash = 6,
            [EnumDescription("Branch")]
            Branch = 7,
            [EnumDescription("Sea Port")]
            Sea_Port = 8,
            [EnumDescription("Transfer Safe")]
            Transfer_Safe = 9,
            [EnumDescription("Key Safe")]
            Key_Safe = 10
        }
        public static class RequestStatus
        {
            public const string INPROGRESS = "In-Progress";
            public const string COMPLETED = "Completed";
            public const string CANCELED = "Canceled";
            public const string FAILED = "Failed";
        }
        public static class RequestStatusID
        {
            public const int INPROGRESS = 1;
            public const int COMPLETED = 3;
            public const int CANCELED = 4;
            public const int FAILED = 5;
        }
        public static class OptimizationStatus
        {
            public const string NONE = "None";
            public const string REQUESTING = "Requesting";
            public const string INPROGRESS = "In-Progress";
            public const string COMPLETED = "Completed";
            public const string CANCELING = "Canceling";
            public const string FAILED = "Failed";
            public const string BROKEN = "Optimization Broken";
            public const string CANCELED = "Canceled";
        }
        public static class OptimizationStatusID
        {
            public const int NONE = 0;
            public const int REQUESTING = 1;
            public const int INPROGRESS = 2;
            public const int COMPLETED = 3;
            public const int CANCELING = 4;
            public const int FAILED = 5;
            public const int BROKEN = 6;
            public const int CANCELED = 7;

        }

        public static class OptimizationRouteTypeID
        {
            public const string RM = "RM";
            public const string RD = "RD";
        }

        public enum RoadNetFileConfig
        {
            [EnumDescription("RouteOptimizationDirectoryPath")]
            RouteOptimizationDirectoryPath
        }

        public enum RoadNetSubFolderArchives
        {
            [EnumDescription("OCEAN2ROADNET\\ARCHIVES\\CountryRoadNet_To_OCEAN\\CURRENT\\")]
            CountryRoadNet_To_OCEAN,
            [EnumDescription("OCEAN2ROADNET\\Archives\\OCEAN_To_CountryRoadNet")]
            OCEAN_To_CountryRoadNet


        }

        public enum RoadNetSubFolderSource
        {
            [EnumDescription("OCEAN2ROADNET\\Source\\CountryRoadNet_To_OCEAN")]
            CountryRoadNet_To_OCEAN,
            [EnumDescription("OCEAN2ROADNET\\Source\\OCEAN_To_CountryRoadNet")]
            OCEAN_To_CountryRoadNet
        }
        public enum RoadNetKeyFileName
        {
            [EnumDescription("OptAdvReq")]
            OptAdvReq,

            [EnumDescription("OptAdvUpd")]
            OptAdvUpd,

            [EnumDescription("OptAdvRes")]
            OptAdvRes,

            [EnumDescription("OptUpdErr")]
            OptUpdErr,

            [EnumDescription("OptCancelReq")]
            OptCancelReq,
            [EnumDescription("OptCancelRes")]
            OptCancelRes


        }
                
        /// <summary>
        /// ใช้กับหน้า RunControl ตัวเก่า
        /// </summary>
        public enum ColumnGrid
        {
            [EnumDescription("PathImageStatus")]
            PathImageStatus,
            [EnumDescription("FlagSyncToMobile")]
            FlagSyncToMobile,
            [EnumDescription("JobNo")]
            JobNo,
            [EnumDescription("ServiceJobTypeNameAbb")]
            ServiceJobTypeNameAbb,
            [EnumDescription("JobStatus")]
            JobStatus,
            [EnumDescription("ActionFlag")]
            ActionFlag,
            [EnumDescription("LOBAbbrevaitionName")]
            LOBAbbrevaitionName,
            [EnumDescription("LocationName")]
            LocationName,
            [EnumDescription("CustomerLocationDelivery")]
            CustomerLocationDelivery,
            [EnumDescription("WindowsTimeServiceTimeStart")]
            WindowsTimeServiceTimeStart,
            [EnumDescription("ActualTime")]
            ActualTime,
            [EnumDescription("DepartTime")]
            DepartTime,
            [EnumDescription("OnwardDestinationLocation")]
            OnwardDestinationLocation,
            [EnumDescription("Country")]
            Country,
            [EnumDescription("AirprotCode")]
            AirprotCode,
            [EnumDescription("FlagEarlyFlight")]
            FlagEarlyFlight,
            [EnumDescription("FlightNo")]
            FlightNo,
            [EnumDescription("ETD")]
            ETD,
            [EnumDescription("ETA")]
            ETA,
            [EnumDescription("Weights")]
            Weights,
            [EnumDescription("Pieces")]
            Pieces,
            [EnumDescription("MAWB")]
            MAWB,
            [EnumDescription("HAWB")]
            HAWB,
            [EnumDescription("AirportDocChecked")]
            AirportDocChecked,
            [EnumDescription("RevisedTime")]
            RevisedTime,
            [EnumDescription("UserCreated")]
            UserCreated,
            [EnumDescription("UserModifed")]
            UserModifed,
            [EnumDescription("Remarks")]
            Remarks,
            [EnumDescription("ServiceStopName")]
            ServiceStopName,
            [EnumDescription("VehicleStopName")]
            VehicleStopName,
            [EnumDescription("TimeStart")]
            TimeStart,
            [EnumDescription("TotalJobs")]
            TotalJobs,
            [EnumDescription("RowNo")]
            RowNo
        }
        

    }
}
