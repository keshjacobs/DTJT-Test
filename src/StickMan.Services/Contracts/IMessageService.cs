using System.Collections.Generic;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface IMessageService
	{
		IEnumerable<int> Save(string filePath, int userId, IEnumerable<int> receiverIds);

		IEnumerable<TimelineModel> GetTimeline(int userId, int page, int size);

		void ReadMessage(int id);

		void CleanUpMessages();
	}
}
