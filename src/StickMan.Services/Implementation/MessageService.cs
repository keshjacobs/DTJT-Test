using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Implementation
{
	public class MessageService : IMessageService
	{
		private readonly IUnitOfWork _unitOfWork;

		public MessageService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IEnumerable<TimelineModel> GetTimeline(int userId)
		{
			var timeline = new List<TimelineModel>();

			var messagesInfo = _unitOfWork.MessageRepository.GetSentReceivedMessagesInfo(userId)
				.OrderByDescending(m => m.UploadTime);

			foreach (var message in messagesInfo)
			{
				var timelineMessage = new TimelineModel
				{
					AudioPath = message.AudioFilePath
				};

				if (message.DeleteStatus)
				{
					FillDeletedMessageInfo(timelineMessage);
				}
				else
				{
					FillExistingMessageInfo(userId, timelineMessage, message);
				}

				FillUser(timelineMessage, message, userId);

				timeline.Add(timelineMessage);
			}

			return timeline;
		}

		public void CleanUpMessages()
		{
			var date = DateTime.UtcNow.AddDays(-2);

			var messages = _unitOfWork.MessageRepository.GetMessagesOlderThanDate(date);

			foreach (var message in messages)
			{
				var absolutePath = HostingEnvironment.MapPath("~/Content/Audio/" + message.AudioFilePath);

				if (File.Exists(absolutePath))
				{
					File.Delete(absolutePath);
				}

				message.DeleteStatus = true;
				_unitOfWork.MessageRepository.Update(message);
			}

			_unitOfWork.Save();
		}

		private static void FillExistingMessageInfo(int userId, TimelineModel timelineMessage, StickMan_Users_AudioData_UploadInformation message)
		{
			timelineMessage.Emoji = message.ReadStatus ? Emoji.Grimacing : Emoji.SmilingImp;

			if (message.UserID == userId)
			{
				timelineMessage.Arrow = MessageArrow.Right;
				timelineMessage.Status = MessageStatus.Sent;
			}

			if (message.RecieverID == userId)
			{
				timelineMessage.Arrow = MessageArrow.Left;
				timelineMessage.Status = MessageStatus.Received;
			}
		}

		private static void FillDeletedMessageInfo(TimelineModel timelineMessage)
		{
			timelineMessage.Emoji = Emoji.Smile;
			timelineMessage.Arrow = MessageArrow.None;
			timelineMessage.Status = MessageStatus.Deleted;
		}

		private void FillUser(TimelineModel timelineMessage, StickMan_Users_AudioData_UploadInformation message, int userId)
		{
			var id = message.UserID == userId ? message.RecieverID : message.UserID;

			var user = _unitOfWork.UserRepository.Get(id);
			timelineMessage.UserName = user.UserName;
			timelineMessage.UserId = id;
		}
	}
}
