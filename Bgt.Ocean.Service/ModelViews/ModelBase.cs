using System;

namespace Bgt.Ocean.Service.ModelViews
{
    public abstract class ModelBase
    {
        public Guid Guid { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public bool FlagDisable { get; set; }
        public string UserModify { get; set; }
        public DateTime? DateTimeModify { get; set; }
    }
}
