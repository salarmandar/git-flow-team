using System;

namespace Bgt.Ocean.Service.ModelViews.MasterRoute
{
 public   class MasterRouteServiceStopLegView
    {
        public Guid Guid { get; set; }
        public Guid MasterRouteJobHeaderGuid { get; set; }
        public Guid? MasterRouteDelivery { get; set; }
        public int JobOrder { get; set; }
        public int SeqIndex { get; set; }
        public int SequenceStop { get; set; }
    }
}
