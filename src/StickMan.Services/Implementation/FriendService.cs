using System.Collections.Generic;
using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Comparers;
using StickMan.Services.Contracts;
using StickMan.Services.Models;

namespace StickMan.Services.Implementation
{
	public class FriendService : IFriendService
	{
		private readonly IUnitOfWork _unitOfWork;

		public FriendService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IEnumerable<FriendModel> GetFriends(int userId)
		{
			var friendRequests = GetFriendRequests(userId);
			var friendIds = GetFriendIds(userId, friendRequests);
			var friendsUsers = _unitOfWork.Repository<StickMan_Users>()
				.Get(f => friendIds.Contains(f.UserID))
				.ToList();

			var friends = GetFriends(friendRequests, friendsUsers, userId);

			return friends;
		}

		public void Block(int userId, int friendId)
		{
			var friend = GetUnblockedFriend(userId, friendId);

			friend.BlockedBy = userId;

			_unitOfWork.Repository<StickMan_FriendRequest>().Update(friend);
			_unitOfWork.Save();
		}

		public void Unblock(int userId, int friendId)
		{
			var friend = GetBlockedFriend(userId, friendId);

			friend.BlockedBy = null;

			_unitOfWork.Repository<StickMan_FriendRequest>().Update(friend);
			_unitOfWork.Save();
		}

		public void Delete(int userId, int friendId)
		{
			var friend = GetFriend(userId, friendId);

			friend.FriendRequestStatus = 2;

			_unitOfWork.Repository<StickMan_FriendRequest>().Update(friend);
			_unitOfWork.Save();
		}

		private List<StickMan_FriendRequest> GetFriendRequests(int userId)
		{
			return _unitOfWork.Repository<StickMan_FriendRequest>()
				.Get(f => (f.UserID == userId || f.RecieverID == userId) && f.FriendRequestStatus == 1)
				.ToList();
		}

		private StickMan_FriendRequest GetFriend(int userId, int friendId)
		{
			return _unitOfWork.Repository<StickMan_FriendRequest>()
				.GetSingle(f => (f.UserID == userId && f.RecieverID == friendId)
								|| (f.RecieverID == userId && f.UserID == friendId)
								&& f.FriendRequestStatus == 1);
		}

		private StickMan_FriendRequest GetBlockedFriend(int userId, int friendId)
		{
			return _unitOfWork.Repository<StickMan_FriendRequest>()
				.GetSingle(f => (f.UserID == userId && f.RecieverID == friendId)
								|| (f.RecieverID == userId && f.UserID == friendId)
								&& f.FriendRequestStatus == 1
								&& f.BlockedBy == userId);
		}

		private StickMan_FriendRequest GetUnblockedFriend(int userId, int friendId)
		{
			return _unitOfWork.Repository<StickMan_FriendRequest>()
				.GetSingle(f => (f.UserID == userId && f.RecieverID == friendId)
								|| (f.RecieverID == userId && f.UserID == friendId)
								&& f.FriendRequestStatus == 1
								&& f.BlockedBy == null);
		}

		private IEnumerable<FriendModel> GetFriends(ICollection<StickMan_FriendRequest> friendRequests, ICollection<StickMan_Users> users, int userId)
		{
			var friends = new List<FriendModel>();

			foreach (var friendRequest in friendRequests)
			{
				var friendId = friendRequest.UserID == userId ? friendRequest.RecieverID : friendRequest.UserID;
				var friendUser = users.SingleOrDefault(u => u.UserID == friendId);
				if (friendUser == null)
				{
					continue;
				}

				friends.Add(new FriendModel
				{
					UserId = friendId,
					UserName = friendUser.UserName,
					FullName = friendUser.FullName,
					FriendRequestId = friendRequest.FriendRequestID,
					BlockedByYou = friendRequest.BlockedBy == userId,
					BlockedYou = friendRequest.BlockedBy != null && friendRequest.BlockedBy != userId
				});
			}

			return friends.Distinct(new FriendModelComparer());
		}

		private static IEnumerable<int> GetFriendIds(int userId, ICollection<StickMan_FriendRequest> friendsRecords)
		{
			var friendIds = friendsRecords.Where(f => f.UserID != userId).Select(f => f.UserID).ToList();
			friendIds.AddRange(friendsRecords.Where(f => f.RecieverID != userId).Select(f => f.RecieverID));
			friendIds = friendIds.Distinct().ToList();

			return friendIds;
		}
	}
}
