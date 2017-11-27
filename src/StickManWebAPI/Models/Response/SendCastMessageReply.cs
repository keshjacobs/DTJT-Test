using System.Net;

namespace StickManWebAPI.Models.Response
{
	public class SendCastMessageReply : Reply
	{
		public SendCastMessageReply(HttpStatusCode code, string message) : base(code, message)
		{
		}

		public int CastMessageId { get; set; }
	}
}