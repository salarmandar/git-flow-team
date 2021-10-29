using System;

namespace Bgt.Ocean.Models.MasterRoute
{
    public  class MasterRouteUpdateSeqIndexResult
    {
        public int MsgId { get; set; }
        public string Msg { get; set; }
    }



    class requstModel
    {
        public int ServiceJobTypeID { get; set; }
        public string Name { get; set; }
    }
    public class MasterRouteJobDetailForManualChangeSeqIndexResult
    {
        public Guid LegGuid { get; set; }
        public string LocationName { get; set; }

        public int JobOrder { get; set; }
        public int SeqIndex { get; set; }
        public Guid MasterRouteGroupDetail_Guid { get; set; }
        public int SequenceStop { get; set; }
        public string JobTypeNameAbb { get; set; }
        public string JobAction { get; set; }
        public int OnwardType { get; set; }
        public string OnwardName { get; set; }
    }

  
}
