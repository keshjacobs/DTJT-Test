namespace StickMan.Services.Models
{
	public enum FriendRequestSendStatus
	{
		None = 0,
		Sent = 1,
		Accepted = 2,
		Restored = 3,
		WaitingForAccepting = 4,
		FriendExist = 5
	}
}