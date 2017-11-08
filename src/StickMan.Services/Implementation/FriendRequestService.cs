using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;

namespace StickMan.Services.Implementation
{
	public class FriendRequestService : IFriendRequestService
	{
		private readonly IUnitOfWork _unitOfWork;

		public FriendRequestService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
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
	}
}
