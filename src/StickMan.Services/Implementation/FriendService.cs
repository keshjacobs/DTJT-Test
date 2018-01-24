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

		public int GetUnansweredRequestsCount(int userId)
		{
			return _unitOfWork.Repository<StickMan_FriendRequest>()
				.Count(x => x.UserID == userId && x.FriendRequestStatus == 0);
		}

		public IEnumerable<FriendModel> GetFriends(int userId)
		{
			var friendRequests = _unitOfWork.Repository<StickMan_FriendRequest>()
				.Get(f => f.UserID == userId || f.RecieverID == userId && f.FriendRequestStatus != 0)
				.ToList();
			var friendIds = GetFriendIds(userId, friendRequests);
			var friendsUsers = _unitOfWork.Repository<StickMan_Users>()
				.Get(f => friendIds.Contains(f.UserID))
				.ToList();

			var friends = GetFriends(friendRequests, friendsUsers, userId);

			return friends;
		}

		public StickMan_FriendRequest GetFriendRequest(int userId, int receiverId)
		{
			var friendRequest = _unitOfWork.Repository<StickMan_FriendRequest>()
				.Get(x => x.UserID == userId && x.RecieverID == receiverId)
				.ToList()
				.FirstOrDefault();

			return friendRequest;
		}

		public void AcceptFriendRequest(int friendRequestId)
		{
			var friendRequest = _unitOfWork.Repository<StickMan_FriendRequest>().GetSingle(x => x.FriendRequestID == friendRequestId);
			friendRequest.FriendRequestStatus = 1;

			var friend = new StickMan_UsersFriendList
			{
				UserID = friendRequest.UserID,
				FriendID = friendRequest.RecieverID
			};

			_unitOfWork.Repository<StickMan_FriendRequest>().Update(friendRequest);
			_unitOfWork.Repository<StickMan_UsersFriendList>().Insert(friend);

			_unitOfWork.Save();
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
					Blocked = friendRequest.BlockedBy != null
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
