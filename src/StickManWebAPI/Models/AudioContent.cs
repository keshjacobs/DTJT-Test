using System.Collections.Generic;
using StickManWebAPI.Models.Response;

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

	public class SessionData
	{
		public int UserId { get; set; }

		public string SessionToken { get; set; }
	}

	public class CastAudioContent : SessionData
	{
		public string FilePath { get; set; }

		public string Title { get; set; }
	}
}