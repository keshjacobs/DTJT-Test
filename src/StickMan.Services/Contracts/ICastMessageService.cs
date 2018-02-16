using System.Collections.Generic;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface ICastMessageService
	{
		CastMessage Save(string filePath, int userId, string title);

		int ReadMessage(int castMessageId, int currentUserId);

		IEnumerable<CastMessage> GetMessages(int page, int size, int currentUserId);

		IEnumerable<CastMessage> Search(string term, int currentUserId);

		string ChangeTitle(int userId, int castId, string newTitle);
	}
}
