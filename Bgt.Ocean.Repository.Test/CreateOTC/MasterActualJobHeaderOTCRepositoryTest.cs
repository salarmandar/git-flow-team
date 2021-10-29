
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using DataSoruce.Test;
using System;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Repository.Test.CreateOTC
{
    public class MasterActualJobHeaderOTCRepositoryTest : BaseTest
    {
        private readonly IMasterActualJobHeaderOTCRepository _repository;
        public MasterActualJobHeaderOTCRepositoryTest()
        {
            _repository = new MasterActualJobHeaderOTCRepository(
                    _mockDbFactory.Object
                );
        }

        /// <summary>
        /// => TFS#53385:Ability to generate the OTC codes for both legs of Transfer job  -> GetOTCLegsByJobGuids_ShouldBeWorkingNormally
        /// </summary>
        [Fact]
        public void GetOTCLegsByJobGuids_ShouldBeWorkingNormally()
        {

            var jobGuids = new Guid[] { Guid.Parse("46a570fc-b12b-426a-b4fd-c8b57bc99769") };
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.SFOTblSystemLockType)
            .Returns(MockSystemData<SFOTblSystemLockType>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterActualJobServiceStopLegs)
            .Returns(Mock53385<TblMasterActualJobServiceStopLegs>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.SFOTblMasterMachine)
            .Returns(Mock53385<SFOTblMasterMachine>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.SFOTblMasterMachine_LockType)
            .Returns(Mock53385<SFOTblMasterMachine_LockType>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCustomerLocation)
            .Returns(Mock53385<TblMasterCustomerLocation>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCustomer)
            .Returns(Mock53385<TblMasterCustomer>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterSite)
            .Returns(Mock53385<TblMasterSite>.Data);
            _mockDbFactory
            .Setup(fn => fn.GetCurrentDbContext.TblMasterCountry)
            .Returns(Mock53385<TblMasterCountry>.Data);

            var result = _repository.GetOTCLegsByJobGuids(jobGuids);
            Assert.True(result.Count() == 2);
        }
    }
}
