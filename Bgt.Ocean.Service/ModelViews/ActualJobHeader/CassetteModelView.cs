using Bgt.Ocean.Models;
using System;

namespace Bgt.Ocean.Service.ModelViews.ActualJobHeader
{
    public class CashAddTransactionHeader
    {
  
        public TblMasterActualJobSumActualCount TbSumAc { get; set; }
        public TblMasterActualJobSumCashAdd TbSumCa { get; set; }

        public TblMasterActualJobSumMachineReport TbSumMr { get; set; }
        public TblMasterActualJobSumCashReturn TbSumCr { get; set; }



    }
 
    public class CassetteModelView
    {
        public Guid Guid { get; set; }
        public Guid SumHeadGuid { get; set; }
        public Guid MasterDenomination_Guid { get; set; }
        public Guid MasterCurrency_Guid { get; set; }
        public decimal DenominationValue { get; set; }
        public string CurrencyAbbr { get; set; }
        public int CassetteSequence { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public DateTimeOffset? UniversalDatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public DateTime? DatetimeModified { get; set; }
        public DateTimeOffset? UniversalDatetimeModified { get; set; }


    }
}
