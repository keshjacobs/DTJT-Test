using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Models;
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

		public void SaveCastMessage(string filePath, int userId)
		{
			var message = new StickMan_Users_Cast_AudioData_UploadInformation
			{
				AudioFilePath = filePath,
				UserID = userId,
				ReadStatus = false,
				DeleteStatus = false,
				ClickCount = 0,
				UploadTime = DateTime.UtcNow
			};

			_unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().Insert(message);
			_unitOfWork.Save();
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

		public int IncreaseCastClickCount(int castMessageId)
		{
			var castMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(m => m.Id == castMessageId);

			castMessage.ClickCount++;
			_unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().Update(castMessage);
			_unitOfWork.Save();

			return castMessage.ClickCount.GetValueOrDefault();
		}

		public IEnumerable<CastMessage> GetCastMessages(int page, int size)
		{
			var castUploadInfo = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>()
				.GetQueryAll()
				.OrderByDescending(u => u.UploadTime)
				.Skip(page * size)
				.Take(size)
				.ToList();

			var castMessages = new List<CastMessage>();
			var userIds = castUploadInfo.Select(i => i.UserID).Distinct();
			var users = _unitOfWork.Repository<StickMan_Users>().Get(u => userIds.Any(m => m == u.UserID)).ToList();

			foreach (var uploadInfo in castUploadInfo)
			{
				var user = users.FirstOrDefault(u => u.UserID == uploadInfo.UserID);

				var message = new CastMessage
				{
					MessageInfo = new AudioInfo
					{
						Id = uploadInfo.Id,
						AudioFilePath = uploadInfo.AudioFilePath,
						UploadTime = uploadInfo.UploadTime.GetValueOrDefault(),
						Clicks = uploadInfo.ClickCount.GetValueOrDefault()
					}
				};

				if (user != null)
				{
					message.User = new UserModel
					{
						UserId = user.UserID,
						ImagePath = user.ImagePath,
						UserName = user.UserName,
						DOB = user.DOB,
						DeviceId = user.DeviceId,
						Email = user.EmailID,
						FullName = user.FullName,
						MobileNo = user.MobileNo,
						Sex = user.Sex
					};
				}

				castMessages.Add(message);
			}

			return castMessages;
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
