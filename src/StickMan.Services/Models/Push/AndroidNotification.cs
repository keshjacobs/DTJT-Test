using System.Collections.Generic;
using Newtonsoft.Json;

namespace StickMan.Services.Models.Push
{
	public class AndroidNotification
	{
		[JsonProperty("registration_ids")]
		public IEnumerable<string> RegistrationIds { get; set; }

		[JsonProperty("data")]
		public AndroidData Data { get; set; }

		public int ReceiverId { get; set; }
	}
}