namespace StickManWebAPI.Models
{
	public class NewAudioMessage
	{
		public User user { get; set; }

		public int id { get; set; }

		public string message { get; set; }

		public long fileSize { get; set; }

		public string filter { get; set; }

		public string time { get; set; }

		public string MessageType { get; set; }

		public bool readstatus { get; set; }

		public bool deletestatus { get; set; }

		public bool iscasted { get; set; }

		public int clickcount { get; set; }
	}
}