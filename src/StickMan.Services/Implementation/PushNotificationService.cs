using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceStack;
using StickMan.Database;
using StickMan.Services.Contracts;
using StickMan.Services.Models.Message;
using StickMan.Services.Models.Push;
using StickMan.Services.Models.User;

namespace StickMan.Services.Implementation
{
	// TODO use logger if any instead of Console.Write
	public class PushNotificationService : IPushNotificationService
	{
		private readonly IUserService _userService;
		private readonly IFriendRequestService _friendRequestService;

		public PushNotificationService(IUserService userService, IFriendRequestService friendRequestService)
		{
			_userService = userService;
			_friendRequestService = friendRequestService;
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
					sender.UserName, receiver.UserId, castMessage));
			}
		}

		public void SendMessagePush(int senderId, IEnumerable<int> receiverIds, IEnumerable<StickMan_Users_AudioData_UploadInformation> messages)
		{
			var sender = _userService.GetUser(senderId);
			var receivers = _userService.GetUsers(receiverIds);

			foreach (var receiver in receivers)
			{
				var message = messages.Single(m => m.UserID == senderId && m.RecieverID == receiver.UserId);
				SendMessagePush(receiver.DeviceId, sender, receiver.UserId, message);
			}
		}

		public void SendMessagePush(int senderId, string deviceId, int receiverId)
		{
			var sender = _userService.GetUser(senderId);

			SendMessagePush(deviceId, sender, receiverId, new object());
		}

		public void SendFriendRequestPush(int senderId, string deviceId, int friendRequestId)
		{
			var sender = _userService.GetUser(senderId);
			var message = $"{sender.FullName} sent you a friend request.";

			var friendRequest = _friendRequestService.Get(friendRequestId);
			PushAndroidNotification(deviceId, message, sender.UserName, friendRequest.RecieverID, friendRequest);
		}

		public void SendFriendRequestPush(int senderId, StickMan_FriendRequest friendRequest)
		{
			var receiver = _userService.GetUser(friendRequest.RecieverID);
			var sender = _userService.GetUser(senderId);
			var message = $"{sender.FullName} sent you a friend request.";

			PushAndroidNotification(receiver.DeviceId, message, sender.UserName, friendRequest.RecieverID, friendRequest);
		}

		private void SendMessagePush<TBody>(string deviceId, UserModel sender, int receiverId, TBody body)
		{
			var message = $"{sender.FullName} sent you a new message";
			PushAndroidNotification(deviceId, message, sender.UserName, receiverId, body);
		}

		private void PushAndroidNotification<TBody>(string deviceId, string message, string senderUserName, int receiverId, TBody messageBody)
		{
			PushAndroidNotification(new List<string> { deviceId }, message, senderUserName, receiverId, messageBody);
		}

		private void PushAndroidNotification<TBody>(IEnumerable<string> deviceIds, string message, string senderUserName, int receiverId, TBody messageBody)
		{
			using (var client = new JsonServiceClient("https://fcm.googleapis.com"))
			{
				client.Headers.Add("Authorization", "key=AAAAiYYf1YI:APA91bEpV9iycjVv0IvKxWR8YPIylkEY3Or9HFw8ONhGuGXmdr3dPhlcZN9XN5tEY3ZLgVK7yUlAyFzK8QRrJaEhmMTtoSp_tqzMy8N33rvEjaV4hqvbX-7ESnU-kX5QBRK6FnhzJRpD");

				var notification = new AndroidNotification
				{
					Data = new AndroidData
					{
						Message = message,
						UserName = senderUserName,
						Flag = "search",
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
