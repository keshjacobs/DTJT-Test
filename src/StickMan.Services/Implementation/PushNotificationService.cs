using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceStack;
using StickMan.Services.Contracts;
using StickMan.Services.Models;
using StickMan.Services.Models.Message;
using StickMan.Services.Models.Push;

namespace StickMan.Services.Implementation
{
	// TODO use logger if any instead of Console.Write
	public class PushNotificationService : IPushNotificationService
	{
		private readonly IUserService _userService;
        private readonly ICastMessageService _castMessageService;
        

        public PushNotificationService(IUserService userService, ICastMessageService castMessageService)
		{
			_userService = userService;
            _castMessageService = castMessageService;
        }

		public void SendCastPush(int senderId, CastMessage castMessage)
		{
			var sender = _userService.GetUser(senderId);
			var receivers = _userService.GetAll()
				.Where(u => u.UserId != senderId);

			foreach (var receiver in receivers)
			{
				if (string.IsNullOrEmpty(receiver.DeviceId))
				{
					continue;
				}

				Task.Run(() => PushAndroidNotification(receiver.DeviceId, $"User {sender.UserName} uploaded new cast message",
					sender.UserName, receiver.UserId, NotificationType.Cast, castMessage));
			}
		}

		public void SendMessagePush(int senderId, IEnumerable<int> receiverIds, ICollection<JustSentMessage> messages)
		{
			var sender = _userService.GetUser(senderId);
			var receivers = _userService.GetUsers(receiverIds);

			foreach (var receiver in receivers)
			{
				var message = messages.Single(m => m.ReceiverId == receiver.UserId);
				var notificationMessage = $"{sender.FullName} sent you a new message";
				PushAndroidNotification(receiver.DeviceId, notificationMessage, sender.UserName, receiver.UserId, NotificationType.Message, message);
			}
		}
        public void SendCastPush(int userId, int postId, NotificationType type)
        {
            string message;
            var castMessage = _castMessageService.GetMessage(postId);
            var receiver = _userService.GetUser(castMessage.UserID.Value);
            var sender = _userService.GetUser(userId);
            //var message = $"{sender.FullName} Repost your Cast {castMessage.Title}.";
            switch (type) {
                case NotificationType.Repost:
                    message= $"{sender.FullName} Repost your Cast {castMessage.Title}.";
                    break;
                case NotificationType.Replay:
                    message = $"{sender.FullName} Replay your Cast {castMessage.Title}.";
                    break;
                case NotificationType.Like:
                    message = $"{sender.FullName} Like your Cast {castMessage.Title}.";
                    break;
                case NotificationType.DisLike:
                    message = $"{sender.FullName} Dislike your Cast {castMessage.Title}.";
                    break;
                default:
                    message = $"{sender.FullName} Push Notification.";
                    break;
            }

            PushAndroidNotification(receiver.DeviceId, message, sender.UserName, receiver.UserId, NotificationType.Repost, castMessage.Title);
        }
        public void SendFriendRequestPush(int senderId, FriendRequestDto friendRequest)
		{
			var receiver = _userService.GetUser(friendRequest.ReceiverId);
			var sender = _userService.GetUser(senderId);
			var message = $"{sender.FullName} sent you a friend request.";

			PushAndroidNotification(receiver.DeviceId, message, sender.UserName, friendRequest.ReceiverId, NotificationType.FriendRequest, friendRequest);
		}

		private void PushAndroidNotification<TBody>(string deviceId, string message, string senderUserName, int receiverId, NotificationType type, TBody messageBody)
		{
			if (string.IsNullOrEmpty(deviceId))
			{
				return;
			}

			PushAndroidNotification(new List<string> { deviceId }, message, senderUserName, receiverId, type, messageBody);
		}

		private void PushAndroidNotification<TBody>(IEnumerable<string> deviceIds, string message, string senderUserName, int receiverId, NotificationType type, TBody messageBody)
		{
			using (var client = new JsonServiceClient("https://fcm.googleapis.com"))
			{
				client.Headers.Add("Authorization", "key=AAAAeUdKrWE:APA91bGyBE4DDn3X2-u1jtXOWlQgjQQm8z3OzHk5agB6qv_j2BiVZKcCd4ny-5b8lW4cpdI5yEd37y0ruL7ysj9oNmWsCAQuM87TpY3P-JxGxO-CiRdnsFSmdnfaMSkN7Zo51VzaYRnZ");

				var notification = new AndroidNotification
				{
					Data = new AndroidData
					{
						Message = message,
						UserName = senderUserName,
						Flag = type,
						Body = messageBody,
						ReceiverId = receiverId
					},
					RegistrationIds = deviceIds
				};

                
                var body = JsonConvert.SerializeObject(notification);

				var response = client.Post<string>("fcm/send", body);
				Console.WriteLine($"Sending android push notification response: {response}");
			}
		}
    }
}
