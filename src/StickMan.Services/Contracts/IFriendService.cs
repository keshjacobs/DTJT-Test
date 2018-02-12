using System.Collections.Generic;
using StickMan.Services.Models;

namespace StickMan.Services.Contracts
{
	public interface IFriendService
	{
		IEnumerable<FriendModel> GetFriends(int userId);

		void Block(int userId, int friendId);

		void Unblock(int userId, int friendId);

		void Delete(int userId, int friendId);
	}
}
