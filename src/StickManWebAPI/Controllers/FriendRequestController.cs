using System.Net;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class FriendRequestController : ApiController
	{
		private readonly ISessionService _sessionService;
		private readonly IFriendRequestService _friendRequestService;
		private readonly IPushNotificationService _notificationService;

		public FriendRequestController(ISessionService sessionService, IFriendRequestService friendRequestService, IPushNotificationService notificationService)
		{
			_sessionService = sessionService;
			_friendRequestService = friendRequestService;
			_notificationService = notificationService;
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

			var count = _friendRequestService.Delete(friendData.UserId, friendData.FriendId);

			return new Reply($"Friend requests removed (Count: {count})");
		}

		[HttpPost]
		public Reply Send(FriendData friendData)
		{
			try
			{
				_sessionService.Validate(friendData.UserId, friendData.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			var sendingFriendRequestResult = _friendRequestService.Send(friendData.UserId, friendData.FriendId);

			if (sendingFriendRequestResult.Status == FriendRequestSendStatus.Sent || sendingFriendRequestResult.Status == FriendRequestSendStatus.Restored)
			{
				_notificationService.SendFriendRequestPush(friendData.UserId, sendingFriendRequestResult.Request);
			}

			return new SendFriendRequestResponse(sendingFriendRequestResult.Status)
			{
				Details = new FriendRequest
				{
					FriendRequestId = sendingFriendRequestResult.Request.FriendRequestId,
					FriendRequestState = sendingFriendRequestResult.Status.ToString()
				}
			};
		}

		[HttpPost]
		public Reply Accept(FriendRequestData friendRequestData)
		{
			try
			{
				_sessionService.Validate(friendRequestData.UserId, friendRequestData.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			_friendRequestService.Accept(friendRequestData.FriendRequestId);

			return new Reply("Friend request has been accepted");
		}

		[HttpGet]
		public Reply GetPending([FromUri]SessionData data)
		{
			try
			{
				_sessionService.Validate(data.UserId, data.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			var requests = _friendRequestService.GetPending(data.UserId);

			return new FriendRequestsReply
			{
				Requests = requests
			};
		}

		[HttpGet]
		public Reply GetSent([FromUri]SessionData data)
		{
			try
			{
				_sessionService.Validate(data.UserId, data.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			var requests = _friendRequestService.GetSent(data.UserId);

			return new FriendRequestsReply
			{
				Requests = requests
			};
		}
	}
}
