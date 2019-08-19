using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class ActionBarController : ApiController
	{
		private readonly ISessionService _sessionService;
		private readonly IFriendRequestService _friendRequestService;
		private readonly IMessageService _messageService;
        private readonly INotificationService _notificationService;


        public ActionBarController(ISessionService sessionService, IFriendRequestService friendRequestService, IMessageService messageService,INotificationService notificationService)
		{
			_sessionService = sessionService;
			_friendRequestService = friendRequestService;
			_messageService = messageService;
            _notificationService = notificationService;

        }

		[HttpGet]
		public Reply Get([FromUri]SessionData session)
		{
			try
			{
				_sessionService.Validate(session.UserId, session.SessionToken);
			}
			catch (InvalidSessionException ex)
			{
				return new Reply
				{
					replyCode = (int)EnumReply.processFail,
					replyMessage = ex.Message
				};
			}

			return new ActionBarInfo
			{
				UnansweredFriendRequests = _friendRequestService.GetUnansweredCount(session.UserId),
				UnreadMessages = _messageService.GetUnreadMessagesCount(session.UserId),
                UnreadNotification=_notificationService.GetUnreadNotificationCount(session.UserId)
			};
		}
	}
}
