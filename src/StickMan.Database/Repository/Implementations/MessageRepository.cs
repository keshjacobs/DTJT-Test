using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using StickMan.Database.Repository.Contracts;

namespace StickMan.Database.Repository.Implementations
{
	public class MessageRepository : IMessageRepository
	{
		private readonly EfStickManContext _context;

		public MessageRepository(EfStickManContext context)
		{
			_context = context;
		}

		public ICollection<StickMan_Users_AudioData_UploadInformation> GetSentReceivedMessagesInfo(int userId)
		{
			var messages = _context.StickMan_Users_AudioData_UploadInformation
				.Where(x => x.UserID == userId || x.RecieverID == userId).ToList();

			return messages;
		}

		public ICollection<StickMan_Users_AudioData_UploadInformation> GetMessagesOlderThanDate(DateTime date)
		{
			var messages = _context.StickMan_Users_AudioData_UploadInformation
				.Where(x => x.UploadTime < date && !x.DeleteStatus).ToList();

			return messages;
		}

		public void Update(StickMan_Users_AudioData_UploadInformation message)
		{
			_context.Entry(message).State = EntityState.Modified;
		}
	}
}
