using System;
using EntityFrameworkExtras.EF6;
using System.Data;

namespace Bgt.Ocean.Models.RunControl
{

    [StoredProcedure("Up_OceanOnlineMVC_CloseRunManually_Set")]
    public class CloseRunManuallyProcedure
    {
        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "DailyRun")]
        public Guid DailyRun { get; set; }

        [StoredProcedureParameter(SqlDbType.UniqueIdentifier, ParameterName = "UserGuid")]
        public Guid UserGuid { get; set; }

        [StoredProcedureParameter(SqlDbType.NVarChar, ParameterName = "StrClientDate")]
        public string StrClientDate { get; set; }

        [StoredProcedureParameter(SqlDbType.Int, Direction = ParameterDirection.Output, ParameterName = "MsgID")]
        public int MsgID { get; set; }

        [StoredProcedureParameter(SqlDbType.NVarChar, Direction = ParameterDirection.Output, ParameterName = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
