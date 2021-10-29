using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Xunit;

namespace Bgt.Ocean.Repository.Test.DailyRunAlarm
{
    public class MasterDailyRunAlarmRepositoryTest : BaseTest
    {
        private readonly IMasterDailyRunResourceAlarmRepository _repo;

        public MasterDailyRunAlarmRepositoryTest()
        {
            _repo = new MasterDailyRunResourceAlarmRepository(
                    _mockDbFactory.Object
                );
        }

        [Fact]
        public void Create_ShouldCleanupDirtyCorrectly()
        {
            var model = new TblMasterDailyRunResource_Alarm();

            model.EmployeeName = RandomString(510);
            model.Phone = RandomString(510);
            model.SiteName = RandomString(510);
            model.RouteName = RandomString(510);
            model.RunNo = RandomString(510);
            model.Latitude = RandomString(510);
            model.Longitude = RandomString(510);
            model.UserCreated = RandomString(510);
            model.UserAcknowledged = RandomString(510);
            model.UserDeactivated = RandomString(510);

            _repo.Create(model);

            Assert.Equal(500, model.EmployeeName.Length);
            Assert.Equal(100, model.Phone.Length);
            Assert.Equal(150, model.SiteName.Length);
            Assert.Equal(300, model.RouteName.Length);
            Assert.Equal(200, model.RunNo.Length);
            Assert.Equal(50, model.Latitude.Length);
            Assert.Equal(50, model.Longitude.Length);
            Assert.Equal(200, model.UserCreated.Length);
            Assert.Equal(200, model.UserAcknowledged.Length);
            Assert.Equal(200, model.UserDeactivated.Length);
        }

        [Fact]
        public void Create_ShouldNotCleanupDirtyWhenDataOk()
        {
            var model = new TblMasterDailyRunResource_Alarm();

            model.EmployeeName = RandomString(20);
            model.Phone = RandomString(80);
            model.SiteName = RandomString(70);
            model.RouteName = RandomString(100);
            model.RunNo = RandomString(96);
            model.Latitude = RandomString(31);
            model.Longitude = RandomString(24);
            model.UserCreated = RandomString(174);
            model.UserAcknowledged = RandomString(130);
            model.UserDeactivated = RandomString(34);

            _repo.Create(model);

            Assert.Equal(20, model.EmployeeName.Length);
            Assert.Equal(80, model.Phone.Length);
            Assert.Equal(70, model.SiteName.Length);
            Assert.Equal(100, model.RouteName.Length);
            Assert.Equal(96, model.RunNo.Length);
            Assert.Equal(31, model.Latitude.Length);
            Assert.Equal(24, model.Longitude.Length);
            Assert.Equal(174, model.UserCreated.Length);
            Assert.Equal(130, model.UserAcknowledged.Length);
            Assert.Equal(34, model.UserDeactivated.Length);
        }

        [Fact]
        public void Create_ShouldNotCleanupDirtyWhenDataNull()
        {
            var model = new TblMasterDailyRunResource_Alarm();

            model.EmployeeName = RandomString(20);
            model.Phone = RandomString(80);
            model.SiteName = RandomString(70);
            model.RouteName = RandomString(100);
            model.RunNo = RandomString(96);
            model.Latitude = RandomString(31);
            model.Longitude = RandomString(24);
            model.UserCreated = RandomString(174);

            _repo.Create(model);

            Assert.Equal(20, model.EmployeeName.Length);
            Assert.Equal(80, model.Phone.Length);
            Assert.Equal(70, model.SiteName.Length);
            Assert.Equal(100, model.RouteName.Length);
            Assert.Equal(96, model.RunNo.Length);
            Assert.Equal(31, model.Latitude.Length);
            Assert.Equal(24, model.Longitude.Length);
            Assert.Equal(174, model.UserCreated.Length);
            Assert.Null(model.UserAcknowledged);
            Assert.Null(model.UserDeactivated);
        }

        [Fact]
        public void Modify_ShouldCleanupDirtyCorrectly()
        {
            var model = new TblMasterDailyRunResource_Alarm();

            model.EmployeeName = RandomString(510);
            model.Phone = RandomString(510);
            model.SiteName = RandomString(510);
            model.RouteName = RandomString(510);
            model.RunNo = RandomString(510);
            model.Latitude = RandomString(510);
            model.Longitude = RandomString(510);
            model.UserCreated = RandomString(510);
            model.UserAcknowledged = RandomString(510);
            model.UserDeactivated = RandomString(510);

            _repo.Modify(model);

            Assert.Equal(500, model.EmployeeName.Length);
            Assert.Equal(100, model.Phone.Length);
            Assert.Equal(150, model.SiteName.Length);
            Assert.Equal(300, model.RouteName.Length);
            Assert.Equal(200, model.RunNo.Length);
            Assert.Equal(50, model.Latitude.Length);
            Assert.Equal(50, model.Longitude.Length);
            Assert.Equal(200, model.UserCreated.Length);
            Assert.Equal(200, model.UserAcknowledged.Length);
            Assert.Equal(200, model.UserDeactivated.Length);
        }

        [Fact]
        public void Modfy_ShouldNotCleanupDirtyWhenDataOk()
        {
            var model = new TblMasterDailyRunResource_Alarm();

            model.EmployeeName = RandomString(30);
            model.Phone = RandomString(80);
            model.SiteName = RandomString(70);
            model.RouteName = RandomString(100);
            model.RunNo = RandomString(96);
            model.Latitude = RandomString(31);
            model.Longitude = RandomString(24);
            model.UserCreated = RandomString(174);
            model.UserAcknowledged = RandomString(130);
            model.UserDeactivated = RandomString(34);

            _repo.Modify(model);

            Assert.Equal(30, model.EmployeeName.Length);
            Assert.Equal(80, model.Phone.Length);
            Assert.Equal(70, model.SiteName.Length);
            Assert.Equal(100, model.RouteName.Length);
            Assert.Equal(96, model.RunNo.Length);
            Assert.Equal(31, model.Latitude.Length);
            Assert.Equal(24, model.Longitude.Length);
            Assert.Equal(174, model.UserCreated.Length);
            Assert.Equal(130, model.UserAcknowledged.Length);
            Assert.Equal(34, model.UserDeactivated.Length);
        }

        [Fact]
        public void Modfy_ShouldNotCleanupDirtyWhenDataNull()
        {
            var model = new TblMasterDailyRunResource_Alarm();

            model.EmployeeName = RandomString(30);
            model.Phone = RandomString(80);
            model.SiteName = RandomString(70);
            model.RouteName = RandomString(100);
            model.RunNo = RandomString(96);
            model.Latitude = RandomString(31);
            model.Longitude = RandomString(24);

            _repo.Modify(model);

            Assert.Equal(30, model.EmployeeName.Length);
            Assert.Equal(80, model.Phone.Length);
            Assert.Equal(70, model.SiteName.Length);
            Assert.Equal(100, model.RouteName.Length);
            Assert.Equal(96, model.RunNo.Length);
            Assert.Equal(31, model.Latitude.Length);
            Assert.Equal(24, model.Longitude.Length);
            Assert.Null(model.UserCreated);
            Assert.Null(model.UserAcknowledged);
            Assert.Null(model.UserDeactivated);
        }
    }
}
