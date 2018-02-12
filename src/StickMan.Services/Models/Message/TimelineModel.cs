namespace StickMan.Services.Models.Message
{
	public class TimelineModel
	{
		public int MessageId { get; set; }

		public Emoji Emoji { get; set; }

		public string UserName { get; set; }

		public int UserId { get; set; }

		public string AudioPath { get; set; }

		public MessageStatus Status { get; set; }

		public MessageArrow Arrow { get; set; }

		public string TimePassedSinceUploaded { get; set; }

		public string Duration { get; set; }
	}
}
