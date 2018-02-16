using StickMan.Database;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface IMessageConverter
	{
		TimelineModel MapToTimeLine(StickMan_Users_AudioData_UploadInformation message, int userId);

		JustSentMessage MapToJustSentMessage(StickMan_Users_AudioData_UploadInformation message, int receiverId);
	}
}
