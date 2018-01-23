using StickMan.Database;

namespace StickMan.Services.Contracts
{
	public interface IFriendRequestService
	{
		int GetUnansweredRequestsCount(int userId);

		StickMan_FriendRequest GetFriendRequest(int userId, int receiverId);

		void AcceptFriendRequest(int friendRequestId);
	}
}
