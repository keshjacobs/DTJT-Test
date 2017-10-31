using Microsoft.Practices.Unity;
using StickMan.Database.UnitOfWork;

namespace StickManWebAPI
{
	public class DiRegistrations
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<IUnitOfWork, UnitOfWork>();
		}
	}
}