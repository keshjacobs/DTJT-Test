using System;
using System.Collections.Generic;
using StickMan.Database;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface IPushNotificationService
	{
		void SendCastPush(int senderId, CastMessage castMessage);

		void SendMessagePush(int senderId, IEnumerable<int> receiverIds, IEnumerable<StickMan_Users_AudioData_UploadInformation> messages);

		void SendMessagePush(int senderId, string deviceId, int receiverId);

		[Obsolete]
		void SendFriendRequestPush(int senderId, string deviceId, int friendRequestId);

		void SendFriendRequestPush(int senderId, StickMan_FriendRequest friendRequest);
	}
}
