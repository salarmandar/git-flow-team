using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.PreVault;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.PreVault
{
    public class VaultBalanceRequest
    {
        public Guid? SiteGuid { get; set; }
        public EnumPageName PageId { get; set; }
        public IEnumerable<Guid?> InternalGuidList { get; set; } = new List<Guid?>();

        
        public IEnumerable<Guid?> SealGuidList { get; set; } = new List<Guid?>();
        public IEnumerable<Guid?> CommodityGuidList { get; set; } = new List<Guid?>();
        public IEnumerable<Guid?> ConsolidateGuidList { get; set; } = new List<Guid?>();
    }

    public class VaultBalanceAutoUpdateRequest
    {
        public IEnumerable<AutoUpdateVaultBalanceModel> ItemsList { get; set; }
    }

    public class VaultBalanceStateResponse : BaseResponse
    {
        public IEnumerable<VaultStateModel> VaultState { get; set; }
    }

    public class VaultBalanceAutoUpdateResponse : BaseResponse
    {
        public VaultBalanceView VaultBalanceDetail { get; set; } = new VaultBalanceView();
    }


    public class VaultBalanceAsyncRequest : VaultBalanceAutoUpdateRequest
    {
        //From Session
        public Guid LanguageGuid { get; set; }
        public String UserName { get; set; }
        public DateTime ClientDateTime { get; set; }
        public DateTimeOffset UniversalDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        public String FormatDateTime { get; set; }
        public String FormatDate { get; set; }
    }
}
