using StickMan.Database;

namespace StickMan.Services.Contracts
{
	public interface IFriendRequestService
	{
		StickMan_FriendRequest GetFriendRequest(int userId, int receiverId);

		void AcceptFriendRequest(int friendRequestId);
	}
}
