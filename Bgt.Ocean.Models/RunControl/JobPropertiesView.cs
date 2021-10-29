using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.Masters;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{

    public class JobPropertiesView
    {
        //head
        public Guid JobGuid { get; set; }
        public string JobNo { get; set; }
        public string JobStatusName { get; set; }
        //tab
        public TabDetailView tabDetail { get; set; }
        public TabLegView tabLeg { get; set; }
        public TabServiceDetailView tabServiceDetail { get; set; }
        public TabHistoryView tabHistory { get; set; }

    }

    #region *** Sharing Class
    public class DenoBase
    {
        public decimal? DenominationValue { get; set; }
        public string CurrencyAbbr { get; set; }
        public int DenoSequence { get; set; }
    }

    public class CassetteBase
    {
        public string CassetteName { get; set; }
        public CassetteType CassetteTypeID { get; set; }
        public Guid? CassetteGuid { get; set; }
        public int CassetteSequence { get; set; }

        //Some Cassette has deno
        public decimal? DenominationValue { get; set; }
        public string CurrencyAbbr { get; set; }
    }

    public class MasterIDCollection
    {
        public string MasterID { get; set; }
        public string MasterID_Route { get; set; }
        public double Liability { get; set; }
        public string CurrencyAbbr { get; set; }
        public List<SmallBagAndBulkCashSealView> SealList { get; set; } = new List<SmallBagAndBulkCashSealView>();
    }
    public class SuspectFakeTransectionView : DenoBase
    {
        public int Report { get; set; }
        public int Counted { get; set; }
        public int Diff { get; set; }
    }
    public class SuspectFakeDetailView : DenoBase
    {
        public DateTime? DepositDate { get; set; }
        public string DepositDateStr { get; set; }
        public string SerialNo { get; set; }
    }

    public class SealBagView
    {
        public string SealNo { get; set; }
    }

    public class ReturnBagView : SealBagView
    {
        public string Commodity { get; set; }
    }

    #endregion
}
