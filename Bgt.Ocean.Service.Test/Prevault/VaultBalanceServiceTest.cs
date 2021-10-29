using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Service.Implementations.PreVault;
using Bgt.Ocean.Service.Messagings.PreVault.VaultBalance;
using Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Service.Test.Prevault
{
    public class VaultBalanceServiceTest
    {
        private readonly IVaultBalanceService _vaultBalanceService;
        public VaultBalanceServiceTest()
        {
            _vaultBalanceService = Util.CreateInstance<VaultBalanceService>();
        }

        [Fact]
        public void BlindScanNotBalance_ShouldReturnDiscrepancyList()
        {
            var itemScan = new List<VaultBalanceSealModelView>
            {
                new VaultBalanceSealModelView { SealNo = "SealS00", ScanItemState = EnumItemState.Scanned },
                new VaultBalanceSealModelView { SealNo = "seals01", ScanItemState = EnumItemState.Scanned },
                new VaultBalanceSealModelView { SealNo = "seals07", ScanItemState = EnumItemState.Scanned },
                new VaultBalanceSealModelView { SealNo = "seals08", ScanItemState = EnumItemState.Scanned },
                new VaultBalanceSealModelView { SealNo = "MLOC001", ScanItemState = EnumItemState.Scanned },
                new VaultBalanceSealModelView { SealNo = "mloc001", ScanItemState = EnumItemState.Scanned },
            };

            var request = Util.CreateDummyAndModify<VaultBalanceItemsRequest>(data =>
            {
                data.SealList = itemScan;
                data.SealItemsMapping = Enumerable.Empty<VaultBalanceMappingModelView>();
                data.SummaryList = Enumerable.Empty<VaultBalanceSummaryModelView>();
            });

            #region Item in vault
            List<VaultBalanceSealModelView> mockItemsInVault = new List<VaultBalanceSealModelView>();
            //Seals
            mockItemsInVault.AddRange(Util.CreateDummy<VaultBalanceSealModelView>(5)
                .Select((j, i) =>
                {
                    j.SealNo = "SealS0" + i.ToString();
                    j.IsMasterLoc = false;
                    j.IsMasterRoute = false;
                    j.ScanItemState = EnumItemState.NotScan;
                    return j;
                }).ToList());

            //Master
            mockItemsInVault.AddRange(Util.CreateDummy<VaultBalanceSealModelView>(2)
               .Select((j, i) =>
               {
                   j.SealNo = "MLOC00" + i.ToString();
                   j.JobGuid = null;
                   j.IsMasterLoc = i == 0;
                   j.IsMasterRoute = i == 1;
                   j.ScanItemState = EnumItemState.NotScan;
                   return j;
               }).ToList());
            #endregion

            var result = (List<VaultBalanceSummaryModelView>)_vaultBalanceService.InvokeMethod("CalSealSummary_Blind", request, mockItemsInVault);

            var resultOver = result.Count(o => o.Overage > 0);
            var resultShort = result.Count(o => o.Shortage > 0);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(3, resultOver);
            Assert.Equal(4, resultShort);
            Assert.Contains("SealS02".ToLower(), result.Where(o => o.Shortage > 0).Select(o => o.ItemName.ToLower()));
            Assert.Contains("seals07".ToLower(), result.Where(o => o.Overage > 0).Select(o => o.ItemName.ToLower()));
        }
    }
}
