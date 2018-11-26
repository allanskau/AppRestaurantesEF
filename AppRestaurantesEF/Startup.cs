using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AppRestaurantesEF.Startup))]
namespace AppRestaurantesEF
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
