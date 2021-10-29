using System.Web;
using System.Web.Mvc;

namespace Bgt.Ocean.RunControl.WebAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
