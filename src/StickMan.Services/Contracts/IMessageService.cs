using System.Collections.Generic;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface IMessageService
	{
		IEnumerable<TimelineModel> GetTimeline(int userId, int page, int size);

		void SaveCastMessage(string filePath, int userId);

		void ReadMessage(int id);

		int IncreaseCastClickCount(int castMessageId);

		IEnumerable<CastMessage> GetCastMessages(int page, int size);

		void CleanUpMessages();
	}
}
