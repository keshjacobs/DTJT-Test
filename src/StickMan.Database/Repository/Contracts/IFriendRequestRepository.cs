using System.Collections.Generic;

namespace StickMan.Database.Repository.Contracts
{
	public interface IFriendRequestRepository
	{
		ICollection<StickMan_FriendRequest> GetMany(int userId, int receiverId);
	}
}
