using StickMan.Database;

namespace StickMan.Services.Models
{
	public class SendFriendRequestResultDto
	{
		public FriendRequestSendStatus Status { get; set; }

		public StickMan_FriendRequest Request { get; set; }
	}
}