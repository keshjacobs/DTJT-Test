using System;

namespace StickMan.Services.Models.Message
{
	public class CastMessage
	{
		public AudioInfo MessageInfo { get; set; }

		public UserModel User { get; set; }
	}

	public class AudioInfo
	{
		public int Id { get; set; }

		public string AudioFilePath { get; set; }

		public DateTime UploadTime { get; set; }

		public int Clicks { get; set; }
	}
}
