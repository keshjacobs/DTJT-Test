using System;
using System.Net;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class FriendController : ApiController
	{
		private readonly ISessionService _sessionService;
		private readonly IFriendService _friendService;

		public FriendController(ISessionService sessionService, IFriendService friendService)
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

		[HttpPost]
		public Reply Block(FriendData friendData)
		{
			try
			{
				_sessionService.Validate(friendData.UserId, friendData.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			try
			{
				_friendService.Block(friendData.UserId, friendData.FriendId);
			}
			catch (InvalidOperationException e)
			{
				return new Reply(HttpStatusCode.BadRequest, $"Unable to block user. Error: {e.Message}");
			}

			return new Reply($"Friend {friendData.FriendId} blocked");
		}

		[HttpPost]
		public Reply Unblock(FriendData friendData)
		{
			try
			{
				_sessionService.Validate(friendData.UserId, friendData.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			try
			{
				_friendService.Unblock(friendData.UserId, friendData.FriendId);
			}
			catch (InvalidOperationException e)
			{
				return new Reply(HttpStatusCode.BadRequest, $"Unable to unblock user. Error: {e.Message}");
			}

			return new Reply($"Friend {friendData.FriendId} unblocked");
		}

		[HttpPost]
		public Reply Delete(FriendData friendData)
		{
			try
			{
				_sessionService.Validate(friendData.UserId, friendData.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			try
			{
				_friendService.Delete(friendData.UserId, friendData.FriendId);
			}
			catch (Exception e)
			{
				return new Reply(HttpStatusCode.BadRequest, $"Unable to delete friend. Error: {e.Message}");
			}

			return new Reply($"Friend {friendData.FriendId} removed");
		}
	}
}
