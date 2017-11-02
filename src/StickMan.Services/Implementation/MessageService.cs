using System.Collections.Generic;
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

			var messagesInfo = _unitOfWork.AudioDataUploadInfoRepository.GetSentReceivedMessagesInfo(userId);

			foreach (var message in messagesInfo)
			{
				var timelineMessage = new TimelineModel
				{
					AudioPath = message.AudioFilePath
				};

				if (message.DeleteStatus.GetValueOrDefault())
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

		private static void FillExistingMessageInfo(int userId, TimelineModel timelineMessage, StickMan_Users_AudioData_UploadInformation message)
		{
			timelineMessage.Emoji = message.ReadStatus.GetValueOrDefault() ? Emoji.Grimacing : Emoji.SmilingImp;

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
			var id = message.UserID == userId ? message.UserID : message.RecieverID;

			var user = _unitOfWork.UserRepository.Get(id.GetValueOrDefault());
			timelineMessage.UserName = user.UserName;
		}
	}
}
