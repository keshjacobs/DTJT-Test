using System.Collections.Generic;
using System.Net;
using StickMan.Services.Models;

namespace StickManWebAPI.Models.Response
{
	public class FriendRequestsReply : Reply
	{
		public FriendRequestsReply() : base(HttpStatusCode.OK, "Friend requests")
		{
		}

		public IEnumerable<FriendRequestDto> Requests { get; set; }
	}
}