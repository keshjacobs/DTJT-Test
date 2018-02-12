namespace StickMan.Services.Models.User
{
	public class SearchUserModel : UserModel
	{
		public int? FriendRequestId { get; set; }

		public FriendStatus FriendStatus { get; set; }
	}
}