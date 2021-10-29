using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Data;
using Bgt.Ocean.Models.OTCManagement;
using System.Data.SqlClient;
using Dapper;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOTblTransactionOTCRepository: IRepository<object>
    {
        IEnumerable<SFOJobListByRunResult> Func_SFO_JobListByRun_Get(IEnumerable<GetJobListByRunModel> jobHeaderGuidList);
        IEnumerable<SFOOTCRequestResult> Func_SFO_GetOTCRequest_Get_Result(IEnumerable<GetOTCRequestModel> jobList, Guid MasterCountry_Guid);
        IEnumerable<OTCStatusResult> GetOTCStatus(Guid countryGuid, string lockSerialNumber);
    }
    public class SFOTblTransactionOTCRepository : Repository<SFOLogDbEntities, object>, ISFOTblTransactionOTCRepository
    {
        public SFOTblTransactionOTCRepository(IDbFactory<SFOLogDbEntities> dbFactory) : base(dbFactory)
        {
           
        }

        public IEnumerable<SFOJobListByRunResult> Func_SFO_JobListByRun_Get(IEnumerable<GetJobListByRunModel> jobHeaderGuidList)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Guid");
            dt.Columns.Add("MasterSite_Guid");
            dt.Columns.Add("MasterActualJobHeader_Guid");
            dt.Columns.Add("JobNo");
            dt.Columns.Add("LOBAbbrevaitionName");
            dt.Columns.Add("ServiceJobTypeNameAbb");
            dt.Columns.Add("LocationName");

            foreach (GetJobListByRunModel job in jobHeaderGuidList)
            {
                dt.Rows.Add(job.Guid, job.MasterSite_Guid, job.MasterActualJobHeader_Guid, job.JobNo, job.LOBAbbrevaitionName, job.ServiceJobTypeNameAbb, job.LocationName);
            }
            SqlParameter parameter = new SqlParameter("UDTOTCJobListRequest", dt) { TypeName = "UDT_SFO_OTCJobListRequest" };
            var query = DbContext.Database.SqlQuery<SFOJobListByRunResult>("Up_OceanOnlineMVC_SFO_JobListByRun_Get @UDTOTCJobListRequest", parameter);
            var result = query.ToList();

            return result;
        }
        public IEnumerable<SFOOTCRequestResult> Func_SFO_GetOTCRequest_Get_Result(IEnumerable<GetOTCRequestModel> jobList, Guid MasterCountry_Guid)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Guid");
            dt.Columns.Add("MasterActualJobHeader_Guid");
            dt.Columns.Add("MachineLockID");
            dt.Columns.Add("SerialNumber");
            dt.Columns.Add("ReferenceCode");
            dt.Columns.Add("MasterEmployee_Guid");
            dt.Columns.Add("MasterEmployee_Guid2");
            dt.Columns.Add("LockMode");
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Hour");
            dt.Columns.Add("TimeBlock");
            foreach (GetOTCRequestModel job in jobList)
            {
                dt.Rows.Add(
                    Guid.NewGuid(),
                    job.MasterActualJobHeader_Guid,
                    job.MachineLockID,
                    job.SerialNumber,
                    job.ReferenceCode,
                    job.MasterEmployee_Guid,
                    job.MasterEmployee_Guid2,
                    job.LockMode,
                    job.Date,
                    job.Hour,
                    job.TimeBlock
                );
            }
            SqlParameter param = new SqlParameter("MasterCountry_Guid", MasterCountry_Guid);
            SqlParameter param2 = new SqlParameter("UDTOTCRequest", dt) { TypeName = "UDT_SFO_OTCRequest" };
            var query = DbContext.Database.SqlQuery<SFOOTCRequestResult>("Up_OceanOnlineMVC_SFO_GetOTCRequest_Get @MasterCountry_Guid, @UDTOTCRequest", param, param2);
            var result = query.ToList();

            return result;
        }

        public IEnumerable<OTCStatusResult> GetOTCStatus(Guid countryGuid, string lockSerialNumber)
        {
            var result = DbContext.Database.Connection.Query<OTCStatusResult>("Up_OceanOnlineMVC_SFO_OTCStatus_Get", new
            {
                MasterCountry_Guid = countryGuid,
                SerialNumber = lockSerialNumber
            }, commandType: CommandType.StoredProcedure);

            return result;
        }
    }    
}
