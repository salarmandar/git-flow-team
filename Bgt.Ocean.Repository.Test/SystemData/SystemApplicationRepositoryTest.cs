using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Xunit;

namespace Bgt.Ocean.Repository.Test.SystemData
{
    public class SystemApplicationRepositoryTest : BaseTest
    {
        private readonly ISystemApplicationRepository _systemApplicationRepository;

        public SystemApplicationRepositoryTest()
        {
            _systemApplicationRepository = new SystemApplicationRepository(_mockDbFactory.Object);
        }

        //[Fact]
        public void CheckIfGetAny()
        {
            Assert.True(_systemApplicationRepository.Any());
        }

        //[Fact]
        public void CheckIfGetAnyWithCondition()
        {
            Assert.True(_systemApplicationRepository.Any(o => o.ApplicationID == 1));
        }
    }
}
