using System.Linq;
using StickMan.Database.Repository.Contracts;

namespace StickMan.Database.Repository.Implementations
{
	public class UserRepository : IUserRepository
	{
		private readonly EfStickManContext _context;

		public UserRepository(EfStickManContext context)
		{
			_context = context;
		}

		public StickMan_Users Get(int id)
		{
			var user = _context.StickMan_Users.Single(u => u.UserID == id);

			return user;
		}
	}
}