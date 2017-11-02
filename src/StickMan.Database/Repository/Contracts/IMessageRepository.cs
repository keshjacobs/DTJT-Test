using System;
using System.Collections.Generic;

namespace StickMan.Database.Repository.Contracts
{
	public interface IMessageRepository
	{
		ICollection<StickMan_Users_AudioData_UploadInformation> GetSentReceivedMessagesInfo(int userId);

		ICollection<StickMan_Users_AudioData_UploadInformation> GetMessagesOlderThanDate(DateTime date);

		void Update(StickMan_Users_AudioData_UploadInformation message);
	}
}