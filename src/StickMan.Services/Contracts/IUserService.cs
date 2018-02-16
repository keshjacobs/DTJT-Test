using System.Collections.Generic;
using StickMan.Services.Models.User;

namespace StickMan.Services.Contracts
{
	public interface IUserService
	{
		IEnumerable<SearchUserModel> Search(int userId, string searchTerm);

		IEnumerable<UserModel> GetUsers(IEnumerable<int> ids);

		IEnumerable<UserModel> GetAll();

		UserModel GetUser(int id);
	}
}
