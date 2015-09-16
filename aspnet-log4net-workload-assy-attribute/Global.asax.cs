using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace aspnet_log4net_workload_assy_attribute
{
    using log4net;
    using log4net.Core;

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            Logger.Warn("Starting sample workload marked with XmlConfigurator attribute. (At WARN)");
            Logger.Info("Sample log at INFO.");
            Logger.Debug("Sample log at DEBUG");
            Logger.Error("Sample log at ERROR");
            Logger.Fatal("Sample log at FATAL");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
