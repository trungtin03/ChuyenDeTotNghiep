using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CDTN.Startup))] // namespace phải đúng tên project bạn

namespace CDTN
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Map SignalR
            app.MapSignalR();
        }
    }
}
