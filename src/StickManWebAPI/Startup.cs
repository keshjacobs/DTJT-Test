using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(StickManWebAPI.Startup))]

namespace StickManWebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Map Dashboard to the `http://<your-app>/hangfire` URL.
            app.UseHangfireDashboard();
        }
    }
}
