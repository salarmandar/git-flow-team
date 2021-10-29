using System;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class CurrencyView : ModelBase
    {
        public Guid MasterCurrency_Guid
        {
            get
            {
                return Guid;
            }
            set { Guid = value; }
        }
        public string MasterCurrencyAbbreviation { get; set; }
        public decimal MasterCurrencyDiscrepancyAcceptable { get; set; }
    }
    public class Nautilus_CurrencyView : ModelBase
    {
        public Guid MasterCurrency_Guid
        {
            get
            {
                return Guid;
            }
            set { Guid = value; }
        }
        public string MasterCurrencyAbbreviation { get; set; }
        public string MasterCurrencyReportDisplay { get; set; }
        public string MasterCurrencyDescription { get; set; }
        public decimal MasterCurrencyDiscrepancyAcceptable { get; set; }

    }
}
