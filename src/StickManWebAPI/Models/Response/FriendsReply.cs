using System.Collections.Generic;
using System.Net;
using StickMan.Services.Models;

namespace StickManWebAPI.Models.Response
{
	public class FriendsReply : Reply
	{
		public FriendsReply() : base(HttpStatusCode.OK, "Friends list")
		{
		}

		public IEnumerable<FriendModel> Friends { get; set; }
	}
}