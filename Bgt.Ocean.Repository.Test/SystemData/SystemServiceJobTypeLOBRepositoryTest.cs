using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using System;
using System.Collections.Generic;
using Xunit;

namespace Bgt.Ocean.Repository.Test.SystemData
{
    public class SystemServiceJobTypeLOBRepositoryTest
    {
        public class GetServiceTypeByLOBs_MassUpdate : BaseTest
        {
            private readonly ISystemServiceJobTypeLOBRepository _repo;

            public GetServiceTypeByLOBs_MassUpdate()
            {
                _repo = new SystemServiceJobTypeLOBRepository(_mockDbFactory.Object);
            }

            //[Theory]
            //[InlineData(true, true)]
            //[InlineData(true, false)]
            //[InlineData(false, true)]
            //[InlineData(false, false)]
            public void FlagPAndFlagD_ShouldWorkinFine(bool flagPCustomer, bool flagDCustomer)
            {
                var result = _repo.GetServiceTypeByLOBs_MassUpdate(new List<Guid>(0), flagPCustomer, flagDCustomer, true);
                Assert.Empty(result);                
            }
        }

        
    }
}
