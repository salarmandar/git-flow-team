using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.ModelViews.ActualJobHeader
{
    public class AdhocJobDetailJobHoldOverView
    {
        public AdhocJobHeaderHoldOverView AdhocJobHeaderView { get; set; }
        public UserModifyDetailView UserModifyDetailView { get; set; }
        public Nullable<Guid> CurrencyGuid { get; set; }

        // ฝั่ง Customer
        [Required]
        public Guid? BrinksSite_Guid { get; set; }
        public string BrinksSiteCode { get; set; }          //[No Need] Get from BrinksSite_Guid
        public Guid? CompanylocationGuidPK { get; set; }     //[No Need] Get from Back-End
        [Required]
        public Guid? CustomerGuid { get; set; }
        [Required]
        public Guid? LocationGuid { get; set; }
        public Nullable<Guid> RouteGuid { get; set; }       //[No Need]
        public string RouteName { get; set; }               //[No Need]
        public Nullable<Guid> RouteDetailGuid { get; set; } 
        public Nullable<Guid> RunResoureGuid { get; set; }
        public string RunResoureName { get; set; }          //[No Need]
        [Required]
        public Nullable<DateTime> WorkDate { get; set; }
        public string strTime { get; set; }                 //[No Need]
        [Required]
        public Nullable<DateTime> Time { get; set; }                             
        public List<Guid?> SpecialCommandGuid { get; set; }
        [Required]
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
        [Required]
        public Guid? BrinksSite_GuidDEL { get; set; }
        public string BrinksSiteCodeDEL { get; set; }   //[No Need] Get from BrinksSite_GuidDEL
        [Required]
        public Guid? CustomerGuidDEL { get; set; }
        [Required]
        public Guid? LocationGuidDEL { get; set; }
        public Nullable<Guid> RouteGuidDEL { get; set; }
        public string RouteNameDEL { get; set; }        //[No Need]
        public Nullable<Guid> RouteDetailGuidDEL { get; set; }  
        public Nullable<Guid> RunResoureGuidDEL { get; set; }
        public string RunResoureNameDEL { get; set; }   //[No Need]
        [Required]
        public Nullable<DateTime> WorkDateDEL { get; set; }
        public string strWorkDateDEL { get; set; }      //[No Need]
        public string strTimeDEL { get; set; }          //[No Need]
        [Required]
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

        //public Guid LanguagueGuid { get; set; }


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

    public class AdhocJobHeaderHoldOverView
    {
        public Guid? JobLegGuid { get; set; }   //#กรณี EditJobDetail From Pupup RunControl
        public Guid? JobGuid { get; set; }      //When click 'save' and want to Edit while didn't close Adhoc
        public int? DayInVaults { get; set; }   //[No Need] TV only (Calculated from back-end)
        public string JobNo { get; set; }       //[No Need] Can get from JobGuid
        public int? StatusJob { get; set; }     //[No Need] Get from back-end
        [Required]
        public Guid? LineOfBusiness_Guid { get; set; }
        [Required]
        public int? ServiceTypeID { get; set; }
        public Guid? ServiceTypeGuid { get; set; }              //[No Need] Get from back-end
        public Guid? ServiceStopTypesGuid { get; set; }         //[No Need] Get from back-end
        public Guid? SubServiceTypeJobTypeGuid { get; set; }
        [Required]
        public Nullable<double> SaidToContain { get; set; }
        [Required]
        public string strInformTime { get; set; }
        public Nullable<DateTime> InformTime { get; set; }      //[No Need] Get from strInformTime
        public string Remarks { get; set; }
        public bool FlagPageFromMapping { get; set; }           //Is this page from Non-Mapping? or Run Control
        public bool FlagJobInterBranch { get; set; }
        public Guid? MasterRouteJobHeader_Guid { get; set; } // FROM MasterRoute

    }
}
