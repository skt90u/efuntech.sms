using Microsoft.Owin;
using Owin;
using System.Web.Routing;

[assembly: OwinStartupAttribute(typeof(EFunTech.Sms.Portal.Startup))]
namespace EFunTech.Sms.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMapper(app);
            ConfigureAuth(app);
            ConfigureHangFire(app);
        }
    }
}
