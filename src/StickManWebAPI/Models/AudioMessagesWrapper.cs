using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
	public class AudioMessagesWrapper
	{
		public Reply reply { get; set; }
		public List<AudioMessage> audioMessages { get; set; }
	}

	public class AudioMessage
	{
		public User user { get; set; }
		public string message { get; set; }
		public long fileSize { get; set; }
		public string filter { get; set; }
		public string time { get; set; }
		public int SenderId { get; set; }
		public string MessageType { get; set; }
	}
}