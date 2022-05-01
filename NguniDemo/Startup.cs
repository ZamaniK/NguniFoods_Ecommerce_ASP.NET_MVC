using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NguniDemo.Startup))]
namespace NguniDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
