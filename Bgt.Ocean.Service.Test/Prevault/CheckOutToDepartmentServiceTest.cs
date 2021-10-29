using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Implementations.Prevault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bgt.Ocean.Service.Test.Prevault
{
    public class CheckOutToDepartmentServiceTest
    {
        private readonly ICheckOutDepartmentService _CheckOutDepartmentService;
        public CheckOutToDepartmentServiceTest()
        {
            _CheckOutDepartmentService = Util.CreateInstance<CheckOutDepartmentService>();
        }

        [Theory]
        [MemberData(nameof(GetItemScan))]
        public void ScanItemCompleteHasDailyRun_ReturnTrue(
            Guid DailyRunGuid,
            IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> inputSeal, 
            IEnumerable<PrevaultDepartmentBarcodeScanOutResult> inputNon)
        {
            bool result = (bool)_CheckOutDepartmentService.InvokeMethod("HasNotCompleteScanInDailyRun", DailyRunGuid, inputNon,inputSeal);
            Assert.True(result);
        }

        public static IEnumerable<object[]> GetItemScan()
        {
            Guid DailyRunGuid = new Guid("8B5B8713-B1A2-4D07-838E-E5EA56AFAB0A");
            var inputSeal = Util.CreateDummy<PrevaultDepartmentSealConsolidateScanOutResult>(3);
            foreach (var item in inputSeal)
            {
                item.MasterRunResourceDaily_Guid = DailyRunGuid;
                item.FlagScan = true;
            }

            var inputNon = Util.CreateDummy<PrevaultDepartmentBarcodeScanOutResult>(3);
            foreach (var item in inputNon)
            {
                item.MasterRunResourceDaily_Guid = DailyRunGuid;
                item.FlagScan = true;
            }

            yield return new object[] { DailyRunGuid, inputSeal, inputNon };
            yield return new object[] { DailyRunGuid, inputSeal, null };
            yield return new object[] { DailyRunGuid, null, inputNon };
        }
    }
}
