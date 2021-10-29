using System;

namespace Bgt.Ocean.Service.ModelViews.ActualJobHeader
{
    public class UserModifyDetailView
    {
        public Guid UserModifedGuid { get; set; }
        public Guid? LanguageGuid { get; set; }
        public string UserModifed { get; set; }
        public DateTime DatetimeModified { get; set; }
        public DateTime UniversalDatetimeModified { get; set; }
    }
}
