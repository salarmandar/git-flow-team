using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.ActualJob;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{
    public class CreateJobAdHocResponse : SystemMessageView
    {
        public CreateJobAdHocResponse(TblSystemMessage tblmsg) : base(tblmsg)
        {

        }

        public CreateJobAdHocResponse(TblSystemMessage tblmsg, AdhocTempData tempdata) : base(tblmsg, tempdata)
        {
            JobGuid = tempdata.JobGuid;
            JobNo = tempdata.JobNo;
            ValidateMaxNumberJobs = tempdata.ValidateMaxNumberJobs;
            switch (tempdata.ServiceJobTypeId)
            {
                case IntTypeJob.P:
                case IntTypeJob.BCP:
                case IntTypeJob.FLM:
                case IntTypeJob.ECash: //added: 2018/01/22
                case IntTypeJob.FSLM: //added: 2018/01/22
                case IntTypeJob.TM: //added: 2018/01/22
                case IntTypeJob.T:
                case IntTypeJob.MCS:
                case IntTypeJob.TV:
                case IntTypeJob.TV_MultiBr:

                    this.FlagAllowChangeRoute = tempdata.isDispatchRun;
                    if (this.FlagAllowChangeRoute)
                        this.FlagAllowChangeRoute = tempdata.IsUseDolphin_LegP;

                    break;
                case IntTypeJob.D:
                case IntTypeJob.AC:
                case IntTypeJob.AE:
                case IntTypeJob.BCD:
                case IntTypeJob.BCD_MultiBr:
                    this.FlagAllowChangeRoute = tempdata.isDispatchRun;
                    if (this.FlagAllowChangeRoute)
                        this.FlagAllowChangeRoute = tempdata.IsUseDolphin_LegD;

                    break;
            }
        }

        public Guid? JobGuid { get; set; }
        public string JobNo { get; set; }

        public string JobGuid_list { get; set; }
        public string JobNo_list { get; set; }

        public bool FlagAllowChangeRoute { get; set; }
        public SystemMessageView ValidateMaxNumberJobs { get; set; }    //Let user know that current Run has jobs more than expected, then show warning message
    }

    public class CreateJobAdHocRequest : RequestBase
    {
        public AdhocJobHeaderRequest AdhocJobHeaderView { get; set; }
        public AdhocLegRequest ServiceStopLegPickup { get; set; }
        public AdhocLegRequest ServiceStopLegDelivery { get; set; }
        public Guid LanguagueGuid { get; set; } = ApiSession.UserLanguage_Guid.GetValueOrDefault();
        public string DateTimeFormat { get; set; }

        //For Insert Stop
        public bool flagAddSpot { get; set; } = false;
    }

    public class AdhocLegRequest
    {
        public Guid? LegGuid { get; set; }
        public Guid? BrinkCompanyGuid { get; set; }  /*For source or destination is brink [Job type P or D]*/
        public Guid BrinkSiteGuid { get; set; }
        public string BrinkSiteCode { get; set; }
        public Guid? CustomerGuid { get; set; }
        public Guid? LocationGuid { get; set; }      /* will be obsolete soon */
        public List<Guid?> arr_LocationGuid { get; set; }
        public Guid? RouteGuid { get; set; }
        public Guid? RouteGroupDetailGuid { get; set; }
        public Guid? RunResourceGuid { get; set; }
        public string StrWorkDate_Date { get; set; }
        public string StrWorkDate_Time { get; set; }
        public int? OnwardTypeID { get; set; }
        public Guid? OnwardDestGuid { get; set; }  /*Internal Department*/
        public Guid? TripIndicatorGuid { get; set; }
        public IEnumerable<Guid> SpecialCommandGuid { get; set; }

        public string RouteName { get; set; }
        public string RunResourceName { get; set; }

        public bool? FlagNonBillable { get; set; }
        public bool FlagAddSpot { get; set; } = false;
        public int? jobOrderSpot { get; set; } = 0;

        /// <summary>
        /// Get data either from arr_LocationGuid or LocationGuid property
        /// </summary>
        public List<Guid?> LocationByLegGuid
        {
            get
            {
                return arr_LocationGuid != null && arr_LocationGuid.Count > 0
                    ? arr_LocationGuid
                    : new List<Guid?> { LocationGuid };
            }
        }

    }

    public class MachineCashServiceRequest : RequestBase
    {

        public Guid JobHeaderGuid { get; set; }
        public Guid JobLegGuid { get; set; }
        public IEnumerable<DenominationOnMachineCassetteView> Cassette { get; set; }
        public bool IsAscSequence { get; set; }
        public Guid CurrencyGuid { get; set; }
        public string CurrencyAbb { get; set; }
    }

    public class PrepareBulkDenomination
    {
        public TblMasterActualJobMCSBulkRetract Retract { get; set; }
        public TblMasterActualJobMCSBulkJammed Jammet { get; set; }
        public TblMasterActualJobMCSBulkSuspectFake SuspectFake { get; set; }

    }
}
