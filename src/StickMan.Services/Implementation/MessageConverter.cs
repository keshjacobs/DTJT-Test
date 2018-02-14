using System;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Extensions;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Implementation
{
	public class MessageConverter : IMessageConverter
	{
		private readonly IAudioFileService _audioFileService;
		private readonly IUnitOfWork _unitOfWork;

		public MessageConverter(IAudioFileService audioFileService, IUnitOfWork unitOfWork)
		{
			_audioFileService = audioFileService;
			_unitOfWork = unitOfWork;
		}

		public TimelineModel MapToTimeLine(StickMan_Users_AudioData_UploadInformation message, int userId)
		{
			var justSentMessage = MapToJustSentMessage(message, userId);
			var timeline = (TimelineModel) justSentMessage;

			return timeline;
		}

		public JustSentMessage MapToJustSentMessage(StickMan_Users_AudioData_UploadInformation message, int receiverId)
		{
			var justSent = new JustSentMessage
			{
				AudioPath = message.AudioFilePath,
				MessageId = message.Id,
				TimePassedSinceUploaded = (DateTime.UtcNow - message.UploadTime).FormatDuration(),
				Duration = _audioFileService.GetDuration(message.AudioFilePath).FormatDuration(),
				ReceiverId = receiverId
			};

			if (message.DeleteStatus)
			{
				FillDeletedMessageInfo(justSent, message);
			}
			else
			{
				FillExistingMessageInfo(receiverId, justSent, message);
			}

			FillUser(justSent, message, receiverId);

			return justSent;
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
