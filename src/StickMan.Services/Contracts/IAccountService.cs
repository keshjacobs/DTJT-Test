using StickMan.Database;
using StickMan.Services.Models.User;

namespace StickMan.Services.Contracts
{
	public interface IAccountService
	{
		UserSessionModel Login(string userName, string password, string deviceId);
        UserSessionModel FacebookLogin(StickMan_Users userLogin);
        ResetPasswordModel ResetPassword(int userId);
    }
}
