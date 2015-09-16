using System.Web;
using System.Web.Mvc;

namespace aspnet_log4net_workload_assy_attribute
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
