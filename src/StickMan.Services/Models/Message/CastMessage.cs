using System;

namespace StickMan.Services.Models.Message
{
	public class CastMessage
	{
		public CastAudioInfo MessageInfo { get; set; }

		public UserModel User { get; set; }

		public bool Listened { get; set; }
	}

	public class CastAudioInfo : AudioInfo
	{
		public string Title { get; set; }

		public TimeSpan UploadedTimeAgo { get; set; }

		public TimeSpan Duration { get; set; }
	}

	public class AudioInfo
	{
		public int Id { get; set; }

		public string AudioFilePath { get; set; }

		public DateTime UploadTime { get; set; }

		public int Clicks { get; set; }
	}
}
