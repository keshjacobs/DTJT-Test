using System.Collections.Generic;
using StickMan.Database;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Contracts
{
	public interface ICastMessageService
	{
		CastMessage Save(string filePath, int userId, string title, int? replyPostId = null);

		int ReadMessage(int castMessageId, int currentUserId);

		IEnumerable<CastMessage> GetMessages(int page, int size, int currentUserId,int postId=-1);

		IEnumerable<CastMessage> Search(string term, int currentUserId);

		string ChangeTitle(int userId, int castId, string newTitle);

        int LikeMessage(int castMessageId, int userId);

        int DislikeMessage(int castMessageId, int userId);

        CastMessage Repost(int userId, int postId, string comment, int replyId = 0);

        StickMan_Users_Cast_AudioData_UploadInformation GetMessage(int castMessageId);
    }
}
