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
		private readonly IMessageConverter _messageConverter;

		public MessageService(IUnitOfWork unitOfWork, IPathProvider pathProvider, IMessageConverter messageConverter)
		{
			_unitOfWork = unitOfWork;
			_pathProvider = pathProvider;
			_messageConverter = messageConverter;
		}

		public int GetUnreadMessagesCount(int userId)
		{
			return _unitOfWork.Repository<StickMan_Users_AudioData_UploadInformation>()
				.Count(m => m.RecieverID == userId && !m.ReadStatus && !m.DeleteStatus);
		}

		public ICollection<JustSentMessage> Save(string filePath, int userId, IEnumerable<int> receiverIds)
		{
			var messages = new List<JustSentMessage>();
			foreach (var receiverId in receiverIds)
			{
				//string[] filePathsplit = filePath.Split('/');
                		//string copyPath = "/" + userId.ToString() + "/" + filePath.Replace("/" + userId.ToString(), receiverIds.ToString());                
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
				_unitOfWork.Save();

				var justSentMessage = _messageConverter.MapToJustSentMessage(message, receiverId);
				messages.Add(justSentMessage);
			}

			return messages;
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
				var timelineMessage = _messageConverter.MapToTimeLine(message, userId);

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
	}
}
