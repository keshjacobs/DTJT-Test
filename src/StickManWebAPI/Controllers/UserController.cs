using System.Net;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class UserController : ApiController
	{
		private readonly ISessionService _sessionService;
		private readonly IUserService _userService;

		public UserController(ISessionService sessionService, IUserService userService)
		{
			_sessionService = sessionService;
			_userService = userService;
		}

		[HttpGet]
		public Reply Search([FromUri]SearchUserData data)
		{
			try
			{
				_sessionService.Validate(data.UserId, data.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			var users = _userService.Search(data.UserId, data.SearchKeyword);

			return new SearchUserReply
			{
				Users = users
			};
		}
	}
}