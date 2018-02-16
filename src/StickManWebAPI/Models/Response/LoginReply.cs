using System.Net;
using StickMan.Services.Models.User;

namespace StickManWebAPI.Models.Response
{
	public class LoginReply : Reply
	{
		public LoginReply() : base(HttpStatusCode.OK, "Authenticated user")
		{
		}

		public UserSessionModel User { get; set; }
	}
}