using System;

namespace Bgt.Ocean.Service.ModelViews.Systems
{
    public class LanguageView
    {
        public Guid Guid { get; set; }
        public string LanguageName { get; set; }
        public string Abbreviation { get; set; }
        public string LanguageCode { get; set; }
        public bool FlagDisable { get; set; }
    }
}
