using System.Collections.Generic;
using System.Net;

namespace StickManWebAPI.Models.Response
{
	public class SendMessageReply : Reply
	{
		public SendMessageReply(HttpStatusCode code, string message) : base(code, message)
		{
		}

		public IEnumerable<int> MessageIds { get; set; }
	}
}