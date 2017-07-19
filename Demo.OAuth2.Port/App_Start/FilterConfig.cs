using System.Web;
using System.Web.Mvc;

namespace Demo.OAuth2.Port
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
