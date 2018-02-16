using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StickMan.Services.Models.Message
{
	public class TimelineModel
	{
		public int MessageId { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public Emoji Emoji { get; set; }

		public string UserName { get; set; }

		public int UserId { get; set; }

		public string AudioPath { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MessageStatus Status { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MessageArrow Arrow { get; set; }

		public string TimePassedSinceUploaded { get; set; }

		public string Duration { get; set; }
	}
}
