using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Bgt.Ocean.Infrastructure.Util.EnumOTC;

namespace Bgt.Ocean.Service.Test.Machine
{
    public class MachineServiceTest : BaseTest
    {
        private readonly IMachineService _machineService;

        public MachineServiceTest()
        {
            _machineService = Util.CreateInstance<MachineService>();
            _machineService.GetMock<ISFOSystemLockTypeRepository>()
                .Setup(fn => fn.FindOne(It.IsAny<Expression<Func<SFOTblSystemLockType, bool>>>()))
                .Returns(new SFOTblSystemLockType { Guid = LockTypeGuid.Crypto.ToGuid(), LockTypeName = "Crypto" });
        }

        public static IEnumerable<object[]> IsHasCryptoLock_ReturnTrueData
        {
            get
            {
                return new List<object[]>
                {
                    new object[] { new Guid[] { Guid.NewGuid() } },
                    new object[] { new Guid[] { Guid.NewGuid(), Guid.NewGuid() } }
                };
            }
        }

        [Theory]
        [MemberData(nameof(IsHasCryptoLock_ReturnTrueData))]
        public void IsHasCryptoLock_ReturnTrue(Guid[] machineGuids)
        {
            var mockUplist = machineGuids.Select(e => new SFOTblMasterMachine_LockType { SFOMasterMachine_Guid = e, SFOSystemLockType_Guid = LockTypeGuid.Crypto.ToGuid() });

            _machineService.GetMock<ISFOMasterMachineLockTypeRepository>()
                .Setup(fn => fn.FindAllAsQueryable())
                .Returns(mockUplist.AsQueryable());

            var result = _machineService.IsMachineHasCryptoLock(machineGuids);

            Assert.True(result);
        }

        public static IEnumerable<object[]> IsHasCryptoLock_ReturnFalseData
        {
            get
            {
                return new List<object[]>
                {
                    new object[] 
                    {
                        new List<CryptoTest>
                        {
                           new CryptoTest { LockType = LockTypeGuid.Cencon, MachineGuid = Guid.NewGuid() },
                           new CryptoTest { LockType = LockTypeGuid.SpinDial, MachineGuid = Guid.NewGuid() },
                        }
                    },
                    new object[]
                    {
                        new List<CryptoTest>
                        {
                           new CryptoTest { LockType = LockTypeGuid.SG, MachineGuid = Guid.NewGuid() },
                        }
                    },
                    new object[]
                    {
                        new List<CryptoTest>()
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(IsHasCryptoLock_ReturnFalseData))]
        public void IsHasCryptoLock_ReturnFalse(List<CryptoTest> lockList)
        {
            var mockUplist = lockList.Select(e => new SFOTblMasterMachine_LockType { SFOMasterMachine_Guid = e.MachineGuid, SFOSystemLockType_Guid = e.LockType.ToGuid() });

            _machineService.GetMock<ISFOMasterMachineLockTypeRepository>()
                .Setup(fn => fn.FindAllAsQueryable())
                .Returns(mockUplist.AsQueryable());

            var result = _machineService.IsMachineHasCryptoLock(lockList.Select(e => e.MachineGuid).ToArray());

            Assert.False(result);
        }

        public class CryptoTest
        {
            public Guid MachineGuid { get; set; }
            public string LockType { get; set; }
        }
    }
}
