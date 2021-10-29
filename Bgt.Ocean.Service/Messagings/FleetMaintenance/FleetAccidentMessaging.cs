using Bgt.Ocean.Models.FleetMaintenance;
using Bgt.Ocean.Service.ModelViews.FleetMaintenanceViewModel.FleetAccidentViewModel;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.FleetMaintenance
{
    public class FleetAccidentRequest : RequestBase
    {
        public AccidentInfoViewRequest Request { get; set; }
    }

    public class FleetAccidentResponse : BaseResponse
    {
        public IEnumerable<AccidentInfoView> AccidentViewInfo { get; set; } = new List<AccidentInfoView>();

    }

    public class AccidentRequest
    {
        public Guid? AccidentGuid { get; set; }
        public Guid RunresourceGuid { get; set; }
        public Guid SiteGuid { get; set; }
        public AccidentDetailModelView AccidentDetail { get; set; }
        public CounterpartyDetailModelView CounterpartyDetail { get; set; }
        public List<AccidentListDetailDamagedModelView> BrinksDetailList { get; set; } = new List<AccidentListDetailDamagedModelView>();
        public List<AccidentListDetailDamagedModelView> CounterDetailList { get; set; } = new List<AccidentListDetailDamagedModelView>();
        public List<AccidentImageModelView> ImageList { get; set; }
    }

    public class DisableAccidentRequest
    {
        public Guid AccidentGuid { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class AccidentResponse : RequestBase
    {
        public Guid Guid { get; set; }
        public SystemMessageView Msg { get; set; }

    }
    public class AccidentDetailResponse
    {
        public AccidentDetailModelView AccidentDetailTab { get; set; } = new AccidentDetailModelView();
        public CounterpartyDetailModelView CounterPartyDetailTab { get; set; } = new CounterpartyDetailModelView();
    }

    public class AccidentImageRespone
    {
        public Guid ImageGuid { get; set; }
        public string ImageContent { get; set; }
        public bool IsActualImage { get; set; }
    }
 
}
