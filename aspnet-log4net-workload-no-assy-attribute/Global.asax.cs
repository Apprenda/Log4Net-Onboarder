namespace aspnet_log4net_workload_no_assy_attribute
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using log4net;
    using log4net.Config;

    /// <summary>
    /// The mvc application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));
        /// <summary>
        /// The application_ start.
        /// </summary>
        protected void Application_Start()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("default.log4net"));
            Logger.Warn("Starting sample workload configured with XmlConfigurator.ConfigureAndWatch for default.log4net. (At WARN)");
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
