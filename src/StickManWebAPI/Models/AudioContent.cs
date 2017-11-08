using System.Collections.Generic;

namespace StickManWebAPI.Models
{
	public class AudioWrapper
	{
		public Reply reply { get; set; }
		public PushInfo pushInfo { get; set; }
	}

	public class AudioContent
	{
		public int userId { get; set; }
		public List<string> recieverId { get; set; }
		public string filePath { get; set; }
		public string sessionToken { get; set; }
		public string filter { get; set; }
	}

	public class CastAudioContent
	{
		public int UserId { get; set; }

		public string FilePath { get; set; }

		public string SessionToken { get; set; }
	}
}