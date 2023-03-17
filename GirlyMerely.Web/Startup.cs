using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GirlyMerely.Web.Startup))]
namespace GirlyMerely.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
