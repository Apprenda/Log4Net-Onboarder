using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using log4net;

namespace aspnet_webconfig_log4netconfighandler
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
            Logger.Warn(
                "Starting sample workload configured with XmlConfigurator.ConfigureAndWatch for default.log4net. (At WARN)");
            Logger.Info("Sample log at INFO.");
            Logger.Debug("Sample log at DEBUG");
            Logger.Error("Sample log at ERROR");
            Logger.Fatal("Sample log at FATAL");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}