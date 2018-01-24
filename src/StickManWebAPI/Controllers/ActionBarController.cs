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
		private readonly IFriendService _friendService;
		private readonly IMessageService _messageService;

		public ActionBarController(ISessionService sessionService, IFriendService friendService, IMessageService messageService)
		{
			_sessionService = sessionService;
			_friendService = friendService;
			_messageService = messageService;
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
				UnansweredFriendRequests = _friendService.GetUnansweredRequestsCount(session.UserId),
				UnreadMessages = _messageService.GetUnreadMessagesCount(session.UserId)
			};
		}
	}
}
