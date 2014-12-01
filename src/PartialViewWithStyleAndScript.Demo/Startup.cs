using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PartialViewWithStyleAndScript.Demo.Startup))]
namespace PartialViewWithStyleAndScript.Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
