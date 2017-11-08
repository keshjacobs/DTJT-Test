namespace StickManWebAPI.Models
{
	public class AudioMessage
	{
		public User user { get; set; }

		public string message { get; set; }

		public long fileSize { get; set; }

		public string filter { get; set; }

		public string time { get; set; }

		public int SenderId { get; set; }

		public string MessageType { get; set; }

		public bool readstatus { get; set; }

		public bool deletestatus { get; set; }

		public int clickcount { get; set; }
	}
}