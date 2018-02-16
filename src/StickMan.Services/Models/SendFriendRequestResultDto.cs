namespace StickMan.Services.Models
{
	public class SendFriendRequestResultDto
	{
		public FriendRequestSendStatus Status { get; set; }

		public FriendRequestDto Request { get; set; }
	}
}