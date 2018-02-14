using System.Net;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class AccountController : ApiController
	{
		private readonly IAccountService _accountService;

		public AccountController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		[HttpPost]
		public Reply Login(LoginModel loginModel)
		{
			try
			{
				var user = _accountService.Login(loginModel.Username, loginModel.Password, loginModel.DeviceId);
				return new LoginReply
				{
					User = user
				};
			}
			catch (AuthenticationException e)
			{
				return new Reply(HttpStatusCode.Unauthorized, e.Message);
			}
		}
	}
}
