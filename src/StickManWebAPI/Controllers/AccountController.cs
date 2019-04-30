using System;
using System.Globalization;
using System.Net;
using System.Web.Http;
using StickMan.Database;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.User;
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
        public Reply FacebookLogin(SignUpModel loginModel)
        {

            try
            {
                StickMan_Users temuser = new StickMan_Users();
                temuser.DeviceId = loginModel.DeviceId;
                temuser.DOB = Convert.ToDateTime(loginModel.Dob, CultureInfo.InvariantCulture);
                temuser.EmailID = loginModel.EmailID;
                temuser.FullName = loginModel.FullName;
                temuser.ImagePath = loginModel.ImagePath;
                temuser.MobileNo = loginModel.MobileNo;
                temuser.Password = loginModel.Password;
                temuser.Sex = loginModel.Sex;
                temuser.UserName = loginModel.Username;
                var user = _accountService.FacebookLogin(temuser);
                return new LoginReply
                {
                    User = user
                };
            }
            catch (AuthenticationException e)
            {
                return new Reply(HttpStatusCode.Unauthorized, e.Message);
            }
            catch (Exception ex) {
                return new Reply(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        public Reply ResetPassword(Models.ResetPasswordModel model)
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
