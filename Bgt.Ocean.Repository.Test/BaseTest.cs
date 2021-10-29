using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bgt.Ocean.Repository.Test
{
    public abstract class BaseTest : IDisposable
    {
        protected readonly Mock<IDbFactory<OceanDbEntities>> _mockDbFactory;
        protected readonly Mock<ISystemLog_HistoryErrorRepository> _mockLogErrorRepo;
        private readonly OceanDbEntities _db;

        public static object _lockDbEntities = new object();

        protected BaseTest()
        {
            _db = new OceanDbEntities();

            _db.Database.Log = log => Debug.WriteLine(log);

            _mockDbFactory = CreateMock<IDbFactory<OceanDbEntities>>();
            _mockLogErrorRepo = Util.CreateMock<ISystemLog_HistoryErrorRepository>();

            _mockDbFactory
                .Setup(fn => fn.GetCurrentDbContext)
                .Returns(_db);

            _mockDbFactory
                .Setup(fn => fn.GetNewDataContext)
                .Returns(_db);
        }

        protected OceanDbEntities GetOODbContext()
            => Util.GetDbContextWithLog();

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

        protected string RandomString(int length) => Util.RandomString(length);


        protected void Reset()
        {
            _mockLogErrorRepo.Reset();

            lock (_lockDbEntities)
            {
                _db.Dispose();
            }
        }

        public virtual void Dispose()
        {
            Reset();
        }


    }
}
