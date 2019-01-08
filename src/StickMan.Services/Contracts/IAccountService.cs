using StickMan.Services.Models.User;

namespace StickMan.Services.Contracts
{
	public interface IAccountService
	{
		UserSessionModel Login(string userName, string password, string deviceId);

        ResetPasswordModel ResetPassword(int userId);
    }
}
