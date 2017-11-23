using System.Collections.Generic;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface ICastMessageService
	{
		void Save(string filePath, int userId, string title);

		int ReadMessage(int castMessageId);

		IEnumerable<CastMessage> GetMessages(int page, int size);

		IEnumerable<CastMessage> Search(string term);

		string ChangeTitle(int userId, int castId, string newTitle);
	}
}
