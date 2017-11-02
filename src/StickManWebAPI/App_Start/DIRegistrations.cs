using Microsoft.Practices.Unity;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Implementation;

namespace StickManWebAPI
{
	public class DiRegistrations
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<IUnitOfWork, UnitOfWork>();
			container.RegisterType<IMessageService, MessageService>();
		}
	}
}