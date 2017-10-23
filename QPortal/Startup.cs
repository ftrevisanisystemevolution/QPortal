using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QPortal.Startup))]
namespace QPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
