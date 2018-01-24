using System.Net;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class FriendsController : ApiController
	{
		private readonly ISessionService _sessionService;
		private readonly IFriendService _friendService;

		public FriendsController(ISessionService sessionService, IFriendService friendService)
		{
			_sessionService = sessionService;
			_friendService = friendService;
		}

		[HttpGet]
		public Reply Get([FromUri]SessionData session)
		{
			try
			{
				_sessionService.Validate(session.UserId, session.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			var friends = _friendService.GetFriends(session.UserId);

			return new FriendsReply
			{
				Friends = friends
			};
		}
	}
}
