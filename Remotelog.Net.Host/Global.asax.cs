using System.Web.Http;
using Remotelog.Net.Server;
using Remotelog.Net.Server.AzureStorage;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Remotelog.Net.Host
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // register types
            var container = new Container();
            container.RegisterWebApiRequest<ILogWriter, LogWriter>();

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // initialize tables. always do this on app start to ensure tables exist
            new LogWriter().Initialize();

            // set up api routes
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}