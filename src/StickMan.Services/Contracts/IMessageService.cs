using System.Collections.Generic;
using StickMan.Database;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface IMessageService
	{
		int GetUnreadMessagesCount(int userId);

		IEnumerable<StickMan_Users_AudioData_UploadInformation> Save(string filePath, int userId, IEnumerable<int> receiverIds);

		IEnumerable<TimelineModel> GetTimeline(int userId, int page, int size);

		void ReadMessage(int id);

		void CleanUpMessages();
	}
}
