using System;

namespace Bgt.Ocean.Service.ModelViews.ActualJobHeader
{
    public class AdhocJobHeaderView
    {
        public Guid? JobLegGuid { get; set; } // #กรณี EditJobDetail From Pupup RunControl
        public Guid? JobGuid { get; set; }
        public int DayInVaults { get; set; }
        public string JobNo { get; set; }
        public int StatusJob { get; set; }
        public Guid LineOfBusiness_Guid { get; set; }
        public int ServiceTypeID { get; set; }
        public Guid ServiceTypeGuid { get; set; }
        public Guid ServiceStopTypesGuid { get; set; }
        public Guid? SubServiceTypeJobTypeGuid { get; set; }
        public Nullable<double> SaidToContain { get; set; }
        public string strInformTime { get; set; }
        public Nullable<DateTime> InformTime { get; set; }
        public string Remarks { get; set; }
        public bool FlagPageFromMapping { get; set; }
        public bool FlagJobInterBranch { get; set; }
        public Guid? MasterRouteJobHeader_Guid { get; set; } // FROM MasterRoute

    }

    public class AdhocJobHeaderRequest
    {
        #region Prop center
        public Guid? JobGuid { get; set; }
        public string JobNo { get; set; }
        public Guid LineOfBusiness_Guid { get; set; }
        public int ServiceJobTypeID { get; set; }
        public Guid ServiceJobTypeGuid { get; set; }
        public Guid? ServiceStopTypesGuid { get; set; }
        public Guid? SubServiceTypeJobTypeGuid { get; set; }
        public double? SaidToContain { get; set; }
        public Guid? CurrencyGuid { get; set; }
        public DateTime? InformTime { get; set; }
        public string strInformTime { get; set; }
        public string Remarks { get; set; }
        public int DayInVaults { get; set; }
        public Guid BrinkSiteGuid { get; set; }
        #endregion

        public bool FlagPageFromMapping { get; set; }
        public bool FlagJobInterBranch { get; set; }
        public Guid? MasterRouteJobHeader_Guid { get; set; } // FROM MasterRoute

        /*--Use by service--*/
        public int StatusJob { get; set; }

        public Guid SitePathGuid { get; set; }
        public Guid? ContractGuid { get; set; }

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

        //public Guid? JobLegGuid { get; set; } // #กรณี EditJobDetail From Pupup RunControl --> Comment By P'Tum
    }
}
