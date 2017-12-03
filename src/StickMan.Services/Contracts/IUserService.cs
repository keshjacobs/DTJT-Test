using System.Collections.Generic;
using StickMan.Services.Models;

namespace StickMan.Services.Contracts
{
	public interface IUserService
	{
		IEnumerable<UserModel> GetUsers(IEnumerable<int> ids);

		UserModel GetUser(int id);
	}
}
