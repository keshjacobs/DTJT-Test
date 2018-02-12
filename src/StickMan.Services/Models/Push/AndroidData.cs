using Newtonsoft.Json;

namespace StickMan.Services.Models.Push
{
	public class AndroidData
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("flag")]
		public string Flag { get; set; }

		[JsonProperty("username")]
		public string UserName { get; set; }

		[JsonProperty("body")]
		public object Body { get; set; }

		[JsonProperty("receiverId")]
		public int ReceiverId { get; set; }
	}
}
