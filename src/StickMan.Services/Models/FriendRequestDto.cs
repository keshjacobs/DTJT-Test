using System;

namespace StickMan.Services.Models
{
	public class FriendRequestDto
	{
		public int FriendRequestId { get; set; }

		public int SenderId { get; set; }

		public int ReceiverId { get; set; }

		public string SenderUserName { get; set; }

		public DateTime SendTime { get; set; }
	}
}
