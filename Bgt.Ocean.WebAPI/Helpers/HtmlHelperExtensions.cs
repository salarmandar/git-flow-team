using System.Configuration;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Helpers
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Return the Current Version from the AssemblyInfo.cs file.
        /// </summary>
        public static string CurrentVersion(this HtmlHelper helper)
        {
            try
            {
                var version = ConfigurationManager.AppSettings["Version"];
                return version;
            }
            catch
            {
                return "?.?.?.?";
            }
        }
    }
}