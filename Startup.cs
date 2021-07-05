using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HumanResources.Startup))]
namespace HumanResources
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
