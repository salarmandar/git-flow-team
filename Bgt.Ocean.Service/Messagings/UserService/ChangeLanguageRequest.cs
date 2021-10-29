using System;

namespace Bgt.Ocean.Service.Messagings.UserService
{
    public class ChangeLanguageRequest
    {
        public int ApplicationID { get; set; }
        public Guid MasterUser_Guid { get; set; }
        public Guid SystemLanguage_Guid { get; set; }
    }
}
