using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Implementation
{
	public class MessageService : IMessageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPathProvider _pathProvider;

		public MessageService(IUnitOfWork unitOfWork, IPathProvider pathProvider)
		{
			_unitOfWork = unitOfWork;
			_pathProvider = pathProvider;
		}

		public int GetUnreadMessagesCount(int userId)
		{
			return _unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>()
				.Count(m => m.RecieverID == userId && !m.ReadStatus);
		}

		public IEnumerable<int> Save(string filePath, int userId, IEnumerable<int> receiverIds)
		{
			var messages = new List<StickMan_Users_AudioData_UploadInformation>();
			foreach (var receiverId in receiverIds)
			{
				var message = new StickMan_Users_AudioData_UploadInformation
				{
					AudioFilePath = filePath,
					UserID = userId,
					RecieverID = receiverId,
					ReadStatus = false,
					DeleteStatus = false,
					UploadTime = DateTime.UtcNow
				};

				_unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>().Insert(message);
				messages.Add(message);
			}

			_unitOfWork.Save();

			return messages.Select(m => m.Id);
		}

		public IEnumerable<TimelineModel> GetTimeline(int userId, int page, int size)
		{
			var timeline = new List<TimelineModel>();

			var messagesInfo = _unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>()
				.GetQuery(x => x.UserID == userId || x.RecieverID == userId)
				.OrderByDescending(m => m.UploadTime)
				.Skip(page * size)
				.Take(size)
				.ToList();

			foreach (var message in messagesInfo)
			{
				var timelineMessage = new TimelineModel
				{
					AudioPath = message.AudioFilePath,
					MessageId = message.Id
				};

				if (message.DeleteStatus)
				{
					FillDeletedMessageInfo(timelineMessage, message);
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

		public void ReadMessage(int id)
		{
			var message = _unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>().GetSingle(x => x.Id == id);

			if (!message.ReadStatus)
			{
				message.ReadStatus = true;
				_unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>().Update(message);
				_unitOfWork.Save();
			}
		}

		public void CleanUpMessages()
		{
			var date = DateTime.UtcNow.AddDays(-2);

			var messages = _unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>()
				.Get(x => x.UploadTime < date && !x.DeleteStatus);

			foreach (var message in messages)
			{
				var absolutePath = _pathProvider.BuildAudioPath(message.AudioFilePath);

				if (File.Exists(absolutePath))
				{
					File.Delete(absolutePath);
				}

				message.DeleteStatus = true;
				_unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>().Update(message);
			}

			_unitOfWork.Save();
		}

		private static void FillExistingMessageInfo(int userId, TimelineModel timelineMessage, StickMan_Users_AudioData_UploadInformation message)
		{
			timelineMessage.Emoji = message.ReadStatus ? Emoji.Grimacing : Emoji.Smile;

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

		private static void FillDeletedMessageInfo(TimelineModel timelineMessage, StickMan_Users_AudioData_UploadInformation message)
		{
			timelineMessage.Emoji = message.ReadStatus ? Emoji.Grimacing : Emoji.SmilingImp;

			timelineMessage.Arrow = MessageArrow.None;
			timelineMessage.Status = MessageStatus.Deleted;
		}

		private void FillUser(TimelineModel timelineMessage, StickMan_Users_AudioData_UploadInformation message, int userId)
		{
			var id = message.UserID == userId ? message.RecieverID : message.UserID;

			var user = _unitOfWork.Repository<StickMan_Users>().GetSingle(x => x.UserID == id);
			timelineMessage.UserName = user.UserName;
			timelineMessage.UserId = id;
		}
	}
}
