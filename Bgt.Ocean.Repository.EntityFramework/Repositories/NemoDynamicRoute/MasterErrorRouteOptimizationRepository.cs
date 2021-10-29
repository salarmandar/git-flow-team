using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.NemoDynamicRoute
{
    /// <summary>
    /// Interface with definitions of actions related with table TblMasterErrorRouteOptimization.
    /// </summary>
    public interface IMasterErrorRouteOptimizationRepository : IRepository<TblMasterNemoErrorRouteOptimization>
    {
        //public TblMasterNemoErrorRouteOptimization GetNemoErrorRouteOptimization(Guid? siteGuid);
    }

    public class MasterErrorRouteOptimizationRepository : Repository<OceanDbEntities, TblMasterNemoErrorRouteOptimization>, IMasterErrorRouteOptimizationRepository
    {
        #region Contructor

        /// <summary>
        /// Inititalizes a new instance of the <see cref="MasterErrorRouteOptimizationRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">factory with dataContext.</param>
        public MasterErrorRouteOptimizationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        #endregion

        #region Methods

        #region Public Methods
        //public TblMasterNemoErrorRouteOptimization GetNemoErrorRouteOptimization(Guid? siteGuid)
        //{
        //    var company = DbContext.TblMasterNemoErrorRouteOptimization.Where(e => e.MasterSite_Guid == siteGuid && e.FlagDisable == false)
        //        .Join(DbContext.TblMasterCustomer.Where(e => e.FlagDisable == false && e.FlagChkCustomer == false),
        //        CL => CL.MasterCustomer_Guid,
        //        C => C.Guid,
        //       (CL, C) => new { CusLocation = CL, Customer = C }).Select(x => x.Customer).FirstOrDefault();

        //    return company;
        //}

        #endregion

        #region Public Private
        #endregion

        #endregion
    }
}
