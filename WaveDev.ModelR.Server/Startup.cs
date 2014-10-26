using Microsoft.Owin;
using Owin;
using WaveDev.ModelR.Server.Security;

[assembly: OwinStartup(typeof(WaveDev.ModelR.Server.Startup))]

namespace WaveDev.ModelR.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.Use(typeof(ModelRAuthenticationMiddleware));

            app.MapSignalR();
        }
    }
}
