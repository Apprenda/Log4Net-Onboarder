using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using log4net.Config;

namespace aspnet_webconfig_netfx4
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            XmlConfigurator.Configure(); 
            // load from web.config ignored section.

            var logger = LogManager.GetLogger(GetType());
            logger.Warn("Starting sample workload with XmlConfigurator direct call. (At WARN)");
            logger.Info("Sample log at INFO.");
            logger.Debug("Sample log at DEBUG");
            logger.Error("Sample log at ERROR");
            logger.Fatal("Sample log at FATAL"); 
            
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}