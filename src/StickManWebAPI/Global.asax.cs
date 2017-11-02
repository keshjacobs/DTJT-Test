using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentScheduler;
using Microsoft.Practices.Unity;
using StickManWebAPI.Scheduler;

namespace StickManWebAPI
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var container = new UnityContainer();

			AreaRegistration.RegisterAllAreas();
			UnityConfig.RegisterComponents(container);
			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			var config = GlobalConfiguration.Configuration;
			config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

			config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

			JobManager.JobFactory = new StructureMapJobFactory(container);
			JobManager.Initialize(new JobsRegistry());
		}
	}
}