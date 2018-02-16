using StickMan.Services.Models;

namespace StickManWebAPI.Models.Response
{
	public class SendFriendRequestResponse : Reply
	{
		public SendFriendRequestResponse(FriendRequestSendStatus status) : base(
			$"Sending friend request is finished. Result: {status}")
		{

		}

		public FriendRequest Details { get; set; }
	}
}