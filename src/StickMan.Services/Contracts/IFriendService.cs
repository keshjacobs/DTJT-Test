using System.Collections.Generic;
using StickMan.Database;
using StickMan.Services.Models;

namespace StickMan.Services.Contracts
{
	public interface IFriendService
	{
		int GetUnansweredRequestsCount(int userId);

		IEnumerable<FriendModel> GetFriends(int userId);

		StickMan_FriendRequest GetFriendRequest(int userId, int receiverId);

		void AcceptFriendRequest(int friendRequestId);
	}
}
