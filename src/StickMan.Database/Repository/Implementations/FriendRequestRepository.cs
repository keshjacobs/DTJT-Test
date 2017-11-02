using System.Collections.Generic;
using System.Linq;
using StickMan.Database.Repository.Contracts;

namespace StickMan.Database.Repository.Implementations
{
	public class FriendRequestRepository : IFriendRequestRepository
	{
		private readonly EfStickManContext _context;

		public FriendRequestRepository(EfStickManContext context)
		{
			_context = context;
		}

		public ICollection<StickMan_FriendRequest> GetMany(int userId, int receiverId)
		{
			var firendRequests = _context.StickMan_FriendRequest.Where(x => x.UserID == userId && x.RecieverID == receiverId).ToList();

			return firendRequests;
		}
	}
}