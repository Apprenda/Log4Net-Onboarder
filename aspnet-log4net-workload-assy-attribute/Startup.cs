using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(aspnet_log4net_workload_assy_attribute.Startup))]
namespace aspnet_log4net_workload_assy_attribute
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
