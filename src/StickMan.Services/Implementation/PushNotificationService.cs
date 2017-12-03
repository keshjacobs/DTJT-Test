using System;
using System.Collections.Generic;
using System.Web.Hosting;
using Newtonsoft.Json;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using ServiceStack;
using StickMan.Services.Contracts;
using StickMan.Services.Models;
using StickMan.Services.Models.Push;

namespace StickMan.Services.Implementation
{
	// TODO use logger if any instead of Console.Write
	public class PushNotificationService : IPushNotificationService
	{
		private readonly IUserService _userService;

		public PushNotificationService(IUserService userService)
		{
			_userService = userService;
		}

		public void SendMessagePush(int senderId, IEnumerable<int> receiverIds)
		{
			var sender = _userService.GetUser(senderId);
			var receivers = _userService.GetUsers(receiverIds);

			foreach (var receiver in receivers)
			{
				SendMessagePush(receiver.DeviceId, sender);
			}
		}

		public void SendMessagePush(int senderId, string deviceId)
		{
			var sender = _userService.GetUser(senderId);

			SendMessagePush(deviceId, sender);
		}

		public void SendFriendRequestPush(int senderId, string deviceId)
		{
			var sender = _userService.GetUser(senderId);
			var message = $"{sender.FullName} sent you a friend request.";

			PushAppleNotification(deviceId, message);
			PushAndroidNotification(deviceId, message, sender.UserName);
		}

		private void SendMessagePush(string deviceId, UserModel sender)
		{
			var message = $"{sender.FullName} sent you a new message";
			PushAppleNotification(deviceId, message);
			PushAndroidNotification(deviceId, message, sender.UserName);
		}

		private void PushAndroidNotification(string deviceId, string message, string senderUserName)
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
						Flag = "search"
					},
					RegistrationIds = new List<string> { deviceId }
				};

				var body = JsonConvert.SerializeObject(notification);

				var response = client.Post<string>("fcm/send", body);
				Console.WriteLine($"Sending android push notification response: {response}");
			}
		}

		private void PushAppleNotification(string deviceId, string message)
		{
			//Create our push services broker
			var push = new PushBroker();

			//Wire up the events for all the services that the broker registers
			push.OnNotificationSent += NotificationSent;
			push.OnChannelException += ChannelException;
			push.OnServiceException += ServiceException;
			push.OnNotificationFailed += NotificationFailed;
			push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
			//push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
			push.OnChannelCreated += ChannelCreated;
			push.OnChannelDestroyed += ChannelDestroyed;


			//-------------------------
			// APPLE NOTIFICATIONS
			//-------------------------
			//Configure and start Apple APNS
			// IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to generate one for connecting to Sandbox,
			//   and one for connecting to Production.  You must use the right one, to match the provisioning profile you build your
			//   app with!
			//var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/CertificatesNew.p12"));

			//var appleCert = System.Web.Hosting.HostingEnvironment.MapPath("~/utilities/Certificates_dis_final.p12");

			var appleCert = HostingEnvironment.MapPath("~/utilities/CertificatesKeshP2.p12");

			//IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server 
			//  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
			//  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
			//  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
			push.RegisterAppleService(new ApplePushChannelSettings(appleCert, "123123", true)); //Extension method
																								 //Fluent construction of an iOS notification
																								 //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
																								 //  for registered for remote notifications is called, and the device token is passed back to you
			push.QueueNotification(new AppleNotification()
									   .ForDeviceToken(deviceId)
									   .WithAlert(message)
									   .WithBadge(7)
									   .WithSound("sound.caf"));

			//Console.WriteLine("Waiting for Queue to Finish...");

			//Stop and wait for the queues to drains
			push.StopAllServices(waitForQueuesToFinish: true);

			//Console.WriteLine("Queue Finished, press return to exit...");
			//Console.ReadLine();
		}

		private static void NotificationSent(object sender, INotification notification)
		{
			Console.WriteLine("Sent: " + sender + " -> " + notification);
		}

		private static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
		{
			Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
		}

		private static void ChannelException(object sender, IPushChannel channel, Exception exception)
		{
			Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
		}

		private static void ServiceException(object sender, Exception exception)
		{
			Console.WriteLine("Service Exception: " + sender + " -> " + exception);
		}

		private static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
		{
			Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
		}

		private static void ChannelDestroyed(object sender)
		{
			Console.WriteLine("Channel Destroyed for: " + sender);
		}

		private static void ChannelCreated(object sender, IPushChannel pushChannel)
		{
			Console.WriteLine("Channel Created for: " + sender);
		}
	}
}
