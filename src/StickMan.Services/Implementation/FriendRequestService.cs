using System;
using System.Collections.Generic;
using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Models;

namespace StickMan.Services.Implementation
{
	public class FriendRequestService : IFriendRequestService
	{
		private readonly IUnitOfWork _unitOfWork;

		public FriendRequestService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public SendFriendRequestResultDto Send(int userId, int friendId)
		{
			var sender = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == userId);
			var friendRequest = _unitOfWork.Repository<StickMan_FriendRequest>().Get(f =>
					(f.UserID == userId && f.RecieverID == friendId) || (f.RecieverID == userId && f.UserID == friendId))
				.ToList()
				.SingleOrDefault();

			FriendRequestSendStatus status;
			if (friendRequest == null)
			{
				friendRequest = SaveNewFriendRequest(userId, friendId);

				status = FriendRequestSendStatus.Sent;
			}
			else
			{
				if (friendRequest.FriendRequestStatus == 1)
				{
					status = FriendRequestSendStatus.FriendExist;
				}
				else if(friendRequest.FriendRequestStatus == 0 && friendRequest.UserID == userId)
				{
					status = FriendRequestSendStatus.WaitingForAccepting;
				}
				else
				{
					status = UpdateFriendRequest(friendRequest);
				}
			}

			return new SendFriendRequestResultDto
			{
				Status = status,
				Request = Map(friendRequest, sender)
			};
		}

		public int GetUnansweredCount(int userId)
		{
			return _unitOfWork.Repository<StickMan_FriendRequest>()
				.Count(x => x.RecieverID == userId && x.FriendRequestStatus == 0);
		}

		public StickMan_FriendRequest Get(int userId, int receiverId)
		{
			var friendRequest = _unitOfWork.Repository<StickMan_FriendRequest>()
				.Get(x => x.UserID == userId && x.RecieverID == receiverId)
				.ToList()
				.FirstOrDefault();

			return friendRequest;
		}

		public StickMan_FriendRequest Get(int friendRequestId)
		{
			return _unitOfWork.Repository<StickMan_FriendRequest>().GetSingle(f => f.FriendRequestID == friendRequestId);
		}

		public IEnumerable<FriendRequestDto> GetSent(int senderId)
		{
			var friendRequests = _unitOfWork.Repository<StickMan_FriendRequest>()
				.Get(f => f.UserID == senderId && f.FriendRequestStatus == 0)
				.ToList();

			return MapFriendRequests(friendRequests);
		}

		public IEnumerable<FriendRequestDto> GetPending(int userId)
		{
			var friendRequests = _unitOfWork.Repository<StickMan_FriendRequest>()
				.Get(f => f.RecieverID == userId && f.FriendRequestStatus == 0)
				.ToList();

			return MapFriendRequests(friendRequests);
		}

		public void Accept(int friendRequestId)
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

		public int Delete(int userId, int friendId)
		{
			var friendRequests = _unitOfWork.Repository<StickMan_FriendRequest>().Get(f =>
					(f.UserID == userId && f.RecieverID == friendId)
					|| (f.RecieverID == userId && f.UserID == friendId)
					&& f.FriendRequestStatus == 0)
				.ToList();

			foreach (var stickManFriendRequest in friendRequests)
			{
				stickManFriendRequest.FriendRequestStatus = 2;
				_unitOfWork.Repository<StickMan_FriendRequest>().Update(stickManFriendRequest);
			}

			_unitOfWork.Save();

			return friendRequests.Count;
		}

		private IEnumerable<FriendRequestDto> MapFriendRequests(IReadOnlyCollection<StickMan_FriendRequest> friendRequests)
		{
			var sendersIds = friendRequests.Select(f => f.UserID);
			var senders = _unitOfWork.Repository<StickMan_Users>().Get(u => sendersIds.Contains(u.UserID)).ToList();

			var requests = new List<FriendRequestDto>();
			foreach (var friendRequest in friendRequests)
			{
				requests.Add(Map(friendRequest, senders.Single(s => s.UserID == friendRequest.UserID)));
			}

			return requests;
		}

		private FriendRequestDto Map(StickMan_FriendRequest friendRequest, StickMan_Users sender)
		{
			return new FriendRequestDto
			{
				SenderId = friendRequest.UserID,
				ReceiverId = friendRequest.RecieverID,
				FriendRequestId = friendRequest.FriendRequestID,
				SenderUserName = sender.UserName,
				SendTime = friendRequest.DateTimeStamp.GetValueOrDefault()
			};
		}

		private FriendRequestSendStatus UpdateFriendRequest(StickMan_FriendRequest friendRequest)
		{
			FriendRequestSendStatus status;
			switch (friendRequest.FriendRequestStatus)
			{
				case 0:
					friendRequest.FriendRequestStatus = 1;
					status = FriendRequestSendStatus.Accepted;
					break;
				case 2:
					friendRequest.FriendRequestStatus = 0; 
					status = FriendRequestSendStatus.Restored;
					break;
				default:
					status = FriendRequestSendStatus.None;
					break;
			}

			_unitOfWork.Repository<StickMan_FriendRequest>().Update(friendRequest);
			_unitOfWork.Save();
			return status;
		}

		private StickMan_FriendRequest SaveNewFriendRequest(int userId, int friendId)
		{
			var friendRequest = new StickMan_FriendRequest
			{
				UserID = userId,
				RecieverID = friendId,
				FriendRequestStatus = 0,
				DateTimeStamp = DateTime.UtcNow
			};
			_unitOfWork.Repository<StickMan_FriendRequest>().Insert(friendRequest);
			_unitOfWork.Save();

			return friendRequest;
		}
	}
}