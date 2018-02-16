using System.Net;

namespace StickManWebAPI.Models.Response
{
	public class Reply
	{
		public Reply()
		{
		}

		public Reply(string message) : this(HttpStatusCode.OK, message)
		{
		}

		public Reply(HttpStatusCode code, string message)
		{
			replyCode = (int) code;
			replyMessage = message;
		}

		public int replyCode { get; set; }

		public string replyMessage { get; set; }
	}
}