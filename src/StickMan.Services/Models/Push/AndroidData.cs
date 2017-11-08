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
	}
}
