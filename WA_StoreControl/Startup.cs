using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WA_StoreControl.Startup))]
namespace WA_StoreControl
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
