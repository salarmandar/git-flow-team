
namespace Bgt.Ocean.Infrastructure.Util
{
    public class EnumMasterRoute
    {
        public enum EnumMasterRouteLogCategory
        {
            Defult,
            [EnumDescription("Job_Create")]
            Job_Create,
            [EnumDescription("Job_Edit")]
            Job_Edit,
            [EnumDescription("Job_MassUpdateJob")]
            Job_MassUpdateJob,
            [EnumDescription("Job_Remove")]
            Job_Remove,
            [EnumDescription("Job_SetScheduleTime")]
            Job_SetScheduleTime,
            [EnumDescription("Job_UpdateRoute")]
            Job_UpdateRoute,
            [EnumDescription("Job_UpdateRouteDragAndDrop")]
            Job_UpdateRouteDragAndDrop,
            [EnumDescription("Seq_ChangeStopSequence")]
            Seq_ChangeStopSequence,
            [EnumDescription("Seq_DragAndDrop")]
            Seq_DragAndDrop,
            [EnumDescription("Seq_EditJob")]
            Seq_EditJob,
            [EnumDescription("Seq_ScheduleTime")]
            Seq_ScheduleTime,
            [EnumDescription("Seq_SequenceChanges")]
            Seq_SequenceChanges,
            [EnumDescription("Seq_SequenceChangesLegD")]
            Seq_SequenceChangesLegD,
            [EnumDescription("Seq_SortSameStop")]
            Seq_SortSameStop,
            [EnumDescription("Seq_UpdateRoute")]
            Seq_UpdateRoute,

            //generate
            [EnumDescription("Job_GenPerStopAuto")]
            Job_GenPerStopAuto,
            [EnumDescription("Job_GenPerStopAutoLegD")]
            Job_GenPerStopAutoLegD,
            [EnumDescription("Job_GenPerStopSelect")]
            Job_GenPerStopSelect,
            [EnumDescription("Job_GenPerStopSelectLegD")]
            Job_GenPerStopSelectLegD,
            [EnumDescription("Job_GenPerStopUnassign")]
            Job_GenPerStopUnassign,
            [EnumDescription("Job_GenPerStopUnassignLegD")]
            Job_GenPerStopUnassignLegD,
            [EnumDescription("Job_GenPerTemplateAuto")]
            Job_GenPerTemplateAuto,
            [EnumDescription("Job_GenPerTemplateAutoLegD")]
            Job_GenPerTemplateAutoLegD,
            [EnumDescription("Job_GenPerTemplateSelect")]
            Job_GenPerTemplateSelect,
            [EnumDescription("Job_GenPerTemplateSelectLegD")]
            Job_GenPerTemplateSelectLegD,
            [EnumDescription("Job_GenPerTemplateUnassign")]
            Job_GenPerTemplateUnassign,
            [EnumDescription("Job_GenPerTemplateUnassignLegD")]
            Job_GenPerTemplateUnassignLegD
        }

        public enum EnumMasterRouteProcess
        {
            [EnumDescription("MASTER_ROUTE_TEMPLATE")]
            MASTER_ROUTE_TEMPLATE,
            [EnumDescription("MASTER_ROUTE_JOB")]
            MASTER_ROUTE_JOB,
            [EnumDescription("MASTER_ROUTE_GROUP")]
            MASTER_ROUTE_GROUP
        }
        public enum EnumMasterRouteConfigKey
        {
            [EnumDescription("MRJ_EDIT_JOB")]
            MRJ_EDIT_JOB,
            [EnumDescription("MRJ_EDIT_LEG")]
            MRJ_EDIT_LEG,
            [EnumDescription("MRJ_EDIT_JOB_API")]
            MRJ_EDIT_JOB_API,
            [EnumDescription("MRJ_EDIT_LEG_API")]
            MRJ_EDIT_LEG_API
        }

        public static class MasterRouteJobLegDisplay
        {
            public const string StrPickUp = "Pickup Leg ";
            public const string StrDelivery = "Delivery Leg ";
        }

        public static class MasterRouteJobCheckbox
        {
            public const string StrSelected = "Selected";
            public const string StrUnSelected = "Unselected";
        }
    }
}
