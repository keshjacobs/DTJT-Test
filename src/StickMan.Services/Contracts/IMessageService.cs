using System.Collections.Generic;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface IMessageService
	{
		void Save(string filePath, int userId, int receiverId);

		IEnumerable<TimelineModel> GetTimeline(int userId, int page, int size);

		void ReadMessage(int id);

		void CleanUpMessages();
	}
}
