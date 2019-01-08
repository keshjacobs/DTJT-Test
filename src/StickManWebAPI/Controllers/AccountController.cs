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
        private readonly ISessionService _sessionService;

        public AccountController(
            IAccountService accountService,
            ISessionService sessionService)
		{
			_accountService = accountService;
            _sessionService = sessionService;
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

        [HttpPost]
        public Reply ResetPassword(ResetPasswordModel model)
        {
            try
            {
                _sessionService.Validate(model.UserId, model.SessionToken);
                var user = _accountService.ResetPassword(model.UserId);

                return new ResetPasswordReply
                {
                    User = user
                };
            }
            catch (InvalidSessionException)
            {
                return new Reply(HttpStatusCode.BadRequest, "Invalid session");
            }
        }
    }
}
