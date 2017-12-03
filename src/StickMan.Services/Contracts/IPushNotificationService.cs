using System.Collections.Generic;

namespace StickMan.Services.Contracts
{
	public interface IPushNotificationService
	{
		void SendMessagePush(int senderId, IEnumerable<int> receiverIds);

		void SendMessagePush(int senderId, string deviceId);

		void SendFriendRequestPush(int senderId, string deviceId);
	}
}
