using System;

namespace Bgt.Ocean.Service.ModelViews.RunControls
{
    public class GetMasterSpecialCommandsView
    {
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterActualJobServiceStop_Guid { get; set; }
        public Nullable<System.Guid> MasterSpecialCommand_Guid { get; set; }
        public string SpecialCommand_Guid { get; set; }
        public string SpecialCommandName { get; set; }
        public string SpecialCommandNames { get { return this.ActionNameAbbrevaition + " : " + this.SpecialCommandName; } }
        public string ActionNameAbbrevaition { get; set; }
    }
}
