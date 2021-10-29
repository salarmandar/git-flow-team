using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Configuration;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    #region Interface

    public interface IReasonCodeRepository : IRepository<TblMasterReasonType>
    {
        List<ReasonCodeView> GetReasonCodeList(ReasonCodeView_Request request);
    }

    #endregion

    public class ReasonCodeRepository : Repository<OceanDbEntities, TblMasterReasonType>, IReasonCodeRepository
    {
        public ReasonCodeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public List<ReasonCodeView> GetReasonCodeList(ReasonCodeView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<ReasonCodeView>
                (
                    "Up_OceanOnlineMVC_API_GetReasonCodeList",
                    new
                    {
                        @MaxRow = WebConfigurationManager.AppSettings["MaxRow"],
                        @CountryAbb = request.countryAbb,
                        @CategoryId = request.reasonTypeCategoryId,
                        @CreatedFrom = request.createdDatetimeFrom,
                        @CreatedTo = request.createdDatetimeTo
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result;
        }
    }
}
