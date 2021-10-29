using System;

namespace Bgt.Ocean.Service.ModelViews.PreVault
{
    public class CheckOutDepartmentViewModel
    {
        public Guid PrevaultGuid { get; set; }
        public Guid InternalDepartmentGuid { get; set; }
        public bool FlagIncludeD { get; set; }
        public bool FlagGroupNonbarcode { get; set; }
        public bool FlagOWDOnly { get; set; }
        /// <summary>
        /// TFS#35898 : Add capability to filter the packages by route name and route date
        /// </summary>
        public string WorkDate { get; set; }
        public Guid? DailyRunGuid { get; set; }
        public bool FlagShowAll { get; set; }
    }
}