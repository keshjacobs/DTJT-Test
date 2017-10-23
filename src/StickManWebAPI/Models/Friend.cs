using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
	public class Friend
	{
		public string SessionToken { get; set; }
		public int FriendRequestId { get; set; }
		public int UserId { get; set; }
		public int RespondingToUserID { get; set; }
		public int RecieverUserId { get; set; }
		public int FriendRequestReply { get; set; }
	}
}