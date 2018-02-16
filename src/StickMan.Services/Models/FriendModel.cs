namespace StickMan.Services.Models
{
	public class FriendModel
	{
		public int UserId { get; set; }

		public string UserName { get; set; }

		public string FullName { get; set; }

		public int FriendRequestId { get; set; }

		public bool BlockedByYou { get; set; }

		public bool BlockedYou { get; set; }
	}
}
