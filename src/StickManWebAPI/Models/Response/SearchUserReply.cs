using System.Collections.Generic;
using System.Net;
using StickMan.Services.Models;
using StickMan.Services.Models.User;

namespace StickManWebAPI.Models.Response
{
	public class SearchUserReply : Reply
	{
		public SearchUserReply() : base(HttpStatusCode.OK, "Search result")
		{
		}

		public IEnumerable<SearchUserModel> Users { get; set; }
	}
}