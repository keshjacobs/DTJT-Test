using StickMan.Services.Models;

namespace StickMan.Services.Contracts
{
	public interface IUserService
	{
		UserModel GetUser(int id);
	}
}
