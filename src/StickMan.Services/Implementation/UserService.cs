using System.Collections.Generic;
using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Models.User;

namespace StickMan.Services.Implementation
{
	public class UserService : IUserService
	{
		private static readonly IDictionary<int?, FriendStatus> StatusMapping = new Dictionary<int?, FriendStatus>
		{
			{1, FriendStatus.Friend},
			{2, FriendStatus.FriendRemoved}
		};
		private readonly IUnitOfWork _unitOfWork;

		public UserService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IEnumerable<SearchUserModel> Search(int userId, string searchTerm)
		{
			var search = searchTerm.ToLower();
			var users = _unitOfWork.Repository<StickMan_Users>()
				.Get(u => (u.FullName.ToLower().Contains(search) || u.UserName.ToLower().Contains(search)) && u.UserID != userId)
				.ToList();
			var userIds = users.Select(u => u.UserID).ToList();

			var friendRequests = _unitOfWork.Repository<StickMan_FriendRequest>()
				.Get(f => (f.UserID == userId && userIds.Contains(f.RecieverID)) ||
				          (f.RecieverID == userId && userIds.Contains(f.UserID)))
				.ToList();
			var models = new List<SearchUserModel>();

			foreach (var user in users)
			{
				var friendRequest = friendRequests.FirstOrDefault(f => f.UserID == user.UserID || f.RecieverID == user.UserID);
				var model = MapSearchUserModels(user, friendRequest, userId);

				models.Add(model);
			}

			return models;
		}

		public IEnumerable<UserModel> GetUsers(IEnumerable<int> ids)
		{
			var users = _unitOfWork.Repository<StickMan_Users>().Get(u => ids.Contains(u.UserID));

			return users.Select(MapUserModels);
		}

		public IEnumerable<UserModel> GetAll()
		{
			var users = _unitOfWork.Repository<StickMan_Users>().GetAll();

			return users.Select(MapUserModels);
		}

		public UserModel GetUser(int id)
		{
			var dbUser = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == id);

			return MapUserModels(dbUser);
		}

		private static UserModel MapUserModels(StickMan_Users dbUser)
		{
			return new UserModel
			{
				ImagePath = dbUser.ImagePath,
				UserName = dbUser.UserName,
				UserId = dbUser.UserID,
				FullName = dbUser.FullName,
				DOB = dbUser.DOB,
				MobileNo = dbUser.MobileNo,
				Sex = dbUser.Sex,
				Email = dbUser.EmailID,
				DeviceId = dbUser.DeviceId
			};
		}
		private SearchUserModel MapSearchUserModels(StickMan_Users dbUser, StickMan_FriendRequest friendRequest, int currentUserId)
		{
			var userModel = new SearchUserModel
			{
				ImagePath = dbUser.ImagePath,
				UserName = dbUser.UserName,
				UserId = dbUser.UserID,
				FullName = dbUser.FullName,
				DOB = dbUser.DOB,
				MobileNo = dbUser.MobileNo,
				Sex = dbUser.Sex,
				Email = dbUser.EmailID,
				DeviceId = dbUser.DeviceId,
			};

			if (friendRequest == null)
			{
				userModel.FriendStatus = FriendStatus.Stranger;
			}
			else
			{
				userModel.FriendRequestId = friendRequest.FriendRequestID;
				if (friendRequest.FriendRequestStatus == 0)
				{
					userModel.FriendStatus =
						friendRequest.UserID == currentUserId ? FriendStatus.InviteSent : FriendStatus.InviteReceived;
				}
				else
				{
					userModel.FriendStatus = StatusMapping[friendRequest.FriendRequestStatus];
				}
			}

			return userModel;
		}
	}
}
