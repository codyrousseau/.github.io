using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(crousseau2_College_Strike.Startup))]
namespace crousseau2_College_Strike
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
