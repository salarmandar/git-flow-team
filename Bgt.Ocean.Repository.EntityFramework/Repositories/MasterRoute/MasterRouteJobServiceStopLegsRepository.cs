using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Bgt.Ocean.Models.MasterRoute;
using Bgt.Ocean.Repository.EntityFramework.StringQuery.Masterroute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute
{
    public interface IMasterRouteJobServiceStopLegsRepository : IRepository<TblMasterRouteJobServiceStopLegs>
    {
        IEnumerable<MasterRouteUpdateJobOrderSetResult> MasterRouteUpdateJobOrder(Guid? masterRouteGuid, string routeGroupDetail, bool flagAllRoute, Guid actionProcess, Guid category, string userModify, DateTime clientDateTime, Guid? masterRouteEnableGuid);
        MasterRouteUpdateSeqIndexResult UpdateMasterRouteJobSeqIndex(Guid masterRouteGuid, IEnumerable<Guid?> routeGroupDetail, string userCreate, DateTime clientDate);
        IEnumerable<MasterRouteJobDetailForManualChangeSeqIndexResult> GetManualChangeSeqIndex(Guid masterRouteGuid, Guid? routeGroupDetail, string userCreate, DateTime clientDate, int jobOrder, int sourceSeqIndex, int targetSeqIndex);
        IEnumerable<T> ExectueQuery<T>(string queryStr, Dictionary<string, object> param);
        MassUpdateDataView ExectueMassUpdateData(string queryStr, Dictionary<string, object> param);

    }

    public class MasterRouteJobServiceStopLegsRepository : Repository<OceanDbEntities, TblMasterRouteJobServiceStopLegs>, IMasterRouteJobServiceStopLegsRepository
    {
        public MasterRouteJobServiceStopLegsRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<MasterRouteUpdateJobOrderSetResult> MasterRouteUpdateJobOrder(Guid? masterRouteGuid, string routeGroupDetail, bool flagAllRoute, Guid actionProcess, Guid category, string userModify, DateTime clientDateTime, Guid? masterRouteEnableGuid)
        {
            return DbContext.Up_OceanOnlineMVC_MasterRoute_UpdateJobOrder_Set(masterRouteGuid, routeGroupDetail, flagAllRoute, actionProcess, category, "", userModify, clientDateTime, masterRouteEnableGuid);
        }

        public MasterRouteUpdateSeqIndexResult UpdateMasterRouteJobSeqIndex(Guid masterRouteGuid, IEnumerable<Guid?> routeGroupDetail, string userCreate, DateTime clientDate)
        {
            var queryStr = new UpdateSeqIndexQuery().QueryUpdateSeqIndex;
            var DyParam = new DynamicParameters();
            DyParam.Add("@MasterRoute_Guid", masterRouteGuid);
            DyParam.Add("@MasterRouteGroupDetail_Guid", string.Join(",", routeGroupDetail.Select(s => s.IsNullOrEmpty() ? "N/A" : s.ToString())));
            DyParam.Add("@UserModifed", userCreate);
            DyParam.Add("@ClientDate", clientDate);
            return DbContextConnection.Query<MasterRouteUpdateSeqIndexResult>(queryStr, DyParam).FirstOrDefault();
        }
        public IEnumerable<MasterRouteJobDetailForManualChangeSeqIndexResult> GetManualChangeSeqIndex(Guid masterRouteGuid, Guid? routeGroupDetail, string userCreate, DateTime clientDate, int jobOrder, int sourceSeqIndex, int targetSeqIndex)
        {
            var queryGetMasterRouteJob = new UpdateSeqIndexQuery().QueryGetMasterRouteJob;
            string queryStr = string.Format(queryGetMasterRouteJob, "");
            var DyParam = new DynamicParameters();
            DyParam.Add("@MasterRoute_Guid", masterRouteGuid);
            DyParam.Add("@MasterRouteGroupDetail_Guid", routeGroupDetail);
            return DbContextConnection.Query<MasterRouteJobDetailForManualChangeSeqIndexResult>(queryStr, DyParam).ToList();
        }

        public IEnumerable<T> ExectueQuery<T>(string queryStr, Dictionary<string, object> param)
        {
            DynamicParameters dyParam;

            if (param.Any())
            {
                dyParam = new DynamicParameters();
                foreach (var item in param)
                {
                    dyParam.Add(item.Key, item.Value);
                }
                var result = DbContextConnection.Query<T>(queryStr, dyParam);
                return result;
            }

            return DbContextConnection.Query<T>(queryStr).ToList();
        }

        public MassUpdateDataView ExectueMassUpdateData(string queryStr, Dictionary<string, object> param)
        {

            DynamicParameters dyParam;
            MassUpdateDataView result = new MassUpdateDataView();
            SqlMapper.GridReader queryResult;
            if (param.Any())
            {
                dyParam = new DynamicParameters();
                foreach (var item in param)
                {
                    dyParam.Add(item.Key, item.Value);
                }
                queryResult = DbContextConnection.QueryMultiple(queryStr, dyParam);

            }
            else
            {
                queryResult = DbContextConnection.QueryMultiple(queryStr);
            }
            result.Masterroute = queryResult.Read<MasterrouteView>();
            result.Msg = queryResult.Read<MessageResponseView>().FirstOrDefault();
            return result;
        }
    }

}
