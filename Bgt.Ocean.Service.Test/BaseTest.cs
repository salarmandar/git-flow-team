using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Moq;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Test
{
    public abstract class BaseTest : IDisposable
    {

        protected readonly Mock<IUnitOfWork<OceanDbEntities>> _mockUow;
        protected readonly Mock<ISystemLog_HistoryErrorRepository> _mockLogErrorRepo;

        protected BaseTest()
        {
            _mockUow = Util.CreateMock<IUnitOfWork<OceanDbEntities>>();
            _mockLogErrorRepo = Util.CreateMock<ISystemLog_HistoryErrorRepository>();
            Util.CreateFakeContext();
        }

     

        protected Mock<TMock> CreateMock<TMock>() where TMock : class
            => Util.CreateMock<TMock>();

        protected TDummy CreateDummy<TDummy>() where TDummy : class
            => Util.CreateDummy<TDummy>();

        protected TDummy CreateDummy<TDummy>(Action<AutoFixture.Fixture> setup) where TDummy : class
            => Util.CreateDummy<TDummy>(setup);

        protected List<TDummy> CreateDummy<TDummy>(int count) where TDummy : class
            => Util.CreateDummy<TDummy>(count);

        protected List<TDummy> CreateDummy<TDummy>(int count, Action<AutoFixture.Fixture> setup) where TDummy : class
            => Util.CreateDummy<TDummy>(count, setup);

        protected Mock<IUnitOfWork<OceanDbEntities>> GetNewMockDbEntities()
            => Util.CreateMock<IUnitOfWork<OceanDbEntities>>();


        protected void Reset()
        {
            _mockUow.Reset();
            _mockLogErrorRepo.Reset();
        }

        public void Dispose()
        {
            Reset();
        }
    }
}
