using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Models.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.FleetMaintenance
{
    #region request get gasoline info list
    public class FleetGasolineRequest : PagingBase
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Guid MasterRunResource_Guid { get; set; }
        public Guid MasterSite_Guid { get; set; }
        public Guid? CurrencyUnit_Guid { get; set; }
        public Guid? GasolineVendorGuid { get; set; }
        public decimal? TopUpAmount { get; set; }
        public bool flagShowAll { get; set; }
        public EnumFleetOperator Operator { get; set; }
        public string DocumentRef { get; set; }
    }
    #endregion

    #region response data gasoline info list
    public class FleetGasolineResponse : BaseResponse
    {
        public IEnumerable<FleetGasolineView> FleetGasolineList { get; set; }
    }
    #endregion
    
    #region request input gasoline data
    public class FleetGasolineDataRequest : RequestBase
    {
        public Guid? RunResourceGasolineGuid { get; set; }
        [Required]
        public Guid MasterSite_Guid { get; set; }
        public Guid MasterRunResource_Guid { get; set; }
        [Description("Top up Date + Time of Gasoline")]
        [Required]
        public DateTime TopUpDate { get; set; }
        [Description("Top up Amount")]
        public decimal TopUpAmount { get; set; }
        [Description("Of Top up Amount")]
        public Guid CurrencyAmount_Guid { get; set; }
        [Description("Quantity")]
        [Required]
        public double TopUpQty { get; set; }
        [Description("Price per Unit")]
        [Required]
        public decimal Unit_Price { get; set; }
        [Description("Of Price per Unit")]
        public Guid CurrencyUnit_Guid { get; set; }
        [Description("Fuel Type")]
        [Required]
        public Guid MasterGasloine_Guid { get; set; }
        [Description("Reference Doc No.")]
        public string DocumentRef { get; set; }
        [Required]
        public string OdoMeter { get; set; }
        [Description("Of Quantity")]
        public Guid? TopupQtyUnit_Guid { get; set; }
        public bool FlagDisable { get; set; }
    }
    public class FleetGasolineDisableRequest : RequestBase
    {
        public Guid? RunResourceGasolineGuid { get; set; }
        public bool IsDisable { get; set; }
    }
    #endregion

    #region response data gasoline
    public class FleetGasolineDataResponse
    {
        public Guid? RunResourceGasolineGuid { get; set; }
        public string StrTopUpDate { get; set; }
        public string StrTopUpTime { get; set; }
        public string OdoMeter { get; set; }
        [Description("Fuel Type")]
        public Guid MasterGasloine_Guid { get; set; }
        [Description("Fuel Station (Local)")]
        public Guid FuelStationGuid { get; set; }
        [Description("Reference Doc No.")]
        public string DocumentRef { get; set; }
        [Description("Price per Unit")]
        public decimal Unit_Price { get; set; }
        [Description("Of Price per Unit")]
        public Guid CurrencyUnit_Guid { get; set; }
        [Description("Quantity")]
        public double TopUpQty { get; set; }
        [Description("Of Quantity")]
        public Guid? TopupQtyUnit_Guid { get; set; }
        [Description("Top up Amount")]
        public decimal TopUpAmount { get; set; }
        [Description("Of Top up Amount")]
        public Guid CurrencyAmount_Guid { get; set; }
    }
    #endregion
}
