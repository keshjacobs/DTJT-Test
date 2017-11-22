using System.Collections.Generic;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface ICastMessageService
	{
		void SaveMessage(string filePath, int userId);

		int ReadMessage(int castMessageId);

		IEnumerable<CastMessage> GetMessages(int page, int size);

		IEnumerable<CastMessage> Search(string term);
	}
}
