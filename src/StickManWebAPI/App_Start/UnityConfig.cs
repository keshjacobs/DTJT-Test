using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace StickManWebAPI
{
	public static class UnityConfig
	{
		public static void RegisterComponents(UnityContainer container)
		{
			DiRegistrations.Register(container);

			GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
		}
	}
}