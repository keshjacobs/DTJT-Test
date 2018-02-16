using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StickMan.Services.Models.Push
{
	public class AndroidData
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("flag")]
		[JsonConverter(typeof(StringEnumConverter))]
		public NotificationType Flag { get; set; }

		[JsonProperty("username")]
		public string UserName { get; set; }

		[JsonProperty("body")]
		public object Body { get; set; }

		[JsonProperty("receiverId")]
		public int ReceiverId { get; set; }
	}
}
