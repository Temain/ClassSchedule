using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClassSchedule.Web.Startup))]
namespace ClassSchedule.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
