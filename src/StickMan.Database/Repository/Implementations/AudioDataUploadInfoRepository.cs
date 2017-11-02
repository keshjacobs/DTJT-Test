using System.Collections.Generic;
using System.Linq;
using StickMan.Database.Repository.Contracts;

namespace StickMan.Database.Repository.Implementations
{
	public class AudioDataUploadInfoRepository : IAudioDataUploadInfoRepository
	{
		private readonly EfStickManContext _context;

		public AudioDataUploadInfoRepository(EfStickManContext context)
		{
			_context = context;
		}

		public ICollection<StickMan_Users_AudioData_UploadInformation> GetSentReceivedMessagesInfo(int userId)
		{
			var information = _context.StickMan_Users_AudioData_UploadInformation
				.Where(x => x.UserID == userId || x.RecieverID == userId).ToList();

			return information;
		}
	}
}
