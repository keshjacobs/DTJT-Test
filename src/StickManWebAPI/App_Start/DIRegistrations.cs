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
			container.RegisterType<ICastMessageService, CastMessageService>();
			container.RegisterType<IPathProvider, PathProvider>();
			container.RegisterType<ISessionService, SessionService>();
			container.RegisterType<IUserService, UserService>();
			container.RegisterType<IPushNotificationService, PushNotificationService>();
			container.RegisterType<IFriendService, FriendService>();
			container.RegisterType<IFriendRequestService, FriendRequestService>();
			container.RegisterType<IFileService, FileService>();
			container.RegisterType<IAudioFileService, AudioFileService>();
		}
	}
}