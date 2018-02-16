using System.Collections.Generic;
using StickMan.Database;
using StickMan.Services.Models;

namespace StickMan.Services.Contracts
{
	public interface IFriendRequestService
	{
		void Accept(int friendRequestId);

		StickMan_FriendRequest Get(int userId, int receiverId);

		StickMan_FriendRequest Get(int friendRequestId);

		IEnumerable<FriendRequestDto> GetPending(int userId);

		IEnumerable<FriendRequestDto> GetSent(int senderId);

		SendFriendRequestResultDto Send(int userId, int friendId);

		int GetUnansweredCount(int userId);

		int Delete(int userId, int friendId);
	}
}