using System;
using System.Collections.Generic;
using EntityFrameworkExtras.EF6;
using System.Data;

namespace Bgt.Ocean.Models.MasterRoute
{

    [StoredProcedure("Up_OceanOnlineMVC_MasterRoute_CopyJobs_Set")]
    public class MasterRouteCopyJobsProcedure
    {
        // SOURCE:PARAMS
        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "Src_RouteGuid")]
        public Guid Src_RouteGuid { get; set; }

        // SOURCE:UDT
        [StoredProcedureParameter(SqlDbType.Udt, ParameterName = "tmpUDTOverNight")]
        public List<MasterRouteOverNightJobs> OverNightJobs { get; set; }

        [StoredProcedureParameter(SqlDbType.Udt, ParameterName = "tmpUDTSelectedJobs")]
        public List<MasterRouteSeletedJobs> SeletedJobs { get; set; }


        // DESTINATION:PARAMS
        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "Dst_DayOfWeekGuid")]
        public Guid Dst_DayOfWeekGuid { get; set; }

        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "Dst_RouteGroupDetailGuid")]
        public Guid? Dst_RouteGroupDetailGuid { get; set; }

        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "Dst_RouteGuid")]
        public Guid Dst_RouteGuid { get; set; }

        [StoredProcedureParameter(SqlDbType.Bit, ParameterName = "FlagAllTemplate")]
        public bool FlagAllTemplate { get; set; }


        //USER CONFIG
        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "LanguageGuid")]
        public Guid LanguageGuid { get; set; }

        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "UserGuid")]
        public Guid UserGuid { get; set; }

        [StoredProcedureParameter(SqlDbType.DateTime, ParameterName = "ClientDate")]
        public DateTime ClientDate { get; set; }



        // OUTPUT
        [StoredProcedureParameter(SqlDbType.Int, Direction = ParameterDirection.Output, ParameterName = "MsgID")]
        public int MsgID { get; set; }

        [StoredProcedureParameter(SqlDbType.NVarChar, Direction = ParameterDirection.Output, ParameterName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        // SELECT RESULT
        public IEnumerable<MasterRouteCopyJobsResult> CantCopyJobs { get; set; }
    }



    [UserDefinedTableType("UDT_MVC_MasterRoute_OverNightJobs")]
    public class MasterRouteOverNightJobs
    {
        [UserDefinedTableTypeColumn(1)]
        public int? SequenceStop { get; set; }
        [UserDefinedTableTypeColumn(2)]
        public Guid? JobHeaderGuid { get; set; }
        [UserDefinedTableTypeColumn(3)]
        public Guid? LegGuid { get; set; }
        [UserDefinedTableTypeColumn(4)]
        public Guid? RouteGuid { get; set; }
        [UserDefinedTableTypeColumn(5)]
        public Guid? RouteDetailGuid { get; set; }

    }

    [UserDefinedTableType("UDT_MVC_MasterRoute_SeletedJobs")]
    public class MasterRouteSeletedJobs
    {
        [UserDefinedTableTypeColumn(1)]
        public Guid? JobHeaderGuid { get; set; }
    }

    public class MasterRouteCopyJobsResult
    {
        public int SeqIndex { get; set; }
        public string JobTypeAbb { get; set; }
        public string Action { get; set; }
        public string LocationName { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
    }
}
