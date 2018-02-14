using System.Collections.Generic;
using StickMan.Services.Models;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface IPushNotificationService
	{
		void SendCastPush(int senderId, CastMessage castMessage);

		void SendMessagePush(int senderId, IEnumerable<int> receiverIds, ICollection<JustSentMessage> messages);

		void SendFriendRequestPush(int senderId, FriendRequestDto friendRequest);
	}
}
