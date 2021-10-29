using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.ActualJobHeader
{
    public class AdhocJobDetailJobView
    {
        public AdhocJobHeaderView AdhocJobHeaderView { get; set; }
        public UserModifyDetailView UserModifyDetailView { get; set; }
        public Nullable<Guid> CurrencyGuid { get; set; }

        // ฝั่ง Customer
        public Guid BrinksSite_Guid { get; set; }
        public string BrinksSiteCode { get; set; }
        public Guid CompanylocationGuidPK { get; set; }
        public Guid CustomerGuid { get; set; }
        public Guid LocationGuid { get; set; }
        public Nullable<Guid> RouteGuid { get; set; }
        public string RouteName { get; set; }
        public Nullable<Guid> RouteDetailGuid { get; set; }
        public Nullable<Guid> RunResoureGuid { get; set; }
        public string RunResoureName { get; set; }
        public Nullable<DateTime> WorkDate { get; set; }
        public string strTime { get; set; }
        public DateTime Time { get; set; }
        public Guid? ContractGuid { get; set; }

        //ฝั่ง Brinksite
        public Guid BrinksCompanyGuid { get; set; }
        public Guid CompanylocationGuidDEL { get; set; }
        public Nullable<int> OnwaryTypeID { get; set; }
        public Nullable<Guid> OnwaryDestinationGuid { get; set; }
        public Nullable<Guid> TripIndicator_Guid { get; set; }
        public Guid? InterbranchGuid { get; set; }
        public Guid? InterbranchCompanyLocationGuid { get; set; }

        // TV && T leg D
        public Guid BrinksSite_GuidDEL { get; set; }
        public string BrinksSiteCodeDEL { get; set; }
        public Guid CustomerGuidDEL { get; set; }
        public Guid LocationGuidDEL { get; set; }
        public Nullable<Guid> RouteGuidDEL { get; set; }
        public string RouteNameDEL { get; set; }
        public Nullable<Guid> RouteDetailGuidDEL { get; set; }
        public Nullable<Guid> RunResoureGuidDEL { get; set; }
        public string RunResoureNameDEL { get; set; }
        public Nullable<DateTime> WorkDateDEL { get; set; }
        public string strWorkDateDEL { get; set; }
        public string strTimeDEL { get; set; }
        public DateTime TimeDEL { get; set; }
        public List<Guid?> SpecialCommandGuidDEL { get; set; }

        // Airport
        public bool FlagBgsAirport { get; set; }
        public bool FlagEarlyFlight { get; set; }
        public string FlightNo { get; set; }
        public Nullable<DateTime> EstimateDepartureTime { get; set; }
        public Nullable<DateTime> EstimateArrivalTime { get; set; }
        public Nullable<double> WeightShip { get; set; }
        public Nullable<int> Pieces { get; set; }
        public string MawbNumber { get; set; }
        public Nullable<int> NoOfHAWB { get; set; }
        public string AirportRemarks { get; set; }
        public Nullable<bool> FlagAirportDocCheck { get; set; }
        public Nullable<int> SystemTransportWay_ID { get; set; }
        public Nullable<Guid> MasterTrasportWay_Guid { get; set; }

        public Guid LanguagueGuid { get; set; }

        #region SFO Information

        public bool FlagJobSFO { get; set; }
        public string TicketNumber { get; set; }
        public int SFOMaxWorkingTime { get; set; }
        public bool SFOFlagRequiredTechnician { get; set; }
        /// <summary>
        /// Techmeet Name
        /// </summary>
        public string SFOTechnicianName { get; set; }
        public string SFOTechnicianID { get; set; }
        public int? SFOMaxTechnicianWaitingTime { get; set; }
        public bool FlagRequireOpenLock { get; set; }

        #endregion
    }
}
