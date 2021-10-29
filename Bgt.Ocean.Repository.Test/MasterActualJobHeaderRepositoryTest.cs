using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using DataSoruce.Test;
using System;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Repository.Test
{
    public class MasterActualJobHeaderRepositoryTest : BaseTest
    {
        private readonly IMasterActualJobHeaderRepository _repository;
        public MasterActualJobHeaderRepositoryTest()
        {
            _repository = new MasterActualJobHeaderRepository(
                    _mockDbFactory.Object
                );
        }

        /// <summary>
        /// => TFS#64185 -> MCS - CIT Delivery - Customer Order Number should be alphanumeric
        /// </summary>
        [Fact]
        public void Get_ShouldWorkingNormally()
        {

            var jobGuid = Guid.Parse("D9D82D15-D00D-4558-8436-FB9F3D5D7E6A");
            var B = MockSystemData<TblSystemMCSCITDeliveryScanStatus>.Data;
            var C = MockSystemData<TblSystemMCSCITDeliveryCommodityType>.Data;
            var queryableList = Mock64185<TblMasterActualJobMCSCITDelivery>.Data.AsEnumerable().Select(o =>
            {
                o.TblSystemMCSCITDeliveryCommodityType = C.FirstOrDefault(c => c.Guid == o.SystemMCSCITDeliveryCommodityType_Guid);
                o.TblSystemMCSCITDeliveryScanStatus = B.FirstOrDefault(b => b.Guid == o.SystemMCSCITDeliveryScanStatus_Guid);
                return o;
            }).AsQueryable();

            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobMCSCITDelivery)
            .Returns(DataSoruce.Test.Util.ReBuildDbSet(queryableList));

            _mockDbFactory
.Setup(fn => fn.GetCurrentDbContext.TblMasterCurrency)
.Returns(Mock64185<TblMasterCurrency>.Data);


            var result = _repository.GetCitDeliveryView(jobGuid).ToList();

            Assert.NotNull(result);
            Assert.True(result.Count == 3);
        }
    }
}
