using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute;
using DataSoruce.Test;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Repository.Test.MasterRoute
{
    public class MasterRouteJobHeaderRepositoryTest : BaseTest
    {
        private const string USER_CREATED = "Xunit_GetMasterRouteDeliveryLegs";
        private readonly IMasterRouteJobHeaderRepository _repository;

        public MasterRouteJobHeaderRepositoryTest()
        {
            _repository = new MasterRouteJobHeaderRepository(
                    _mockDbFactory.Object
                );
        }

        /// <summary>
        /// => TFS#62737: GET|Copy/Move TV-D List(API) => GetMasterRouteDeliveryLegs
        /// </summary>
        [Fact]
        public void GetMasterRouteDeliveryLegs_ShouldHasCorrectList()
        {
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblSystemJobAction)
            .Returns(MockSystemData<TblSystemJobAction>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblSystemServiceJobType)
            .Returns(MockSystemData<TblSystemServiceJobType>.Data);

            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterRouteJobServiceStopLegs)
            .Returns(Mock62737<TblMasterRouteJobServiceStopLegs>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterRouteJobHeader)
            .Returns(Mock62737<TblMasterRouteJobHeader>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterRoute)
            .Returns(Mock62737<TblMasterRoute>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCustomerLocation)
            .Returns(Mock62737<TblMasterCustomerLocation>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterSite)
            .Returns(Mock62737<TblMasterSite>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterRouteGroup_Detail)
            .Returns(Mock62737<TblMasterRouteGroup_Detail>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterRouteGroup)
            .Returns(Mock62737<TblMasterRouteGroup>.Data);
            var jobs = Mock62737<TblMasterRouteJobHeader>.Data.Select(o => o.Guid);
            var result = _repository.GetMasterRouteDeliveryLegs(jobs);

            Assert.True(result.Count() == jobs.Count());

        }

    }
}
