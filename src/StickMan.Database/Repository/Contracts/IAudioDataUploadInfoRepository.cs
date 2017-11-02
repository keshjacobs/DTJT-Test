using System.Collections.Generic;

namespace StickMan.Database.Repository.Contracts
{
	public interface IAudioDataUploadInfoRepository
	{
		ICollection<StickMan_Users_AudioData_UploadInformation> GetSentReceivedMessagesInfo(int userId);
	}
}